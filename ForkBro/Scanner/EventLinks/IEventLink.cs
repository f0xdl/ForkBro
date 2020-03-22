using ForkBro.Common;

namespace ForkBro.Scanner.EventLinks
{
    public interface IEventLink
    {
        public Bookmaker Bookmaker { get; set; }
        public bool Updated { get; set; }
        public StatusEvent Status { get; set; }

        public long Id{ get; }
        public Sport Sport{ get; }
        public Command CommandA{ get; }
        public Command CommandB { get; }
    }
}