using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SorakaSharp.Source.Handler;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SorakaSharp.Source.Manager.Heal
{
    internal static class CHeal
    {
        private static Spell W
        {
            get { return CSpell.W; }
        }

        private static bool IsBlocked(this AIHeroClient unit)
        {
            return CConfig.ConfigMenu.SubMenu("Heal").SubMenu("DontHeal").Items.Any(entry => entry.DisplayName == unit.BaseSkinName && entry.IsActive());
        }

        private static AIHeroClient GetHealTarget()
        {
            switch (CConfig.ConfigMenu.Item("priority").GetValue<StringList>().SelectedIndex)
            {
                case 0: // MostAD
                    return
                        HeroManager.Allies.Where(ally => ally.IsValidTarget(W.Range + 50, false) && !ally.IsMe && !ally.IsBlocked())
                            .OrderByDescending(dmg => dmg.TotalAttackDamage())
                            .First();
                case 1: // MostAP
                    return
                        HeroManager.Allies.Where(ally => ally.IsValidTarget(W.Range + 50, false) && !ally.IsBlocked())
                            .OrderByDescending(ap => ap.TotalMagicalDamage())
                            .First();

                case 2: //LowestHP
                    return
                        HeroManager.Allies.Where(ally => ally.IsValidTarget(W.Range + 50, false) && !ally.IsBlocked())
                            .OrderBy(health => health.HealthPercentage())
                            .First();
            }
            return null;
        }

        internal static void AutoHeal()
        {
            if (!CConfig.ConfigMenu.Item("useHeal").GetValue<bool>() || !W.IsReady())
                return;

            if (GetHealTarget().HealthPercentage() <= CConfig.ConfigMenu.Item("percentage").GetValue<Slider>().Value)
            {
                W.Cast(GetHealTarget());
            }
        }
    }
}
