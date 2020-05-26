using System.Collections.Generic;
using Newtonsoft.Json;

namespace ForkBro.Scanner.EventLinks
{
    public class GameList_1xBet : IGameList
    {
        public bool Success { get; set; }
        [JsonProperty("Value")]
        public List<EventLink_1xbet> events { get; set; }
        public IEventLink[] EventsArray => events.ToArray();
    }
}