using ForkBro.Model.EventModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkBro.Model.Manager
{
    public struct EventPool
    {
        public int ID { get; set; }
        public Command CommandA  { get; set; }
        public Command CommandB { get; set; }
        public IBookmakerEvent[] Games { get; set; }
        public bool[] CommandOrder { get; set; }

        //Игры имеют обновления
        public bool haveUpdates => Games.Count(x => x.status == EStatusEvent.Updated) > 0;
        
    }
    public class EventHub
    {
        EventPool[] Pools;

        public EventHub(int countEvent = 100)
        {
            Pools = new EventPool[countEvent];
        }


        public void IncomingEvent(BetEvent bet)
        {

        }
            //проверить BetEvent
            //Add
            //OverRemove
            //
    }
}
