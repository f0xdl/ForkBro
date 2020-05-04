using ForkBro.BookmakerModel;
using ForkBro.Common;
using ForkBro.Scanner.EventLinks;

namespace ForkBro.Mediator
{
    public interface IBookmakerMediator
    {
        bool HaveNewLink();
        bool TryGetNewEvent(Bookmaker bookmaker, out IEventLink link);
        void AddEvent(IEventLink link, ref BetEvent bookmakerEvent);
        void OverEvent(int idPool, Bookmaker bm);
        int GetEventPoolId(Bookmaker bookmaker, long id);
        
        void UpdateBookmakerStatus(Bookmaker bm);
    }
}