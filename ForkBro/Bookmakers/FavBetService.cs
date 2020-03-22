using ForkBro.Mediator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForkBro.Bookmakers
{
    class FavBetService : BackgroundService
    {
        ILogger<FavBetService> _logger;
        IBookmakerMediator hub;
        public FavBetService(ILogger<FavBetService> logger, IBookmakerMediator mediator)
        {
            _logger = logger;
            hub = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
