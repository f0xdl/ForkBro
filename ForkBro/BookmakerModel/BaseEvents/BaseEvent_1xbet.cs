using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.BookmakerModel.BaseEvents
{
    public class E
    {

        [JsonProperty("C")]
        public double C { get; set; }

        [JsonProperty("G")]
        public int G { get; set; }

        [JsonProperty("P")]
        public double P { get; set; }

        [JsonProperty("T")]
        public int T { get; set; }

        [JsonProperty("CE")]
        public int? CE { get; set; }
    }

    //public class MEC
    //{

    //    [JsonProperty("EC")]
    //    public int EC { get; set; }

    //    [JsonProperty("MT")]
    //    public int MT { get; set; }
    //}

    //public class MIO
    //{

    //    [JsonProperty("MaF")]
    //    public string MaF { get; set; }
    //}

    //public class MI
    //{

    //    [JsonProperty("K")]
    //    public int K { get; set; }

    //    [JsonProperty("V")]
    //    public string V { get; set; }
    //}

    //public class FS
    //{

    //    [JsonProperty("S1")]
    //    public int S1 { get; set; }

    //    [JsonProperty("S2")]
    //    public int S2 { get; set; }
    //}

    //public class ValueP
    //{

    //    [JsonProperty("S1")]
    //    public int S1 { get; set; }

    //    [JsonProperty("S2")]
    //    public int S2 { get; set; }
    //}

    //public class P
    //{

    //    [JsonProperty("Key")]
    //    public int Key { get; set; }

    //    [JsonProperty("Value")]
    //    public ValueP Value { get; set; }
    //}

    //public class Value
    //{

    //    [JsonProperty("ID")]
    //    public int ID { get; set; }

    //    [JsonProperty("N")]
    //    public object N { get; set; }

    //    [JsonProperty("S1")]
    //    public string S1 { get; set; }

    //    [JsonProperty("S2")]
    //    public string S2 { get; set; }
    //}

    //public class ST
    //{

    //    [JsonProperty("Key")]
    //    public int Key { get; set; }

    //    [JsonProperty("Value")]
    //    public IList<Value> Value { get; set; }
    //}

    //public class SC
    //{

    //    [JsonProperty("CP")]
    //    public int CP { get; set; }

    //    [JsonProperty("CPS")]
    //    public string CPS { get; set; }

    //    [JsonProperty("FS")]
    //    public FS FS { get; set; }

    //    [JsonProperty("PS")]
    //    public IList<P> PS { get; set; }

    //    [JsonProperty("S")]
    //    public IList<object> S { get; set; }

    //    [JsonProperty("ST")]
    //    public IList<ST> ST { get; set; }

    //    [JsonProperty("TR")]
    //    public int TR { get; set; }

    //    [JsonProperty("TS")]
    //    public int TS { get; set; }
    //}

    //public class SG
    //{

    //    [JsonProperty("I")]
    //    public int I { get; set; }

    //    [JsonProperty("N")]
    //    public int N { get; set; }

    //    [JsonProperty("P")]
    //    public int P { get; set; }

    //    [JsonProperty("PN")]
    //    public string PN { get; set; }

    //    [JsonProperty("SI")]
    //    public int SI { get; set; }

    //    [JsonProperty("SS")]
    //    public int SS { get; set; }

    //    [JsonProperty("T")]
    //    public int T { get; set; }

    //    [JsonProperty("R")]
    //    public int R { get; set; }
    //}

    public class BaseEvent_1xbet
    {

        //[JsonProperty("CHIMG")]
        //public string CHIMG { get; set; }

        //[JsonProperty("CN")]
        //public string CN { get; set; }

        //[JsonProperty("CO")]
        //public int CO { get; set; }

        //[JsonProperty("COI")]
        //public int COI { get; set; }

        [JsonProperty("E")]
        public IList<E> E { get; set; }

        //[JsonProperty("EC")]
        //public int EC { get; set; }

        //[JsonProperty("I")]
        //public int I { get; set; }

        //[JsonProperty("IV")]
        //public int IV { get; set; }

        //[JsonProperty("KI")]
        //public int KI { get; set; }

        //[JsonProperty("L")]
        //public string L { get; set; }

        //[JsonProperty("LI")]
        //public int LI { get; set; }

        //[JsonProperty("LR")]
        //public string LR { get; set; }

        //[JsonProperty("MEC")]
        //public IList<MEC> MEC { get; set; }

        //[JsonProperty("MIO")]
        //public MIO MIO { get; set; }

        //[JsonProperty("MIS")]
        //public IList<MI> MIS { get; set; }

        //[JsonProperty("N")]
        //public int N { get; set; }

        //[JsonProperty("O1")]
        //public string O1 { get; set; }

        //[JsonProperty("O1C")]
        //public int O1C { get; set; }

        //[JsonProperty("O1I")]
        //public int O1I { get; set; }

        //[JsonProperty("O1IMG")]
        //public IList<string> O1IMG { get; set; }

        //[JsonProperty("O1IS")]
        //public IList<int> O1IS { get; set; }

        //[JsonProperty("O1R")]
        //public string O1R { get; set; }

        //[JsonProperty("O2")]
        //public string O2 { get; set; }

        //[JsonProperty("O2C")]
        //public int O2C { get; set; }

        //[JsonProperty("O2I")]
        //public int O2I { get; set; }

        //[JsonProperty("O2IMG")]
        //public IList<string> O2IMG { get; set; }

        //[JsonProperty("O2IS")]
        //public IList<int> O2IS { get; set; }

        //[JsonProperty("O2R")]
        //public string O2R { get; set; }

        //[JsonProperty("S")]
        //public int S { get; set; }

        //[JsonProperty("SGI")]
        //public string SGI { get; set; }

        //[JsonProperty("SI")]
        //public int SI { get; set; }

        //[JsonProperty("SN")]
        //public string SN { get; set; }

        //[JsonProperty("SR")]
        //public string SR { get; set; }

        //[JsonProperty("SS")]
        //public int SS { get; set; }

        //[JsonProperty("STI")]
        //public string STI { get; set; }

        //[JsonProperty("T")]
        //public int T { get; set; }

        //[JsonProperty("TN")]
        //public string TN { get; set; }

        //[JsonProperty("V")]
        //public string V { get; set; }

        //[JsonProperty("VR")]
        //public string VR { get; set; }

        //[JsonProperty("AM")]
        //public bool AM { get; set; }

        //[JsonProperty("HMH")]
        //public int HMH { get; set; }

        //[JsonProperty("OuR")]
        //public bool OuR { get; set; }

        //[JsonProperty("R")]
        //public int R { get; set; }

        //[JsonProperty("SC")]
        //public SC SC { get; set; }

        //[JsonProperty("SG")]
        //public IList<SG> SG { get; set; }

        //[JsonProperty("SVoAP")]
        //public bool SVoAP { get; set; }

        //[JsonProperty("VA")]
        //public int VA { get; set; }

        //[JsonProperty("VI")]
        //public string VI { get; set; }
    }

    public class RequestEvent_1xbet
    {

        [JsonProperty("Error")]
        public string Error { get; set; }

        //[JsonProperty("ErrorCode")]
        //public int ErrorCode { get; set; }

        //[JsonProperty("Guid")]
        //public string Guid { get; set; }

        //[JsonProperty("Id")]
        //public int Id { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Value")]
        public BaseEvent_1xbet Value { get; set; }
    }


}
