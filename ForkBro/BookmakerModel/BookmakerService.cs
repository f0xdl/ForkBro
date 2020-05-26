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
using Microsoft.Extensions.Options;

namespace ForkBro.BookmakerModel
{
    public abstract class ServiceBookmaker : BackgroundService
    {
        protected readonly Bookmaker Bookmaker;
        readonly int _delay;
        internal readonly List<BetEvent> Events;
        readonly IBookmakerMediator _hub;
        readonly ILogger<ServiceBookmaker> _logger;
        protected readonly BaseRequest HttpRequest;

        public ServiceBookmaker(ILogger<ServiceBookmaker> logger, IOptions<AppSettings> setting, IBookmakerMediator mediator, Bookmaker BM)
        {
            _logger = logger;
            _hub = mediator;
            Bookmaker = BM;
            _delay = setting.Value.Companies.First(x => x.id == BM).repeat;
            Events = new List<BetEvent>(); 
            HttpRequest = BaseRequest.GetInstance(Bookmaker);
        }

        private void CheckNewEvents()
        {
            //Check new link
            if (_hub.HaveNewLink())
                for (int n = 0; n < 10; n++)
                    if (_hub.TryGetNewEvent(Bookmaker, out IEventLink link))
                    {
                        _logger.LogDebug($"Received a new link to the event - {link.Id}({link.CommandA.NameEng},{link.CommandB.NameEng})");
                        int poolId = _hub.GetEventPoolId(link.Bookmaker, link.Id);
                        if (poolId != -1)//Event exist in hub
                            if (Events.Exists(x => x.EventId == link.Id))
                                continue;   //This event exist
                            else
                                _hub.OverEvent(poolId, Bookmaker);//Drop event from hub 
                        else
                            Events.RemoveAll(x => x.EventId == link.Id);//Drop events from model 

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
                        Events.Add(bmEvent);//Model
                        _logger.LogDebug($"Event[{link.Id}] - Add in model");
                    }

            //Check over event
            long[] eventsIDs = Events.Where(x => x.Status == StatusEvent.Over)?.Select(x => x.EventId).ToArray();
            if (eventsIDs.Length > 1)
                foreach (BetEvent item in Events.Where(item => eventsIDs.Contains(item.EventId)))
                {
                    _hub.OverEvent(item.PoolId, Bookmaker);
                    _logger.LogDebug($"Event[{item.EventId}] - Drop from pool");
                    Events.Remove(item);
                    _logger.LogDebug($"Event[{item.EventId}] - Remove from model");
                }
        }

        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //FIX Проверить асинхронность данного блока
            bool connected = false;
            while (!connected)
                try
                {
                    connected = HttpRequest.TestConnection();
                    _logger.LogInformation("Проверка связи с букмекером прошла успешно");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"Ошибка во время проверки соединения с букмекером "+Bookmaker.ToString());
                    _logger.LogInformation("Следующая попытка через 30 секунд...");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }

            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    CheckNewEvents();
                    UpdateEvents();
                    await Task.Delay(_delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка во время выполнения модели букмеккера - {0}", Bookmaker.ToString());
                }
                finally
                {
                    _hub.UpdateBookmakerStatus(Bookmaker);
                }
        }

        protected abstract void UpdateEvents();
    }
}
