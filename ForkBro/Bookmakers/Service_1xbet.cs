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
    class Service_1xbet : BookmakerService
    {
        public Service_1xbet(ILogger<Service_favbet> logger, IBookmakerMediator mediator)
            :base(logger,mediator, Bookmaker._1xbet)
        {
        }

        protected internal override async void OnWork()
        {
            //TODO throw new NotImplementedException();
        }
    }
}
