using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Hiki.Kled.Champions;
using EloBuddy;

namespace Hiki.Kled
{
    class Program
    {
        public static void Main()
        {
            OnLoad();
        }

        private static void OnLoad()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Kled":
                    new Champions.Kled();
                    break;
            }
        }
    }
}
