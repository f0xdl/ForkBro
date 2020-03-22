using ForkBro.Configuration;
using ForkBro.Controller.Event;
using ForkBro.Controller.Scanner;
using ForkBro.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Controller.Hub
{
    class HubManager
    {
        Bookmaker[] bookmakers;

        BetEventScanner onlineScanner;
        Dictionary<Bookmaker, BookmakerClient> scanners;
        EventHub Hub;

        public HubManager(BookmakersProp[] companies)
        {
            Hub = new EventHub(bookmakers);
            bookmakers = new Bookmaker[0];
            onlineScanner = new BetEventScanner();
            scanners = new Dictionary<Bookmaker, BookmakerClient>();
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
                if (!scanners[company.companyID].WorkStatus())
                    scanners[company.companyID].Start();
            }
        }

    }
}
