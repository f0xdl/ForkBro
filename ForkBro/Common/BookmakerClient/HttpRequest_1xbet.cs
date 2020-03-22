using ForkBro.Common;
using System.Collections.Generic;
using System.IO;
using ForkBro.Scanner.EventLinks;

namespace ForkBro.Common.BookmakerClient
{
    public class HttpRequest_1xbet : BaseHttpRequest
    {
        public override List<IEventLink> GetListEvent()
        {
            List<IEventLink> events = new List<IEventLink>();
            //sports=3&
            var httpResult = GetAsync(@"https://xparibet.com/LiveFeed/Get1x2_VZip", "count=500&mode=8").Result;
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_1xBet>(httpResult);
            //DEBUG
            //File.WriteAllText(this.GetType().Name +".json", httpResult);
            if (jsonData.Success)
            {
                foreach (var item in jsonData.events)
                {
                    IEventLink betEvent = item;
                    betEvent.Bookmaker = Bookmaker._1xbet;

                    //Добавить событие только если оно активно и выбран спорт
                    if (betEvent.Sport != Sport.None && betEvent.Status != StatusEvent.Over)
                        events.Add(betEvent);
                }
            }
            return events;
        }
    }
}