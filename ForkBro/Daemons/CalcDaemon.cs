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
    public abstract class CalcDaemon
    {
        long index;
        public CalcDaemon(int id)
        {
            index = id;
        }
    }
}
