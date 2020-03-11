using ForkBro.Model.EventModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkBro.Model.Manager
{
    public struct EventPool
    {
        public long ID { get; set; }
        public Command CommandA  { get; set; }
        public Command CommandB { get; set; }
        public IBookmakerEvent[] Games;
        public bool[] CommandOrder { get; set; }

        //Игры имеют обновления
        public bool haveUpdates => Games.Count(x => x.status == EStatusEvent.Updated) > 0;
    }
    public class EventHub
    {
        List<EventPool> Pools;
        public long MaxId => Pools.Max(x=>x.ID);


        public EventHub()
        {
            Pools = new List<EventPool>();
        }


        public void IncomingEvent(BetEvent bet)
        {
            //Попробовать найти в существующих
                //Проверить в сканере букмекера
                    //Добавить если нет
            //Добавить новый пул
                //Отправить создание игры
                //Получить ссылку и добавить игру

        }

        //Pool CRUD
        public void AddPool(EventPool pool)
        {
            pool.ID = MaxId;
            Pools.Add(pool);
        }
        public void RemovePool(int idPool) => Pools.RemoveAll(x => x.ID == idPool);
        public EventPool GetPool(long id) => Pools.First(x => x.ID == id);
        //public void AddEvent(int idPool,IBookmakerEvent bEvent)=>Pools.First(x=>x.ID == idPool).Games





        //проверить BetEvent
        //Add
        //OverRemove
        //
    }
}
