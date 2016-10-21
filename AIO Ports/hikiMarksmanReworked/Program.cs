using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework
{
    class Program
    {
        public static void Game_OnGameLoad()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Graves":
                    new Champions.Graves();
                    break;
                case "Sivir":
                    new Champions.Sivir();
                    break;
                case "Lucian":
                    new Champions.Lucian();
                    break;
                case "Ezreal":
                    new Champions.Ezreal();
                    break;
                case "Vayne":
                    new Champions.Vayne();
                    break;
                case "Draven":
                    new Champions.Draven();
                    break;
                case "Corki":
                    new Champions.Corki();
                    break;
            }
            return;
        }
    }
}
