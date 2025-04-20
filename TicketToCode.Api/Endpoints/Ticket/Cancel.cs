using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class Cancel : IEndpoint
    {
        // Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapDelete("/tickets/{id}", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Cancel a ticket for an event for the authenticated user")
            .RequireAuthorization(); // Require authentication

        // Request and Response models
        public record Request(int Id);
        public record Response(int TicketId);

        // Logic
        private static Results<Ok<Response>, NotFound<string>, BadRequest<string>> Handle(
            [AsParameters] Request request,
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

            // Find the ticket
            var ticket = db.Tickets.FirstOrDefault(t => t.ID == request.Id && t.UserID == userIdInt);
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
