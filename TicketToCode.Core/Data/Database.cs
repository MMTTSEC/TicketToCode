using TicketToCode.Core.Models;

namespace TicketToCode.Core.Data;

public interface IDatabase
{
    List<Event> Events { get; set; }
    List<User> Users { get; set; }
    List<Ticket> Tickets { get; set; }
}

public class Database : IDatabase
{
    public List<Event> Events { get; set; } = new List<Event>();
    public List<User> Users { get; set; } = new List<User>();
    public List<Ticket> Tickets { get; set; } = new List<Ticket>();

    public Database()
    {
        // Create default users if they don't exist
        if (!Users.Any(u => u.Username == "admin"))
        {
            var adminHash = BCrypt.Net.BCrypt.HashPassword("12345678");
            Users.Add(new User("admin", adminHash) { Id = 1, Role = UserRoles.Admin });
        }

        if (!Users.Any(u => u.Username == "user"))
        {
            var userHash = BCrypt.Net.BCrypt.HashPassword("12345678");
            Users.Add(new User("user", userHash) { Id = 2, Role = UserRoles.User });
        }
    }
}
