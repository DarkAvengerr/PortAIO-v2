using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso.Plugins
{
    class Compare
    {
        /// <summary>
        /// Marksman Array for Compare
        /// </summary>
        public static string[] MarksmanStrings = { "Graves" };

        public static void Compares()
        {
            var enemy = HeroManager.Enemies.Where(x => MarksmanStrings.Contains(x.ChampionName)).ToList().FirstOrDefault();
            var screenpos = Game.CursorPos.To2D().To3D2();
            Drawing.DrawText(screenpos.X,screenpos.Y,System.Drawing.Color.Gold,"Deneme");
        }
    }
}
