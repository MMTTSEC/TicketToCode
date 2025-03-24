namespace TicketToCode.Api.Endpoints.User
{
    public class GetAll : IEndpoint
    {
        //Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app
            .MapGet("/user/all", Handle)
            .WithSummary("View All Users Database");
        }

        public record Request();
        public record Results(
            int Id,
            string Username,
            string Role,
            DateTime createdAt
            );

        private static List<Results> Handle(IDatabase db)
        {
            return db.Users
                .Select(u => new Results(
                    u.Id,
                    u.Username,
                    u.Role,
                    u.CreatedAt
                    )).ToList();
        }
    }
}

