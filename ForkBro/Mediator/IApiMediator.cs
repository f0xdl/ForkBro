using System;
using System.Collections.Generic;
using ForkBro.Common;

namespace ForkBro.Mediator
{
    public interface IApiMediator
    {
        Dictionary<Bookmaker, int> CountEvents();
        Dictionary<Bookmaker, DateTime> GetLastUpdate();

    }
}