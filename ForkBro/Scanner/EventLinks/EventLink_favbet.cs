using System;
using System.Collections.Generic;
using ForkBro.Common;
using ForkBro.OnlineScanner.EventLinks;
using Newtonsoft.Json;

namespace ForkBro.Scanner.EventLinks
{
    public class EventLink_favbet: IEventLink
    {
        public Bookmaker bookmaker { get; set; }
        public bool updated { get; set; }
        public StatusEvent status { get; set; }
        public long id => event_id;
        public Sport sport
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
        public Command commandA => new Command()
        {
            City = "",
            NameEng = event_name.Split(" - ")[0],
            Name = "",
            Id = 0
        };
        public Command commandB => new Command()
        {
            City = "",
            NameEng = event_name.Split(" - ")[1],
            Name = "",
            Id = 0
        };

        public int event_id { get; set; }
        public string event_name { get; set; }
        public string sport_name { get; set; }

        //public EventLink ConvertToBetEvent()
        //{
        //    EventLink eventLink = new EventLink();
        //    eventLink.id = event_id;
        //    switch (sport_name)
        //    {
        //        case "Football":
        //            eventLink.sport = Sport.Football; break;
        //        case "Basketball":
        //            eventLink.sport = Sport.Basketball; break;
        //        case "Ice Hockey":
        //            eventLink.sport = Sport.Hockey; break;
        //        case "Volleyball":
        //            eventLink.sport = Sport.VoleyBall; break;
        //        case "Handball":
        //            eventLink.sport = Sport.Handball; break;
        //        case "Baseball":
        //            eventLink.sport = Sport.Baseball; break;
        //        case "Table Tennis":
        //            eventLink.sport = Sport.TableTennis; break;
        //        case "Tennis":
        //            eventLink.sport = Sport.Tennis; break;
        //        default:
        //            eventLink.sport = Sport.None; break;
        //    }//Определение вида спорта
        //    string[] CommandName = event_name.Split(" - ");
        //    Command commandA = new Command()
        //    {
        //        City = "",
        //        NameEng = CommandName[0],
        //        Name = "",
        //        Id = 0
        //    };
        //    Command commandB = new Command()
        //    {
        //        City = "",
        //        NameEng = CommandName[1],
        //        Name = "",
        //        Id = 0
        //    };
        //    eventLink.commands = new Command[] { commandA, commandB };

        //    //TODO определить завершённые игры
        //    //betEvent.eventOver = true;

        //    return eventLink;
        //}
    }

    public class GameList_favbet
    {
        public bool Success => events!=null && events.Count > 0;
        [JsonProperty("events")]
        public List<EventLink_favbet> events { get; set; }
    }
}