namespace TicketToCode.Core.Models;
public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = "New event";
    public string Description { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxAttendees { get; set; }
    public int Bookings { get; set; }
}

public enum EventType
{
    Concert = 0,
    Festival,
    Theatre,
    Other
}