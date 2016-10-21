using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Nechrito_Nidalee.Handlers;
using Nechrito_Nidalee.Extras;
using System.Collections.Generic;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Nidalee
{
    class Killsteal : Core
    {
        public static void KillSteal()
        {
            if(MenuConfig.SpellsKS)
            {
                if(!CatForm())
                {
                    if (Champion.Javelin.IsReady())
                    {
                        var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Champion.Javelin.Range) && !x.IsZombie);
                        foreach (var target in targets)
                        {
                            if (target.Health < Champion.Javelin.GetDamage(target) && !target.IsInvulnerable && (Player.Distance(target.Position) <= Champion.Javelin.Range))
                            { Champion.Javelin.Cast(target); }

                        }
                    }
                }
                if(CatForm())
                {
                    if (Champion.Takedown.IsReady())
                    {
                        var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Champion.Javelin.Range) && !x.IsZombie);
                        foreach (var target in targets)
                        {
                            if (target.Health < Champion.Takedown.GetDamage(target) && !target.IsInvulnerable && (Player.Distance(target.Position) <= Champion.Takedown.Range))
                            { Champion.Takedown.Cast(target); }
                        }
                    }
                    if (Champion.Pounce.IsReady())
                    {
                        var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Champion.Javelin.Range) && !x.IsZombie);
                        foreach (var target in targets)
                        {
                            if (target.Health < Champion.Pounce.GetDamage(target) && !target.IsInvulnerable && (Player.Distance(target.Position) <= Champion.Pounce.Range))
                            { Champion.Pounce.Cast(target); }
                        }
                    }
                    if (Champion.Swipe.IsReady())
                    {
                        var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Champion.Javelin.Range) && !x.IsZombie);
                        foreach (var target in targets)
                        {
                            if (target.Health < Champion.Swipe.GetDamage(target) && !target.IsInvulnerable && (Player.Distance(target.Position) <= Champion.Swipe.Range))
                            { Champion.Swipe.Cast(target); }
                        }
                    }
                    if (Champion.Ignite.IsReady() && MenuConfig.ComboIgnite)
                    {
                        var target = TargetSelector.GetTarget(600f, TargetSelector.DamageType.True);
                        if (target.IsValidTarget(600f) &&  Dmg.IgniteDamage(target) >= target.Health)
                        {
                            Player.Spellbook.CastSpell(Champion.Ignite, target);
                        }
                    }
                    if (Item.Smite.IsReady() && MenuConfig.ComboSmite)
                    {
                        var target = TargetSelector.GetTarget(600f, TargetSelector.DamageType.True);
                        if (target.IsValidTarget(600f) && Dmg.SmiteDamage(target) >= target.Health)
                        {
                            Player.Spellbook.CastSpell(Item.Smite, target);
                        }
                    }
                }
            }
        }
    }
}
