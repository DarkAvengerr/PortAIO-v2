using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Spells;
using hikiMarksmanRework.Core.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Calculators
{
    class SivirCalculator
    {
        public static float SivirTotalDamage(AIHeroClient enemy)
        {
            if (SivirSpells.Q.LSIsReady() && Helper.SEnabled("sivir.q.combo"))
            {
                return SivirSpells.Q.GetDamage(enemy);
            }
            if (SivirSpells.W.LSIsReady() && Helper.SEnabled("sivir.w.combo"))
            {
                return SivirSpells.W.GetDamage(enemy);
            }
            return 0;
        }
    }
}
