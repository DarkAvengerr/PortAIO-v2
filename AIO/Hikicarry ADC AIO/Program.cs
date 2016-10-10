using System;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry
{
    internal class Program
    {
        public static void Main()
        {
            OnGameLoad();
        }
        private static void OnGameLoad()
        {
            Initializer.Load();
        }
    }
}
