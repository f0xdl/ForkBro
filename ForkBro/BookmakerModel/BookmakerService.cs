using ForkBro.Common;
using ForkBro.Common.BookmakerClient;
using ForkBro.Mediator;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ForkBro.Configuration;

namespace ForkBro.BookmakerModel
{
    public abstract class BookmakerService : BackgroundService
    {
        protected readonly Bookmaker Bookmaker;
        readonly int _delay;
        readonly BaseRequest _httpRequest;
        readonly List<BetEvent> events;
        readonly IConversionDataOdds conversion;
        readonly IBookmakerMediator _hub;
        readonly ILogger<BookmakerService> _logger;

        protected BookmakerService(ILogger<BookmakerService> logger,ISetting setting, IBookmakerMediator mediator, Bookmaker BM)
        {
            _logger = logger;
            _hub = mediator;
            Bookmaker = BM;
            _delay = setting.Companies.First(x => x.id == BM).repeat;
            //_httpClient = BaseRequest.GetInstance(Bookmaker);
            events = new List<BetEvent>();
        }
    
        async Task CheckEvents()
        {
            //Check NEW link
            if (_hub.HaveNewLink())
                for (int n = 0; n < 10; n++)
                    if (_hub.TryGetNewEvent(Bookmaker, out IEventLink link))
                    {
                        _logger.LogDebug($"Received a new link to the event - {link.Id}({link.CommandA.NameEng},{link.CommandB.NameEng})");
                        int poolId = _hub.GetEventPoolId(link.Bookmaker, link.Id);
                        if (poolId != -1)//Event exist in hub
                            if (events.Exists(x => x.EventId == link.Id))
                                continue;   //This event exist
                            else
                                _hub.OverEvent(poolId, Bookmaker);//Drop event from hub 
                        else
                            events.RemoveAll(x => x.EventId == link.Id);//Drop events from model 

                        //Create new event
                        _logger.LogDebug($"Event[{link.Id}] - Create new event");
                        BetEvent bmEvent = new BetEvent
                        {
                            EventId = link.Id,
                            Bookmaker = link.Bookmaker,
                            Sport = link.Sport,
                            CommandA = link.CommandA,
                            CommandB = link.CommandB,
                            Status = StatusEvent.New
                        };

                        //Add event
                        _hub.AddEvent(link, ref bmEvent);//Pool
                        _logger.LogDebug($"Event[{link.Id}] - Add in pool");
                        events.Add(bmEvent);//Model
                        _logger.LogDebug($"Event[{link.Id}] - Add in model");
                    }

            //Check OVER event
            long[] eventsIDs = events.Where(x => x.Status == StatusEvent.Over)?.Select(x => x.EventId).ToArray();
            if (eventsIDs.Length > 1)
                foreach (var item in events)
                    if (eventsIDs.Contains(item.EventId))
                    {
                        _hub.OverEvent(item.PoolId, Bookmaker);
                        _logger.LogDebug($"Event[{item.EventId}] - Drop from pool");
                        events.Remove(item);
                        _logger.LogDebug($"Event[{item.EventId}] - Remove from model");
                    }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //FIX Проверить асинхронность данного блока
            //if (_httpClient == null)
            //    throw new Exception($"Model {Bookmaker} don't have httpClient ");
            //while (!stoppingToken.IsCancellationRequested)
            //    try
            //    {
            //        CheckEvents();

            //        //Update odds in events
            //        Parallel.ForEach(events, x =>
            //            x.UpdateOdds(
            //                _httpClient.GetDictionaryOdds(x.EventId, x.Sport)
            //            ));
            //        await Task.Delay(_delay, stoppingToken);
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Ошибка во время выполнения модели букмеккера - {0}", Bookmaker.ToString());
            //    }
            //    finally
            //    {
            //        _hub.UpdateBookmakerStatus(Bookmaker);
            //    }
        }
    }

    #region Заглушки для запуска служб
    public class Service_1xbet : BookmakerService
    {
        public Service_1xbet(ILogger<Service_1xbet> logger, ISetting setting, IBookmakerMediator mediator)
            : base(logger, setting, mediator,  Bookmaker._1xbet)
        { }
    }

    public class Service_favbet : BookmakerService
    {
        public Service_favbet(ILogger<Service_favbet> logger, ISetting setting, IBookmakerMediator mediator)
            : base(logger, setting, mediator,  Bookmaker._favbet)
        { }
    }
    #endregion
}
