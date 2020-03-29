using ForkBro.BookmakerModel;
using ForkBro.Common;
using ForkBro.Scanner;
using SentencesFuzzyComparison;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForkBro.Mediator
{
    public class HubManager
    {
        PoolRaw[] pool;
        double minQuality;
        public HubManager(int count, double minimumQuality)
        {
            pool = new PoolRaw[count];
            minQuality = minimumQuality;
        }
        int GetNextIndex()
        {
            int i = 0;
            while (i < pool.Length)
                if (pool[i] == null)
                    return i;
                else
                    i++;

            //Pools array don't have free Pool
            Array.Resize<PoolRaw>(ref pool, i + 10);
            return i + 1;
        }

        public int AddPoolRaw(EventProps props)
        {
            int id = GetNextIndex();
            this.pool[id] = new PoolRaw(props);
            return id;
        }
        public void RemovePoolRaw(long index) => pool[index] = null;
        public int HasEventInPool(Bookmaker bookmaker, long id)
        {
            for (int i = 0; i < pool.Length; i++)
                if (pool[i]?.ExistsEvent(bookmaker, id) ?? false)
                    return i;
            return -1;
        }
        public BetEvent GetEvent(Bookmaker bookmaker, long idEvent) 
        {
            for (int i = 0; i < pool.Length; i++)
                if (pool[i].ExistsEvent(bookmaker, idEvent))
                    return pool[i].GetSnapshot(bookmaker);
            return null;
        }
        public void AddSnapshot(int idPool, ref BetEvent bookmakerEvent) => pool[idPool].AddSnapshot(ref bookmakerEvent);
        public void RemoveSnapsot(int idPool, Bookmaker bookmaker) => pool[idPool].RemoveSnapshot(bookmaker);
        public void UpdateSnapshot(int idPool, ref BetEvent bookmakerEvent) => pool[idPool].UpdateSnapshot(ref bookmakerEvent);
        public PoolRaw GetSnapshotsToComparison()
        {
            int idPool = -1;
            for (int i = 0; i < pool.Length; i++)
                if (pool[i] != null && pool[i].HasUpdate)
                { 
                    idPool = i;
                    break;
                }
            
            if (idPool == -1)
                return null;

            pool[idPool].UpdateDtComparison();
            return pool[idPool];
        }

        /// <summary>
        /// Return -1 if Hub Don
        /// </summary>
        /// <param name="sport"></param>
        /// <param name="CommandA"></param>
        /// <param name="CommandB"></param>
        /// <returns></returns>
        public double CalculateFuzzyEvent(Sport sport, Command CommandA, Command CommandB, out int idPool, out bool reverse)
        {
            FuzzyComparer fuzzyComparer = new FuzzyComparer();
            string[] newEvent = new string[] { CommandA.NameEng, CommandB.NameEng };
            idPool = -1;
            reverse = false;
            double quality = 0;
            double maxQuality = 0;
            for (int i = 0; i < pool.Length; i++)
                if (pool[i]?.props.sport == sport)
                {
                    string[] inPool = new string[] { pool[i].props.CommandA.NameEng, pool[i].props.CommandB.NameEng };
                    quality += fuzzyComparer.CalculateFuzzyEqualValue(inPool[0], newEvent[0]);
                    quality += fuzzyComparer.CalculateFuzzyEqualValue(inPool[1], newEvent[1]);
                    quality /= 2;
                    if (maxQuality < quality)
                        maxQuality = quality;
                    if (quality > minQuality)
                    {
                        idPool = i;
                        reverse = false;
                        break;
                    }
                    quality = 0;

                    quality += fuzzyComparer.CalculateFuzzyEqualValue(inPool[0], newEvent[0]);
                    quality += fuzzyComparer.CalculateFuzzyEqualValue(inPool[1], newEvent[1]);
                    quality /= 2;
                    if (maxQuality < quality)
                        maxQuality = quality;
                    if (quality > minQuality)
                    {
                        idPool = i;
                        reverse = false;
                        break;
                    }
                    quality = 0;
                }
            return quality;
        }
    }
}