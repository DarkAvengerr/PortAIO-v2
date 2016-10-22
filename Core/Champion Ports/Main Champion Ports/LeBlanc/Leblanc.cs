#region
using System;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

#endregion

namespace Leblanc
{
    internal class Leblanc
    {
        public static string ChampionName => "Leblanc";

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != ChampionName)
            {
                return;
            }

            Champion.PlayerSpells.Init();
            Modes.ModeConfig.Init();
            Common.CommonItems.Init();
            var fVersion = "<font color='#FFFFFF'>Version=" + Assembly.GetExecutingAssembly().GetName().Version + "</font>";
            Chat.Print(
                "<font color='#ff3232'>Successfully Loaded: </font><font color='#d4d4d4'><font color='#FFFFFF'>Leblanc II </font>" +
                fVersion);

            //Console.Clear();
        }
    }
}