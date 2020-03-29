using ForkBro.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForkBro.Configuration
{
    public class AppSettings : ISetting
    {
        public int LiveScanRepeat { get; set; }
        public int CountDaemon { get; set; }
        public int CountPool { get; set; }
        public double MinQuality { get; set; }

        public List<BookmakersProp> Companies { get; set; }
        public Sport[] TrackedSports { get; set; }

        public Bookmaker[] GetEBookmakers()
        {
            Bookmaker[] bookmakers;
            if (Companies == null)
                bookmakers = new Bookmaker[0];
            else
                bookmakers = Companies.Where(x => x.enable)
                                      .Select(x => x.id)
                                      .ToArray();
            return bookmakers;
        }

        public AppSettings(HostBuilderContext hostContext)
        {
            hostContext.Configuration.Bind("AppSetting", this);
        }
    }
}

