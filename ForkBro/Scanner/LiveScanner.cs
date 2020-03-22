using ForkBro.Common;
using ForkBro.Common.Client;
using ForkBro.Configuration;
using ForkBro.Mediator;
using ForkBro.OnlineScanner.EventLinks;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Scanner
{
    public class LiveScanner : BackgroundService
    {
        readonly IScannerMediator hub;
        readonly Bookmaker[] bookmakers;
        private readonly ILogger<LiveScanner> _logger;
        int delay;

        Dictionary<Bookmaker, List<IEventLink>> events;
        BaseHttpRequest[] httpClients;


        public LiveScanner(ILogger<LiveScanner> logger, IScannerMediator mediator, ISetting setting)
        {
            delay = setting.LiveScanRepeat;
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
            //Stopwatch stopwatch = new Stopwatch();
            while (!stoppingToken.IsCancellationRequested)
            {
                //stopwatch.Restart();
                _logger.LogInformation("Start scan bookmakers at: {time}", DateTimeOffset.Now);
                try
                {
                    await Task.Run(() => Parallel.ForEach(httpClients, x => GetEventChanges(x)));
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.ToString());
                }
                //stopwatch.Stop();
                _logger.LogInformation("End scan bookmakers at: {time} (ElapsedMilliseconds:{int})", DateTimeOffset.Now, 0);
                await Task.Delay(delay, stoppingToken);
            }
        }

        //Methods
        void GetEventChanges(BaseHttpRequest httpRequest)
        {
            try
            {
                //Выполнить запрос онлайн событий на букмекере
                List<IEventLink> newEventLinks = httpRequest.GetListEvent();

                //Клонирование текущего списка событий и установка флага обновления
                events[httpRequest.BM].ForEach(x => x.updated = false);

                //Удалить события, которых нет в текущей выборке
                for (int i = 0; i < events[httpRequest.BM].Count; i++)
                {
                    for (int n = 0; n < newEventLinks.Count; n++)
                        if (newEventLinks[n].id == events[httpRequest.BM][i].id)
                        {
                            //events[bookmaker][i].updated = true;
                            //newEventLinks[n].updated = true;
                            break;
                        }

                    if (!events[httpRequest.BM][i].updated)
                        CloseEvent(events[httpRequest.BM][i]);
                }
                //Добавить событие если его нет в старом списке
                for (int n = 0; n < newEventLinks.Count; n++)
                    if (!newEventLinks[n].updated)
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
            ev.status = StatusEvent.New;
            for (int i = 0; i < events[ev.bookmaker].Count; i++)
                if (events[ev.bookmaker][i].id == ev.id)
                {
                    ev.updated = true;
                    events[ev.bookmaker][i] = ev;
                    break;
                }

            if (!ev.updated)
            {
                ev.updated = true;
                events[ev.bookmaker].Add(ev);
            }
            //Добавление события на букмекера
            hub.AddEvent(ev.bookmaker, ev.sport, ev.id, ev.commandA.NameEng, ev.commandB.NameEng);
        }
        void CloseEvent(IEventLink ev)
        {
            //Удаление из массива
            ev.status = StatusEvent.Over;
            for (int i = 0; i < events[ev.bookmaker].Count; i++)
                if (events[ev.bookmaker][i].id == ev.id)
                {
                    events[ev.bookmaker].RemoveAt(i);
                    break;
                }
            //Удаление события
            hub.OverEvent(ev.bookmaker, ev.id);
        }

    }
}
