using ForkBro.BookmakerModel;
using ForkBro.BookmakerModel.BaseEvents;
using ForkBro.Common;
using ForkBro.Scanner.EventLinks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NLog;

namespace ForkBro.Common.BookmakerClient
{
    public class HttpRequest_favbet : BaseRequest
    {
        public HttpRequest_favbet() { bookmaker = Bookmaker._favbet; }

        public override T GetBetOdds<T>(long eventId)
        {
            //Получение данных с конторы
            var httpResult = PostAsync(@"https://www.favbet.com/frontend_api2/",
                "{\"jsonrpc\":\"2.0\",\"method\":\"frontend/market/get\",\"id\":2379,\"params\":{\"by\":{\"lang\":\"ru\","
                + "\"service_id\":1,\"event_id\":" + eventId.ToString() + "}}}",
                "application/json"
            ).Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(httpResult);
        }

        public override bool TestConnection()
        {
            var result = GetBetOdds<RequestEvent_favbet>(-1);
            return result.Id > 0;
        }

        public override IGameList GetEventsList()
        {
            string httpResult = PostAsync(@"https://www.favbet.com/frontend_api/events_short/",
                                           "{\"lang\":\"en\",\"service_id\":1}"
                                           , "application/json"
                                           ).Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GameList_favbet>(httpResult);
        }
        
    }
}