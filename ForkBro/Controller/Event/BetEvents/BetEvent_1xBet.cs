
using System;
using System.Collections.Generic;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using Newtonsoft.Json;
namespace ForkBro.Controller.Event.BetEvents
{

    public class BetEvent_1xBet
    {
        public int I { get; set; }//ID матча
        //public List<object> AE { get; set; }
        //public string CHIMG { get; set; }
        //public string CN { get; set; }
        //public int CO { get; set; }
        //public int COI { get; set; }
        //public List<object> E { get; set; }
        //public int EC { get; set; }
        //public bool HLU { get; set; }
        //public int HS { get; set; }
        //public bool HSI { get; set; }
        //public int IV { get; set; }
        //public int KI { get; set; }
        public string L { get; set; }//Лига
        public string LE { get; set; }//Лига [Eng]
        public int LI { get; set; }//id Лиги?
        //public MIO MIO { get; set; }
        //public List<MI> MIS { get; set; }
        //public int N { get; set; }
        public string O1 { get; set; }  //Название
        //public int O1C { get; set; }
        public string O1CT { get; set; }//Город?
        public string O1E { get; set; } //Название [Eng]
        public int O1I { get; set; }    //ID команды?
        //public List<string> O1IMG { get; set; }
        //public List<int> O1IS { get; set; }
        public string O2 { get; set; }  //Название
        //public int O2C { get; set; }
        public string O2CT { get; set; }//Город?
        public string O2E { get; set; } //Название [Eng]
        public int O2I { get; set; }    //ID команды?
        //public List<string> O2IMG { get; set; }
        //public List<int> O2IS { get; set; }
        //public int S { get; set; }  //
        public string SE { get; set; } //Спорт
        //public int SGC { get; set; }
        //public string SGI { get; set; }
        //public int SI { get; set; }
        //public string SN { get; set; }
        //public int SS { get; set; }
        //public int SST { get; set; }
        //public string STI { get; set; }
        //public int T { get; set; }
        //public string TN { get; set; }
        //public string V { get; set; }
        //public string VE { get; set; }
        //public int HMH { get; set; }
        //public int R { get; set; }
        //public SC SC { get; set; }
        //public int ZP { get; set; }
        //public int? MG { get; set; }
        //public string TG { get; set; }
        //public int? TI { get; set; }
        //public int? VA { get; set; }
        //public string VI { get; set; }

        public EventPool ConvertToBetEvent()
        {
            EventPool betEvent = new EventPool();
            betEvent.id = I;
            switch(SE)
            {
                //case "Football":
                //    betEvent.sport = ESport.Football; break;
                case "Basketball":
                    betEvent.sport = ESport.Basketball; break;
                //case "Ice Hockey":
                //    betEvent.sport = ESport.Hockey; break;                
                //case "Volleyball":
                //    betEvent.sport = ESport.VoleyBall; break;
                //case "Handball":
                //    betEvent.sport = ESport.Handball; break;
                //case "Baseball":
                //    betEvent.sport = ESport.Baseball; break;
                //case "Table Tennis":
                //    betEvent.sport = ESport.TableTennis; break;
                case "Tennis":
                    betEvent.sport = ESport.Tennis; break;
                default:
                    betEvent.sport = ESport.None; break;
            }//Определение вида спорта

            Command commandA = new Command()
            {
                City = O1CT,
                NameEng = O1E,
                Name = O1,
                Id = O1I
            };
            Command commandB = new Command()
            {
                City = O2CT,
                NameEng = O2E,
                Name = O2,
                Id = O2I
            };
            betEvent.commands = new Command[] { commandA, commandB };

            //TODO определить завершённые игры
            //betEvent.eventOver = true;

            return betEvent;
        }
    }

    public class GameList_1xBet
    {
        public bool Success { get; set; }
        [JsonProperty("Value")]
        public List<BetEvent_1xBet> events { get; set; }
    }
}