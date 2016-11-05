using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Hikigaya_Lux.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hikigaya_Lux.Logic
{
    class ELogic
    {
        public static readonly AIHeroClient Lux = ObjectManager.Player;

        public static void NormalE(AIHeroClient enemy)
        {
            if (Spells.E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("e.hit.chance"))
            {
                Spells.E.Cast(enemy);
            }
            if (Helper.LuxE != null && Helper.EInsCheck() == 2)
            {
                Spells.E.Cast();
            }
        }

        
    }
}
