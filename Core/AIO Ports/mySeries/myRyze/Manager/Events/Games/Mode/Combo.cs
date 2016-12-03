using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events.Games.Mode
{
    using Spells;
    using System.Linq;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Combo : Logic
    {
        internal static void Init()
        {
            CanShield = ((Menu.GetList("ComboMode") == 0 && Me.HealthPercent <= Menu.GetSlider("ComboShieldHP")) ||
                         Menu.GetList("ComboMode") == 1) && Q.Level > 0 && W.Level > 0 && E.Level > 0;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target.Check(Q.Range))
            {
                if (Menu.GetBool("ComboIgnite") && Ignite != SpellSlot.Unknown && Ignite.IsReady() &&
                    target.IsValidTarget(600) &&
                    (target.Health < DamageCalculate.GetComboDamage(target) ||
                     target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

                if (Utils.TickCount - LastCastTime > 500)
                {
                    switch (Menu.GetList("ComboMode"))
                    {
                        case 0:
                            NormalCombo(target, Menu.GetBool("ComboQ"), Menu.GetBool("ComboW"), Menu.GetBool("ComboE"));
                            break;
                        case 1:
                            ShieldCombo(target, Menu.GetBool("ComboQ"), Menu.GetBool("ComboW"), Menu.GetBool("ComboE"));
                            break;
                        default:
                            BurstCombo(target, Menu.GetBool("ComboQ"), Menu.GetBool("ComboW"), Menu.GetBool("ComboE"));
                            break;
                    }
                }
            }
        }

        private static void NormalCombo(Obj_AI_Base target, bool useQ, bool useW, bool useE)
        {
            if (target.Check(Q.Range))
            {
                if (Utils.TickCount - LastCastTime > 500)
                {
                    if (CanShield)
                    {
                        if (useQ && Q.IsReady() &&
                            (SpellManager.FullStack || SpellManager.NoStack ||
                             (SpellManager.HalfStack && !W.IsReady() && Wcd > 1 && !E.IsReady() && Ecd > 1)) &&
                            target.IsValidTarget(Q.Range))
                        {
                            var qPred = Q.GetPrediction(target);

                            if (qPred.Hitchance >= HitChance.High)
                            {
                                Q.Cast(qPred.CastPosition, true);
                            }
                        }

                        if (useW && W.IsReady() && (!SpellManager.FullStack || SpellManager.HaveShield) &&
                            target.IsValidTarget(W.Range) &&
                            ((Ecd >= 2) || target.HasBuff("ryzee")))
                        {
                            W.CastOnUnit(target, true);
                        }

                        if (useE && E.IsReady() && (!SpellManager.FullStack || SpellManager.HaveShield) &&
                            target.IsValidTarget(E.Range))
                        {
                            if (SpellManager.NoStack)
                            {
                                E.CastOnUnit(target, true);
                            }

                            var minion =
                                MinionManager
                                    .GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                                    .FirstOrDefault(
                                        x =>
                                            x.Health < E.GetDamage(x) &&
                                            HeroManager.Enemies.Any(a => a.Distance(x.Position) <= 290));

                            E.CastOnUnit(minion ?? target, true);
                        }
                    }
                    else if (!CanShield)
                    {
                        if (useQ && Q.IsReady() && (SpellManager.NoStack || SpellManager.HalfStack) &&
                            target.IsValidTarget(Q.Range))
                        {
                            var qPred = Q.GetPrediction(target);

                            if (qPred.Hitchance >= HitChance.High)
                            {
                                Q.Cast(qPred.CastPosition, true);
                            }
                        }

                        if (useW && W.IsReady() && SpellManager.NoStack && target.IsValidTarget(W.Range) &&
                            ((Ecd >= 2) || target.HasBuff("ryzee")))
                        {
                            W.CastOnUnit(target, true);

                        }

                        if (useE && E.IsReady() && SpellManager.NoStack && target.IsValidTarget(E.Range))
                        {
                            var minion =
                                MinionManager
                                    .GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                                    .FirstOrDefault(
                                        x =>
                                            x.Health < E.GetDamage(x) &&
                                            HeroManager.Enemies.Any(a => a.Distance(x.Position) <= 290));

                            E.CastOnUnit(minion ?? target, true);
                        }
                    }
                }
            }
        }

        private static void ShieldCombo(Obj_AI_Base target, bool useQ, bool useW, bool useE)
        {
            if (target.Check(Q.Range))
            {
                if (Utils.TickCount - LastCastTime > 500)
                {
                    if (useQ && Q.IsReady() &&
                        (SpellManager.FullStack || SpellManager.NoStack ||
                         (SpellManager.HalfStack && !W.IsReady() && Wcd > 1 && !E.IsReady() && Ecd > 1)) &&
                        target.IsValidTarget(Q.Range))
                    {
                        var qPred = Q.GetPrediction(target);

                        if (qPred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(qPred.CastPosition, true);
                        }
                    }

                    if (useW && W.IsReady() && (!SpellManager.FullStack || SpellManager.HaveShield) &&
                        target.IsValidTarget(W.Range) &&
                        ((Ecd >= 2) || target.HasBuff("ryzee")))
                    {
                        W.CastOnUnit(target, true);
                    }

                    if (useE && E.IsReady() && (!SpellManager.FullStack || SpellManager.HaveShield) &&
                        target.IsValidTarget(E.Range))
                    {
                        if (SpellManager.NoStack)
                        {
                            E.CastOnUnit(target, true);
                        }

                        var minion =
                            MinionManager
                                .GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                                .FirstOrDefault(
                                    x =>
                                        x.Health < E.GetDamage(x) &&
                                        HeroManager.Enemies.Any(a => a.Distance(x.Position) <= 290));

                        E.CastOnUnit(minion ?? target, true);
                    }
                }
            }
        }

        private static void BurstCombo(Obj_AI_Base target, bool useQ, bool useW, bool useE)
        {
            if (target.Check(Q.Range))
            {
                if (Utils.TickCount - LastCastTime > 500)
                {
                    if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.CastOnUnit(target, true);
                    }

                    if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        var qPred = Q.GetPrediction(target);

                        if (qPred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(qPred.CastPosition, true);
                        }
                    }

                    if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(target, true);
                    }

                    if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        var qPred = Q.GetPrediction(target);

                        if (qPred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(qPred.CastPosition, true);
                        }
                    }
                }
            }
        }
    }
}
