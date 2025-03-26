using TicketToCode.Api.Services;

namespace TicketToCode.Api.Endpoints.Auth;

public class GetRole : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapGet("/auth/role", Handle)
        .WithSummary("Get the current user's role from the auth cookie")
        .WithTags("Auth EndPoints");

    // Models
    public record Response(string Username, string Role);

    // Logic
    private static Results<Ok<Response>, NotFound<string>> Handle(HttpContext context)
    {
        // Get the auth cookie
        var authCookie = context.Request.Cookies["auth"];
        if (string.IsNullOrEmpty(authCookie))
        {
            return TypedResults.NotFound("Not authenticated");
        }

        // Parse the auth cookie (format: username:role)
        var parts = authCookie.Split(':');
        if (parts.Length != 2)
        {
            return TypedResults.NotFound("Invalid authentication data");
        }

        var username = parts[0];
        var role = parts[1];

        return TypedResults.Ok(new Response(username, role));
    }
}