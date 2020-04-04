using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.Common;
using ForkBro.Configuration;
using ForkBro.Mediator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Daemons
{
    class DaemonMaster: BackgroundService
    {
        IDaemonMasterMediator hub;
        List<Task<Fork[]>> forks;
        private readonly ILogger<DaemonMaster> _logger;
        readonly int daemonCount;
        public DaemonMaster(ILogger<DaemonMaster> logger, IDaemonMasterMediator mediator, ISetting setting)
        {
            forks = new List<Task<Fork[]>>(mediator.CountDaemons);
            hub = mediator;
            daemonCount = setting.CountDaemon;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Создание потоков
            CalcDaemon[] daemons = new CalcDaemon[daemonCount];
            for (int i = 0; i < daemons.Length; i++)
            {
                daemons[i] = new CalcDaemon(_logger, hub, i);
                daemons[i].Start(500);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //Проверка состояние потоков, запуск
                    foreach (var daemon in daemons)
                        if (!daemon.IsAlive)
                            daemon.Start(500);
                }
                catch (Exception ex) 
                { 
                    _logger.LogError(ex, "Ошибка в менеджере вычислений");
                }
                finally
                {
                    await Task.Delay(5000, stoppingToken);
                }
            }

            //завершение потоков
            for (int i = 0; i < daemons.Length; i++)
                daemons[i].Stop(1000);
        }
      
    }
}
