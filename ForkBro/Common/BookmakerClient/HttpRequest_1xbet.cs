using ForkBro.Common;
using System.Collections.Generic;
using System.IO;
using ForkBro.Scanner.EventLinks;
using ForkBro.BookmakerModel;
using System;
using System.Collections.Concurrent;
using ForkBro.BookmakerModel.BaseEvents;
using System.Linq;
using System.Diagnostics;

namespace ForkBro.Common.BookmakerClient
{
    public class HttpRequest_1xbet : BaseHttpRequest
    {
        public HttpRequest_1xbet() => BM = Bookmaker._1xbet;
        public override ConcurrentDictionary<ushort, double[,]> GetDictionaryOdds(long eventId, Sport sport)
        {
            //stopwatch.Restart();
            Dictionary<ushort, List<double[]>> oddsDict = new Dictionary<ushort, List<double[]>>();

            //Получение данных с конторы
            string httpResult = GetAsync(@"https://xparibet.com/LiveFeed/GetGameZip", $"id={eventId.ToString()}&lng=en&isSubGames=true&allEventsGroupSubGames=true&grMode=1").Result;
            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestEvent_1xbet>(httpResult);
            //Заполнение odds
            if (jsonResult.Success)
            {
                for (int i = 0; i < jsonResult.Value.E.Count; i++)
                {
                    int type = jsonResult.Value.E[i].T;
                    double value = jsonResult.Value.E[i].P;
                    double coef = jsonResult.Value.E[i].C;
                    ushort id;
                    bool isLeft;
                    bool isFora = false;
                    //Опрделение идентификатора
                    switch (type)
                    {
                        case 1:
                        case 3:
                            id = (byte)BetType.Win + (((byte)EventUnit.MainTime) << 8);
                            break;
                        case 7:
                        case 8:
                            id = (byte)BetType.Fora + (((byte)EventUnit.MainTime) << 8);
                            isFora = true;
                            break;
                        case 9:
                        case 10:
                            id = (byte)BetType.Total + (((byte)EventUnit.MainTime) << 8);
                            break;
                        case 11:
                        case 12:
                            id = (byte)BetType.IndTotal_A + (((byte)EventUnit.MainTime) << 8);
                            break;
                        case 13:
                        case 14:
                            id = (byte)BetType.IndTotal_B + (((byte)EventUnit.MainTime) << 8);
                            break;
                        default: continue;
                    }

                    //Опрделение идентификатора
                    switch (type)
                    {
                        case 1:
                        case 7:
                        case 9:
                        case 11:
                        case 13:
                            isLeft = true;
                            break;
                        case 3:
                        case 8:
                        case 10:
                        case 12:
                        case 14:
                            isLeft = false;
                            break;
                        default: continue;
                    }
                    //Создание нового списка
                    if (!oddsDict.ContainsKey(id))
                        oddsDict.Add(id,new List<double[]>());
                    
                    bool found = false;
                    for (int n = 0; n < oddsDict[id].Count; n++)
                        if(!isFora)
                            if (oddsDict[id][n][0] == value)
                            {
                                if (isLeft)
                                    oddsDict[id][n][1] = coef;
                                else
                                    oddsDict[id][n][2] = coef;
                                found = true;
                                break;
                            }
                    else
                        if (oddsDict[id][n][0] == (-1*value))
                            {
                                if (isLeft)
                                    oddsDict[id][n][1] = coef;
                                else
                                    oddsDict[id][n][2] = coef;
                                found = true;
                                break;
                            }


                    //Добавление параметров
                    if (!found)
                        if (isLeft)
                            oddsDict[id].Add(new double[3] { 
                                value, 
                                coef, 
                                0 
                            });
                        else
                            oddsDict[id].Add(new double[3] {
                                value,
                                0,
                                coef
                            });

                }

            }

            //Transfer to Array
            ConcurrentDictionary<ushort, double[,]> resultArray = new ConcurrentDictionary<ushort, double[,]>();
            foreach (var oddsType in oddsDict)
            {
                double[,] odds = new double[oddsType.Value.Count, 3];
                for (int i = 0; i < oddsType.Value.Count; i++)
                {
                    odds[i, 0] = oddsType.Value[i][0];
                    odds[i, 1] = oddsType.Value[i][1];
                    odds[i, 2] = oddsType.Value[i][2];
                }
                resultArray.TryAdd(oddsType.Key, odds);
            }


            //stopwatch.Stop();
            //File.AppendAllText($"Logs\\stopwatch\\fav_{eventId}.log", $"---{stopwatch.ElapsedMilliseconds}({stopwatch.Elapsed})---" + "\r\n");
            return resultArray;
        }
        public override IGameList GetEventsList()
        {
            string httpResult = GetAsync(@"https://1xbet.com/LiveFeed/Get1x2_VZip", "count=500&mode=8").Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_1xBet>(httpResult);

        }
        public override string GetBetOdds(long eventId) 
            => GetAsync(@"https://1xbet.com/LiveFeed/GetGameZip", $"id={eventId.ToString()}&lng=en&isSubGames=true&allEventsGroupSubGames=true&grMode=1").Result;

    }
}