using ForkBro.Common;
using System.Collections.Generic;
using System.IO;
using ForkBro.Scanner.EventLinks;
using ForkBro.BookmakerModel;
using System;
using System.Collections.Concurrent;
using ForkBro.BookmakerModel.BaseEvents;
using System.Linq;
using System.Diagnostics;

namespace ForkBro.Common.BookmakerClient
{
    public class HttpRequest_1xbet : BaseRequest
    {
        public HttpRequest_1xbet() => bookmaker = Bookmaker._1xbet;
        
        public override IGameList GetEventsList()
        {
            string httpResult = GetAsync(@"https://1xbet.com/LiveFeed/Get1x2_VZip", "count=500&mode=8").Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_1xBet>(httpResult);
        }

        public override T GetBetOdds<T>(long eventId)
        {
            //Получение данных с конторы
            var httpResult = GetAsync(@"https://1xbet.com/LiveFeed/GetGameZip",
                $"id={eventId.ToString()}&isSubGames=true&GroupEvents=true&allEventsGroupSubGames=true&countevents=250").Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(httpResult);
        }

        public override bool TestConnection() => 
            Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_1xBet>(
                GetAsync(@"https://1xbet.com/LiveFeed/Get1x2_VZip","")
                    .Result
                ).Success;
    }
}