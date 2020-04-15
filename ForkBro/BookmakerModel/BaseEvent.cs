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
        ConcurrentDictionary<ushort, double[,]> bettingOdds; // Key = (ushort)((byte)BetType + (((byte)EventUnit) << 8))

        public long EventId { get; set; }
        public int PoolId { get; set; }


        public BetEvent()
        {
            bettingOdds = new ConcurrentDictionary<ushort, double[,]>();
            Sport = Sport.None;
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
        public void AddOrUpdate(ushort type, double[,] coefArray) => bettingOdds.AddOrUpdate(type, (k)=>coefArray, (k, v)=>coefArray);
        public void UpdateOdds(ConcurrentDictionary<ushort, double[,]> newOdds)
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
        public double[,] GetBetTypeOdds(ushort type) => bettingOdds[type];
        public Dictionary<ushort, double[,]> AllOdds => bettingOdds.ToDictionary(x => x.Key, x => x.Value);//Debug write to json file
        public ConcurrentDictionary<ushort, double[,]> GetSnapshotOdds() => bettingOdds;
       
    }
}
