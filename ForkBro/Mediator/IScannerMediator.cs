using ForkBro.Common;
using ForkBro.Scanner.EventLinks;

namespace ForkBro.Mediator
{
    public interface IScannerMediator
    {
        Bookmaker[] GetBookmakers();
        void EventEnqueue(IEventLink link);
        void UpdateScannerStatus();
        int ScannerDelay { get; set; }
    }
}