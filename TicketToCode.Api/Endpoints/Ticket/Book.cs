using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class Book : IEndpoint
    {
        // Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapPost("/tickets/book", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Book a ticket for an event")
            .RequireAuthorization(); // Add authorization requirement

        // Request and Response types
        public record Request([Required] int EventId); // Input validation
        public record Response(int TicketId);

        // Logic
        private static Results<Ok<Response>, NotFound<string>, BadRequest<string>, UnauthorizedHttpResult> Handle(
            [Microsoft.AspNetCore.Mvc.FromBody] Request request,
            IDatabase db,
            HttpContext context)
        {
            // Get user from the JWT token
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
            {
                return TypedResults.Unauthorized();
            }

            var user = db.Users.FirstOrDefault(u => u.Id == userIdInt);
            if (user is null)
                return TypedResults.Unauthorized();

            // Event validation
            var ev = db.Events.FirstOrDefault(e => e.Id == request.EventId);
            if (ev is null)
                return TypedResults.NotFound("Event not found");

            // Capacity check
            var bookedTickets = db.Tickets.Count(t => t.EventID == ev.Id);
            if (bookedTickets >= ev.MaxAttendees)
                return TypedResults.BadRequest("Event is fully booked");

            // Create ticket
            var newTicket = new TicketToCode.Core.Models.Ticket
            {
                ID = db.Tickets.Any() ? db.Tickets.Max(t => t.ID) + 1 : 1,
                UserID = userIdInt,
                EventID = ev.Id
            };

            db.Tickets.Add(newTicket);
            return TypedResults.Ok(new Response(newTicket.ID));
        }
    }
}
