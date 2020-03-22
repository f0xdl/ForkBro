using System;
using System.Collections.Generic;
using ForkBro.Common;
using Newtonsoft.Json;
using ForkBro.OnlineScanner.EventLinks;

namespace ForkBro.Scanner.EventLinks
{

    public class EventLink_1xBet : IEventLink
    {
        public Bookmaker bookmaker { get; set; }
        public bool updated { get; set; }
        public StatusEvent status { get; set; }
        public long id => I;
        public Sport sport
        {
            get
            {
                switch (SE)
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
            City = O1CT,
            NameEng = O1E,
            Name = O1,
            Id = O1I
        };
        public Command commandB => new Command()
        {
            City = O2CT,
            NameEng = O2E,
            Name = O2,
            Id = O2I
        };

        public int I { get; set; }//ID матча
        public string L { get; set; }//Лига
        public string LE { get; set; }//Лига [Eng]
        public int LI { get; set; }//id Лиги?
        public string O1 { get; set; }  //Название
        public string O1CT { get; set; }//Город?
        public string O1E { get; set; } //Название [Eng]
        public int O1I { get; set; }    //ID команды?
        public string O2 { get; set; }  //Название
        public string O2CT { get; set; }//Город?
        public string O2E { get; set; } //Название [Eng]
        public int O2I { get; set; }    //ID команды?
        public string SE { get; set; } //Спорт


        //public EventLink ConvertToBetEvent()
        //{
        //    //EventLink eventLink = new EventLink();
        //    //eventLink.id = I;
        //    //switch(SE)
        //    //{
        //    //    case "Football":
        //    //        eventLink.sport = Sport.Football; break;
        //    //    case "Basketball":
        //    //        eventLink.sport = Sport.Basketball; break;
        //    //    case "Ice Hockey":
        //    //        eventLink.sport = Sport.Hockey; break;
        //    //    case "Volleyball":
        //    //        eventLink.sport = Sport.VoleyBall; break;
        //    //    case "Handball":
        //    //        eventLink.sport = Sport.Handball; break;
        //    //    case "Baseball":
        //    //        eventLink.sport = Sport.Baseball; break;
        //    //    case "Table Tennis":
        //    //        eventLink.sport = Sport.TableTennis; break;
        //    //    case "Tennis":
        //    //        eventLink.sport = Sport.Tennis; break;
        //    //    default:
        //    //        eventLink.sport = Sport.None; break;
        //    //}//Определение вида спорта

        //    //Command commandA = new Command()
        //    //{
        //    //    City = O1CT,
        //    //    NameEng = O1E,
        //    //    Name = O1,
        //    //    Id = O1I
        //    //};
        //    //Command commandB = new Command()
        //    //{
        //    //    City = O2CT,
        //    //    NameEng = O2E,
        //    //    Name = O2,
        //    //    Id = O2I
        //    //};
        //    //eventLink.commands = new Command[] { commandA, commandB };

        //    //TODO определить завершённые игры
        //    //betEvent.eventOver = true;

        //    return eventLink;
        //}
    }

    public class GameList_1xBet
    {
        public bool Success { get; set; }
        [JsonProperty("Value")]
        public List<EventLink_1xBet> events { get; set; }
    }
}