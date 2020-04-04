using ForkBro.BookmakerModel;
using ForkBro.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ForkBro.Mediator
{
    public class PoolRaw
    {
        //public long id { get; private set; }
        public EventProps props { get; private set; }
        private ConcurrentDictionary<Bookmaker, BetEvent> snapshots;

        public PoolRaw(EventProps props)
        {
            snapshots = new ConcurrentDictionary<Bookmaker, BetEvent>();
            this.props = props;
        }
        public void AddSnapshot(ref BetEvent bookmakerEvent)
        {
            //if(!snapshots.ContainsKey(bookmakerEvent.bookmaker))
            snapshots.TryAdd(bookmakerEvent.Bookmaker, bookmakerEvent);
        }
        public void RemoveSnapshot(Bookmaker bookmaker) => snapshots.TryRemove(bookmaker, out BetEvent baseEvent);
        public void UpdateSnapshot(ref BetEvent bookmakerEvent)
        {
            snapshots[bookmakerEvent.Bookmaker] = bookmakerEvent;
        }
        public BetEvent GetSnapshot(Bookmaker bookmaker) => snapshots[bookmaker];
        public BetEvent[] GetAllSnapshot() => snapshots.Values.ToArray(); 
        public bool HasUpdate
        {
            get
            {
                foreach (var snapshot in snapshots.Values)
                    if (snapshot.HasUpdate)
                        return true;
                return false;
            }
        }
        public void UpdateDtComparison()
        {
            foreach (var snapshot in snapshots.Values)
                snapshot.DtComparison = DateTime.Now;
        }
        public bool ExistsEvent(Bookmaker bookmaker, long id)
        {
            if(snapshots.ContainsKey(bookmaker))
                if(snapshots[bookmaker].EventId == id)
                    return true;
            return false;
        }
    }
}