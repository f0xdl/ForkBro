using System;
using System.Text.Json.Serialization;
using ForkBro.Common;

namespace ForkBro.Configuration
{
    public class BookmakersProp
    {
        public string name
        {
            get => id.ToString();
            set
            {
                if (Enum.TryParse<Bookmaker>(value, true, out Bookmaker result))
                    id = result;
            }
        }

        public Bookmaker id;
        public bool enable { get; set; }
        public int repeat { get; set; }
    }
}