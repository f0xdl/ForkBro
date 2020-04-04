using ForkBro.BookmakerModel.BaseEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ForkBro.Common
{
   public struct Fork
    {
        //Data
        public double Percent; //0.11
        public long Timestamp;

        //Event Type
        [JsonConverter(typeof(StringEnumConverter))]
        public Sport Sport;
        [JsonConverter(typeof(StringEnumConverter))]
        public EventUnit Unit;
        [JsonConverter(typeof(StringEnumConverter))]
        public BetType Type;

        //EventA
        public long IdEventA;
        [JsonConverter(typeof(StringEnumConverter))] 
        public Bookmaker BookmakerA;
        public double ValueA;
        public double CoefficientA;
        public bool ReverseA;

        //EventB 
        public long IdEventB;
        [JsonConverter(typeof(StringEnumConverter))] 
        public Bookmaker BookmakerB;
        public double ValueB;
        public double CoefficientB;
        public bool ReverseB;

    }
}