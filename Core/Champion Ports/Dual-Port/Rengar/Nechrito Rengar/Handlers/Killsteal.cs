using System;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Nechrito_Rengar.Main;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Rengar.Handlers
{
    class Killsteal : Core
    {
        public static void KillSteal()
        {
            if (Champion.W.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Champion.W.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Champion.W.GetDamage(target))
                        Champion.W.Cast(target);
                }
            }
            if (Champion.E.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Champion.E.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Champion.E.GetDamage(target))
                        Champion.E.Cast(target);
                }
            }
            if (Champion.Ignite.IsReady() && MenuConfig.KillStealSummoner)
            {
                var target = TargetSelector.GetTarget(600f, TargetSelector.DamageType.True);
                if (target.IsValidTarget(600f) && Dmg.IgniteDamage(target) >= target.Health)
                {
                    Player.Spellbook.CastSpell(Champion.Ignite, target);
                }
            }
            if (Champion.Smite.IsReady() && MenuConfig.KillStealSummoner)
            {
                var target = TargetSelector.GetTarget(600f, TargetSelector.DamageType.True);
                if (target.IsValidTarget(600f) && Dmg.SmiteDamage(target) >= target.Health)
                {
                    Player.Spellbook.CastSpell(Champion.Smite, target);
                }
            }
        }
    }
}
