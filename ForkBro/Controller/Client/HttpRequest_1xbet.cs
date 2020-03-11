using ForkBro.Controller.Event.BetEvents;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using System.Collections.Generic;
using System.IO;

namespace ForkBro.Controller.Client
{
    public class HttpRequest_1xbet : BaseHttpRequest
    {
        public override List<EventPool> GetListEvent()
        {
            List<EventPool> events = new List<EventPool>();
            //sports=3&
            var httpResult = GetAsync(@"https://xparibet.com/LiveFeed/Get1x2_VZip", "count=500&mode=8").Result;
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_1xBet>(httpResult);
            //DEBUG
            //File.WriteAllText(this.GetType().Name +".json", httpResult);
            if (jsonData.Success)
            {
                foreach (var item in jsonData.events)
                {
                    EventPool betEvent = item.ConvertToBetEvent();
                    betEvent.bookmaker = EBookmakers._1xbet;

                    //Добавить событие только если оно активно и выбран спорт
                    if (betEvent.sport != ESport.None && betEvent.status != EStatusEvent.Over)
                        events.Add(betEvent);
                }
            }
            return events;
        }
    }
}