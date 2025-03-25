namespace TicketToCode.Api.Endpoints.Ticket
{
    public class GetStats : IEndpoint
    {
        // Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapGet("/tickets/stats", Handle)
            .WithTags("Ticket EndPoints")
            .WithSummary("Get statistics for all tickets (admin)");
        // .RequireAuthorization() commented out for testing

        // Response model
        public record Response(
            int TotalTickets,
            int TotalEventsWithTickets,
            int AverageTicketsPerEvent,
            int MaxTicketsForSingleEvent
        );

        // Logic
        private static Results<Ok<Response>, BadRequest<string>> Handle(
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
            

            // Calculate ticket statistics
            var totalTickets = db.Tickets.Count;
            var eventsWithTickets = db.Tickets
                .Select(t => t.EventID)
                .Distinct()
                .Count();
            var averageTicketsPerEvent = eventsWithTickets > 0
                ? (int)Math.Round((double)totalTickets / eventsWithTickets)
                : 0;
            var maxTicketsForSingleEvent = db.Tickets
                .GroupBy(t => t.EventID)
                .Select(g => g.Count())
                .DefaultIfEmpty(0)
                .Max();

            // Return the stats
            var response = new Response(
                TotalTickets: totalTickets,
                TotalEventsWithTickets: eventsWithTickets,
                AverageTicketsPerEvent: averageTicketsPerEvent,
                MaxTicketsForSingleEvent: maxTicketsForSingleEvent
            );

            return TypedResults.Ok(response);
        }
    }
}
