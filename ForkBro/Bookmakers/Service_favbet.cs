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
    class Service_favbet : BookmakerService
    {
        public Service_favbet(ILogger<Service_favbet> logger, IBookmakerMediator mediator)
            :base(logger,mediator, Bookmaker._favbet)
        {
        }

        protected internal override async void OnWork()
        {
            throw new NotImplementedException();
        }
    }
}
