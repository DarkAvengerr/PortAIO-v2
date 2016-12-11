using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series
{
    using System;
    using LeagueSharp.Common;

    internal static class Program
    {
        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs Args)
        {
            Logic.InitAIO();
        }
    }
}
