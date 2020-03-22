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
    abstract class BookmakerService : BackgroundService
    {
        readonly Bookmaker bookmaker;
        int delay;
        IBookmakerMediator hub;
        private readonly ILogger<BookmakerService> _logger;

        public BookmakerService(ILogger<BookmakerService> logger, IBookmakerMediator mediator)
        {
            _logger = logger;
            hub = mediator;
            delay = mediator.GetBookmakerDelay(bookmaker);
        }

        public static BookmakerService CreateBookmaker(ILogger<Worker> logger, IBookmakerMediator mediator, Bookmaker _bookmaker, int delay)
        {
            BookmakerService bookmaker;
            switch (_bookmaker)
            {
                //case Bookmaker._10bet:bookmaker = new BookmakerModel1();break;
                default: throw new Exception($"Букмекера {_bookmaker} не существует");
            }
            bookmaker.SetDelay(delay);
            return bookmaker;
        }

        public bool SetDelay(int ms)
        {
            bool result;
            if (0 < ms)
            {
                delay = ms;
                result = true;
            }
            else
                result = false;

            return result;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            { 
                _logger.LogInformation("BookmakerModel{id} running at: {time}", bookmaker.ToString(), DateTimeOffset.Now);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }

}
