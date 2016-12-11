using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace PRADA_Vayne.MyUtils
{
    public static class MyWizard
    {
        public static bool UltActive()
        {
            return Heroes.Player.HasBuff("VayneInquisition");
        }

        public static bool TumbleActive()
        {
            return Heroes.Player.HasBuff("vaynetumblebonus");
        }

        public static bool ShouldSaveCondemn()
        {
            var katarina =
                HeroManager.Enemies.FirstOrDefault(h => h.BaseSkinName == "Katarina" && h.IsValidTarget(1400));
            if (katarina != null)
            {
                var kataR = katarina.GetSpell(SpellSlot.R);
                return kataR.IsReady() ||
                       (katarina.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready);
            }
            var galio =
                HeroManager.Enemies.FirstOrDefault(h => h.BaseSkinName == "Galio" && h.IsValidTarget(1400));
            if (galio != null)
            {
                var galioR = galio.GetSpell(SpellSlot.R);
                return galioR.IsReady() ||
                       (galio.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready);
            }
            return false;
        }
    }
}
