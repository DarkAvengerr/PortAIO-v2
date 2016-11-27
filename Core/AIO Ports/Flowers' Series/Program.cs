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
            OnGameLoad();
        }

        private static void OnGameLoad()
        {
            Logic.InitAIO();
        }
    }
}
