using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hCamille.Champions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hCamille
{
    class Program
    {
        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Camille")
            {
                new hCamille.Champions.Camille();
            }
            else
            {
                Console.WriteLine("XD");
                return;
            }
        }
    }
}
