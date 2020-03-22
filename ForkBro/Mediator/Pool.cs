using ForkBro.Bookmakers;
using ForkBro.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForkBro.Mediator
{
    public class PoolRaw 
    {
        //public long id { get; private set; }
        public EventProps props { get; private set; }
        private Dictionary<Bookmaker, BookmakerEvent> snapshots;
        public int CountUpdate { get; private set; }

        public PoolRaw(EventProps props)
        {
            CountUpdate = 0;
            snapshots = new Dictionary<Bookmaker, BookmakerEvent>();
            this.props = props;
        }
        public void AddSnapshot(ref BookmakerEvent bookmakerEvent)
        {
            snapshots.Add(bookmakerEvent.bookmaker, bookmakerEvent);
        }
        public void RemoveSnapshot(Bookmaker bookmaker) => snapshots.Remove(bookmaker);
        public void UpdateSnapshot(ref BookmakerEvent bookmakerEvent)
        {
            snapshots[bookmakerEvent.bookmaker] = bookmakerEvent;
            CountUpdate++;
        } 
        public void GetUpdates() => CountUpdate = 0;

        //public bool ExistsSnapshot(Bookmaker bookmaker) => snapshots.ContainsKey(bookmaker);
        public bool? ExistsEvent(Bookmaker bookmaker, long id) => snapshots[bookmaker]?.EventId == id;
    }
}