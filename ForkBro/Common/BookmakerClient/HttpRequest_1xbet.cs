using ForkBro.Common;
using System.Collections.Generic;
using System.IO;
using ForkBro.Scanner.EventLinks;
using ForkBro.BookmakerModel;
using System;
using System.Collections.Concurrent;
using ForkBro.BookmakerModel.BaseEvents;
using System.Linq;

namespace ForkBro.Common.BookmakerClient
{
    public class HttpRequest_1xbet : BaseHttpRequest
    {
        public HttpRequest_1xbet() => BM = Bookmaker._1xbet; 

        public override List<IEventLink> GetListEvent()
        {
            List<IEventLink> events = new List<IEventLink>();
            
            var httpResult = GetAsync(@"https://xparibet.com/LiveFeed/Get1x2_VZip", "count=500&mode=8").Result;
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_1xBet>(httpResult);
            
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
            string httpResult = GetAsync(@"https://xparibet.com/LiveFeed/GetGameZip", $"id={eventId.ToString()}&lng=en&isSubGames=true&allEventsGroupSubGames=true&grMode=1").Result;
            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestEvent_1xbet>(httpResult);

            //Создание пустых списков
            foreach (OldBetType key in Enum.GetValues(typeof(OldBetType)))
                tempList.Add(key, new List<BettingOdds>());
            //Заполнение odds
            if (jsonResult.Success)
            { 
                for (int i = 0; i < jsonResult.Value.E.Count; i++)
                    switch (jsonResult.Value.E[i].T)
                    {
                        //Win (1,X,2)
                        case 1:
                            tempList[OldBetType.Win].Add(new BettingOdds()
                            {
                                Group = BetGroup.Cmd_A,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;
                        case 2:
                            tempList[OldBetType.Win].Add(new BettingOdds()
                            {
                                Group = BetGroup.Draw,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;
                        case 3:
                            tempList[OldBetType.Win].Add(new BettingOdds()
                            {
                                Group = BetGroup.Cmd_B,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;

                        //Fora
                        case 7:
                            tempList[OldBetType.Fora].Add(new BettingOdds()
                            {
                                Group = BetGroup.Cmd_A,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;
                        case 8:
                            tempList[OldBetType.Fora].Add(new BettingOdds()
                            {
                                Group = BetGroup.Cmd_B,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;

                        //Total
                        case 9:
                            tempList[OldBetType.Total].Add(new BettingOdds()
                            {
                                Group = BetGroup.Over,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;
                        case 10:
                            tempList[OldBetType.Total].Add(new BettingOdds()
                            {
                                Group = BetGroup.Under,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;

                        //IndTotal1
                        case 11:
                            tempList[OldBetType.IndTotal1].Add(new BettingOdds()
                            {
                                Group = BetGroup.Over,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;
                        case 12:
                            tempList[OldBetType.IndTotal1].Add(new BettingOdds()
                            {
                                Group = BetGroup.Under,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;

                        //IndTotal12
                        case 13:
                            tempList[OldBetType.IndTotal2].Add(new BettingOdds()
                            {
                                Group = BetGroup.Over,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;
                        case 14:
                            tempList[OldBetType.IndTotal2].Add(new BettingOdds()
                            {
                                Group = BetGroup.Under,
                                Value = jsonResult.Value.E[i].P,
                                Coef = jsonResult.Value.E[i].C
                            });
                            break;

                        default: continue;
                    }
            }

            //Transfer to Array
            ConcurrentDictionary<OldBetType, BettingOdds[]> resultArray = new ConcurrentDictionary<OldBetType, BettingOdds[]>();
            foreach (var item in tempList)
                            if(item.Value.Count>0)
                                resultArray.TryAdd(item.Key, item.Value.ToArray());

            return resultArray;
        }

    }
}