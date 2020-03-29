using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkBro.Configuration;
using ForkBro.BookmakerModel;
using ForkBro.Daemons;
using ForkBro.Mediator;
using ForkBro.Scanner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ForkBro.Common;

namespace ForkBro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostContext, logging) =>
                {
                logging.AddFile(hostContext.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((hostContext, services) =>
                {
        #region ISetting [HostBuilderContext]
        services.AddTransient<ISetting, AppSettings>();
        #endregion

        #region Mediator [ISetting]
        services.AddSingleton<HubMediator>();
        services.AddSingleton<IScannerMediator>(x => x.GetRequiredService<HubMediator>());
        services.AddSingleton<IBookmakerMediator>(x => x.GetRequiredService<HubMediator>());
        services.AddSingleton<IDaemonMasterMediator>(x => x.GetRequiredService<HubMediator>());
        services.AddSingleton<IApiMediator>(x => x.GetRequiredService<HubMediator>());
        #endregion

        #region LiveScanner [ILogger,IScannerMediator,ISetting]
        services.AddHostedService<LiveScanner>();
        #endregion

        #region BookmakerService [ILogger,IBookmakerMediator]
        foreach (var item in (new AppSettings(hostContext)).Companies)
            switch (item.id)
            {
                case Bookmaker._favbet:
                    services.AddHostedService<Service_favbet>(); break;
                case Bookmaker._1xbet:
                    services.AddHostedService<Service_1xbet>(); break;
                default: throw new Exception("Неизвесный тип букмекера: " + item.id.ToString());
            }
        #endregion

        #region DaemonMaster [ILogger,ICalcDaemonMediator]
        services.AddHostedService<DaemonMaster>();
                    #endregion

        #region Worker [ILogger,HubMediator]
        //services.AddHostedService<Worker>(); //TODO Worker
        #endregion
                });
    }
}
