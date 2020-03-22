using ForkBro.Common;
using ForkBro.OnlineScanner.EventLinks;

namespace ForkBro.Scanner.EventLinks
{
    public interface IEventLink
    {
        public Bookmaker bookmaker { get; set; }
        public bool updated { get; set; }
        public StatusEvent status { get; set; }

        public long id{ get; }
        public Sport sport{ get; }
        public Command commandA{ get; }
        public Command commandB { get; }
    }
}