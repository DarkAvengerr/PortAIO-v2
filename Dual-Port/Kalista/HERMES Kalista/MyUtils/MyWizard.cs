using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HERMES_Kalista.MyUtils
{
    public static class MyWizard
    {
        public static bool UltActive()
        {
            return Heroes.Player.Buffs.Any(b => b.Name.ToLower().Contains("vayneinquisition"));
        }

        public static bool TumbleActive()
        {
            return Heroes.Player.Buffs.Any(b => b.Name.ToLower().Contains("vaynetumblebonus"));
        }

        public static bool ShouldSaveCondemn()
        {
            var katarina =
                HeroManager.Enemies.FirstOrDefault(h => h.CharData.BaseSkinName == "Katarina" && h.LSIsValidTarget(1400));
            if (katarina != null)
            {
                var kataR = katarina.GetSpell(SpellSlot.R);
                return kataR.LSIsReady() ||
                       (katarina.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready);
            }
            var galio =
                HeroManager.Enemies.FirstOrDefault(h => h.CharData.BaseSkinName == "Galio" && h.LSIsValidTarget(1400));
            if (galio != null)
            {
                var galioR = galio.GetSpell(SpellSlot.R);
                return galioR.LSIsReady() ||
                       (galio.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready);
            }
            return false;
        }
    }
}
