using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager
{
    using Common;
    using Spells;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;

    internal class AfterAttackManager : Logic
    {
        internal static void Init(AttackableUnit unit, AttackableUnit t)
        {
            if (!unit.IsMe || SpellManager.HaveQ3 || !Me.Spellbook.GetSpell(SpellSlot.Q).IsReady() || IsDashing)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = t as AIHeroClient;

                if (target != null && target.IsValidTarget(Q.Range))
                {
                    if (Menu.Item("ComboQ", true).GetValue<bool>())
                    {
                        Q.Cast(target, true);
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                ResetTurret(t);
                ResetJungle(t);
            }
        }

        private static void ResetTurret(AttackableUnit t)
        {
            var turret = t as Obj_AI_Turret;

            if (turret != null && t.IsValid)
            {
                if (HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= 600f) ||
                    MinionManager.GetMinions(Me.Position, 600f, MinionTypes.All, MinionTeam.NotAlly).Any())
                {
                    return;
                }

                if (!Items.HasItem(3057) || !Items.HasItem(3078))
                {
                    return;
                }

                if (Items.CanUseItem(3057) || Items.CanUseItem(3078))
                {
                    Q.Cast(turret.Position, true);
                }
            }
        }

        private static void ResetJungle(AttackableUnit t)
        {
            var mob = t as Obj_AI_Minion;
            var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (Menu.Item("JungleClearQ", true).GetValue<bool>() && mob != null && mobs.Contains(t) && t.Health > 0)
            {
                Q.Cast(mob.Position, true);
            }
        }
    }
}
