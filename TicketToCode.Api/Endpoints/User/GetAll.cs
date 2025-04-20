using Microsoft.AspNetCore.Authorization;

namespace TicketToCode.Api.Endpoints.User
{
    public class GetAll : IEndpoint
    {
        //Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapGet("/users/all", Handle)
            .WithTags("User EndPoints")
            .WithSummary("View All Users Database")
            .RequireAuthorization(policy => policy.RequireRole("Admin")); // Require Admin role

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

