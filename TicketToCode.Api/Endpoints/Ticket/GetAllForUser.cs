using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Reflection.Metadata;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class GetAllForUser : IEndpoint
    {
        // Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapGet("/tickets/my-tickets", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Get all tickets for the authenticated user")
            .RequireAuthorization(); // Add this to require JWT authentication

        public record Response(int TicketId, int EventId, string EventName, DateTime EventStart, DateTime EventEnd);

        // Logic
        private static Results<Ok<List<Response>>, BadRequest<string>> Handle(
            IDatabase db,
            HttpContext context)
        {
            // Get user from the JWT token claims
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
            {
                return TypedResults.BadRequest("User not authenticated or invalid user ID");
            }

            var user = db.Users.FirstOrDefault(u => u.Id == userIdInt);
            if (user == null)
            {
                return TypedResults.BadRequest("User not found");
            }

            Console.WriteLine($"Found user with ID: {user.Id} from JWT token");

            // Get all tickets for the user
            var userTickets = db.Tickets
                .Where(t => t.UserID == user.Id)
                .ToList();

            // Map to response
            var response = userTickets
                .Select(t =>
                {
                    var ev = db.Events.FirstOrDefault(e => e.Id == t.EventID);
                    if (ev == null)
                    {
                        return null;
                    }
                    return new Response(
                        TicketId: t.ID,
                        EventId: t.EventID,
                        EventName: ev.Name,
                        EventStart: ev.StartTime,
                        EventEnd: ev.EndTime
                    );
                })
                .Where(r => r != null)
                .ToList();

            return TypedResults.Ok(response);
        }
    }
}