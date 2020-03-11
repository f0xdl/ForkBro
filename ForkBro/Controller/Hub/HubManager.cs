using ForkBro.Configuration;
using ForkBro.Controller.Event;
using ForkBro.Controller.Scanner;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using ForkBro.Model.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Controller.Hub
{
    class HubManager : IWorker
    {
        EBookmakers[] bookmakers;

        BetEventScanner onlineScanner;
        Dictionary<EBookmakers, BookmakerClient> scanners;
        EventHub Hub;

        public HubManager(BookmakersProp[] companies)
        {
            Hub = new EventHub(bookmakers);
            bookmakers = new EBookmakers[0];
            onlineScanner = new BetEventScanner();
            scanners = new Dictionary<EBookmakers, BookmakerClient>();
        }
        public void UpdateBookmakers(BookmakersProp[] companies)
        {
            bookmakers = companies.Select(x => x.companyID).ToArray();
            //Update online scanners
            onlineScanner.UpdateBookmakers(bookmakers);

            //Remove prev bookmakers scanners
            foreach (var scanner in scanners)
                if (!bookmakers.Contains(scanner.Key))
                {
                    scanner.Value.Stop(1000);
                    scanners.Remove(scanner.Key);
                }

            //Add missing bookmakers scanners
            foreach (BookmakersProp company in companies)
            {
                if (!scanners.Keys.Contains(company.companyID))
                    scanners.Add(company.companyID, new BookmakerClient(company.companyID, company.maxEvents));
                scanners[company.companyID].UpdatePeriod = company.mapRepeat;
                if(!scanners[company.companyID].WorkStatus())
                    scanners[company.companyID].Start();
            }
        }

        //IWorker
        bool IWorker.IsWork { get; set; }
        Thread IWorker.thread { get; set; }
        public void Start(int ms_onlineScanner)
        {
            //Game scanner
            onlineScanner.ScannerStart(ms_onlineScanner);
            //Scanner starts
            foreach (var scanner in scanners)
                scanner.Value.Start();

            //Calculate daemon
                //TODO Calculate daemon
            //Hub
            ((IWorker)this).StartWork(0);
        }
        public async void Stop(int ms_wait)
        {
            List<Task> tasks = new List<Task>();
            //Close Online
            tasks.Add(Task.Factory.StartNew(() => onlineScanner.ScannerStop(ms_wait)));
            //Close Scanners 
            foreach (var scanner in scanners)
                tasks.Add(Task.Factory.StartNew(() => scanner.Value.Stop(ms_wait)));
            //Close Calculate
                //TODO Add Calculate close task
            //Close Hub
            tasks.Add(Task.Factory.StartNew(()=>((IWorker)this).StopWork(ms_wait)));

            Task.WaitAll(tasks.ToArray());
        }
        async void IWorker.Work(object delay)
        {
            while (((IWorker)this).IsWork)
                try
                {
                    var betEvent = onlineScanner.GetNextEvent();
                    //TODO Hub.;




                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds((int)delay));
                }
        }
    }
}
