namespace TicketToCode.Api.Endpoints.Auth;

public class Logout : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPost("/auth/logout", Handle)
        .WithSummary("Logout - client will discard the JWT token");

    // Models
    public record Response(bool Success, string Message);

    // Logic
    private static IResult Handle()
    {
        // With JWT, logout is primarily handled client-side
        return TypedResults.Ok(new Response(true, "Logged out successfully"));
    }
}