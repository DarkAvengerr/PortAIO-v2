using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CjShuJinx
{
    #region

    using LeagueSharp;
    using LeagueSharp.Common;
    using System;

    #endregion

    internal class Jinx
    {
        public static string ChampionName => "Jinx";
        public static void Init()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != ChampionName)
            {
                return;
            }

            Champion.PlayerSpells.Init();
            Modes.ModeConfig.Init();
            Common.CommonItems.Init();

            Chat.Print("<font color='#DDDDFF'><b> Taiwan By: CjShu :) </b></font>");
            Chat.Print("<font color='#FF8EFF'><b> If you like.</font><font color='#96FED1'><b>Thank you my friend xQx. And NightMoon. Aid!</b></font>");
            Chat.Print(
                "<font color='#ff3232'>Successfully Loaded: </font><font color='#d4d4d4'><font color='#FFFFFF'>" +
                ChampionName + "</font>");

            Console.Clear();
        }
    }
}