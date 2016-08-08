using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace xSaliceResurrected.Managers
{
    class AzirManager : Orbwalking.Orbwalker
    {
        private static readonly AIHeroClient MyHero = ObjectManager.Player;
        public static readonly List<Obj_AI_Minion> Soldiers = new List<Obj_AI_Minion>();

        public AzirManager(Menu attachToMenu) : base(attachToMenu)
        {
        }

        public static double GetAzirAaSandwarriorDamage(AttackableUnit target)
        {
            var unit = (Obj_AI_Base)target;
            var dmg = MyHero.LSGetSpellDamage(unit, SpellSlot.W);

            var count = Soldiers.Count(obj => obj.Position.LSDistance(unit.Position) < 350);

            return dmg * count;
        }

        public static bool InSoldierAttackRange(AttackableUnit target)
        {
            return Soldiers.Count(obj => obj.Position.LSDistance(target.Position) < 350 && MyHero.LSDistance(target) < 1000 && !obj.IsMoving) > 0;
        }

        private static float GetAutoAttackRange(Obj_AI_Base source = null, AttackableUnit target = null)
        {
            if (source == null)
                source = MyHero;
            var ret = source.AttackRange + MyHero.BoundingRadius;
            if (target != null)
                ret += target.BoundingRadius;
            return ret;
        }

        public override bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.LSIsValidTarget())
                return false;
            if (Orbwalking.InAutoAttackRange(target))
                return true;
            if (!(target is Obj_AI_Base))
                return false;
            if (InSoldierAttackRange(target))
            {
                return true;
            }
            return false;
        }

        public override AttackableUnit GetTarget()
        {
            AttackableUnit tempTarget = null;

            if ((ActiveMode == Orbwalking.OrbwalkingMode.Mixed || ActiveMode == Orbwalking.OrbwalkingMode.Combo))
            {
                tempTarget = GetBestHeroTarget();
                if (tempTarget != null)
                    return tempTarget;
            }

            //last hit
            if (ActiveMode == Orbwalking.OrbwalkingMode.Mixed || ActiveMode == Orbwalking.OrbwalkingMode.LastHit || ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                foreach (var minion in from minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget() && minion.Name != "Beacon" && InAutoAttackRange(minion)
                && minion.Health < 3 * (MyHero.BaseAttackDamage + MyHero.FlatPhysicalDamageMod))
                                       let t = (int)(MyHero.AttackCastDelay * 1000) - 100 + Game.Ping / 2
                                       let predHealth = HealthPrediction.GetHealthPrediction(minion, t, 0)
                                       where minion.Team != GameObjectTeam.Neutral && predHealth > 0 && predHealth <= (InSoldierAttackRange(minion) ? GetAzirAaSandwarriorDamage(minion) - 30 : MyHero.LSGetAutoAttackDamage(minion, true))
                                       select minion)
                    return minion;
            }

            //turret
            if (ActiveMode == Orbwalking.OrbwalkingMode.Mixed || ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {

                foreach (
                    var turret in
                        ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.LSIsValidTarget(GetAutoAttackRange(MyHero, turret))))
                    return turret;
            }

            //jungle
            if (ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                float[] maxhealth;
                if (MyHero.ChampionName == "Azir" && Soldiers.Count > 0)
                {
                    maxhealth = new float[] { 0 };
                    var maxhealth1 = maxhealth;
                    var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 800, MinionTypes.All, MinionTeam.Neutral);
                    foreach (
                        var minion in
                            minions
                                .Where(minion => InSoldierAttackRange(minion) && minion.Name != "Beacon" && minion.LSIsValidTarget())
                                .Where(minion => minion.MaxHealth >= maxhealth1[0] || Math.Abs(maxhealth1[0] - float.MaxValue) < float.Epsilon))
                    {
                        tempTarget = minion;
                        maxhealth[0] = minion.MaxHealth;
                    }
                    if (tempTarget != null)
                        return tempTarget;
                }

                maxhealth = new float[] { 0 };
                var maxhealth2 = maxhealth;
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget(GetAutoAttackRange(MyHero, minion)) && minion.Name != "Beacon" && minion.Team == GameObjectTeam.Neutral).Where(minion => minion.MaxHealth >= maxhealth2[0] || Math.Abs(maxhealth2[0] - float.MaxValue) < float.Epsilon))
                {
                    tempTarget = minion;
                    maxhealth[0] = minion.MaxHealth;
                }
                if (tempTarget != null)
                    return tempTarget;
            }

            if (ShouldWaits())
                return null;

            //lane clear
            if (ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                return (ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget() && InAutoAttackRange(minion))).MaxOrDefault(x => x.Health);
            }

            return null;
        }

        private bool ShouldWaits()
        {
            return ObjectManager.Get<Obj_AI_Minion>()
            .Any(
            minion =>
            minion.LSIsValidTarget(850) && minion.Team != GameObjectTeam.Neutral &&
            InAutoAttackRange(minion) &&
            HealthPrediction.LaneClearHealthPrediction(minion, (int)((MyHero.AttackDelay * 1000) * 2f), 0) <=
            (InSoldierAttackRange(minion) ? GetAzirAaSandwarriorDamage(minion) - 30 : MyHero.LSGetAutoAttackDamage(minion, true)));
        }

        private AIHeroClient GetBestHeroTarget()
        {
            var bestTarget = HeroManager.Enemies.Where(InAutoAttackRange).OrderByDescending(GetAzirAaSandwarriorDamage).FirstOrDefault();

            return bestTarget ?? TargetSelector.GetTarget(GetAutoAttackRange(), TargetSelector.DamageType.Magical);
        }

        public static void OnDelete(GameObject sender, EventArgs args)
        {
            Soldiers.RemoveAll(s => s.NetworkId == sender.NetworkId);
        }

        public static void Obj_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion))
                return;

            if (sender.Name == "AzirSoldier" && sender.IsAlly)
            {
                Obj_AI_Minion soldier = (Obj_AI_Minion) sender;
                if (soldier.BaseSkinName == "AzirSoldier")
                    Soldiers.Add(soldier);
            }
        }
    }
}
