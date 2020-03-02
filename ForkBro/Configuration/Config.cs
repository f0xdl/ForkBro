using ForkBro.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForkBro.Configuration
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct Config
    {
        [JsonProperty(Order = 0)]
        public int eventsUpdate; //Период обновление списка игр на букмекерах 
        [JsonProperty(Order = 1)]
        public CompanyProp[] companies;

        public static Config Empty => new Config() {
            companies = new CompanyProp[0],
            eventsUpdate = 300000
        };
        public EBookmakers[] GetEBookmakers()
        {
            EBookmakers[] bookmakers;
            if (companies == null)
                bookmakers = new EBookmakers[0];
            else
                bookmakers = companies.Where(x => x.enable)
                                      .Select(x => x.companyID)
                                      .ToArray();
            return bookmakers;
        }
    }

    public struct CompanyProp
    {
        [JsonProperty(Order = 0)]
        public EBookmakers companyID;
        [JsonProperty(Order = 1)]
        public bool enable;
        [JsonProperty(PropertyName = "Refresh Coef", Order = 2)]
        public int mapRepeat;

        public static CompanyProp Empty => new CompanyProp()
        {
            companyID = 0,
            enable = false,
            mapRepeat = 1000
        };
    }

    
}