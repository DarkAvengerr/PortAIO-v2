using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using hikiMarksmanRework.Core.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Calculators
{
    class EzrealCalculator
    {
        public static float EzrealTotalDamage(AIHeroClient enemy)
        {
            if (EzrealSpells.Q.IsReady() && Helper.EEnabled("ezreal.q.combo"))
            {
                return EzrealSpells.Q.GetDamage(enemy);
            }
            if (EzrealSpells.W.IsReady() && Helper.EEnabled("ezreal.w.combo"))
            {
                return EzrealSpells.W.GetDamage(enemy);
            }
            if (EzrealSpells.R.IsReady() && Helper.EEnabled("ezreal.r.combo"))
            {
                return EzrealSpells.R.GetDamage(enemy);
            }
            return 0;
        }
    }
}
