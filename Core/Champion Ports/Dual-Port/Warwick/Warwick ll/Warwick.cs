#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Geometry = WarwickII.Common.CommonGeometry;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII
{
    internal class Warwick
    {
        public static string ChampionName => "Warwick";
        public static void Init()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != ChampionName)
            {
                return;
            }

            Champion.PlayerSpells.Init();
            Modes.ModeConfig.Init();
            Common.CommonItems.Init();

            Chat.Print(
                "<font color='#ff3232'>Successfully Loaded: </font><font color='#d4d4d4'><font color='#FFFFFF'>" +
                ChampionName + "</font>");

            Console.Clear();
        }
    }
}