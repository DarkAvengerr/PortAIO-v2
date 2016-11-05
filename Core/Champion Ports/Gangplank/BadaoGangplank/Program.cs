using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoGP
{
    static class Program
    {
        public static readonly List<string> SupportedChampion = new List<string>()
        {
            "Gangplank"
        };
        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            if (!SupportedChampion.Contains(ObjectManager.Player.ChampionName))
            {
                return;
            }
            Chat.Print("<font color=\"#24ff24\">Badao </font>" + "<font color=\"#ff8d1a\">" +
                ObjectManager.Player.ChampionName + "</font>" + "<font color=\"#24ff24\"> loaded!</font>");
            BadaoKingdom.BadaoChampion.BadaoGangplank.BadaoGangplank.BadaoActivate();
        }
    }
}
