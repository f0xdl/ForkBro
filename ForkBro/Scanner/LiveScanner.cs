using ForkBro.Common;
using ForkBro.Common.BookmakerClient;
using ForkBro.Configuration;
using ForkBro.Mediator;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Scanner
{
    public class LiveScanner : BackgroundService
    {
        readonly IScannerMediator hub;
        readonly Bookmaker[] bookmakers;
        readonly Sport[] sports;
        readonly ILogger<LiveScanner> _logger;
        readonly int delay;

        Dictionary<Bookmaker, List<IEventLink>> events;
        BaseHttpRequest[] httpClients;


        public LiveScanner(ILogger<LiveScanner> logger, IScannerMediator mediator, ISetting setting)
        {
            delay = setting.LiveScanRepeat;
            sports = setting.TrackedSports;
            bookmakers = setting.GetEBookmakers();

            hub = mediator;
            _logger = logger;

            //Добавление букмекеров
            events = new Dictionary<Bookmaker, List<IEventLink>>();
            foreach (Bookmaker bm in bookmakers)
                events.Add(bm, new List<IEventLink>());

            //Добавление парсеров
            httpClients = new BaseHttpRequest[bookmakers.Length];
            for (int i = 0; i < bookmakers.Length; i++)
                httpClients[i] = BaseHttpRequest.GetHttpRequest(bookmakers[i]);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Start scan bookmakers at: {time}", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    await Task.Run(() => Parallel.ForEach(httpClients, x => GetEventChanges(x)));
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.ToString());
                }
                finally
                {
                    hub.UpdateScannerStatus();
                    await Task.Delay(delay, stoppingToken);
                }
            _logger.LogInformation("End scan bookmakers at: {time} (ElapsedMilliseconds:{int})", DateTimeOffset.Now, 0);
        }

        void GetEventChanges(BaseHttpRequest httpRequest)
        {
            DateTime utcNow = DateTime.UtcNow;
            List<IEventLink> newEventLinks = new List<IEventLink>();
            try
            {
                //Запрос списка онлайн событий на букмекере 
                IGameList jsonData = httpRequest.GetEventsList();

                //Выборка подходящих событий
                if (jsonData.Success)
                    foreach (var item in jsonData.EventsArray)
                        if (item.Sport != Sport.None && (sports == null || sports.Contains(item.Sport)))/*item.Status != StatusEvent.Over &&*/
                            newEventLinks.Add(item);

                //Удалить события, которых нет в текущей выборке
                for (int i = 0; i < events[httpRequest.BM].Count; i++)
                {
                    //Поиск события в текущей выборке
                    for (int n = 0; n < newEventLinks.Count; n++)
                        if (newEventLinks[n]?.Id == events[httpRequest.BM][i].Id)
                        {
                            events[httpRequest.BM][i].Updated = utcNow;
                            newEventLinks[n] = null;
                            break;
                        }
                    //Если ивент не найден в новом списке и прошло 15 минут
                    if (events[httpRequest.BM][i].Updated + new TimeSpan(0, 15, 0) < utcNow)
                        CloseEvent(events[httpRequest.BM][i]);
                }
                //Добавить событие если его нет в старом списке
                for (int n = 0; n < newEventLinks.Count; n++)
                    if (newEventLinks[n] != null)
                        AddEvent(newEventLinks[n]);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
        }
        //Event action
        void AddEvent(IEventLink ev)
        {
            ev.Updated = DateTime.UtcNow;

            //Поиск события в текущем списке events[Bookmaker]
            for (int i = 0; i < events[ev.Bookmaker].Count; i++)
                if (events[ev.Bookmaker][i].Id == ev.Id)
                {
                    events[ev.Bookmaker][i] = ev;
                    return;
                }

            //При отсутствии добавить в очередь событий букмекера
            events[ev.Bookmaker].Add(ev);
            hub.EventEnqueue(ev);
        }
        void CloseEvent(IEventLink ev)
        {
            //Удаление из списка событий
            for (int i = 0; i < events[ev.Bookmaker].Count; i++)
                if (events[ev.Bookmaker][i].Id == ev.Id)
                {
                    events[ev.Bookmaker].RemoveAt(i);
                    break;
                }
        }

    }
}
