using ForkBro.Model;

namespace ForkBro.Model
{
    public struct RefEvent
    {
        public Bookmaker bookmaker;
        public long id;
        public bool updated;
        public StatusEvent status;
    }
}