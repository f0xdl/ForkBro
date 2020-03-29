using ForkBro.Common;
using ForkBro.Scanner;
using System;

namespace ForkBro.Mediator
{
    public struct EventProps
    {//TODO заменить на обращение к EventBase
        public DateTime StartDT;
        public DateTime OverDT;

        public Sport sport;
        public StatusEvent status;

        public Command CommandA { get; set; }
        public Command CommandB { get; set; }
    }
}