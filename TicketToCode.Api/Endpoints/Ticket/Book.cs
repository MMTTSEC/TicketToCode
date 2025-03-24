using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class Book : IEndpoint
    {
        // Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapPost("/tickets/book", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Book a ticket for an event");
           // .RequireAuthorization(); 

        // Request and Response types
        public record Request([Required] int EventId); // Input validation
        public record Response(int TicketId);

        // Logic
        private static Results<Ok<Response>, NotFound<string>, BadRequest<string>, UnauthorizedHttpResult> Handle(
            [AsParameters] Request request,
            IDatabase db,
            HttpContext context)
        {
             // Authentication check
            var authCookie = context.Request.Cookies["auth"];
            if (authCookie is null) return TypedResults.Unauthorized();

            var parts = authCookie.Split(':');
            var username = parts[0];
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user is null) return TypedResults.Unauthorized();

            

            // 2. Event validation
            var ev = db.Events.FirstOrDefault(e => e.Id == request.EventId);
            if (ev is null) return TypedResults.NotFound("Event not found");

            // 3. Capacity check
            var bookedTickets = db.Tickets.Count(t => t.EventID == ev.Id);
            if (bookedTickets >= ev.MaxAttendees)
                return TypedResults.BadRequest("Event is fully booked");

            // 4. Create ticket
            var newTicket = new TicketToCode.Core.Models.Ticket
            {
                ID = db.Tickets.Any() ? db.Tickets.Max(t => t.ID) + 1 : 1,
                UserID = user.Id,                                   // temporary hardcoded user id until we have proper auth
                EventID = ev.Id
            };

            db.Tickets.Add(newTicket);
            return TypedResults.Ok(new Response(newTicket.ID));
        }
    }

}
