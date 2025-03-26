using System.Reflection.Metadata;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class GetAllTickets : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapGet("/tickets/all", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Get all tickets");

        public record Response(
            int TicketId, 
            int UserId, 
            int EventId
            );

        private static Results<Ok<List<Response>>, NotFound<string>, BadRequest<string>> Handle(
           IDatabase db,
           HttpContext context)
        {
            // Authentication check
            var authCookie = context.Request.Cookies["auth"];
            if (string.IsNullOrEmpty(authCookie))
            {
                return TypedResults.BadRequest("User not authenticated");
            }
            var username = authCookie.Split(':')[0];
            var role = authCookie.Split(':')[1];
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || role != "Admin")
            {
                return TypedResults.BadRequest("Admin access required");
            }

            // Check if the ticket is empty
            var ticketsList = db.Tickets.Any();
            if (!ticketsList)
            {
                return TypedResults.NotFound($"Not tickets were found");
            }

            // Get all tickets map to response
            else
            {

                var allTickets = db.Tickets
                    .Select(t => new Response(

                        TicketId: t.ID,
                        UserId: t.UserID,
                        EventId: t.EventID
                    )).ToList();

                return TypedResults.Ok(allTickets);
            }
        }
    }
}
