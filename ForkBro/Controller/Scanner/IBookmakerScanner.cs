using ForkBro.Controller.Client;
using ForkBro.Model;

namespace ForkBro.Controller.Scanner
{
    internal interface IBookmakerScanner
    {
        Bookmaker BookmakerName { get; set; }
        //BaseHttpRequest httpClient { get; set; }
        BookmakerEvent newEvent();
    }
}