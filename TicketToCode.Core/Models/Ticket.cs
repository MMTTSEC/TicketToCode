using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketToCode.Core.Models
{
    public class Ticket
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
    }
}
