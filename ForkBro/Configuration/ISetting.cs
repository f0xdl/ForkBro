using ForkBro.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.Configuration
{
    public interface ISetting
    {
        public int LiveScanRepeat { get; set; }
        public int CountDaemon { get; set; }
        public List<BookmakersProp> Companies { get; set; }
        int CountPool { get; set; }
        double MinQuality { get; set; }

        public Bookmaker[] GetEBookmakers();
    }

    public class BookmakersProp
    {
        public Bookmaker id { get; set; }
        public bool enable { get; set; }
        public int repeat { get; set; }
    }
}
