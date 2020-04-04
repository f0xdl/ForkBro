using ForkBro.BookmakerModel.BaseEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.Common
{
    public enum BetType : byte
    {
        None = 0,
        Win = 1,//1 2
        Total = 2,
        Fora = 3,
        TotalEven = 4, //Event-Odd
        IndTotal_A = 5,
        IndTotal_B = 6,
    }
}
