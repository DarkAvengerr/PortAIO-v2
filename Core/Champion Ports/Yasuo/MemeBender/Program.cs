using System;
using LeagueSharp.Common;

using EloBuddy;
using LeagueSharp.Common;
namespace YasuoTheLastMemebender
{
    class Program
    {
        public static void Game_OnGameLoad()
        {
            try
            {
                new YasuoMemeBender();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to load Yasuo - The Memebender: " + exception);
            }
        }
    }
}
