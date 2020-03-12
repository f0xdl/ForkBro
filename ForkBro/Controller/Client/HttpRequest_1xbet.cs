using ForkBro.Controller.Event.BetEvents;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using System.Collections.Generic;
using System.IO;

namespace ForkBro.Controller.Client
{
    public class HttpRequest_1xbet : BaseHttpRequest
    {
        public override List<BetEvent> GetListEvent()
        {
            List<BetEvent> events = new List<BetEvent>();
            //sports=3&
            var httpResult = GetAsync(@"https://xparibet.com/LiveFeed/Get1x2_VZip", "count=500&mode=8").Result;
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_1xBet>(httpResult);
            //DEBUG
            //File.WriteAllText(this.GetType().Name +".json", httpResult);
            if (jsonData.Success)
            {
                foreach (var item in jsonData.events)
                {
                    BetEvent betEvent = item.ConvertToBetEvent();
                    betEvent.bookmaker = Bookmaker._1xbet;

                    //Добавить событие только если оно активно и выбран спорт
                    if (betEvent.sport != Sport.None && betEvent.status != StatusEvent.Over)
                        events.Add(betEvent);
                }
            }
            return events;
        }
    }
}