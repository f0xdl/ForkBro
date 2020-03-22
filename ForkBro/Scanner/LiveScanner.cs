using ForkBro.Common;
using ForkBro.Common.BookmakerClient;
using ForkBro.Configuration;
using ForkBro.Mediator;
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
                    await Task.Delay(delay, stoppingToken);
                }
            _logger.LogInformation("End scan bookmakers at: {time} (ElapsedMilliseconds:{int})", DateTimeOffset.Now, 0);
        }

        //Methods
        void GetEventChanges(BaseHttpRequest httpRequest)
        {
            try
            {
                //Выполнить запрос онлайн событий на букмекере
                List<IEventLink> newEventLinks = httpRequest.GetListEvent();

                //Клонирование текущего списка событий и установка флага обновления
                events[httpRequest.BM].ForEach(x => x.Updated = false);

                //Удалить события, которых нет в текущей выборке
                for (int i = 0; i < events[httpRequest.BM].Count; i++)
                {
                    for (int n = 0; n < newEventLinks.Count; n++)
                        if (newEventLinks[n].Id == events[httpRequest.BM][i].Id)
                        {
                            events[httpRequest.BM][i].Updated = true;
                            newEventLinks[n].Updated = true;
                            break;
                        }

                    if (!events[httpRequest.BM][i].Updated)
                        CloseEvent(events[httpRequest.BM][i]);
                }
                //Добавить событие если его нет в старом списке
                for (int n = 0; n < newEventLinks.Count; n++)
                    if (!newEventLinks[n].Updated)
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
            ev.Status = StatusEvent.New;
            for (int i = 0; i < events[ev.Bookmaker].Count; i++)
                if (events[ev.Bookmaker][i].Id == ev.Id)
                {
                    ev.Updated = true;
                    events[ev.Bookmaker][i] = ev;
                    break;
                }

            if (!ev.Updated)
            {
                ev.Updated = true;
                events[ev.Bookmaker].Add(ev);
            }
            //Добавление события на букмекера
            hub.EventEnqueue(ev);
        }
        void CloseEvent(IEventLink ev)
        {
            //Удаление из массива
            ev.Status = StatusEvent.Over;
            for (int i = 0; i < events[ev.Bookmaker].Count; i++)
                if (events[ev.Bookmaker][i].Id == ev.Id)
                {
                    events[ev.Bookmaker].RemoveAt(i);
                    break;
                }
            //Удаление события
            //hub.OverEvent(ev.Bookmaker, ev.Id);
        }

    }
}
