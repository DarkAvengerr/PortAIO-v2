using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "xinzhao")
            {
                return;
            }

            Spells.LoadSpells();
            Config.BuildMenu();
            Mainframe.Init();
        }
    }
}
