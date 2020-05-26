using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.Common;
using ForkBro.Common.BookmakerClient;
using ForkBro.Configuration;
using ForkBro.Mediator;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ForkBro.BookmakerModel
{
    public class ServiceFavbet : ServiceBookmaker
    {
        public ServiceFavbet(ILogger<ServiceBookmaker> logger, IOptions<AppSettings> setting, IBookmakerMediator mediator)
            : base(logger, setting, mediator, Bookmaker._favbet)
        {
        }
        protected override void UpdateEvents() => Parallel.ForEach(Events, UpdateEvent);

        private void UpdateEvent(BetEvent obj)
        {
            var jsonObj = HttpRequest.GetBetOdds<RequestEvent_favbet>(obj.EventId);

            if (jsonObj.Result.Count == 0)
                return;

            Dictionary<ushort, List<double[]>> oddsDict = new Dictionary<ushort, List<double[]>>();

            //Выборка данных из jsonObj
            foreach (BaseEvent_favbet reqEvent in jsonObj.Result)
            {
                ushort id;

                int unitType = reqEvent.ResultTypeId;
                double.TryParse(reqEvent.Outcomes[0].OutcomeParam, out double value);
                double coefLeft = reqEvent.Outcomes[0].OutcomeCoef;
                double coefRight = reqEvent.Outcomes[1].OutcomeCoef;
                int betType = int.Parse(reqEvent.MarketOrder.Substring(0, 3));

                //Опрделение идентификатора
                switch (betType)
                {
                    case 002: // Победа с ОТ в матче (Баскетбол)
                    case 007: // Победа в матче (Теннис)
                    case 008:
                        id = (byte) BetType.Win;
                        break;
                    case 003: //Фора (Баскетбол)
                    case 060:
                        id = (byte) BetType.Fora;
                        break;
                    case 004: //Тотал (Баскетбол)
                    case 065:
                        id = (byte) BetType.Total;
                        break;
                    case 029: // Индивидуальный тотал (Баскетбол)
                        if (reqEvent.Outcomes[0].ParticipantNumber == 1)
                            id = (byte) BetType.IndTotal_A;
                        else if (reqEvent.Outcomes[0].ParticipantNumber == 2)
                            id = (byte) BetType.IndTotal_B;
                        else
                            continue;
                        break;
                    case 250: // Чёт / Нечет (Баскетбол)
                        id = (byte) BetType.TotalEven;
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
                        id += (((byte) EventUnit.MainTime) << 8);
                        break;
                    case 80:
                        id += (((byte) EventUnit.Set_1) << 8);
                        break;
                    case 61:
                    case 81:
                        id += (((byte) EventUnit.Set_2) << 8);
                        break;
                    case 62:
                    case 82:
                        id += (((byte) EventUnit.Set_3) << 8);
                        break;
                    case 63:
                    case 83:
                        id += (((byte) EventUnit.Set_4) << 8);
                        break;
                    case 84:
                        id += (((byte) EventUnit.Set_5) << 8);
                        break;
                    default: continue;
                }

                //Создание нового списка
                if (!oddsDict.ContainsKey(id))
                    oddsDict.Add(id, new List<double[]>());

                for (int n = 0; n < oddsDict[id].Count; n++)
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (oddsDict[id][n][0] == value)
                    {
                        oddsDict[id][n][1] = coefLeft;
                        oddsDict[id][n][2] = coefRight;
                        break;
                    }

                //Добавление параметров
                oddsDict[id].Add(new double[3]
                {
                    value,
                    coefLeft,
                    coefRight
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