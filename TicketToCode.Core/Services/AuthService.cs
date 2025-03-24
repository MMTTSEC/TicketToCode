using TicketToCode.Core.Data;
using TicketToCode.Core.Models;

namespace TicketToCode.Api.Services;

public interface IAuthService
{
    User? Login(string username, string password);
    User? Register(string username, string password);
}

// TODO: Implement better auth
/// <summary>
/// Simple auth service to enable registering and login in, should be replaced before release
/// </summary>
public class AuthService : IAuthService
{
    private readonly IDatabase _database;

    public AuthService(IDatabase database)
    {
        _database = database;
    }

    public User? Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        if (_database.Users.Any(u => u.Username == username))
        {
            return null;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(username, passwordHash) { Role = "User" }; // Set default role
        var possibleId = _database.Users.Count() + 1;

        while(_database.Users.Any(u => u.Id == possibleId))
        {
            possibleId++;
        }

        user.Id = possibleId;
        _database.Users.Add(user);
        return new User(user.Username, user.Role); // Return without password hash
    }

    public User? Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = _database.Users.FirstOrDefault(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return new User(user.Username, user.Role);
    }
}