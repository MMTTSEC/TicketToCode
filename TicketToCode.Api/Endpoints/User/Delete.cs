using Microsoft.AspNetCore.Authorization;

namespace TicketToCode.Api.Endpoints.User
{
    public class Delete : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapDelete("/users/{id}", Handle)
            .WithTags("User EndPoints")
            .WithSummary("Delete User")
            .RequireAuthorization(policy => policy.RequireRole("Admin")); // Require Admin role

        public record Request(int Id);
        public record Response(int Id);

        private static Response Handle([AsParameters] Request request, IDatabase db)
        {
            var u = db.Users.Find(u => u.Id == request.Id);
            db.Users.Remove(u);
            return new Response(u.Id);
        }
    }
}
