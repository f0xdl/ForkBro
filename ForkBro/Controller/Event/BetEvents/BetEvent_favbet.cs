
using System;
using System.Collections.Generic;
using ForkBro.Model;
using ForkBro.Model.EventModel;
using Newtonsoft.Json;
namespace ForkBro.Controller.Event.BetEvents
{
    

    public class BetEvent_favbet
    {
        //public int category_id { get; set; }
        //public string category_name { get; set; }
        //public int category_weigh { get; set; }
        //public int country_id { get; set; }
        //public string event_broadcast_url { get; set; }
        //public List<string> event_comment_template_comment { get; set; }
        //public int event_dt { get; set; }
        //public int event_edition { get; set; }
        //public int? event_enet_id { get; set; }
        public int event_id { get; set; }//
        //public int? event_line_position { get; set; }
        public string event_name { get; set; }//
        //public List<object> event_tag { get; set; }
        //public EventTv event_tv { get; set; }
        //public int event_weigh { get; set; }
        //public int sport_id { get; set; }
        public string sport_name { get; set; }
        //public int sport_weigh { get; set; }
        //public int tournament_id { get; set; }
        //public string tournament_name { get; set; }
        //public int tournament_weigh { get; set; }

        public EventPool ConvertToBetEvent()
        {
            EventPool betEvent = new EventPool();
            betEvent.id = event_id;
            switch (sport_name)
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
            string[] CommandName = event_name.Split(" - ");
            Command commandA = new Command()
            {
                City = "",
                NameEng = CommandName[0],
                Name = "",
                Id = 0
            };
            Command commandB = new Command()
            {
                City = "",
                NameEng = CommandName[1],
                Name = "",
                Id = 0
            };
            betEvent.commands = new Command[] { commandA, commandB };

            //TODO определить завершённые игры
            //betEvent.eventOver = true;

            return betEvent;
        }
    }

    public class GameList_favbet
    {
        public bool Success => events!=null && events.Count > 0;
        [JsonProperty("events")]
        public List<BetEvent_favbet> events { get; set; }
    }

    //public class EventTv
    //{
    //    public List<object> countries { get; set; }
    //    public bool is_hd { get; set; }
    //    public bool is_fta { get; set; }
    //}
}