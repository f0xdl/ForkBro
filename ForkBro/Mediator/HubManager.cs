using ForkBro.Bookmakers;
using ForkBro.Common;
using ForkBro.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForkBro.Mediator
{
    public class HubManager
    {
        PoolRaw[] pool;
        public HubManager(int count)
        {
            pool = new PoolRaw[count];
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
        public bool HasEventInPool(Bookmaker bookmaker, long id) => pool.Count(x => x.ExistsEvent(bookmaker, id) ?? false) > 1;
        
        public void AddSnapshot(int idPool, ref BookmakerEvent bookmakerEvent) => pool[idPool].AddSnapshot(ref bookmakerEvent);
        public void RemoveSnapsot(int idPool, Bookmaker bookmaker) => pool[idPool].RemoveSnapshot(bookmaker);
        public void UpdateSnapshot(int idPool, ref BookmakerEvent bookmakerEvent) => pool[idPool].UpdateSnapshot(ref bookmakerEvent);
        public PoolRaw GetNextSnapshots()
        {
            int idPool = pool.Max(x => x.CountUpdate);
            pool[idPool].GetUpdates();
            return pool[idPool];
        }
        
        /// <summary>
        /// Return -1 if Hub Don
        /// </summary>
        /// <param name="sport"></param>
        /// <param name="CommandA"></param>
        /// <param name="CommandB"></param>
        /// <returns></returns>
        public int? FindEvent(Sport sport, Command CommandA, Command CommandB)
        {

            throw new NotImplementedException();
        }
    }
}