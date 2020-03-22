using ForkBro.Bookmakers;
using ForkBro.Common;
using ForkBro.Configuration;
using ForkBro.Daemons;
using ForkBro.Scanner.EventLinks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public HubMediator(ILogger<HubMediator> logger, ISetting setting)
        {
            statusServices = new Dictionary<Bookmaker, DateTime>();
            bookmakersDelay = new Dictionary<Bookmaker, int>();
            hubManager = new HubManager(setting.CountPool,setting.MinQuality);
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
        }

        public Bookmaker[] GetBookmakers() => bookmakersDelay.Keys.ToArray();
        public Dictionary<Bookmaker, DateTime> GetLastUpdate() => statusServices;

        #region IScannerMediator
        public void EventEnqueue(IEventLink link)
        {
            Links.Enqueue(link);
            _logger.LogDebug($"Enqueue: {link.Bookmaker}, {link.CommandA.NameEng}, {link.CommandB.NameEng}, {link.Sport}, {link.Id}");
        }

        public void UpdateScannerStatus() => statusServices[Bookmaker.LiveScanner] = DateTime.Now;
        #endregion

        #region IbookmakerMediator
        public IEventLink GetNewEvent(Bookmaker bookmaker)
        {
            IEventLink link;
            if (Links.TryPeek(out link))
                if (link.Bookmaker == bookmaker)
                    Links.TryDequeue(out link);
            //Если новое событие не относится к букмекеру
            return link;
        }
        public void TryAddEvent(IEventLink link, ref BookmakerEvent bookmakerEvent)
        {

            // Событие уже существет
            if (hubManager.HasEventInPool(link.Bookmaker, link.Id))
                return;

            //События не существует
            //Поиск события в Пуле
            double quality = hubManager.CalculateFuzzyEvent(link.Sport, link.CommandA, link.CommandB, out int idPool, out bool reverse);//Поиск соответствия в pool
            //Добавление нового элемента Пула при отсутствии
            if (quality > 0)
                idPool = hubManager.AddPoolRaw(new EventProps()
                {
                    sport = link.Sport,
                    StartDT = DateTime.Now,
                    status = link.Status,
                    CommandA = link.CommandA,
                    CommandB = link.CommandB,
                });
            bookmakerEvent.poolId = idPool;
            bookmakerEvent.reverse = reverse;
            //Добавление снимка в Пул
            hubManager.AddSnapshot(idPool, ref bookmakerEvent);
            _logger.LogInformation($"[quality={quality}] Bookmaker {link.Bookmaker}, event ADD {link.Id}");
            return;
        }
        public void OverEvent(int idPool, Bookmaker bm)
        {
            if (bm == Bookmaker.None)
                hubManager.RemovePoolRaw(idPool);
            else
                hubManager.RemoveSnapsot(idPool, bm);
            _logger.LogInformation($"Bookmaker {bm}, event OVER {idPool}");
        }

        public void UpdateSnapshot(int idPool, ref BookmakerEvent betEvent) => hubManager.UpdateSnapshot(idPool, ref betEvent);
        public bool HaveNewEvent() => !Links.IsEmpty;

        public void UpdateBookmakerStatus(Bookmaker bm) => statusServices[bm] = DateTime.Now;
        public int GetBookmakerDelay(Bookmaker bookmaker) => bookmakersDelay[bookmaker];
        #endregion

        #region IDaemonMasterMediator
        public void UpdateDaemonMasterStatus() => statusServices[Bookmaker.DaemonMaster] = DateTime.Now;
        public PoolRaw GetNextPool() => hubManager.GetNextSnapshots();
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
        public Bookmaker[] GetBookmakers();
        public void EventEnqueue(IEventLink link);
        //public void OverEvent(Bookmaker bm, long eventID);
        public void UpdateScannerStatus();
        public int ScannerDelay { get; set; }
    }
    public interface IBookmakerMediator
    {
        public IEventLink GetNewEvent(Bookmaker bookmaker);
        public void TryAddEvent(IEventLink link, ref BookmakerEvent bookmakerEvent);
        public void OverEvent(int idPool, Bookmaker bm);
        public bool HaveNewEvent();
        public void UpdateSnapshot(int idPool, ref BookmakerEvent betEvent);
        public void UpdateBookmakerStatus(Bookmaker bm);
        public int GetBookmakerDelay(Bookmaker bookmaker);
    }
    public interface IDaemonMasterMediator
    {
        int CountDaemons { get; set; }

        public PoolRaw GetNextPool();
        public void UpdateDaemonMasterStatus();
    }
    public interface IApiMediator //TODO Connect API
    {
        public Dictionary<Bookmaker, int> CountEvents();
        public Dictionary<Bookmaker, DateTime> GetLastUpdate();

    }
}
