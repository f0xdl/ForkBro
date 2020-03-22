using ForkBro.Common;
using ForkBro.OnlineScanner.EventLinks;

namespace ForkBro.Scanner.EventLinks
{
    public struct dasdaEventLink
    {
        public Bookmaker bookmaker;
        public long id;
        public bool updated;
        public StatusEvent status;
        public Sport sport;
        public Command[] commands;
    }
}