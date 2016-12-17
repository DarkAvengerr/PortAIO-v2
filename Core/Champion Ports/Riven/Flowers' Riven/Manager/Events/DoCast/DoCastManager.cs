using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events
{
    using System.Linq;
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    using FlowersRivenCommon;


    internal class DoCastManager : Logic
    {
        internal static void InitCombo(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            SpellManager.CastItem(true, true);
            var ForcusTarget = TargetSelector.GetSelectedTarget();

            if (ForcusTarget != null && !ForcusTarget.IsDead && !ForcusTarget.IsZombie)
            {
                if (Q.IsReady())
                {
                    SpellManager.CastQ(ForcusTarget);
                    return;
                }

                if (W.IsReady() && ForcusTarget.IsValidTarget(W.Range) &&
                    !ForcusTarget.HasBuffOfType(BuffType.SpellShield) &&
                    (ForcusTarget.IsMelee || ForcusTarget.IsFacing(Me) || !Q.IsReady() ||
                     Me.HasBuff("RivenFeint") ||
                     qStack != 0))
                {
                    W.Cast();
                }
            }
            else if (Args.Target is AIHeroClient)
            {
                var target = (AIHeroClient)Args.Target;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    if (Q.IsReady())
                    {
                        SpellManager.CastQ(target);
                        return;
                    }

                    if (W.IsReady() && target.IsValidTarget(W.Range) && !target.HasBuffOfType(BuffType.SpellShield) &&
                        (target.IsMelee || target.IsFacing(Me) || !Q.IsReady() || Me.HasBuff("RivenFeint") ||
                         qStack != 0))
                    {
                        W.Cast();
                    }
                }
            }
        }

        internal static void InitBurst(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Burst)
            {
                return;
            }

            SpellManager.CastItem(true, true);

            var target = TargetSelector.GetSelectedTarget();

            if (target != null && !target.IsDead && !target.IsZombie)
            {
                if (Q.IsReady())
                {
                    SpellManager.CastQ(target);
                }
                else if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        internal static void InitMixed(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
            {
                return;
            }

            SpellManager.CastItem(true);
            var ForcusTarget = TargetSelector.GetSelectedTarget();

            if (ForcusTarget != null && !ForcusTarget.IsDead && !ForcusTarget.IsZombie)
            {
                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    if (Menu.GetList("HarassMode") == 0)
                    {
                        if (qStack == 1)
                        {
                            SpellManager.CastQ(ForcusTarget);
                        }
                    }
                    else
                    {
                        SpellManager.CastQ(ForcusTarget);
                    }
                }
            }
            else if (Args.Target is AIHeroClient)
            {
                var target = (AIHeroClient)Args.Target;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    if (Menu.GetBool("HarassQ") && Q.IsReady())
                    {
                        if (Menu.GetList("HarassMode") == 0)
                        {
                            if (qStack == 1)
                            {
                                SpellManager.CastQ(target);
                            }
                        }
                        else
                        {
                            SpellManager.CastQ(target);
                        }
                    }
                }
            }
        }

        internal static void InitClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
            {
                return;
            }

            if (Menu.GetBool("LaneClearQ") && Q.IsReady())
            {
                if (Args.Target.Type == GameObjectType.obj_AI_Turret || Args.Target.Type == GameObjectType.obj_Turret ||
                    Args.Target.Type == GameObjectType.obj_LampBulb)
                {
                    if (Q.IsReady() && !Args.Target.IsDead)
                    {
                        SpellManager.CastQ((Obj_AI_Base)Args.Target);
                    }
                }
                else if (Args.Target is Obj_AI_Minion)
                {
                    var minion = (Obj_AI_Minion)Args.Target;
                    var minions = MinionManager.GetMinions(Me.Position, 500f);

                    if (minion != null)
                    {
                        if (minions.Count >= 2)
                        {
                            SpellManager.CastItem(true);
                            SpellManager.CastQ(minion);
                        }
                    }
                }
            }
        }

        internal static void InitJungle(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear ||
                !(Args.Target is Obj_AI_Minion))
            {
                return;
            }

            var mobs = MinionManager.GetMinions(E.Range + Me.AttackRange, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var mob = mobs.FirstOrDefault();

            if (mob != null)
            {
                SpellManager.CastItem(true);

                if (Menu.GetBool("JungleClearE") && E.IsReady())
                {
                    E.Cast(mob.Position, true);
                }
                else if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                {
                    SpellManager.CastQ(mob);
                }
                else if (Menu.GetBool("JungleClearW") && W.IsReady() && mob.IsValidTarget(W.Range))
                {
                    W.Cast(true);
                }
            }
        }
    }
}