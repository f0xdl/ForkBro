using ForkBro.BookmakerModel;
using ForkBro.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ForkBro.Scanner;

namespace ForkBro.Mediator
{
    public class PoolRaw
    {
        private readonly ConcurrentDictionary<Bookmaker, BetEvent> _snapshots;

        public PoolRaw()=>_snapshots = new ConcurrentDictionary<Bookmaker, BetEvent>();
        public void AddSnapshot(ref BetEvent bookmakerEvent)=>_snapshots.TryAdd(bookmakerEvent.Bookmaker, bookmakerEvent);
        public void RemoveSnapshot(Bookmaker bookmaker) => _snapshots.TryRemove(bookmaker, out BetEvent baseEvent);
        public void UpdateSnapshot(ref BetEvent bookmakerEvent)=> _snapshots[bookmakerEvent.Bookmaker] = bookmakerEvent;
        public BetEvent GetSnapshot(Bookmaker bookmaker) => _snapshots[bookmaker];
        public BetEvent[] GetAllSnapshot() => _snapshots.Values.ToArray(); 
        public bool HasUpdate
        {
            get
            {
                foreach (var snapshot in _snapshots.Values)
                    if (snapshot.HasUpdate)
                        return true;
                return false;
            }
        }
        public void UpdateDtComparison()
        {
            foreach (var snapshot in _snapshots.Values)
                snapshot.DtComparison = DateTime.Now;
        }
        public Sport GetSport() => _snapshots.FirstOrDefault().Value.Sport;

        public void GetCommands(out Command commandA, out Command commandB)
        {
            BetEvent betEvent = _snapshots.FirstOrDefault().Value;
            commandA = betEvent.CommandA;
            commandB = betEvent.CommandB;
        }

        public bool ExistsEvent(Bookmaker bookmaker, long id)
        {
            if(_snapshots.ContainsKey(bookmaker))
                if(_snapshots[bookmaker].EventId == id)
                    return true;
            return false;
        }
    }
}