using System.Reflection.Metadata;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class Cancel : IEndpoint
    {
        // Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapDelete("/tickets/{id}", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Cancel a ticket for an event for the authenticated user");
     

        // Request and Response models
        public record Request(int Id); 
        public record Response(int TicketId);

        // Logic
        private static Results<Ok<Response>, NotFound<string>, BadRequest<string>> Handle(
            [AsParameters] Request request,
            IDatabase db,
            HttpContext context)
        {
            //int userId = 1; // Temporary hardcoded user ID for testing

            
            // Authentication check commented out for testing
            var authCookie = context.Request.Cookies["auth"];
            if (string.IsNullOrEmpty(authCookie))
            {
                return TypedResults.BadRequest("User not authenticated");
            }
            var username = authCookie.Split(':')[0];
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return TypedResults.BadRequest("User not found");
            }
            

            // Find the ticket
            var ticket = db.Tickets.FirstOrDefault(t => t.ID == request.Id && t.UserID == user.Id);
            if (ticket == null)
            {
                return TypedResults.NotFound($"Ticket with ID {request.Id} not found or does not belong to user");
            }

            // Remove the ticket
            db.Tickets.Remove(ticket);

            // Return the canceled ticket ID
            return TypedResults.Ok(new Response(ticket.ID));
        }

    }
}
