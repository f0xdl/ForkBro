using ForkBro.Controller.Event.BetEvents;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using System.Collections.Generic;
using System.IO;

namespace ForkBro.Controller.Client
{
    public class HttpRequest_favbet : BaseHttpRequest
    {
        public override List<BetEvent> GetListEvent()
        {
            List<BetEvent> events = new List<BetEvent>();
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
                    BetEvent betEvent = item.ConvertToBetEvent();
                    betEvent.bookmaker = Bookmaker._favbet;

                    //Добавить событие только если оно активно и выбран спорт
                    if (betEvent.sport != Sport.None && betEvent.status != StatusEvent.Over)
                        events.Add(betEvent);
                }
            }
            return events;
        }
    }
}