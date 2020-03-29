using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.Common;
using ForkBro.Mediator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                //logic
                var pool = hub.GetNextPool();
                if (pool != null)
                {
                    var props = pool.props;
                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(pool, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText($"Logs\\Pools\\{pool.props.sport}_P{pool.GetSnapshot(Bookmaker._1xbet).PoolId}_E{pool.GetSnapshot(Bookmaker._1xbet).EventId}_.json", str);//DEBUG
                    //Thread.Sleep(TimeSpan.FromMinutes(0.5));
                }
                else
                    await Task.Delay(1000, stoppingToken);
            }

        }
        //https://docs.microsoft.com/ru-ru/dotnet/standard/parallel-programming/task-based-asynchronous-programming
        //Task.Factory.StartNew(
    }
}
