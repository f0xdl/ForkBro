using ForkBro.Configuration;
using ForkBro.Controller;
using ForkBro.Controller.Event;
using ForkBro.Controller.Scanner;
using ForkBro.Model;
using ForkBro.Model.EventModel;
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
            config.eventsUpdate = Convert.ToInt32(TimeSpan.FromSeconds(1).TotalMilliseconds) * 10;
            config.companies = new CompanyProp[] {
                new CompanyProp() { companyID = EBookmakers._1xbet, enable = true, mapRepeat = 1000 },
                new CompanyProp() { companyID = EBookmakers._favbet, enable = true, mapRepeat = 1000 }
                };
            //END DEBUG
            #endregion

            #region EventManager
            BetEventScanner eventManager = new BetEventScanner();

            eventManager.UpdateBookmakers(config.GetEBookmakers());
            eventManager.ScannerStart(config.eventsUpdate);
            #endregion

            #region BookmakerScanner
            Dictionary<EBookmakers, BookmakerScanner> scanners = new Dictionary<EBookmakers, BookmakerScanner>();
            foreach (CompanyProp company in config.companies)
            {
                //Настройки сканера
                BookmakerScanner bookmakerScanner = new BookmakerScanner();
                bookmakerScanner.SetScanner(company.companyID);
                bookmakerScanner.Start(company.mapRepeat);

                //Добавление сканера в словать 
                scanners.Add(company.companyID, bookmakerScanner);
            }
            #endregion
        }
    }
}