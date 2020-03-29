using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.BookmakerModel.BaseEvents
{
    public struct BettingOdds
    {
        public BetGroup Group;
        public double Value;
        public double Coef;

        public BettingOdds(BetGroup group, double val,double coef)
        {
            Group = group;
            Value = val;
            Coef = coef;
        }
    }
}
