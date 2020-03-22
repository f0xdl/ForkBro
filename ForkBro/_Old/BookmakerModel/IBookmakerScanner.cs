using ForkBro.Controller.Client;
using ForkBro.Common;

namespace ForkBro.Controller.Scanner
{
    internal interface IBookmakerScanner
    {
        Bookmaker BookmakerName { get; set; }
        //BaseHttpRequest httpClient { get; set; }
        BookmakerEvent newEvent();
    }
}