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

namespace ForkBro.Controller.Hub
{
    class HubManager
    {
		EBookmakers[] bookmakers;

        BetEventScanner onlineScanner;
        Dictionary<EBookmakers, BookmakerClient> scanners;
        EventHub Hub;


        public HubManager()
        {
            Hub = new EventHub();
            bookmakers = new EBookmakers[0];
            onlineScanner = new BetEventScanner();
            scanners = new Dictionary<EBookmakers, BookmakerClient>();
        }

        public void Start(int delayOnline,BookmakersProp[] companies)
        {
            //Game scanner
            onlineScanner.ScannerStart(delayOnline);
            foreach (var item in scanners.Values)
                item.Start();

            //Hub work



            //Calculate daemon
        }

        public void UpdateBookmakers(BookmakersProp[] companies)
        {
            bookmakers = companies.Select(x => x.companyID).ToArray();
            //Update online scanners
            onlineScanner.UpdateBookmakers(bookmakers);

            //Remove prev bookmakers scanners
            foreach (var scanner in scanners)
                if(!bookmakers.Contains(scanner.Key))
                { 
                    scanner.Value.Stop(1000);
                    scanners.Remove(scanner.Key);
                }

            //Add missing bookmakers scanners
            foreach (BookmakersProp company in companies)
            {
                if (!scanners.Keys.Contains(company.companyID))
                    scanners.Add(company.companyID, new BookmakerClient(company.companyID,company.maxEvents));
                scanners[company.companyID].UpdatePeriod = company.mapRepeat;
            }
        }

        BetEvent GiveNext()
        {
            onlineScanner.GetNextEvent();
        }
    }
}
