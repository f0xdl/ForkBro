using ForkBro.Common;
using ForkBro.Common.BookmakerClient;
using ForkBro.Mediator;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.BookmakerModel
{
    public abstract class BookmakerService : BackgroundService
    {
        List<BetEvent> events;
        int delay;
        IBookmakerMediator hub;

        protected readonly Bookmaker bookmaker;
        protected internal readonly ILogger<BookmakerService> _logger;
        protected internal BaseHttpRequest HttpClient;

        public BookmakerService(ILogger<BookmakerService> logger, IBookmakerMediator mediator, Bookmaker BM)
        {
            _logger = logger;
            hub = mediator;
            bookmaker = BM;
            delay = mediator.GetBookmakerDelay(BM);
            HttpClient = BaseHttpRequest.GetHttpRequest(bookmaker);
            events = new List<BetEvent>();
        }
        void EventIsOver(long[] eventsId)
        {
            if (eventsId != null)
                foreach (var item in events)
                    if (eventsId.Contains(item.EventId))
                    {
                        hub.OverEvent(item.PoolId, bookmaker);
                        events.Remove(item);
                    }
        }
        void CheckOverEvents() => EventIsOver(events.Where(x => x.Status == StatusEvent.Over)?.Select(x=>x.EventId).ToArray());
        void CheckNewEvents()
        {
            if (hub.HaveNewEvent())
                for (int n = 0; n < 10; n++)
                    if (hub.TryGetNewEvent(bookmaker, out IEventLink link))
                    {
                        int poolId = hub.EventPoolId(link.Bookmaker, link.Id);
                        if (poolId != -1)//Event exist in hub
                            if (events.Exists(x => x.EventId == link.Id))
                                continue;   //This event exist
                            else
                                hub.OverEvent(poolId, bookmaker);//Drop event from hub 
                        else
                            events.RemoveAll(x => x.EventId == link.Id);//Drop events from model 

                        //Create new event
                        BetEvent bmEvent = new BetEvent();
                        bmEvent.EventId = link.Id;
                        bmEvent.Bookmaker = link.Bookmaker;
                        bmEvent.Sport = link.Sport;
                        bmEvent.Status = link.Status;
                        bmEvent.CommandA = link.CommandA;
                        bmEvent.CommandB = link.CommandB;
                        bmEvent.Status = StatusEvent.New;

                        //Add event
                        hub.AddEvent(link, ref bmEvent);//Pool
                        events.Add(bmEvent);//Model
                    }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {//FIX Проверить асинхронность данного блока
            if (HttpClient == null)
                throw new Exception($"Model {bookmaker} don't have httpClient ");
            while (!stoppingToken.IsCancellationRequested)
                try { 
                CheckNewEvents();
                CheckOverEvents();

                //Update odds in events
                Parallel.ForEach(events, x =>
                    x.UpdateOdds(
                        HttpClient.GetDictionaryOdds(x.EventId, x.Sport)
                ));

                await Task.Delay(delay, stoppingToken);
                }
                catch(Exception ex) {
                    _logger.LogError(ex,"Ошибка во время выполнения модели букмеккера - {0}",bookmaker.ToString()); 
                }
        }
        //0080 0001 0001 0000 0000 --Win in Game
        //0600 0001 0000 9985 0000 --Fora(-1,5)
        //0150 0001 0051 0000 0000 -- Кто выиграет 4-ый гейм?
        //0650 0001 0001 0095 0000 -- Тотал геймов
        //0850 0001 0001 0000 0000 --Точный счёт
    }

    #region Заглушки для запуска служб
    public class Service_1xbet : BookmakerService
    {
        public Service_1xbet(ILogger<Service_1xbet> logger, IBookmakerMediator mediator)
            : base(logger, mediator, Bookmaker._1xbet)
        { }
    }

    public class Service_favbet : BookmakerService
    {
        public Service_favbet(ILogger<Service_favbet> logger, IBookmakerMediator mediator)
            : base(logger, mediator, Bookmaker._favbet)
        { }
    }
    #endregion
}
