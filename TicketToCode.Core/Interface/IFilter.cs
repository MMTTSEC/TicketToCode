﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketToCode.Core.Models;

namespace TicketToCode.Core.Interface
{
    public interface IFilter
    {
        List<Event> SearchBar(string sentence);
    }
}
