using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iLucian
{
    class LucianBootstrap
    {
        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Lucian")
            {
                return;
            }
            var lucian = new iLucian();
            lucian.OnLoad();
        }
    }
}
