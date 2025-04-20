using TicketToCode.Api.Services;

namespace TicketToCode.Api.Endpoints.Auth;

public class Login : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPost("/auth/login", Handle)
        .WithSummary("Login with username and password")
        .WithTags("Auth EndPoints")
        .AllowAnonymous();

    // Models
    public record Request(string Username, string Password);
    public record Response(string Username, string Role, string Token);

    // Logic
    private static Results<Ok<Response>, NotFound<string>> Handle(
        Request request,
        IAuthService authService,
        IJwtService jwtService)
    {
        var user = authService.Login(request.Username, request.Password);
        if (user == null)
        {
            return TypedResults.NotFound("Invalid username or password");
        }

        // Generate JWT token
        var token = jwtService.GenerateToken(user);

        // Return token in response
        var response = new Response(user.Username, user.Role, token);
        return TypedResults.Ok(response);
    }
}