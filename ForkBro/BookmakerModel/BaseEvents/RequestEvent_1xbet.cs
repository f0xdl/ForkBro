using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using R = Newtonsoft.Json.Required;
using N = Newtonsoft.Json.NullValueHandling;

namespace ForkBro.BookmakerModel.BaseEvents.Events1xBet
{
    public class RequestEvent_1xBet
    {
        [J("Error")]     public string Error { get; set; }  
        // [J("ErrorCode")] public long ErrorCode { get; set; }
        // [J("Guid")]      public string Guid { get; set; }   
        // [J("Id")]        public long Id { get; set; }       
        [J("Success")]   public bool Success { get; set; }  
        [J("Value")]     public BaseEvent_1xBet Value { get; set; }   
    }
    
    public class BaseEvent_1xBet
    {
        [J("CHIMG")]     public string Chimg { get; set; }  
        [J("CN")]        public string Cn { get; set; }     
        [J("CO")]        public long Co { get; set; }       
        [J("COI")]       public long Coi { get; set; }      
        [J("EC")]        public long Ec { get; set; }       
        [J("EGC")]       public long Egc { get; set; }      
        [J("GE")]        public Ge[] Ge { get; set; }       
        [J("I")]         public long I { get; set; }        
        [J("KI")]        public long Ki { get; set; }       
        [J("L")]         public string L { get; set; }      
        [J("LE")]        public string Le { get; set; }     
        [J("LI")]        public long Li { get; set; }       
        [J("MEC")]       public Mec[] Mec { get; set; }     
        [J("MIO")]       public Mio Mio { get; set; }       
        [J("MIS")]       public Mi[] Mis { get; set; }      
        [J("N")]         public long N { get; set; }        
        [J("O1")]        public string O1 { get; set; }     
        [J("O1C")]       public long O1C { get; set; }      
        [J("O1E")]       public string O1E { get; set; }    
        [J("O1I")]       public long O1I { get; set; }      
        [J("O1IMG")]     public string[] O1Img { get; set; }
        [J("O1IS")]      public long[] O1Is { get; set; }   
        [J("O2")]        public string O2 { get; set; }     
        [J("O2C")]       public long O2C { get; set; }      
        [J("O2E")]       public string O2E { get; set; }    
        [J("O2I")]       public long O2I { get; set; }      
        [J("O2IMG")]     public string[] O2Img { get; set; }
        [J("O2IS")]      public long[] O2Is { get; set; }   
        [J("S")]         public long S { get; set; }        
        [J("SE")]        public string Se { get; set; }     
        [J("SGI")]       public string Sgi { get; set; }    
        [J("SI")]        public long Si { get; set; }       
        [J("SN")]        public string Sn { get; set; }     
        [J("SS")]        public long Ss { get; set; }       
        [J("STI")]       public string Sti { get; set; }    
        [J("T")]         public long T { get; set; }        
        [J("TN")]        public string Tn { get; set; }     
        [J("AM")]        public bool Am { get; set; }       
        [J("HMH")]       public long Hmh { get; set; }      
        [J("OuR")]       public bool OuR { get; set; }      
        [J("R")]         public long R { get; set; }        
        [J("SC")]        public Sc Sc { get; set; }         
        [J("SG")]        public Sg[] Sg { get; set; }       
        [J("VA")]        public long Va { get; set; }       
        [J("VI")]        public string Vi { get; set; }       
        [J("ZP")]        public long Zp { get; set; }       
    }

    public partial class Ge
    {
        [J("E")] public E[][] E { get; set; }
        [J("G")] public long G { get; set; } 
    }

    public partial class E
    {
        [J("C")]  public double C { get; set; } 
        [J("CE")] public long? Ce { get; set; } 
        [J("G")]  public long G { get; set; }   
        [J("P")]  public double? P { get; set; }
        [J("T")]  public long T { get; set; }   
        [J("B")]  public bool? B { get; set; }  
    }

    public partial class Mec
    {
        [J("EC")] public long Ec { get; set; }
        [J("MT")] public long Mt { get; set; }
    }

    public partial class Mio
    {
    }

    public partial class Mi
    {
        [J("K")] public long K { get; set; }  
        [J("V")] public string V { get; set; }
    }

    public partial class Sc
    {
        [J("CP")]  public long Cp { get; set; }    
        [J("CPS")] public string Cps { get; set; } 
        [J("FS")]  public Fs Fs { get; set; }      
        [J("P")]   public long P { get; set; }     
        [J("PS")]  public P[] Ps { get; set; }     
        [J("S")]   public dynamic[] S { get; set; }
        [J("SS")]  public Ss Ss { get; set; }      
        [J("TR")]  public long Tr { get; set; }    
    }

    public partial class Fs
    {
        [J("S1")] public long? S1 { get; set; }
        [J("S2")] public long S2 { get; set; } 
    }

    public partial class P
    {
        [J("Key")]   public long Key { get; set; }
        [J("Value")] public Fs Value { get; set; }
    }

    public partial class Ss
    {
        [J("S1")]  public string S1 { get; set; }
        [J("S2")]  public string S2 { get; set; }
    }

    public partial class Sg
    {
        [J("EC")]  public long Ec { get; set; }  
        [J("EGC")] public long Egc { get; set; } 
        [J("GE")]  public Ge[] Ge { get; set; }  
        [J("I")]   public long I { get; set; }   
        [J("MEC")] public Mec[] Mec { get; set; }
        [J("N")]   public long N { get; set; }   
        [J("P")]   public long P { get; set; }   
        [J("PN")]  public string Pn { get; set; }
        [J("SI")]  public long Si { get; set; }  
        [J("SS")]  public long Ss { get; set; }  
        [J("T")]   public long T { get; set; }   
        [J("R")]   public long R { get; set; }   
    }
}
