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
using Newtonsoft.Json;

namespace ForkBro.Mediator
{
    public class HubMediator : IScannerMediator, IBookmakerMediator, IDaemonMasterMediator, IApiMediator
    {
        static readonly object AddLocker = new object();
        static readonly object ForkFileLocker = new object();
        
        readonly Dictionary<Bookmaker, int> _bookmakersDelay;
        readonly ILogger<HubMediator> _logger;
        readonly Dictionary<Bookmaker, DateTime> _statusServices;
        readonly HubManager _hubManager;
        ConcurrentQueue<IEventLink> Links { get;}

        public int ScannerDelay { get; set; }
        public int CountDaemons { get; set; }

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
        }

        public Bookmaker[] GetBookmakers() => _bookmakersDelay.Keys.ToArray();
        public Dictionary<Bookmaker, DateTime> GetLastUpdate() => _statusServices;

        #region IScannerMediator
        public void EventEnqueue(IEventLink link)
        {
            Links.Enqueue(link);
            _logger.LogTrace($"Enqueue: {link.Bookmaker}, {link.Id}");
        }
        public void UpdateScannerStatus() => _statusServices[Bookmaker.LiveScanner] = DateTime.Now;
        #endregion

        #region IBookmakerMediator

        public bool TryGetNewEvent(Bookmaker bookmaker, out IEventLink link)
        {
            if (Links.TryPeek(out link))
                if (link.Bookmaker == bookmaker)
                {
                    Links.TryDequeue(out IEventLink linkD);
                    if (link == linkD)
                        return true;
                    else
                        Links.Enqueue(linkD);       
                }

            //Если новое событие не относится к букмекеру
            return false;
        }
        public void AddEvent(IEventLink link, ref BetEvent bookmakerEvent)
        {
            lock (AddLocker)
                try
                {
                    
                    bool eventContains = _hubManager.CalculateFuzzyEvent(link.Sport, link.CommandA, link.CommandB, out int idPool, out bool reverse);//Поиск соответствия в pool
                    if (!eventContains)
                        idPool = _hubManager.AddPoolRaw();
                    bookmakerEvent.PoolId = idPool;
                    bookmakerEvent.Reverse = reverse;
                    //Добавление снимка в Пул
                    _hubManager.AddSnapshot(idPool, ref bookmakerEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error add event in pool [{link.Bookmaker}|Event {link.Id}|{link.CommandA.NameEng}|{link.CommandB.NameEng}]");
                }
            _logger.LogInformation($"Snapshot ADD {link.Id} [{link.CommandA.NameEng}|{link.CommandB.NameEng}]");
        }
        public void OverEvent(int idPool, Bookmaker bm = Bookmaker.None)
        {
            if (bm == Bookmaker.None)
                _hubManager.RemovePoolRaw(idPool);
            else
                _hubManager.RemoveSnapshot(idPool, bm);
            _logger.LogInformation($"Snapshot OVER {idPool} on {bm}");
        }
        public bool HaveNewLink() => !Links.IsEmpty;
        public void UpdateBookmakerStatus(Bookmaker bm) => _statusServices[bm] = DateTime.Now;
        public int GetEventPoolId(Bookmaker bookmaker, long idEvent) => _hubManager.HasEventInPool(bookmaker, idEvent);
        #endregion

        #region IDaemonMasterMediator
        public void UpdateDaemonMasterStatus() => _statusServices[Bookmaker.DaemonMaster] = DateTime.Now;
        public PoolRaw GetNextPool() => _hubManager.GetSnapshots(true);


        
        public void AddFork(List<Fork> forks)
        {
            if (forks.Count == 0)
                return;

            string folder = "Log\\Fork";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);


            foreach (var fork in forks)
                _logger.LogInformation($"New fork {fork.Sport.ToString()} {fork.IdEventA}({fork.BookmakerA})\t{fork.IdEventB}({fork.BookmakerB})");
            lock (ForkFileLocker)
                try
                {
                    string fileName = $"{folder}\\{forks[0].Sport}_{forks[0].IdEventA}_{forks[0].IdEventB}.log";
                    string fileText = Newtonsoft.Json.JsonConvert.SerializeObject(forks, Newtonsoft.Json.Formatting.Indented);
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        writer.Write(fileText);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при записи вилки в файл");
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
        bool HaveNewLink();
        bool TryGetNewEvent(Bookmaker bookmaker, out IEventLink link);
        void AddEvent(IEventLink link, ref BetEvent bookmakerEvent);
        void OverEvent(int idPool, Bookmaker bm);
        int GetEventPoolId(Bookmaker bookmaker, long id);
        
        void UpdateBookmakerStatus(Bookmaker bm);
    }
    public interface IDaemonMasterMediator
    {
        int CountDaemons { get; set; }

        void AddFork(List<Fork> forks);
        PoolRaw GetNextPool();
        void UpdateDaemonMasterStatus();
    }
    public interface IApiMediator
    {
        Dictionary<Bookmaker, int> CountEvents();
        Dictionary<Bookmaker, DateTime> GetLastUpdate();

    }
}
