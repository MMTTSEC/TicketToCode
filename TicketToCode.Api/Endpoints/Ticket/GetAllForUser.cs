using System.Reflection.Metadata;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class GetAllForUser : IEndpoint
    {
        // Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapGet("/tickets/my-tickets", Handle)
        .WithTags("Ticket EndPoints")
        .WithSummary("Get all tickets for the authenticated user");
        //.RequireAuthorization();

        public record Response(int TicketId, int EventId, string EventName, DateTime EventStart, DateTime EventEnd);

        // Logic
        private static Results<Ok<List<Response>>, BadRequest<string>> Handle(
            IDatabase db,
            HttpContext context)
        {
            // Authentication 
            var authCookie = context.Request.Cookies["auth"];
            if (string.IsNullOrEmpty(authCookie))
            {
                return TypedResults.BadRequest("User not authenticated");
            }
            
            // Add debugging
            Console.WriteLine($"Auth cookie: {authCookie}");
            
            var username = authCookie.Split(':')[0];
            Console.WriteLine($"Username from cookie: {username}");
            
            var user = db.Users.FirstOrDefault(u => 
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
                
            if (user == null)
            {
                Console.WriteLine($"User '{username}' not found in database");
                return TypedResults.BadRequest("User not found");
            }
            
            Console.WriteLine($"Found user with ID: {user.Id}");
            
            // Get all tickets for the user
            var userTickets = db.Tickets
                .Where(t => t.UserID == user.Id)
                .ToList();
                
            Console.WriteLine($"Found {userTickets.Count} tickets for user ID {user.Id}");
            
            // Map to response
            var response = userTickets
                .Select(t =>
                {
                    var ev = db.Events.FirstOrDefault(e => e.Id == t.EventID); 
                    if (ev == null)
                    {
                        Console.WriteLine($"Event with ID {t.EventID} not found for ticket {t.ID}");
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
                .Where(r => r != null) // Filter out null responses
                .ToList();

            Console.WriteLine($"Returning {response.Count} tickets for user '{username}'");
            return TypedResults.Ok(response);
        }
    }
}