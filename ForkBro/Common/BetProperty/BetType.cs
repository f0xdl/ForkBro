using ForkBro.BookmakerModel.BaseEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.Common
{
    public enum OldBetType
    {
        None = 0,
        Win = 1,
        Total = 2,
        IndTotal1 = 3,
        IndTotal2 = 11,
        Fora = 4,
        Corner = 5,
        WinInSet = 6,
        WinInGame = 7,
        WinPoints = 8,
        WinInCurrentGame = 9,
        Goals = 10
    }
    public enum BetType : byte
    {
        None = 0,
        Win = 1,//1X2
        Total = 2,
        Fora = 3,
        TotalEven = 4, //Event-Odd
        IndTotal_A = 5,
        IndTotal_B = 6,
    }

    public static class UnitTranslate
    {
        static void Stat()
        {
            BetType betType = BetType.Fora;
            EventUnit eventUnit = EventUnit.MainTime;

        }
    }
}
