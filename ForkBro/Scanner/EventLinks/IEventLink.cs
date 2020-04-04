using ForkBro.Common;
using System;
using System.Collections.Generic;

namespace ForkBro.Scanner.EventLinks
{
    public interface IGameList
    {
        public bool Success { get;}
        public IEventLink[] EventsArray { get; }
    }

    public interface IEventLink
    {
        public Bookmaker Bookmaker { get; set; }
        public DateTime Updated { get; set; }
        //public StatusEvent Status { get; set; }
        public string TournamentName { get; }
        public long Id{ get; }
        public Sport Sport{ get; }
        public Command CommandA { get; }
        public Command CommandB { get; }
    }
}