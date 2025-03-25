namespace TicketToCode.Api.Endpoints.User
{
    public class Put : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapPut("/users/edit/{id}", Handle)
            .WithSummary("Edit User attributes");

        public record Request(
            int Id,
            string? username,
            string? role
            );

        public record Response(
            int Id,
            string Username,
            string Role,
            DateTime createdAt
            );

        private static IResult Handle([AsParameters] Request request, IDatabase db)
        {

            var u = db.Users.Find(u => u.Id == request.Id);
            if (request.username != null)
            {
                u.Username = request.username;
            }
            if (request.role != null)
            {
                var roleValues = typeof(UserRoles)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Select(f => f.GetValue(null).ToString())
                .ToHashSet();

                if (roleValues.Contains(request.role))
                {
                    u.Role = request.role;
                }
                else
                {
                    return TypedResults.BadRequest("Invalid Role");
                }
            }
            db.Users.Append(u);

            var response = new Response(u.Id, u.Username, u.Role, u.CreatedAt); 

            return TypedResults.Ok(response);
        }
    }
}
