using ForkBro.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.Configuration
{
    public interface ISetting
    {
        int LiveScanRepeat { get; set; }
        int CountDaemon { get; set; }
        int CountPool { get; set; }
        double MinQuality { get; set; }

        List<BookmakersProp> Companies { get; set; }
        Sport[] TrackedSports { get; set; }
        Bookmaker[] GetEBookmakers();
    }

    public class BookmakersProp
    {
        public Bookmaker id { get; set; }
        public bool enable { get; set; }
        public int repeat { get; set; }
    }
}
