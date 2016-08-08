using EloBuddy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivorSeries
{
    class SurviorSeries
    {
        public static void Main()
        {
            switch (ObjectManager.Player.Hero)
            {
                case Champion.Ashe:
                    SurvivorAshe.Program.Game_OnGameLoad();
                    break;
                case Champion.Brand:
                    SurvivorBrand.Program.Game_OnGameLoad();
                    break;
                case Champion.Malzahar:
                    SurvivorMalzahar.Program.Game_OnGameLoad();
                    break;
                case Champion.Ryze:
                    SurvivorRyze.Program.Game_OnGameLoad();
                    break;
            }
        }
    }
}
