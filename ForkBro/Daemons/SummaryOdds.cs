using ForkBro.Common;

namespace ForkBro.Daemons
{
    public struct SummaryOdds
    {
        public double Value;

        public Bookmaker Bookmaker_A;
        public double Coef_A;
        public bool Reverse_A;

        public Bookmaker Bookmaker_B;
        public double Coef_B;
        public bool Reverse_B;
    }
}