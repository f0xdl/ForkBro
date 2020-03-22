using ForkBro.Mediator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Daemons
{
    class DaemonMaster: BackgroundService
    {
        IDaemonMasterMediator hub;
        List<Task<Fork[]>> forks;
        private readonly ILogger<Worker> _logger;

        public DaemonMaster(ILogger<Worker> logger, IDaemonMasterMediator mediator)
        {
            forks = new List<Task<Fork[]>>(mediator.CountDaemons);
            hub = mediator;
        }
            
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1, stoppingToken);
            }
        }
        //https://docs.microsoft.com/ru-ru/dotnet/standard/parallel-programming/task-based-asynchronous-programming
        //Task.Factory.StartNew(
    }
}
