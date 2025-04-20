using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class GetAllTickets : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapGet("/tickets/all", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Get all tickets")
            .RequireAuthorization(policy => policy.RequireRole("Admin")); // Require Admin role

        public record Response(
            int TicketId,
            int UserId,
            int EventId
            );

        private static Results<Ok<List<Response>>, NotFound<string>> Handle(
           IDatabase db,
           HttpContext context)
        {
            // JWT authentication is handled by the RequireAuthorization attribute with role check

            // Check if the ticket list is empty
            if (!db.Tickets.Any())
            {
                return TypedResults.NotFound("No tickets were found");
            }

            // Get all tickets and map to response
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
