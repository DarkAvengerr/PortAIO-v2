#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Geometry = Nocturne.Common.CommonGeometry;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nocturne
{
    internal class Nocturne
    {
        public static void Init()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            Champion.PlayerSpells.Init();
            Common.CommonItems.Init();
            Modes.ModeConfig.Init();

            Chat.Print("<font color='#ff3232'>Successfully Loaded: </font><font color='#d4d4d4'><font color='#FFFFFF'>" +  Program.ChampionName + "</font>");

            Console.Clear();
        }

     
    }
}