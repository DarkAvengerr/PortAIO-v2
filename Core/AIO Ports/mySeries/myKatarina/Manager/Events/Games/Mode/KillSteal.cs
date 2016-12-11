using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Games.Mode
{
    using myCommon;
    using Spells;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class KillSteal : Logic
    {
        internal static void Init()
        {
            if (SpellManager.isCastingUlt && !Menu.GetBool("KillStealCancel"))
            {
                return;
            }

            foreach (
                var target in
                HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.IsValidTarget(E.Range + 300))
                    .OrderBy(x => x.Health))
            {
                if (target.Check(E.Range + 300))
                {
                    if (target.Health < DamageCalculate.GetQDamage(target) && Menu.GetBool("KillStealQ") && Q.IsReady() && 
                        target.IsValidTarget(Q.Range))
                    {
                        if (SpellManager.isCastingUlt)
                        {
                            SpellManager.CancelUlt(true);
                            Q.CastOnUnit(target, true);
                            return;
                        }
                        Q.CastOnUnit(target, true);
                        return;
                    }

                    if (target.Health < DamageCalculate.GetEDamage(target) && Menu.GetBool("KillStealE") && E.IsReady())
                    {
                        if (target.DistanceToPlayer() <= E.Range + 130)
                        {
                            var pos = Me.Position.Extend(target.Position, target.DistanceToPlayer() + 130);
                            if (SpellManager.isCastingUlt)
                            {
                                SpellManager.CancelUlt(true);
                                E.Cast(pos, true);
                                return;
                            }
                            E.Cast(pos, true);
                            return;
                        }

                        if (target.IsValidTarget(E.Range))
                        {
                            if (SpellManager.isCastingUlt)
                            {
                                SpellManager.CancelUlt(true);
                                E.Cast(target, true);
                                return;
                            }
                            E.Cast(target, true);
                            return;
                        }
                    }

                    if (target.Health < DamageCalculate.GetQDamage(target) + DamageCalculate.GetEDamage(target) && 
                        Menu.GetBool("KillStealQ") && Menu.GetBool("KillStealE") && Q.IsReady() && E.IsReady() &&
                        target.IsValidTarget(E.Range))
                    {
                        if (SpellManager.isCastingUlt)
                        {
                            SpellManager.CancelUlt(true);
                            Q.CastOnUnit(target, true);
                            E.Cast(target, true);
                            return;
                        }
                        Q.CastOnUnit(target, true);
                        E.Cast(target, true);
                        return;
                    }

                    if (target.Health < DamageCalculate.GetPassiveDamage(target) + DamageCalculate.GetEDamage(target) &&
                        Menu.GetBool("KillStealE") && E.IsReady() &&
                        Daggers.Any(
                            x =>
                                x.Dagger.IsValid &&
                                x.Position.Distance(target.Position) <= SpellManager.PassiveRange &&
                                x.Position.DistanceToPlayer() <= E.Range))
                    {
                        foreach (
                            var obj in
                            Daggers.Where(x => x.Position.Distance(target.Position) <= SpellManager.PassiveRange)
                                .OrderBy(x => x.Position.Distance(target.Position)))
                        {
                            if (obj.Dagger != null && obj.Dagger.IsValid && obj.Position.DistanceToPlayer() <= E.Range)
                            {
                                if (SpellManager.isCastingUlt)
                                {
                                    SpellManager.CancelUlt(true);
                                    E.Cast(obj.Position, true);
                                    LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () => E.Cast(target, true));
                                    return;
                                }
                                E.Cast(obj.Position, true);
                                LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () => E.Cast(target, true));
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
