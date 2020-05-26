using System.Collections.Generic;
using Newtonsoft.Json;

namespace ForkBro.Scanner.EventLinks
{
    public class GameList_favbet : IGameList
    {
        public bool Success => events != null && events.Count > 0;
        [JsonProperty("events")]
        public List<EventLink_favbet> events { get; set; }
        public IEventLink[] EventsArray => events.ToArray();
    }
}