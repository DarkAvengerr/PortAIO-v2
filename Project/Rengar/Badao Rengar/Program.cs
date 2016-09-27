using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoRengar
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Rengar")
                return;


            Config.BadaoActivate();
            SubOrb.BadaoActivate();
            Combo.BadaoActivate();
            Clear.BadaoActivate();
            Assasinate.BadaoActivate();
            Magnet.BadaoActivate();
            Auto.BadaoActivate();
        }
    }
}
