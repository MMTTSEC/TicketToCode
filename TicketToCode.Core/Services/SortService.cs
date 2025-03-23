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
    }
}
