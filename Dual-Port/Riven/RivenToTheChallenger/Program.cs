using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RivenToTheChallenger
{
    class Program
    {
        public static void Main()
        {
            GameOnOnGameLoad();
        }

        private static void GameOnOnGameLoad()
        {
            try
            {
                var a = new Riven();
            }
            catch (Exception e)
            {
                Chat.Print("Failed to load Riven to the challanger, check console for details");
                Console.WriteLine($"RTTC: {e}");   
            }
        }
    }
}
