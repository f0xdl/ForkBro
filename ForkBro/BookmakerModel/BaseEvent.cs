using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.Common;
using ForkBro.Scanner;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkBro.BookmakerModel
{
    public class BetEvent
    {
        bool hasUpdate;
        DateTime dtUpdate;
        DateTime dtComparison;
        ConcurrentDictionary<OldBetType, BettingOdds[]> bettingOdds;

        public long EventId { get; set; }
        public int PoolId { get; set; }


        public BetEvent()
        {
            bettingOdds = new ConcurrentDictionary<OldBetType, BettingOdds[]>();
        }
        //Признаки
        public Bookmaker Bookmaker { get; set; }
        public Sport Sport { get; set; }
        public StatusEvent Status { get; set; }
        public Command CommandA { get; set; }
        public Command CommandB { get; set; }
        public bool Reverse { get; set; }
        
        public DateTime DtStart;
        public DateTime DtOver;
        public DateTime DtUpdate
        {
            get => dtUpdate;
            set
            {
                dtUpdate = value;
                hasUpdate = true;
            }
        }
        public DateTime DtComparison
        {
            get => dtComparison;
            set
            {
                dtComparison = value;
                hasUpdate = false;
            }
        }

        //To Model
        public void AddOrUpdate(OldBetType type, BettingOdds[] coefArray) => bettingOdds.AddOrUpdate(type, (k)=>coefArray, (k, v)=>coefArray);
        public void UpdateOdds(ConcurrentDictionary<OldBetType, BettingOdds[]> newOdds)
        {
            bettingOdds = newOdds;
            DtUpdate = DateTime.Now;
        }
        public void EventOver()
        {
            Status = StatusEvent.Over;
            DtOver = DateTime.Now;
        }
        //To Daemons
        public bool HasUpdate { get => hasUpdate; }
        public BettingOdds[] GetBetTypeOdds(OldBetType type) => bettingOdds[type];
        public Dictionary<OldBetType, BettingOdds[]> AllOdds => bettingOdds.ToDictionary(x => x.Key, x => x.Value);//Debug write to json file
        public ConcurrentDictionary<OldBetType, BettingOdds[]> GetSnapshotOdds(OldBetType type) => bettingOdds;
       
    }
}
