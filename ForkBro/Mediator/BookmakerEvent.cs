using ForkBro.Common;

namespace ForkBro.Bookmakers
{
    public class BookmakerEvent
    {
        internal long EventId;
        internal StatusEvent status;
        internal Bookmaker bookmaker;
        internal int poolId;
        public bool reverse;
    }
}