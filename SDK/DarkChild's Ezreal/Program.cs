using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal
{
    using System;

    using DarkEzreal.Main;

    using LeagueSharp.SDK;

    internal class Program
    {
        public static void Main()
        {
            Bootstrap.Init();

            EventsOnOnLoad();
        }

        private static void EventsOnOnLoad()
        {
            Config.Init();
        }
    }
}
