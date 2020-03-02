using ForkBro.Configuration;
using ForkBro.Controller;
using ForkBro.Controller.Event;
using ForkBro.Controller.Scanner;
using ForkBro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ForkBro
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Config
            ConfigManager confManager = new ConfigManager();


            if (!confManager.FileExists)
            {
                confManager.CreateConfig();
                confManager.WriteConfig();
            }
            confManager.ReadConfig();
            ref Config config = ref confManager.GetConfig();

            //DEBUG
            config.eventsUpdate = Convert.ToInt32(TimeSpan.FromSeconds(1).TotalMilliseconds)*10;
            config.companies = new CompanyProp[] {
                new CompanyProp() { companyID = EBookmakers._1xbet, enable = true, mapRepeat = 1000 },
                new CompanyProp() { companyID = EBookmakers._favbet, enable = true, mapRepeat = 1000 }
                };
            //END DEBUG
            #endregion

            #region EventManager
            BetEventManager eventManager = new BetEventManager();

            eventManager.UpdateBookmakers(config.GetEBookmakers());
            eventManager.ScannerStart(config.eventsUpdate);
            #endregion

            #region BookmakerScanner
            List<BookmakerScanner> scanners = new List<BookmakerScanner>();
            foreach (CompanyProp company in config.companies)
            {
                BookmakerScanner bookmakerScanner = new BookmakerScanner();
                bookmakerScanner.SetScanner(company.companyID);
                bookmakerScanner.Start(company.mapRepeat);
                scanners.Add(bookmakerScanner);
            }
            #endregion
        }
    }
}
