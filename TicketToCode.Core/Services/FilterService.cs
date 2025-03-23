using System.Threading.Tasks;
using TicketToCode.Core.Data;
using TicketToCode.Core.Interface;
using TicketToCode.Core.Models;

namespace TicketToCode.Core.Services
{
    public class FilterService : IFilter
    {
        private readonly IDatabase _database;
        public FilterService(IDatabase database)
        {
            _database = database;
        }

        public List<Event> SearchBar(string input)
        {
            var matchedByName = new List<Event>();
            var hold = new List<Event>();
            var matchedByEventType = new List<Event>();

            foreach (var e in _database.Events)
            {
                if (e.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                {
                    matchedByName.Add(e);
                }
                else
                {
                    hold.Add(e);
                }
            }
            foreach (var e in hold)
            {
                var matchingType = Enum.GetValues(typeof(EventType))
                               .Cast<EventType>()
                               .Where(et => et.ToString().Contains(input, StringComparison.OrdinalIgnoreCase))
                               .FirstOrDefault();
                if (matchingType != 0)
                {
                    if (e.Type == matchingType)
                    {
                        matchedByEventType.Add(e);
                    }
                }

            }

            var finalResults = matchedByName
                .Concat(matchedByEventType)
                .ToList();

            return finalResults;
        }
    }
}
