using ForkBro.Common;
using ForkBro.Common.BookmakerClient;
using ForkBro.Configuration;
using ForkBro.Mediator;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Scanner
{
    public class LiveScanner : BackgroundService
    {
        private readonly IScannerMediator _hub;
        private readonly Sport[] _sports;
        private readonly ILogger<LiveScanner> _logger;
        private readonly int _delay;
        private readonly Dictionary<Bookmaker, List<IEventLink>> _events;
        private readonly BaseRequest[] _httpClients;


        public LiveScanner(ILogger<LiveScanner> logger, IScannerMediator mediator, IOptions<AppSettings> setting)
        {
            _delay = setting.Value.LiveScanRepeat;
            _sports = setting.Value.TrackedSports;
            var bookmakers = setting.Value.GetEBookmakers();

            _hub = mediator;
            _logger = logger;

            //Add events list
            _events = new Dictionary<Bookmaker, List<IEventLink>>();
            foreach (Bookmaker bm in bookmakers)
                _events.Add(bm, new List<IEventLink>());

            //Add clients
            _httpClients = new BaseRequest[bookmakers.Length];
            for (int i = 0; i < bookmakers.Length; i++)
                _httpClients[i] = BaseRequest.GetInstance(bookmakers[i]);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Start scan bookmakers");
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    await Task.Run(() => Parallel.ForEach(_httpClients, GetEventChanges), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.ToString());
                }
                finally
                {
                    _hub.UpdateScannerStatus();
                    await Task.Delay(_delay, stoppingToken);
                }
            _logger.LogInformation("End scan bookmakers at: {time} (ElapsedMilliseconds:{int})", DateTimeOffset.Now, 0);
        }

        private void GetEventChanges(BaseRequest httpRequest)
        {
            DateTime utcNow = DateTime.UtcNow;
            List<IEventLink> newEventLinks = new List<IEventLink>();
            try
            {
                //Запрос списка онлайн событий на букмекере 
                IGameList jsonData = httpRequest.GetEventsList();

                //Выборка подходящих событий
                if (jsonData.Success)
                    foreach (IEventLink item in jsonData.EventsArray)
                        if (item.Sport != Sport.None && (_sports == null || _sports.Contains(item.Sport)))/*item.Status != StatusEvent.Over &&*/
                            newEventLinks.Add(item);

                //Удалить события, которых нет в текущей выборке
                for (int i = 0; i < _events[httpRequest.bookmaker].Count; i++)
                {
                    //Поиск события в текущей выборке
                    for (int n = 0; n < newEventLinks.Count; n++)
                        if (newEventLinks[n]?.Id == _events[httpRequest.bookmaker][i].Id)
                        {
                            _events[httpRequest.bookmaker][i].Updated = utcNow;
                            newEventLinks[n] = null;
                            break;
                        }
                    //Если ивент не найден в новом списке и прошло 15 минут
                    if (_events[httpRequest.bookmaker][i].Updated + new TimeSpan(0, 15, 0) < utcNow)
                        CloseEvent(_events[httpRequest.bookmaker][i]);
                }
                //Добавить событие если его нет в старом списке
                foreach (IEventLink evLinking in newEventLinks)
                    if (evLinking != null)
                        AddEvent(evLinking);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
        }

        //Event action
        private void AddEvent(IEventLink ev)
        {
            ev.Updated = DateTime.UtcNow;

            //Поиск события в текущем списке events[Bookmaker]
            for (int i = 0; i < _events[ev.Bookmaker].Count; i++)
                if (_events[ev.Bookmaker][i].Id == ev.Id)
                {
                    _events[ev.Bookmaker][i] = ev;
                    return;
                }

            //При отсутствии добавить в очередь событий букмекера
            _events[ev.Bookmaker].Add(ev);
            _logger.LogDebug($"{ev.Bookmaker.ToString()}\t|" +
                             $"\t{ev.Sport.ToString()}\t|" +
                             $"\t{ev.TournamentName}\t|" +
                             $"\t{ev.Id}\t|\t" +
                             $"{ev.CommandA.NameEng}\t|" +
                             $"\t{ev.CommandB.NameEng}");
            _hub.EventEnqueue(ev);
        }

        private void CloseEvent(IEventLink ev)
        {
            //Удаление из списка событий
            for (int i = 0; i < _events[ev.Bookmaker].Count; i++)
                if (_events[ev.Bookmaker][i].Id == ev.Id)
                {
                    _events[ev.Bookmaker].RemoveAt(i);
                    break;
                }
        }

    }
}
