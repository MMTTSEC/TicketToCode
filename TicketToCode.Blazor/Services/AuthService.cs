using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.JSInterop;

namespace TicketToCode.Blazor.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal? _cachedUser;

        public AuthService(HttpClient http, IJSRuntime jsRuntime)
        {
            _http = http;
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                Console.WriteLine($"Attempting login for user: {username}");

                var response = await _http.PostAsJsonAsync("https://localhost:7206/auth/login",
                    new { Username = username, Password = password });

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login response status: {response.StatusCode}");
                Console.WriteLine($"Login response content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var authResult = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (authResult?.Token != null)
                    {
                        Console.WriteLine("Token received, storing in localStorage");
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "auth_token", authResult.Token);

                        // Debug the token claims
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(authResult.Token);

                        Console.WriteLine("Token claims:");
                        foreach (var claim in jwtToken.Claims)
                        {
                            Console.WriteLine($"  {claim.Type}: {claim.Value}");
                        }

                        _cachedUser = null; // Clear cached user so it will be re-parsed from the new token
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Token was null in response");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in LoginAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_token");
            _cachedUser = null;
        }

        public async Task<ClaimsPrincipal> GetUserAsync()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_token");

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("GetUserAsync: No token found in localStorage");
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            var handler = new JwtSecurityTokenHandler();

            try
            {
                // Just parse the claims, don't validate the token
                var jwtToken = handler.ReadJwtToken(token);

                // Debug token claims
                Console.WriteLine("GetUserAsync: Token claims:");
                foreach (var claim in jwtToken.Claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }

                // Create an identity with authentication type "jwt"
                // and explicitly set nameClaimType and roleClaimType
                var identity = new ClaimsIdentity(
                    jwtToken.Claims,
                    "jwt",
                    nameType: "unique_name", // The name claim type used in your token
                    roleType: "role"         // The role claim type used in your token
                );

                var user = new ClaimsPrincipal(identity);
                _cachedUser = user;

                // Debug authentication state
                Console.WriteLine($"GetUserAsync: IsAuthenticated = {user.Identity?.IsAuthenticated}");
                Console.WriteLine($"GetUserAsync: Name = {user.Identity?.Name ?? "none"}");
                Console.WriteLine($"GetUserAsync: Role = {user.FindFirst("role")?.Value ?? "none"}");
                Console.WriteLine($"GetUserAsync: In Admin Role = {user.IsInRole("Admin")}");

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetUserAsync: {ex.Message}");
                // If token parsing fails, return an empty principal
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var user = await GetUserAsync();
            var result = user.Identity?.IsAuthenticated ?? false;
            Console.WriteLine($"IsAuthenticatedAsync: {result}");
            return result;
        }

        public async Task<string> GetUsernameAsync()
        {
            var user = await GetUserAsync();
            // Try different claim types for username
            var username = user.FindFirst("unique_name")?.Value
                ?? user.FindFirst(ClaimTypes.Name)?.Value
                ?? string.Empty;
            Console.WriteLine($"GetUsernameAsync: {username}");
            return username;
        }

        public async Task<string> GetUserRoleAsync()
        {
            var user = await GetUserAsync();
            // Try different claim types for role
            var role = user.FindFirst("role")?.Value
                ?? user.FindFirst(ClaimTypes.Role)?.Value
                ?? string.Empty;
            Console.WriteLine($"GetUserRoleAsync: {role}");
            return role;
        }

        // Add this helper method to set the JWT token in HTTP Authorization header
        public async Task<HttpRequestMessage> CreateAuthorizedRequestMessage(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine("CreateAuthorizedRequestMessage: No token found in localStorage");
            }

            return request;
        }

        private class LoginResponse
        {
            public string? Username { get; set; }
            public string? Role { get; set; }
            public string? Token { get; set; }
        }
    }
}