namespace ForkBro.Scanner.EventLinks
{
    public interface IGameList
    {
        public bool Success { get;}
        public IEventLink[] EventsArray { get; }
    }
}