using ForkBro.BookmakerModel;
using ForkBro.Common;
using ForkBro.Configuration;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ForkBro.Mediator
{
    public class HubMediator : IScannerMediator, IBookmakerMediator, IDaemonMasterMediator, IApiMediator
    {
        readonly Dictionary<Bookmaker, int> _bookmakersDelay;
        readonly ILogger<HubMediator> _logger;
        readonly Dictionary<Bookmaker, DateTime> _statusServices;
        readonly HubManager _hubManager;
        ConcurrentQueue<IEventLink> Links { get; set; }
        public int ScannerDelay { get; set; }
        public int CountDaemons { get; set; }
        static readonly object AddLocker = new object();

        private Sport[] _trackedSportTypes;

        public HubMediator(ILogger<HubMediator> logger, ISetting setting)
        {
            _statusServices = new Dictionary<Bookmaker, DateTime>();
            _bookmakersDelay = new Dictionary<Bookmaker, int>();
            _hubManager = new HubManager(setting.CountPool, setting.MinQuality);
            Links = new ConcurrentQueue<IEventLink>();

            _logger = logger;
            ScannerDelay = setting.LiveScanRepeat;
            CountDaemons = setting.CountDaemon;

            //Bookmakers
            foreach (BookmakersProp bm in setting.Companies)
            {
                _bookmakersDelay.Add(bm.id, bm.repeat);
                _statusServices.Add(bm.id, DateTime.Now);
            }

            //Status
            _statusServices.Add(Bookmaker.LiveScanner, DateTime.Now);
            _statusServices.Add(Bookmaker.DaemonMaster, DateTime.Now);
            _statusServices.Add(Bookmaker.Client, DateTime.Now);

            //Tracked Sport Types
            _trackedSportTypes = setting.TrackedSports;
        }

        public Bookmaker[] GetBookmakers() => _bookmakersDelay.Keys.ToArray();
        public Dictionary<Bookmaker, DateTime> GetLastUpdate() => _statusServices;

        #region IScannerMediator
        public void EventEnqueue(IEventLink link)
        {
            Links.Enqueue(link);
            _logger.LogTrace($"Enqueue: {link.Bookmaker}, {link.TournamentName}, {link.CommandA.NameEng}, {link.CommandB.NameEng}, {link.Sport}, {link.Id}");
        }

        public void UpdateScannerStatus() => _statusServices[Bookmaker.LiveScanner] = DateTime.Now;
        #endregion

        #region IbookmakerMediator
        public bool TryGetNewEvent(Bookmaker bookmaker, out IEventLink link)
        {
            if (Links.TryPeek(out link))
                if (link.Bookmaker == bookmaker)
                {
                    Links.TryDequeue(out link);
                    return true;
                }
            //Если новое событие не относится к букмекеру
            return false;
        }
        public void AddEvent(IEventLink link, ref BetEvent bookmakerEvent)
        {
            int idPool = 0;
            lock (AddLocker)
                try
                {
                    double quality = _hubManager.CalculateFuzzyEvent(link.Sport, link.CommandA, link.CommandB, out int outIdPool, out bool reverse);//Поиск соответствия в pool
                    idPool = outIdPool;
                    //Добавление нового элемента Пула при отсутствии
                    if (quality == 0)
                        idPool = _hubManager.AddPoolRaw(new EventProps()
                        {
                            sport = bookmakerEvent.Sport,
                            StartDT = DateTime.Now,
                            status = bookmakerEvent.Status,
                            CommandA = bookmakerEvent.CommandA,
                            CommandB = bookmakerEvent.CommandB,
                        });

                    bookmakerEvent.PoolId = idPool;
                    bookmakerEvent.Reverse = reverse;
                    //Добавление снимка в Пул
                    _hubManager.AddSnapshot(idPool, ref bookmakerEvent);
                    _logger.LogInformation($"[quality={quality}] Bookmaker {link.Bookmaker}, event ADD {link.Id} [{link.CommandA.NameEng}|{link.CommandB.NameEng}|Pool {idPool}]");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error add event in pool [{link.Bookmaker}|Event {link.Id}|{link.CommandA.NameEng}|{link.CommandB.NameEng}|Pool {idPool}]");
                }
        }
        public void OverEvent(int idPool, Bookmaker bm)
        {
            if (bm == Bookmaker.None)
                _hubManager.RemovePoolRaw(idPool);
            else
                _hubManager.RemoveSnapsot(idPool, bm);
            _logger.LogInformation($"Bookmaker {bm}, event OVER {idPool}");
        }

        //public void UpdateSnapshot(int idPool, ref EventBase betEvent) => hubManager.UpdateSnapshot(idPool, ref betEvent);
        public bool HaveNewEvent() => !Links.IsEmpty;

        public void UpdateBookmakerStatus(Bookmaker bm) => _statusServices[bm] = DateTime.Now;
        public int GetBookmakerDelay(Bookmaker bookmaker) => _bookmakersDelay[bookmaker];

        public int EventPoolId(Bookmaker bookmaker, long idEvent) => _hubManager.HasEventInPool(bookmaker, idEvent);
        public BetEvent GetEvent(Bookmaker bookmaker, long idEvent) => _hubManager.GetEvent(bookmaker, idEvent);
        #endregion

        #region IDaemonMasterMediator
        public void UpdateDaemonMasterStatus() => _statusServices[Bookmaker.DaemonMaster] = DateTime.Now;
        public PoolRaw GetNextPool() => _hubManager.GetSnapshots(true);

        public void AddFork(List<Fork> forks)
        {
            foreach (var fork in forks)
            {
                string str = Newtonsoft.Json.JsonConvert.SerializeObject(fork, Newtonsoft.Json.Formatting.Indented);
                string name = $"Logs\\Forks\\{fork.IdEventA}{fork.BookmakerA}-{fork.IdEventB}{fork.BookmakerB}.log";
                //if (!File.Exists(name))
                //    File.Create(name);
                File.WriteAllText(name, str);
            }
        }

        #endregion

        #region IApiMediator
        public Dictionary<Bookmaker, int> CountEvents()
        {
            throw new NotImplementedException();
        }
        #endregion

    }

    public interface IScannerMediator
    {
        Bookmaker[] GetBookmakers();
        void EventEnqueue(IEventLink link);
        void UpdateScannerStatus();
        int ScannerDelay { get; set; }
    }
    public interface IBookmakerMediator
    {
        bool TryGetNewEvent(Bookmaker bookmaker, out IEventLink link);
        void AddEvent(IEventLink link, ref BetEvent bookmakerEvent);
        void OverEvent(int idPool, Bookmaker bm);
        bool HaveNewEvent();
        BetEvent GetEvent(Bookmaker bookmaker, long idEvent);
        void UpdateBookmakerStatus(Bookmaker bm);
        int GetBookmakerDelay(Bookmaker bookmaker);
        int EventPoolId(Bookmaker bookmaker, long id);
    }
    public interface IDaemonMasterMediator
    {
        int CountDaemons { get; set; }

        void AddFork(List<Fork> forks);
        PoolRaw GetNextPool();
        void UpdateDaemonMasterStatus();
    }
    public interface IApiMediator //TODO Connect API
    {
        Dictionary<Bookmaker, int> CountEvents();
        Dictionary<Bookmaker, DateTime> GetLastUpdate();

    }
}
