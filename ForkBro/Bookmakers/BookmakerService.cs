using ForkBro.Common;
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
    public abstract class BookmakerService : BackgroundService
    {
        protected internal readonly Bookmaker bookmaker;
        protected internal int delay;
        protected internal IBookmakerMediator hub;
        protected internal readonly ILogger<BookmakerService> _logger;

        public BookmakerService(ILogger<BookmakerService> logger, IBookmakerMediator mediator, Bookmaker BM)
        {
            _logger = logger;
            hub = mediator;
            bookmaker = BM;
            delay = mediator.GetBookmakerDelay(BM);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                OnWork();
                await Task.Delay(delay, stoppingToken);
            }
        }
        protected internal abstract void OnWork();
    }
}
