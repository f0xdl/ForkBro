using ForkBro.BookmakerModel;
using ForkBro.Common;
using ForkBro.Configuration;
using ForkBro.Daemons;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Mediator
{
    public class HubMediator : IScannerMediator, IBookmakerMediator, IDaemonMasterMediator, IApiMediator
    {
        Dictionary<Bookmaker, int> bookmakersDelay;
        ILogger<HubMediator> _logger;
        Dictionary<Bookmaker, DateTime> statusServices;
        HubManager hubManager;
        ConcurrentQueue<IEventLink> Links { get; set; }
        public int ScannerDelay { get; set; }
        public int CountDaemons { get; set; }
        static readonly object _AddLocker = new object();

        Sport[] trackedSportTypes;

        public HubMediator(ILogger<HubMediator> logger, ISetting setting)
        {
            statusServices = new Dictionary<Bookmaker, DateTime>();
            bookmakersDelay = new Dictionary<Bookmaker, int>();
            hubManager = new HubManager(setting.CountPool, setting.MinQuality);
            Links = new ConcurrentQueue<IEventLink>();

            _logger = logger;
            ScannerDelay = setting.LiveScanRepeat;
            CountDaemons = setting.CountDaemon;

            //Bookmakers
            foreach (var BM in setting.Companies)
            {
                bookmakersDelay.Add(BM.id, BM.repeat);
                statusServices.Add(BM.id, DateTime.Now);
            }

            //Status
            statusServices.Add(Bookmaker.LiveScanner, DateTime.Now);
            statusServices.Add(Bookmaker.DaemonMaster, DateTime.Now);
            statusServices.Add(Bookmaker.Client, DateTime.Now);

            //Tracked Sport Types
            trackedSportTypes = setting.TrackedSports;
        }

        public Bookmaker[] GetBookmakers() => bookmakersDelay.Keys.ToArray();
        public Dictionary<Bookmaker, DateTime> GetLastUpdate() => statusServices;

        #region IScannerMediator
        public void EventEnqueue(IEventLink link)
        {
            //Проверка на соответствие спорта
            if (trackedSportTypes==null || trackedSportTypes.Contains(link.Sport))
            {
                Links.Enqueue(link);
                _logger.LogDebug($"Enqueue: {link.Bookmaker}, {link.CommandA.NameEng}, {link.CommandB.NameEng}, {link.Sport}, {link.Id}");
            }
        }

        public void UpdateScannerStatus() => statusServices[Bookmaker.LiveScanner] = DateTime.Now;
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
            lock(_AddLocker)
                try
                {
                    double quality = hubManager.CalculateFuzzyEvent(link.Sport, link.CommandA, link.CommandB, out int outIdPool, out bool reverse);//Поиск соответствия в pool
                    idPool = outIdPool;
                    //Добавление нового элемента Пула при отсутствии
                    if (quality == 0)
                        idPool = hubManager.AddPoolRaw(new EventProps()
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
                    hubManager.AddSnapshot(idPool, ref bookmakerEvent);
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
                hubManager.RemovePoolRaw(idPool);
            else
                hubManager.RemoveSnapsot(idPool, bm);
            _logger.LogInformation($"Bookmaker {bm}, event OVER {idPool}");
        }

        //public void UpdateSnapshot(int idPool, ref EventBase betEvent) => hubManager.UpdateSnapshot(idPool, ref betEvent);
        public bool HaveNewEvent() =>!Links.IsEmpty;

        public void UpdateBookmakerStatus(Bookmaker bm) => statusServices[bm] = DateTime.Now;
        public int GetBookmakerDelay(Bookmaker bookmaker) => bookmakersDelay[bookmaker];

        public int EventPoolId(Bookmaker bookmaker, long idEvent) => hubManager.HasEventInPool(bookmaker, idEvent);
        public BetEvent GetEvent(Bookmaker bookmaker, long idEvent) => hubManager.GetEvent(bookmaker, idEvent);
        #endregion

        #region IDaemonMasterMediator
        public void UpdateDaemonMasterStatus() => statusServices[Bookmaker.DaemonMaster] = DateTime.Now;
        public PoolRaw GetNextPool() => hubManager.GetSnapshotsToComparison();
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
        //void UpdateSnapshot(int idPool, ref EventBase betEvent);
        void UpdateBookmakerStatus(Bookmaker bm);
        int GetBookmakerDelay(Bookmaker bookmaker);
        int EventPoolId(Bookmaker bookmaker, long id);
    }
    public interface IDaemonMasterMediator
    {
        int CountDaemons { get; set; }

        PoolRaw GetNextPool();
        void UpdateDaemonMasterStatus();
    }
    public interface IApiMediator //TODO Connect API
    {
        Dictionary<Bookmaker, int> CountEvents();
        Dictionary<Bookmaker, DateTime> GetLastUpdate();

    }
}
