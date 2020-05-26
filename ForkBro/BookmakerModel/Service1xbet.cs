using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.BookmakerModel.BaseEvents.Events1xBet;
using ForkBro.Common;
using ForkBro.Configuration;
using ForkBro.Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ForkBro.BookmakerModel
{
    public class Service1xBet : ServiceBookmaker
    {
        public Service1xBet(ILogger<ServiceBookmaker> logger, IOptions<AppSettings> setting, IBookmakerMediator mediator) 
            : base(logger, setting, mediator, Bookmaker._1xbet)
        {
        }

        protected override void UpdateEvents() => Parallel.ForEach(Events, UpdateEvent);
        
        void UpdateEvent(BetEvent obj)
        {
            var jsonObj = HttpRequest.GetBetOdds<RequestEvent_1xBet>(obj.EventId);
            
            if(jsonObj.Value.Ge.Length == 0) return;
            
            Dictionary<ushort, List<double[]>> oddsDict = new Dictionary<ushort, List<double[]>>();

            foreach (var item in jsonObj.Value.E)
            {
                int type = item.T;
                double value = item.P;
                double coef = item.C;
                ushort id;
                bool isLeft;
                bool isFora = false;
                
                //Определение идентификатора
                switch (type)
                {
                    case 1:
                    case 3:
                        id = (byte) BetType.Win + (((byte) EventUnit.MainTime) << 8);
                        break;
                    case 7:
                    case 8:
                        id = (byte) BetType.Fora + (((byte) EventUnit.MainTime) << 8);
                        isFora = true;
                        break;
                    case 9:
                    case 10:
                        id = (byte) BetType.Total + (((byte) EventUnit.MainTime) << 8);
                        break;
                    case 11:
                    case 12:
                        id = (byte) BetType.IndTotal_A + (((byte) EventUnit.MainTime) << 8);
                        break;
                    case 13:
                    case 14:
                        id = (byte) BetType.IndTotal_B + (((byte) EventUnit.MainTime) << 8);
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
                    oddsDict.Add(id, new List<double[]>());

                bool found = false;
                for (int n = 0; n < oddsDict[id].Count; n++)
                    if (!isFora)
                        if (oddsDict[id][n][0] == value)
                        {
                            if (isLeft)
                                oddsDict[id][n][1] = coef;
                            else
                                oddsDict[id][n][2] = coef;
                            found = true;
                            break;
                        }
                        else if (oddsDict[id][n][0] == (-1 * value))
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
                        oddsDict[id].Add(new double[3]
                        {
                            value,
                            coef,
                            0
                        });
                    else
                        oddsDict[id].Add(new double[3]
                        {
                            value,
                            0,
                            coef
                        });

            }

            
            //Переобразование в конкурентный словарь
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
            
            //Замена словаря в ивенте 
            obj.ReplaceAllOdds(resultArray);
            
            
        }
    }
}