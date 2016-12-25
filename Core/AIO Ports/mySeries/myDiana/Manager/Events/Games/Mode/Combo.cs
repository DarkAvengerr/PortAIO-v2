using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events.Games.Mode
{
    using myCommon;
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class Combo : Logic
    {
        internal static void Init()
        {
            AIHeroClient target = null;

            target = TargetSelector.GetSelectedTarget() ??
                     TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target.Check(Q.Range))
            {
                if (Menu.GetBool("ComboDot") && Ignite != SpellSlot.Unknown && Ignite.IsReady() &&
                    target.IsValidTarget(600) &&
                    (target.Health < DamageCalculate.GetComboDamage(target) ||
                     target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

                if (Me.Level < 6)
                {
                    if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        var qPred = Q.GetPrediction(target, true);

                        if (qPred.Hitchance >= HitChance.VeryHigh)
                        {
                            Q.Cast(qPred.CastPosition, true);
                        }
                    }

                    if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast(true);
                    }

                    if (Menu.GetBool("ComboE") && E.IsReady() &&
                        target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 50 &&
                        target.IsValidTarget(E.Range - 30) && !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        E.Cast(true);
                    }
                }
                else
                {
                    switch (Menu.GetList("ComboMode"))
                    {
                        case 0:
                            if (Menu.GetBool("ComboR") && R.IsReady() &&
                                Menu.GetBool("ComboQ") && Q.IsReady() && 
                                target.IsValidTarget(R.Range) && target.DistanceToPlayer() >= 600)
                            {
                                Q.Cast(target, true);
                                R.Cast(target, true);
                            }

                            if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                            {
                                var qPred = Q.GetPrediction(target, true);

                                if (qPred.Hitchance >= HitChance.VeryHigh)
                                {
                                    Q.Cast(qPred.CastPosition, true);
                                }
                            }

                            if (Menu.GetBool("ComboR") && R.IsReady() && target.IsValidTarget(R.Range))
                            {
                                LogicCast(target);
                            }

                            if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range))
                            {
                                W.Cast(true);
                            }

                            if (Menu.GetBool("ComboE") && E.IsReady() &&
                                target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 50 &&
                                target.IsValidTarget(E.Range - 30) && !target.HasBuffOfType(BuffType.SpellShield))
                            {
                                E.Cast(true);
                            }
                            break;
                        case 1:
                            if (target.IsValidTarget(R.Range))
                            {
                                if (Menu.GetBool("ComboR") && R.IsReady() &&
                                    Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(R.Range)
                                    && target.DistanceToPlayer() >= Menu.GetSlider("MisayaRange"))
                                {
                                    R.Cast(target, true);
                                    Q.Cast(target, true);
                                }

                                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                                {
                                    var qPred = Q.GetPrediction(target, true);

                                    if (qPred.Hitchance >= HitChance.VeryHigh)
                                    {
                                        Q.Cast(qPred.CastPosition, true);
                                    }
                                }

                                if (Menu.GetBool("ComboR") && R.IsReady() && target.IsValidTarget(R.Range))
                                {
                                    LogicCast(target);
                                }

                                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range))
                                {
                                    W.Cast(true);
                                }

                                if (Menu.GetBool("ComboE") && E.IsReady() &&
                                    target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 50 &&
                                    target.IsValidTarget(E.Range - 30) && !target.HasBuffOfType(BuffType.SpellShield))
                                {
                                    E.Cast(true);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private static void LogicCast(Obj_AI_Base target)
        {
            if (!R.IsReady() || !Menu.Item("ComboR", true).GetValue<bool>() || target.DistanceToPlayer() > R.Range)
            {
                return;
            }

            if (!Menu.Item("ComboRUnder", true).GetValue<bool>() && target.UnderTurret(true))
            {
                return;
            }

            if (SpellManager.HaveQPassive(target))
            {
                R.Cast(target, true);
            }

            if (Menu.Item("ComboSecondR", true).GetValue<bool>() && Utils.TickCount - lastRCast > 800 && qCd >= 3)
            {
                if (target.Health + target.MagicShield + target.HPRegenRate < R.GetDamage(target))
                {
                    R.Cast(target);
                }
            }
        }
    }
}
