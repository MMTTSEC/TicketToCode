namespace TicketToCode.Api.Endpoints.Auth;

public class Logout : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPost("/auth/logout", Handle)
        .WithSummary("Logout the current user by clearing authentication cookies");

    // Models
    public record Response(bool Success, string Message);

    // Logic
    private static IResult Handle(HttpContext context)
    {
        // Check if the user is authenticated
        var authCookie = context.Request.Cookies["auth"];
        if (string.IsNullOrEmpty(authCookie))
        {
            return TypedResults.Ok(new Response(false, "No active session found"));
        }

        // Clear the auth cookie
        context.Response.Cookies.Delete("auth", new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true
        });

        return TypedResults.Ok(new Response(true, "Logged out successfully"));
    }
}