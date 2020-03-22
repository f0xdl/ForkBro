using ForkBro.Common;
using ForkBro.Scanner.EventLinks;
using System.Collections.Generic;
using System.IO;

namespace ForkBro.Common.BookmakerClient
{
    public class HttpRequest_favbet : BaseHttpRequest
    {
        public override List<IEventLink> GetListEvent()
        {
            List<IEventLink> events = new List<IEventLink>();
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
                    IEventLink betEvent = item;
                    betEvent.Bookmaker = Bookmaker._favbet;

                    //Добавить событие только если оно активно и выбран спорт
                    if (betEvent.Sport != Sport.None && betEvent.Status != StatusEvent.Over)
                        events.Add(betEvent);
                }
            }
            return events;
        }
    }
}