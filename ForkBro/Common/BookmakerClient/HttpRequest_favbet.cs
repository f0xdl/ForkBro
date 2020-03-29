using ForkBro.BookmakerModel;
using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.Common;
using ForkBro.Scanner.EventLinks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ForkBro.Common.BookmakerClient
{
    public class HttpRequest_favbet : BaseHttpRequest
    {
        public HttpRequest_favbet() { BM = Bookmaker._favbet; }
        public override List<IEventLink> GetListEvent()
        {
            List<IEventLink> events = new List<IEventLink>();
            
            var httpResult = PostAsync(@"https://www.favbet.com/frontend_api/events_short/", 
                                        "{\"lang\":\"en\",\"service_id\":1}"
                                        , "application/json"
                                        ).Result;
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_favbet>(httpResult);
            
            if (jsonData.Success)
                foreach (var item in jsonData.events)
                    if (item.Sport != Sport.None && item.Status != StatusEvent.Over)
                        events.Add(item);
            return events;
        }
        
        public override ConcurrentDictionary<OldBetType, BettingOdds[]> GetDictionaryOdds(long eventId, Sport sport)
        {
            Dictionary<OldBetType, List<BettingOdds>> tempList = new Dictionary<OldBetType, List<BettingOdds>>();

            //Получение данных с конторы
            var httpResult = PostAsync(@"https://www.favbet.com/frontend_api2/",
                           "{\"jsonrpc\":\"2.0\",\"method\":\"frontend/market/get\",\"id\":2379,\"params\":{\"by\":{\"lang\":\"ru\","
                           + "\"service_id\":1,\"event_id\":" + eventId.ToString() + "}}}",
                            "application/json"
                            ).Result;
            
            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestEvent_favbet>(httpResult);

            //Создание пустых списков
            foreach (OldBetType key in Enum.GetValues(typeof(OldBetType)))
                tempList.Add(key, new List<BettingOdds>());

            //Заполнение odds
            if (jsonResult.Result.Count>0)
            //for (int i = 0; i < jsonResult.Value.E.Count; i++)
            {
                //TODO
                File.WriteAllText($"Logs\\Odds\\{sport}_{eventId}_{DateTime.Now.ToString("HH-mm-ss_fff")}.json", Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult, Newtonsoft.Json.Formatting.Indented));//DEBUG
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
                
                
            //Transfer to Array
            ConcurrentDictionary<OldBetType, BettingOdds[]> resultArray = new ConcurrentDictionary<OldBetType, BettingOdds[]>();
            foreach (var item in tempList)
                if (item.Value.Count > 0)
                    resultArray.TryAdd(item.Key, item.Value.ToArray());

            return resultArray;
        }
    }
}