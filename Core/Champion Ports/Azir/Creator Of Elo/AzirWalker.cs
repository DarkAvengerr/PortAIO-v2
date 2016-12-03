using Azir_Creator_of_Elo;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Azir_Free_elo_Machine
{
    class AzirWalker : Orbwalking.Orbwalker
    {
        private const int SoldierAaRange = 250;
        private readonly AzirMain _azir;
        public AzirWalker(LeagueSharp.Common.Menu attachToMenu,AzirMain azir) : base(attachToMenu)
        {
            this._azir = azir;
        }

        private  float GetDamageValue(Obj_AI_Base target, bool soldierAttack)
        {
            var d = soldierAttack ?    _azir.Hero.GetSpellDamage(target, SpellSlot.W) :_azir.Hero.GetAutoAttackDamage(target);
            return target.Health / (float)d;
        }

        public override bool InAutoAttackRange(AttackableUnit target)
        {
            return CustomInAutoattackRange(target) != 0;
        }

        public int CustomInAutoattackRange(AttackableUnit target)
        {
            if (Orbwalking.InAutoAttackRange(target))
            {
                return 1;
            }

            if (!target.IsValidTarget())
            {
                return 0;
            }
            if (!(target is Obj_AI_Base))
            {
                return 0;
            }

            var soldierAArange = SoldierAaRange + 65 + target.BoundingRadius;
            soldierAArange *= soldierAArange;
            foreach (var soldier in _azir.SoldierManager.ActiveSoldiers)
            {
                if (soldier.Distance(target, true) <= soldierAArange)
                {
                    return 2;
                }
            }

            return 0;
        }

        public override AttackableUnit GetTarget()
        {
            AttackableUnit result;
            if (ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                foreach (var minion in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            minion =>
                                minion.IsValidTarget() &&
                                minion.Health <
                                3 *
                                (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod))
                    )
                {
                    var r = CustomInAutoattackRange(minion);
                    if (r != 0)
                    {
                        var t = (int)(_azir.Hero.AttackCastDelay * 1000) - 100 + Game.Ping / 2;
                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, 0);

                        var damage = (r == 1) ? _azir.Hero.GetAutoAttackDamage(minion, true) :_azir.Hero.GetSpellDamage(minion, SpellSlot.W);
                        if (minion.Team != GameObjectTeam.Neutral && MinionManager.IsMinion(minion, true))
                        {
                            if (predHealth > 0 && predHealth <= damage)
                            {
                                return minion;
                            }
                        }
                    }
                }
            }

            if (ActiveMode != Orbwalking.OrbwalkingMode.LastHit)
            {
                var posibleTargets = new Dictionary<Obj_AI_Base, float>();
                var autoAttackTarget = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                if (autoAttackTarget.IsValidTarget())
                {
                    posibleTargets.Add(autoAttackTarget, GetDamageValue(autoAttackTarget, false));
                }

                foreach (var soldier in _azir.SoldierManager.ActiveSoldiers)
                {
                    var soldierTarget = TargetSelector.GetTarget(SoldierAaRange + 65 + 65, TargetSelector.DamageType.Magical, true, null, soldier.ServerPosition);
                    if (soldierTarget.IsValidTarget())
                    {
                        if (posibleTargets.ContainsKey(soldierTarget))
                        {
                            posibleTargets[soldierTarget] *= 1.25f;
                        }
                        else
                        {
                            posibleTargets.Add(soldierTarget, GetDamageValue(soldierTarget, true));
                        }
                    }
                }

                if (posibleTargets.Count > 0)
                {
                    return posibleTargets.MinOrDefault(p => p.Value).Key;
                }
                var soldiers = _azir.SoldierManager.ActiveSoldiers;
                if (soldiers.Count > 0)
                {
                    var minions = MinionManager.GetMinions(1100, MinionTypes.All, MinionTeam.NotAlly);
                    var validEnemiesPosition = HeroManager.Enemies.Where(e => e.IsValidTarget(1100)).Select(e => e.ServerPosition.To2D()).ToList();
                    const int AAWidthSqr = 100 * 100;
                    //Try to harass using minions
                    foreach (var soldier in soldiers)
                    {
                        foreach (var minion in minions)
                        {
                            var soldierAArange = SoldierAaRange + 65 + minion.BoundingRadius;
                            soldierAArange *= soldierAArange;
                            if (soldier.Distance(minion, true) < soldierAArange)
                            {
                                var p1 = minion.Position.To2D();
                                var p2 = soldier.Position.To2D().Extend(minion.Position.To2D(), 375);
                                foreach (var enemyPosition in validEnemiesPosition)
                                {
                                    if (enemyPosition.Distance(p1, p2, true, true) < AAWidthSqr)
                                    {
                                        return minion;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            /* turrets / inhibitors / nexus */
            if (ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                /* turrets */
                foreach (var turret in
                    ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && Orbwalking.InAutoAttackRange(t)))
                {
                    return turret;
                }

                /* inhibitor */
                foreach (var turret in
                    ObjectManager.Get<Obj_BarracksDampener>().Where(t => t.IsValidTarget() && Orbwalking.InAutoAttackRange(t)))
                {
                    return turret;
                }

                /* nexus */
                foreach (var nexus in
                    ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && Orbwalking.InAutoAttackRange(t)))
                {
                    return nexus;
                }
            }

            /*Jungle minions*/
            if (ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                result =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            mob =>
                                mob.IsValidTarget() && Orbwalking.InAutoAttackRange(mob) && mob.Team == GameObjectTeam.Neutral)
                        .MaxOrDefault(mob => mob.MaxHealth);
                if (result != null)
                {
                    return result;
                }
            }

            if (ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                return (ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && InAutoAttackRange(minion))).MaxOrDefault(m => CustomInAutoattackRange(m) * m.Health);
            }

            return null;
        }
    }
}

