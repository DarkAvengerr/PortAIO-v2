using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoActionsLimiter
{
    public static class Program
    {
        public static Menu Config;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            SpellBlock.BadaoActivate();
            AttackBlock.BadaoActivate();
            MovementBlock.BadaoActivate();
        }
    }
}
