using ForkBro.Model.EventModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkBro.Model.Manager
{
    //public struct EventPool
    //{
    //    public long ID;
    //    public Command CommandA;
    //    public Command CommandB;
    //    public List<BookmakerEvent> events;
    //    //Игры имеют обновления
    //    public bool HaveUpdates => events.Count(x => x.status == EStatusEvent.Updated) > 0;
    //}
    public class EventHub
    {
        Dictionary<long,EventPool> Pools;
        EBookmakers[] bookmakers;

        public long MaxIdInPool => Pools.Count>0?Pools.Max(x=>x.Key):1;

        public EventHub(EBookmakers[] BMs)
        {
            Pools = new Dictionary<long, EventPool>();
            bookmakers = BMs;
        }

        //Pools method
        long AddPool(BookmakerEvent bmEvent)
        {
            if (EventExists(bmEvent))
            {
                EventPool poolEvent = new EventPool();
                poolEvent.id = MaxIdInPool;

                Pools.Add(poolEvent.id, poolEvent);
                AddEvent(poolEvent.id, bmEvent);
                return poolEvent.id;
            }
            else return 0;
        }
        void RemovePool(long poolID) => Pools.Remove(poolID);
        public EventPool GetPool(long poolID) => Pools[poolID];

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



        //Добавить ивент для БМ
        //Удалить ивент для БМ
        //Проверить ивент для БМ


        public void IncomingEvent(EventModel.EventPool bet)
        {
            //Попробовать найти в существующих
            //Проверить в сканере букмекера
            //Добавить если нет
            //Добавить новый пул
            //Отправить создание игры
            //Получить ссылку и добавить игру
            throw new System.NotImplementedException();

        }



        //проверить BetEvent
        //Add
        //OverRemove
        //
    }
}
