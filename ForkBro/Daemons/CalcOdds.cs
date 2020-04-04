using ForkBro.Common;

namespace ForkBro.Daemons
{
    public struct CalcOdds
    {
        public long IdEvent;
        public Bookmaker Bookmaker;
        public bool Reverse;
        public double[,] Values;
    }
}