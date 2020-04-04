using ForkBro.BookmakerModel;
using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.Common;
using ForkBro.Mediator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Daemons
{
    public class CalcDaemon : IWorker
    {
        const int _tblRngMax = 40; //Summary Table Range Max
        long index;
        IDaemonMasterMediator hub;
        ILogger _logger;

        public bool IsAlive => ((IWorker)this).thread.IsAlive;

        public CalcDaemon(ILogger logger, IDaemonMasterMediator hub,int id)
        {
            this.hub = hub;
            _logger = logger;

            index = id;
            Name = "Daemon-" + index;
        }

        public string Name { get; private set; }

        void CalcFunc(Sport sport, EventUnit unit, BetType type, Dictionary<Bookmaker,CalcOdds> structOdds)
        {
            int sumTableIdx = 0;
            SummaryOdds[] sumTable = new SummaryOdds[_tblRngMax];// 6 колонок и до ~40 строк
            List<Fork> forks = new List<Fork>();

            ///Odds Struct
            /// 0 |   1  |   2  |
            ///Val|Coef L|Coef R|
            
            ///Summary Table Struct
            /// 0 | 1| 2| 3| 4
            ///Val|B1|C1|B2|C2
            ///

            //Заполнение суммирующей таблицы
            foreach (CalcOdds sItem in structOdds.Values) //Структура
            {
                double[,] odds = sItem.Values;

                // дублирование для оптимизации производительности, вынос опроса реверсивного массива вне цикла
                if (sItem.Reverse)
                {
                    for (int sId = 0; sId < odds.GetLength(0); sId++)// массив ставок
                    {
                        bool searched = false;  //Статус обработки строки

                        for (int i = 0; i < sumTableIdx; i++) // Проход по всем существущим строкам
                            if (odds[sId, 0] == sumTable[i].Value) //Если значение совпадает 38.5=38.5
                            {
                                //Если коэффициент больше текущего 1.78 > 1.34 
                                if (odds[sId, 1] > sumTable[i].Coef_B) //Right - Reverse
                                {
                                    sumTable[i].Bookmaker_B = sItem.Bookmaker;
                                    sumTable[i].Coef_B = odds[sId, 1];
                                }

                                if (odds[sId, 2] > sumTable[i].Coef_A) //Left - Reverse
                                {
                                    sumTable[i].Bookmaker_A = sItem.Bookmaker;
                                    sumTable[i].Coef_A = odds[sId, 2];
                                }

                                searched = true;
                                break;
                            }

                        if (!searched)  //Добавление ставки при отсутствии
                        {
                            sumTable[sumTableIdx].Value = odds[sId, 0]; // Value
                            sumTable[sumTableIdx].Coef_B = odds[sId, 1]; // Coef Right - Reverse
                            sumTable[sumTableIdx].Coef_A = odds[sId, 2]; // Coef Left - Reverse
                            sumTable[sumTableIdx].Bookmaker_A = sumTable[sumTableIdx].Bookmaker_B = sItem.Bookmaker; // Bookmaker
                            sumTableIdx++;
                        }
                    }
                }
                else
                {
                    for (int sId = 0; sId < odds.GetLength(0); sId++)// массив ставок
                    {
                        bool searched = false;  //Статус обработки строки

                        for (int i = 0; i < sumTableIdx; i++) // Проход по всем существущим строкам
                            if (odds[sId, 0] == sumTable[i].Value) //Если значение совпадает 38.5=38.5
                            {
                                //Если коэффициент больше текущего 1.78 > 1.34 
                                if (odds[sId, 1] > sumTable[i].Coef_A) //Left - Normal
                                {
                                    sumTable[i].Bookmaker_A = sItem.Bookmaker;
                                    sumTable[i].Coef_A = odds[sId, 1];
                                }

                                if (odds[sId, 2] > sumTable[i].Coef_B) //Right - Normal
                                {
                                    sumTable[i].Bookmaker_B = sItem.Bookmaker;
                                    sumTable[i].Coef_B = odds[sId, 2];
                                }

                                searched = true;
                                break ;
                            }

                        if (!searched)  //Добавление ставки при отсутствии
                        {
                            sumTable[sumTableIdx].Value = odds[sId, 0]; // Value
                            sumTable[sumTableIdx].Coef_A = odds[sId, 1]; // Coef Left - Normal
                            sumTable[sumTableIdx].Coef_B = odds[sId, 2]; // Coef Right - Normal
                            sumTable[sumTableIdx].Bookmaker_A = sumTable[sumTableIdx].Bookmaker_B = sItem.Bookmaker; // Bookmaker
                            sumTableIdx++;
                        }
                    }
                }
                
            }

            //Добавление найденных вилок в массив
            for (int i = 0; i < sumTableIdx; i++)
                if (1 / sumTable[i].Coef_A + 1 / sumTable[i].Coef_B < 1) // if percent < 1
                {
                    Fork fork = new Fork()
                    {
                        Timestamp = DateTime.UtcNow.ToFileTimeUtc(),
                        Sport = sport,
                        Unit = unit,
                        Type = type,
                        Percent = 1 - (1 / sumTable[i].Coef_A + 1 / sumTable[i].Coef_B), //Расчёт процента ставки

                        BookmakerA = sumTable[i].Bookmaker_A,
                        ValueA = sumTable[i].Value,
                        CoefficientA = sumTable[i].Coef_A,

                        BookmakerB = sumTable[i].Bookmaker_B,
                        ValueB = sumTable[i].Value,
                        CoefficientB = sumTable[i].Coef_B,
                    };
                    fork.IdEventA = structOdds[fork.BookmakerA].IdEvent;
                    fork.ReverseA = structOdds[fork.BookmakerA].Reverse;

                    fork.IdEventB = structOdds[fork.BookmakerB].IdEvent;
                    fork.ReverseB = structOdds[fork.BookmakerB].Reverse;
                    forks.Add(fork);
                }
            //Отправка массива вилок
            if (forks.Count>0)
                hub.AddFork(forks);
        }


        #region IWorker
        bool IWorker.IsWork { get; set; }
        Thread IWorker.thread { get; set; }

        public void Start(int updatePeriod)
        {
            if (hub == null)
                throw new ArgumentNullException($"Hub in '{Name}' do not connect");

            ((IWorker)this).StartWork(updatePeriod);
        }
        public void Stop(int ms_wait) => ((IWorker)this).StopWork(ms_wait);
        async void IWorker.Work(object args)
        {
            int delay = (int)args;

            while (((IWorker)this).IsWork)
                try
                {
                    var pool = hub.GetNextPool();
                    if (pool != null)
                    {
                        BetEvent[] betEvents = pool.GetAllSnapshot();
                        //Перебор всех комбинаций UnitKey
                        foreach (BetType type in Enum.GetValues(typeof(BetType)))
                            foreach (EventUnit unit in Enum.GetValues(typeof(EventUnit)))
                            {
                                //Создание и заполнение  odds
                                var oddsList = new Dictionary<Bookmaker,CalcOdds>();
                                for (int i = 0; i < betEvents.Length; i++)
                                    if (betEvents[i].AllOdds.TryGetValue((ushort)((byte)type + (((byte)unit) << 8)), out double[,] resultOdds))
                                    {
                                        var odds = new CalcOdds();
                                        odds.Reverse = betEvents[i].Reverse;
                                        odds.IdEvent = betEvents[i].EventId;
                                        odds.Values = resultOdds;
                                        odds.Bookmaker = betEvents[i].Bookmaker;
                                        oddsList.Add(betEvents[i].Bookmaker, odds);
                                    }

                                //Запуск расчёта odds
                                if(oddsList.Count>1)
                                    await Task.Run(() => CalcFunc(betEvents[0].Sport,unit, type, oddsList));
                            }
                    }
                    else //Пауза при отсутствии обновлений
                        Thread.Sleep(delay);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
        }
        #endregion
    }
}
//https://docs.microsoft.com/ru-ru/dotnet/standard/parallel-programming/task-based-asynchronous-programming
//Task.Factory.StartNew(