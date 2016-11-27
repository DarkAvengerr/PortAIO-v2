using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hinata_s_Taliyah.Champions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Hinata_s_Taliyah
{
    class Program
    {
        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Taliyah")
            {
                new Champions.Taliyah();
            }
            else
            {
                return;
            }
        }
    }
}
