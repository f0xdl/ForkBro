using FrokBro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkBro.Model
{
    public class EventHub
    {
        Dictionary<long, Pool> Points;
        Bookmaker[] bookmakers;

        public long MaxIdInPool => Points.Count>0?Points.Max(x=>x.Key):1;

        public EventHub(Bookmaker[] BMs)
        {
            Points = new Dictionary<long, Pool>();
            bookmakers = BMs;
        }

        //Points method
        Pool GetNewPool()
        {
            Pool poolEvent = new Pool();
            poolEvent.id = MaxIdInPool;

            Points.Add(poolEvent.id, poolEvent);
            return poolEvent;
        }
        void RemovePool(long poolID) => Points.Remove(poolID);
        public Pool ElementAt(long poolID) => Points[poolID];




        public void AddEvent(long poolID, BookmakerEvent pool)
        {
            throw new System.NotImplementedException();

        }

        public bool EventExists(BookmakerEvent bmEvent)
        {
            //bmEvent.bookmaker
            //    bmEvent.EventID
            throw new System.NotImplementedException();
        }
        
        public void IncomingEvent(Pool bet)
        {
            //Попробовать найти в существующих
            //Проверить в сканере букмекера
            //Добавить если нет
            //Добавить новый пул
            //Отправить создание игры
            //Получить ссылку и добавить игру
            throw new System.NotImplementedException();

        }

    }
}
