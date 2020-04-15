using ForkBro.BookmakerModel;
using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.Common;
using ForkBro.Scanner.EventLinks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ForkBro.Common.BookmakerClient
{
    public class HttpRequest_favbet : BaseHttpRequest
    {
        public HttpRequest_favbet() { BM = Bookmaker._favbet; }
       
        public override ConcurrentDictionary<ushort, double[,]> GetDictionaryOdds(long eventId, Sport sport)
        {
            //stopwatch.Restart();
            Dictionary<ushort, List<double[]>> oddsDict = new Dictionary<ushort, List<double[]>>();

            //Получение данных с конторы
            var httpResult = PostAsync(@"https://www.favbet.com/frontend_api2/",
                           "{\"jsonrpc\":\"2.0\",\"method\":\"frontend/market/get\",\"id\":2379,\"params\":{\"by\":{\"lang\":\"ru\","
                           + "\"service_id\":1,\"event_id\":" + eventId.ToString() + "}}}",
                            "application/json"
                            ).Result;
            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestEvent_favbet>(httpResult);
            //Заполнение odds
            if (jsonResult.Result.Count > 0)
            {
                for (int i = 0; i < jsonResult.Result.Count; i++)
                {
                    ushort id;

                    int unitType = jsonResult.Result[i].ResultTypeId;
                    double.TryParse(jsonResult.Result[i].Outcomes[0].OutcomeParam, out double value);
                    double coefLeft = jsonResult.Result[i].Outcomes[0].OutcomeCoef;
                    double coefRight = jsonResult.Result[i].Outcomes[1].OutcomeCoef;
                    int betType = int.Parse(jsonResult.Result[i].MarketOrder.Substring(0, 3));

                    //Опрделение идентификатора
                    switch (betType)
                    {
                        case 002:// Победа с ОТ в матче (Баскетбол)
                        case 007:// Победа в матче (Теннис)
                        case 008:
                            id = (byte)BetType.Win;
                            break;
                        case 003://Фора (Баскетбол)
                        case 060:
                            id = (byte)BetType.Fora;
                            break;
                        case 004://Тотал (Баскетбол)
                        case 065:
                            id = (byte)BetType.Total;
                            break;
                        case 029:// Индивидуальный тотал (Баскетбол)
                            if (jsonResult.Result[i].Outcomes[0].ParticipantNumber == 1)
                                id = (byte)BetType.IndTotal_A;
                            else if (jsonResult.Result[i].Outcomes[0].ParticipantNumber == 2)
                                id = (byte)BetType.IndTotal_B;
                            else
                                continue;
                            break;
                        case 250:// Чёт / Нечет (Баскетбол)
                            id = (byte)BetType.TotalEven;
                            //Меняем местами Нечёт-Чёт, контора даёт обратный результат
                            double Odd = coefLeft;
                            coefLeft = coefRight;
                            coefRight = Odd;
                            break;

                        default: continue;
                    }

                    switch (unitType)
                    {
                        case 0:
                        case 1:
                            id += (((byte)EventUnit.MainTime) << 8);
                            break;
                        case 80:
                            id += (((byte)EventUnit.Set_1) << 8);
                            break;
                        case 61:
                        case 81:
                            id += (((byte)EventUnit.Set_2) << 8);
                            break;
                        case 62:
                        case 82:
                            id += (((byte)EventUnit.Set_3) << 8);
                            break;
                        case 63:
                        case 83:
                            id += (((byte)EventUnit.Set_4) << 8);
                            break;
                        case 84:
                            id += (((byte)EventUnit.Set_5) << 8);
                            break;
                        default: continue;
                    }
                    //Создание нового списка
                    if (!oddsDict.ContainsKey(id))
                        oddsDict.Add(id, new List<double[]>());

                    for (int n = 0; n < oddsDict[id].Count; n++)
                        if (oddsDict[id][n][0] == value)
                        {
                            oddsDict[id][n][1] = coefLeft;
                            oddsDict[id][n][2] = coefRight;
                            break;
                        }

                    //Добавление параметров
                    oddsDict[id].Add(new double[3] {
                            value,
                            coefLeft,
                            coefRight
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
            //File.AppendAllText($"Log\\stopwatch\\fav_{eventId}.log", $"---{stopwatch.ElapsedMilliseconds}({stopwatch.Elapsed})---" + "\r\n");
            return resultArray;
        }

        public override IGameList GetEventsList()
        {
            string httpResult = PostAsync(@"https://www.favbet.com/frontend_api/events_short/",
                                           "{\"lang\":\"en\",\"service_id\":1}"
                                           , "application/json"
                                           ).Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_favbet>(httpResult);

        }

        public override string GetBetOdds(long eventId)
        {
            throw new NotImplementedException();
        }
    }
}