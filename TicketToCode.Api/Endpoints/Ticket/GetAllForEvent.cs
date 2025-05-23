﻿using Microsoft.AspNetCore.Authorization;

namespace TicketToCode.Api.Endpoints.Ticket
{
    public class GetAllForEvent : IEndpoint
    {
        // Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapGet("/tickets/event/{eventId}", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Get all tickets for an event (admin)")
            .RequireAuthorization(policy => policy.RequireRole("Admin")); // Require Admin role

        // Request and Response types
        public record Request(int EventId);
        public record Response(int TicketId, int UserId, string Username);

        // Logic
        private static Results<Ok<List<Response>>, NotFound<string>> Handle(
            [AsParameters] Request request,
            IDatabase db,
            HttpContext context)
        {
            // JWT authentication is handled by the RequireAuthorization attribute with role check

            // Check if the event exists
            var eventExists = db.Events.Any(e => e.Id == request.EventId);
            if (!eventExists)
            {
                return TypedResults.NotFound($"Event with ID {request.EventId} not found");
            }

            // Get all tickets for the event and map to response
            var eventTickets = db.Tickets
                .Where(t => t.EventID == request.EventId)
                .Select(t =>
                {
                    var user = db.Users.FirstOrDefault(u => u.Id == t.UserID);
                    return new Response(
                        TicketId: t.ID,
                        UserId: t.UserID,
                        Username: user?.Username ?? "Unknown" // Handle missing user
                    );
                })
                .ToList();

            return TypedResults.Ok(eventTickets);
        }
    }
}
