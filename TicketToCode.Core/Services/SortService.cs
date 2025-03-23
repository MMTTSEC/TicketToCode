using TicketToCode.Core.Data;
using TicketToCode.Core.Interface;
using TicketToCode.Core.Models;

namespace TicketToCode.Core.Services
{
    public class SortService : ISort
    {
        private readonly IDatabase _database;
        public SortService(IDatabase database)
        {
            _database = database;
        }
        public List<Event> SortByStartTime()
        {
            return _database.Events.OrderBy(x => x.StartTime).ToList();
        }
        public List<Event> SortByRemaniningTickets()
        {
            var eventsWithRemainingTickets = _database.Events.Select(e => new
            {
                Event = e,
                // compare event id and ticket to calculate how many tickets are left.
                RemainingTickets = e.MaxAttendees - _database.Tickets.Count(ticket => ticket.EventID == e.Id)
            }).ToList();

            var sortedEvents = eventsWithRemainingTickets
                //sort them to be presented.
               .OrderByDescending(x => x.RemainingTickets)
               .Select(x => x.Event)
               .ToList();

            return sortedEvents;
        }
    }
}
