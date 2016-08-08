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
    class CorkiCalculator
    {
        public static float CorkiTotalDamage(AIHeroClient enemy)
        {
            if (CorkiSpells.Q.LSIsReady() && Helper.CEnabled("corki.q.combo"))
            {
                return CorkiSpells.Q.GetDamage(enemy);
            }
            if (CorkiSpells.W.LSIsReady() && Helper.CEnabled("corki.w.combo"))
            {
                return CorkiSpells.W.GetDamage(enemy);
            }
            if (CorkiSpells.E.LSIsReady() && Helper.CEnabled("corki.e.combo"))
            {
                return CorkiSpells.E.GetDamage(enemy);
            }
            if (CorkiSpells.R.LSIsReady() && Helper.CEnabled("corki.r.combo"))
            {
                return CorkiSpells.R.GetDamage(enemy);
            }
            return 0;
        }
    }
}
