using System;
using System.Collections.Generic;
using ForkBro.Common;
using Newtonsoft.Json;

namespace ForkBro.Scanner.EventLinks
{

    public class EventLink_1xbet : IEventLink
    {
        public Bookmaker Bookmaker { get; set; }
        public bool Updated { get; set; }
        public StatusEvent Status { get; set; }
        public long Id => I;
        public Sport Sport
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
        public Command CommandA => new Command()
        {
            City = O1CT,
            NameEng = O1E,
            Name = O1,
            Id = O1I
        };
        public Command CommandB => new Command()
        {
            City = O2CT,
            NameEng = O2E,
            Name = O2,
            Id = O2I
        };

        public EventLink_1xbet() => Bookmaker = Bookmaker._1xbet;

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
    }

    public class GameList_1xBet
    {
        public bool Success { get; set; }
        [JsonProperty("Value")]
        public List<EventLink_1xbet> events { get; set; }
    }
}