using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;


    internal class Combo : Logic
    {
        internal static void Init()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

            if (target.Check(900f))
            {
                if (Menu.GetBool("ComboIgnite") && Ignite != SpellSlot.Unknown && Ignite.IsReady() &&
                    DamageCalculate.GetComboDamage(target) > target.Health)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                    return;
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && Me.CanMoveMent() && target.DistanceToPlayer() <= 650 &&
                    target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 100)
                {
                    EDash(target);
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() && Me.CanMoveMent() && qStack == 0 &&
                    target.DistanceToPlayer() <= Q.Range + Orbwalking.GetRealAutoAttackRange(Me) &&
                    target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 50 &&
                    Utils.TickCount - lastQTime > 900)
                {
                    if (!Me.IsDashing())
                    {
                        SpellManager.CastQ(target);
                    }
                }

                if (Menu.GetBool("ComboW") && W.IsReady() &&
                    target.IsValidTarget(W.Range) && !target.HasBuffOfType(BuffType.SpellShield))
                {
                    WLogic(target);
                }

                if (Menu.GetBool("ComboR") && R.IsReady())
                {
                    if (Menu.GetKey("R1Combo") && R.Instance.Name == "RivenFengShuiEngine" && !E.IsReady())
                    {
                        if (target.DistanceToPlayer() < 500 && Me.CountEnemiesInRange(500) >= 1)
                        {
                            R.Cast(true);
                        }
                    }
                    else if (R.Instance.Name == "RivenIzunaBlade")
                    {
                        SpellManager.R2Logic(target);
                    }
                }
            }
        }

        private static void WLogic(AIHeroClient target)
        {
            if (target == null || target.IsDead || !W.IsReady())
            {
                return;
            }

            if (Menu.GetBool("ComboQW") && qStack != 0)
            {
                W.Cast(true);
            }

            if (!Q.IsReady() && qStack == 0)
            {
                W.Cast(true);
            }

            if (Menu.GetBool("ComboEW") && Me.HasBuff("RivenFeint"))
            {
                W.Cast(true);
            }

            if (!target.IsFacing(Me))
            {
                W.Cast(true);
            }
        }

        private static void EDash(AIHeroClient target)
        {
            if (target == null || target.IsDead || !E.IsReady())
            {
                return;
            }

            if (Menu.GetBool("ComboQ") && Q.IsReady() && qStack == 0 &&
                target.DistanceToPlayer() < Q.Range + Orbwalking.GetRealAutoAttackRange(Me))
            {
                return;
            }

            if (target.DistanceToPlayer() <= E.Range + (Q.IsReady() && qStack == 0 ? Q.Range : 0))
            {
                E.Cast(target.Position, true);
            }

            if (target.DistanceToPlayer() <= E.Range + (W.IsReady() ? W.Range : 0))
            {
                E.Cast(target.Position, true);
            }

            if (!Q.IsReady() && !W.IsReady() && target.DistanceToPlayer() < E.Range + Me.AttackRange)
            {
                E.Cast(target.Position, true);
            }
        }
    }
}
