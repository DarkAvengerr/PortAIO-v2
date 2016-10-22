using LeagueSharp.Common;
using System;
using System.Linq;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Nidalee.Handlers
{
    class HealManager : Core
    {
        public static void Heal()
        {
            if(!Champion.Primalsurge.IsReady() || Player.InFountain() || Player.IsRecalling())
            { return; }

            if(Player.HealthPercent <= MenuConfig.SelfHeal.Value && Player.ManaPercent <= MenuConfig.ManaHeal.Value && !CatForm())
            {
                Champion.Primalsurge.Cast(Player);
            }
            var canidates = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(Champion.Primalsurge.Range, false) && x.IsAlly && x.HealthPercent < Player.HealthPercent);
            canidates = canidates.OrderByDescending(x => x.TotalAttackDamage);
            var target = canidates.FirstOrDefault(x => !x.InFountain());
            if (target != null && target.HealthPercent <= MenuConfig.allyHeal.Value && !target.InFountain() && Player.ManaPercent <= MenuConfig.ManaHeal.Value && !CatForm())
            {
                Champion.Primalsurge.Cast(target);
            }
        }
    }
}
