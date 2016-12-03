using System;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace SharpShooter
{
    internal class Program
    {
        public static void Game_OnGameLoad()
        {
            Initializer.Initialize();
        }
    }
}