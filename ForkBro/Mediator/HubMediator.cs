using ForkBro.Common;
using ForkBro.Configuration;
using ForkBro.Daemons;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Mediator
{
    public class HubMediator : IScannerMediator, IBookmakerMediator, IDaemonMasterMediator, IApiMediator
    {
        Dictionary<Bookmaker, int> bookmakersProp;
        ILogger<HubMediator> _logger;
        Dictionary<Bookmaker, DateTime> StatusServices;

        public int ScannerDelay { get; set; }
        public int CountDaemons { get; set; }

        public HubMediator(ILogger<HubMediator> logger, ISetting setting)
        {
            StatusServices = new Dictionary<Bookmaker, DateTime>();
            
            _logger = logger;
            ScannerDelay = setting.LiveScanRepeat;
            CountDaemons = setting.CountDaemon;

            //Bookmakers
            bookmakersProp = new Dictionary<Bookmaker, int>();
            foreach (var BM in setting.Companies)
            { 
                bookmakersProp.Add(BM.id, BM.repeat);
                StatusServices.Add(BM.id, DateTime.Now);
            }

            //Status
            StatusServices.Add(Bookmaker._LiveScanner, DateTime.Now);
            StatusServices.Add(Bookmaker._DaemonMaster, DateTime.Now);
            StatusServices.Add(Bookmaker._Client, DateTime.Now);
        }

        public int GetBookmakerDelay(Bookmaker bookmaker) => bookmakersProp[bookmaker];
        public Bookmaker[] GetBookmakers() => bookmakersProp.Keys.ToArray();

        //IScannerMediator
        public void AddEvent(Bookmaker bm, Sport sport, long Id, string CommandA, string CommandB)
        {
            _logger.LogInformation($"Bookmaker {bm}, Sport {sport}, Id {Id}, CommandA {CommandA} , CommandB {CommandB}");
            //TODO throw new NotImplementedException();
        }
        public void OverEvent(Bookmaker bm, long eventID)
        {
            _logger.LogInformation($"Bookmaker {bm}, eventID {eventID}");
            //TODO throw new NotImplementedException();
        }
        public void UpdateScannerStatus()=>StatusServices[Bookmaker._LiveScanner] = DateTime.Now;

        //IbookmakerMediator
        public bool HaveNewEvent(Bookmaker bm, out BookmakerEvent betEvent)
        {
            throw new NotImplementedException();
        }
        public void UpdateSnapshot(Bookmaker bm, BookmakerEvent betEvent)
        {
            throw new NotImplementedException();
        }
        public void UpdateBookmakerStatus(Bookmaker bm)=> StatusServices[bm] = DateTime.Now;

        //DaemonMaster
        public EventHub GetEventHub()
        {
            throw new NotImplementedException();
        }
        public void UpdateDaemonMasterStatus() => StatusServices[Bookmaker._DaemonMaster] = DateTime.Now;

        //ICalcDaemonMediator
        public Dictionary<Bookmaker, int> CountEvents()
        {
            throw new NotImplementedException();
        }
        public Dictionary<Bookmaker, DateTime> GetLastUpdate() => StatusServices;
    }
    
    public interface IScannerMediator
    {
        public Bookmaker[] GetBookmakers();
        public void AddEvent(Bookmaker bm, Sport sport, long Id, string CommandA, string CommandB);
        public void OverEvent(Bookmaker bm, long eventID);
        public void UpdateScannerStatus();
        public int ScannerDelay { get; set; }
    }
    public interface IBookmakerMediator
    {
        public void OverEvent(Bookmaker bm, long eventID);
        public bool HaveNewEvent(Bookmaker bm, out BookmakerEvent betEvent);
        public void UpdateSnapshot(Bookmaker bm, BookmakerEvent betEvent);
        public void UpdateBookmakerStatus(Bookmaker bm);
        public int GetBookmakerDelay(Bookmaker bookmaker);
    }
    public interface IDaemonMasterMediator
    {
        int CountDaemons { get; set; }

        public EventHub GetEventHub();
        public void UpdateDaemonMasterStatus();

    }
    public interface IApiMediator //TODO Connect API
    {
        public Dictionary<Bookmaker,int> CountEvents();
        public Dictionary<Bookmaker,DateTime> GetLastUpdate();

    }
}
