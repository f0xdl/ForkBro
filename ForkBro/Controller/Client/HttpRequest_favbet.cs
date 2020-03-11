using ForkBro.Controller.Event.BetEvents;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using System.Collections.Generic;
using System.IO;

namespace ForkBro.Controller.Client
{
    public class HttpRequest_favbet : BaseHttpRequest
    {
        public override List<EventPool> GetListEvent()
        {
            List<EventPool> events = new List<EventPool>();
            var httpResult = PostAsync(@"https://www.favbet.com/frontend_api/events_short/", 
                                        "{\"lang\":\"en\",\"service_id\":1}"
                                        , "application/json"
                                        ).Result;
            //DEBUG
            File.WriteAllText(this.GetType().Name +".json", httpResult);

            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_favbet>(httpResult);
            if (jsonData.Success)
            {
                foreach (var item in jsonData.events)
                {
                    EventPool betEvent = item.ConvertToBetEvent();
                    betEvent.bookmaker = EBookmakers._favbet;

                    //Добавить событие только если оно активно и выбран спорт
                    if (betEvent.sport != ESport.None && betEvent.status != EStatusEvent.Over)
                        events.Add(betEvent);
                }
            }
            return events;
        }
    }
}