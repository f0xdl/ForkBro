using ForkBro.Configuration;
using ForkBro.Controller;
using ForkBro.Controller.Event;
using ForkBro.Controller.Hub;
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

            #region Config Debug
            ref Config config = ref confManager.CurrConfig;
            config.eventsUpdate = Convert.ToInt32(TimeSpan.FromSeconds(1).TotalMilliseconds) * 10;
            config.companies = new BookmakersProp[] {
                new BookmakersProp() { companyID = EBookmakers._1xbet, enable = true, mapRepeat = 1000 },
                new BookmakersProp() { companyID = EBookmakers._favbet, enable = true, mapRepeat = 1000 }
                };
            #endregion Debug
            #endregion

            HubManager manager = new HubManager(config.companies);
            manager.UpdateBookmakers(config.companies);
            manager.Start(config.eventsUpdate);



        }
    }
}