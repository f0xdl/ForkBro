using ForkBro.Configuration;
using ForkBro.Controller.Event;
using ForkBro.Controller.Scanner;
using ForkBro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkBro.Controller.Hub
{
    class HubManager
    {
		EBookmakers[] bookmakers;

        BetEventScanner eventManager;
        Dictionary<EBookmakers, BookmakerClient> scanners;

        public HubManager()
        {
            bookmakers = new EBookmakers[0];
            eventManager = new BetEventScanner();
            scanners = new Dictionary<EBookmakers, BookmakerClient>();
        }

        public void Start(int delayOnline,BookmakersProp[] companies)
        {
            //Game scanner
            eventManager.ScannerStart(delayOnline);
            foreach (var item in scanners.Values)
                item.Start();

            //Calculate daemon
        }

        public void UpdateBookmakers(BookmakersProp[] companies)
        {
            bookmakers = companies.Select(x => x.companyID).ToArray();
            //Update online scanners
            eventManager.UpdateBookmakers(bookmakers);

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
                {
                    //Add scanner to list
                    scanners.Add(company.companyID, new BookmakerClient(company.companyID,company.maxEvents));
                }
                scanners[company.companyID].UpdatePeriod = company.mapRepeat;
            }
        }

    }
}
