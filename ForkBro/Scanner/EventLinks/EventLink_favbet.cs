using System;
using System.Collections.Generic;
using ForkBro.Common;
using Newtonsoft.Json;

namespace ForkBro.Scanner.EventLinks
{
    public class EventLink_favbet: IEventLink
    {
        public Bookmaker Bookmaker { get; set; }
        public bool Updated { get; set; }
        public StatusEvent Status { get; set; }
        public long Id => event_id;
        public Sport Sport
        {
            get
            {
                switch (sport_name)
                {
                    case "Football":
                        return Sport.Football;
                    case "Basketball":
                        return Sport.Basketball;
                    case "Ice Hockey":
                        return Sport.Hockey;
                    case "Volleyball":
                        return Sport.VoleyBall;
                    case "Handball":
                        return Sport.Handball;
                    case "Baseball":
                        return Sport.Baseball;
                    case "Table Tennis":
                        return Sport.TableTennis;
                    case "Tennis":
                        return Sport.Tennis;
                    default:
                        return Sport.None;
                }//Определение вида спорта
            }
        }
        public Command CommandA => new Command()
        {
            City = "",
            NameEng = event_name.Split(" - ")[0],
            Name = "",
            Id = 0
        };
        public Command CommandB => new Command()
        {
            City = "",
            NameEng = event_name.Split(" - ")[1],
            Name = "",
            Id = 0
        };

        public EventLink_favbet() => Bookmaker = Bookmaker._favbet;

        public int event_id { get; set; }
        public string event_name { get; set; }
        public string sport_name { get; set; }
    }

    public class GameList_favbet
    {
        public bool Success => events!=null && events.Count > 0;
        [JsonProperty("events")]
        public List<EventLink_favbet> events { get; set; }
    }
}