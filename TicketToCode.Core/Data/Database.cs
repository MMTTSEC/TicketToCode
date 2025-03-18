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
}
