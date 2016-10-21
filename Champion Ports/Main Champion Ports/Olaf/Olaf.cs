#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Olaf.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Geometry = Olaf.Common.CommonGeometry;
using EloBuddy;

#endregion

namespace Olaf
{
    internal class Olaf
    {
        public static string ChampionName => "Olaf";

        public static void Game_OnGameLoad()
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