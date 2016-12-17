using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Events
{
    using System.Linq;
    using Spells;
    using FlowersDariusCommon;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class DoCastManager : Logic
    {
        internal static void Init(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Args.Target == null)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Args.Target.Type == GameObjectType.AIHeroClient)
            {
                SpellManager.CastItem();

                var target = (AIHeroClient) Args.Target;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    if (Menu.GetBool("ComboW") && W.IsReady() && Orbwalking.InAutoAttackRange(target))
                    {
                        W.Cast();
                        Orbwalker.ForceTarget(target);
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Args.Target.Type == GameObjectType.AIHeroClient)
            {
                SpellManager.CastItem();

                var target = (AIHeroClient)Args.Target;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    if (Menu.GetBool("HarassW") && W.IsReady() && Orbwalking.InAutoAttackRange(target))
                    {
                        W.Cast();
                        Orbwalker.ForceTarget(target);
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear(Args);
                JungleClear(Args);
            }
        }

        private static void LaneClear(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Args.Target == null ||
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear || !Args.Target.IsEnemy || 
                !ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) || !ManaManager.SpellFarm)
            {
                return;
            }

            if (Menu.GetBool("LaneClearW") && W.IsReady())
            {
                if (Args.Target.Type == GameObjectType.obj_AI_Turret || Args.Target.Type == GameObjectType.obj_Turret ||
                    Args.Target.Type == GameObjectType.obj_LampBulb)
                {
                    if (!Args.Target.IsDead)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var minion = (Obj_AI_Minion)Args.Target;

                    if (minion != null && minion.Health <= DamageCalculate.GetWDamage(minion))
                    {
                        W.Cast();
                        Orbwalker.ForceTarget(minion);
                    }
                }
            }
        }

        private static void JungleClear(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Args.Target == null ||
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear || Args.Target.Type != GameObjectType.obj_AI_Minion ||
                !ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) || !ManaManager.SpellFarm)
            {
                return;
            }

            var mobs = MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(Me), MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var mob = mobs.FirstOrDefault();

            if (mob != null)
            {
                SpellManager.CastItem();

                if (Menu.GetBool("JungleClearW") && W.IsReady() && Orbwalking.InAutoAttackRange(Me))
                {
                    W.Cast(true);
                }
            }
        }
    }
}