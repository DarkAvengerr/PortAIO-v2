using System;
using LeagueSharp.Common;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne
{
    class Program
    {
        /// <summary>
        /// The entry point of the assembly.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            Variables.Instance = new SOLOBootstrap();
        }
    }
}
