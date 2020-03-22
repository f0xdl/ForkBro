using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ForkBro.Bookmakers;
using ForkBro.Configuration;
using ForkBro.Daemons;
using ForkBro.Mediator;
using ForkBro.Scanner;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ForkBro
{
    public class Worker : BackgroundService
    {
        //UI API - SERVICE STATUS 
        private readonly ILogger<Worker> _logger;
        HubMediator hub;

        public Worker(ILogger<Worker> logger,HubMediator mediator)
        {
            hub = mediator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var item in hub.GetLastUpdate())
                    _logger.LogInformation("{string} running at: {time}",item.Key.ToString(), item.Value);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}


