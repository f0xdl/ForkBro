using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using NLog.Extensions.Logging;

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
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("logSettings.json", false, false);
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    var section = hostContext.Configuration.GetSection("NLog");
                    logging.AddNLog(new NLogLoggingConfiguration(section));
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // <InterfaceName> [<InputParams>]

                    #region IOptions [HostBuilderContext]
                    services.Configure<AppSettings>(hostContext.Configuration);

                    #endregion

                    #region Mediator [ILogger,IOptions<AppSettings>]

                    services.AddSingleton<HubMediator>(); //Прикрепление одной реализации на несколько интерфейсов
                    services.AddSingleton<IScannerMediator>(x => x.GetRequiredService<HubMediator>());
                    services.AddSingleton<IBookmakerMediator>(x => x.GetRequiredService<HubMediator>());
                    services.AddSingleton<IDaemonMasterMediator>(x => x.GetRequiredService<HubMediator>());
                    services.AddSingleton<IApiMediator>(x => x.GetRequiredService<HubMediator>());

                    #endregion

                    #region LiveScanner [ILogger,IScannerMediator,IOptions<AppSettings>]

                    services.AddHostedService<LiveScanner>();

                    #endregion

                    #region BookmakerService [ILogger,IBookmakerMediator]
                    {
                        AppSettings appSettings = new AppSettings();
                        hostContext.Configuration.Bind(appSettings);

                        foreach (Bookmaker item in appSettings.GetEBookmakers())
                            switch (item)
                            {
                                case Bookmaker._favbet:
                                    services.AddHostedService<ServiceFavbet>();
                                    break;
                                case Bookmaker._1xbet:
                                    services.AddHostedService<Service1xBet>();
                                    break;
                                default: throw new Exception("Неизвесный тип букмекера: " + item.ToString());
                            }
                    }
                    #endregion

                #region DaemonMaster [ILogger,ICalcDaemonMediator]

                    services.AddHostedService<DaemonMaster>();

                    #endregion
                });
    }
}