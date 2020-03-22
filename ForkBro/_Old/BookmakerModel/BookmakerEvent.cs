using ForkBro.Common;

namespace FrokBro.BookmakerModel
{
    public struct BookmakerEvent
    {
        public int EventID;
        public readonly Bookmaker bookmaker;
        public StatusEvent status;
    }
}