using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public class Mechanics
    {
        public static IDictionary<SpellSlot, SpellData> Spells;
        public static Spell IgniteSpell;
        public static Spell FlashSpell;
        private static int _flashCastTick;

        public static void ProcessCombo(AIHeroClient target)
        {
            ProcessSpells(
                EntryPoint.Menu.Item("l33t.stds.combo.useQ").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.combo.useW").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.combo.useE").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.combo.useR").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.combo.useQE").GetValue<bool>(),
                (EntryPoint.Menu.Item("l33t.stds.mode").GetValue<StringList>().SelectedIndex == 0 &&
                 target.IsValidTarget())
                    ? target
                    : null);
        }

        private static void ProcessSpells(bool useQ,
            bool useW,
            bool useE,
            bool useR,
            bool useSphereE,
            AIHeroClient target = null)
        {
            var targets = GetTargets(target);

            if (useR && Spells[SpellSlot.R].IsReady() && targets[SpellSlot.R].IsValidTarget(Spells[SpellSlot.R].Range))
            {
                ProcessUltimate();
            }

            if (IgniteSpell.IsReady())
            {
                ProcessIgnite(targets[SpellSlot.Ignite]);
            }

            if (useSphereE && targets[SpellSlot.SphereE].IsValidTarget() &&
                !Collision.DetectCollision(targets[SpellSlot.SphereE]) && Spells[SpellSlot.Q].IsReady() &&
                (Spells[SpellSlot.E].IsReady() ||
                 (Spells[SpellSlot.E].Instance.Instance.CooldownExpires - Game.Time < 1 && Spells[SpellSlot.E].Level > 0)) &&
                Spells[SpellSlot.Q].Instance.Instance.SData.Mana + Spells[SpellSlot.E].Instance.Instance.SData.Mana <=
                EntryPoint.Player.Mana)
            {
                ProcessSphereE(targets[SpellSlot.SphereE]);
            }

            if (useQ && targets[SpellSlot.SphereE].IsValidTarget() && Spells[SpellSlot.Q].IsReady())
            {
                ProcessQ(targets[SpellSlot.SphereE]);
            }

            if (useE && Spells[SpellSlot.E].IsReady() &&
                (int) (Game.Time * 0x3E8) - Spells[SpellSlot.W].LastCastAttemptTick > Game.Ping + 150 &&
                (int) (Game.Time * 0x3E8) - Spells[SpellSlot.Q].LastCastAttemptTick > Game.Ping)
            {
                foreach (var enemy in ObjectCache.GetHeroes().Where(e => e.IsValidTarget(Spells[SpellSlot.E].Range)))
                {
                    if (enemy.GetComboDamage(useQ, useW, true, useR) > enemy.Health &&
                        EntryPoint.Player.Distance(enemy, true) <= Math.Pow(Spells[SpellSlot.E].Range, 2))
                    {
                        Spells[SpellSlot.E].Instance.Cast(enemy);
                        Spells[SpellSlot.E].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                    else if (EntryPoint.Player.Distance(enemy, true) <= Math.Pow(Spells[SpellSlot.SphereE].Range, 2))
                    {
                        ProcessE(enemy);
                    }
                }
            }

            if (useW && targets[SpellSlot.W].IsValidTarget() && Spells[SpellSlot.W].IsReady())
            {
                ProcessW(targets[SpellSlot.SphereE], targets[SpellSlot.W]);
            }
        }

        private static void ProcessUltimate()
        {
            var target = TargetSelector.GetTarget(Spells[SpellSlot.R].Range, TargetSelector.DamageType.Magical);

            if (!Collision.DetectCollision(target) && Spells[SpellSlot.R].Damage(target) > target.Health &&
                UnleashedPowerCheck(target) && !target.BuffCheck())
            {
                if (
                    !(Spells[SpellSlot.Q].Damage(target) > target.Health &&
                      Spells[SpellSlot.Q].Instance.Instance.CooldownExpires - Game.Time < 2 &&
                      Spells[SpellSlot.Q].Instance.Instance.CooldownExpires - Game.Time >= 0 && target.IsStunned) &&
                    (int) (Game.Time * 0x3E8) - Spells[SpellSlot.Q].LastCastAttemptTick > 500 + Game.Ping)
                {
                    Spells[SpellSlot.R].Instance.Cast(target);
                    Spells[SpellSlot.R].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                }
            }
        }

        private static void ProcessIgnite(AIHeroClient target)
        {
            if (IgniteSpell.Slot != EloBuddy.SpellSlot.Unknown && target.IsValidTarget() &&
                EntryPoint.Player.Distance(target) <= 360000 && target.GetIgniteDamage() > target.Health)
            {
                if (EntryPoint.Menu.Item("l33t.stds.misc.ignitecd").GetValue<bool>())
                {
                    var currentTick = (int) (Game.Time * 0x3E8);
                    var tickDelay = 750 + Game.Ping;
                    if (!Spells[SpellSlot.Q].IsReady() && !Spells[SpellSlot.W].IsReady() &&
                        !Spells[SpellSlot.E].IsReady() && !Spells[SpellSlot.R].IsReady() &&
                        currentTick - Spells[SpellSlot.Q].LastCastAttemptTick > tickDelay &&
                        currentTick - Spells[SpellSlot.SphereE].LastCastAttemptTick > tickDelay &&
                        currentTick - Spells[SpellSlot.W].LastCastAttemptTick > tickDelay)
                    {
                        IgniteSpell.Cast(target);
                    }
                }
                else
                {
                    IgniteSpell.Cast(target);
                }
            }
        }

        private static IDictionary<SpellSlot, AIHeroClient> GetTargets(AIHeroClient target)
        {
            if (target.IsValidTarget())
            {
                return new Dictionary<SpellSlot, AIHeroClient>
                {
                    { SpellSlot.Q, target },
                    { SpellSlot.W, target },
                    { SpellSlot.E, target },
                    { SpellSlot.R, target },
                    { SpellSlot.SphereE, target },
                    { SpellSlot.Ignite, target }
                };
            }
            var qTarget = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range + 25f, TargetSelector.DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(
                Spells[SpellSlot.W].Range + Spells[SpellSlot.W].Instance.Width, TargetSelector.DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(Spells[SpellSlot.R].Range, TargetSelector.DamageType.Magical);
            if (rTarget.IsValidTarget() &&
                !EntryPoint.Menu.Item("l33t.stds.ups.ignore." + rTarget.ChampionName).GetValue<bool>())
            {
                rTarget =
                    ObjectCache.GetHeroes()
                        .FirstOrDefault(
                            hero => !EntryPoint.Menu.Item("l33t.stds.ups.ignore." + hero.ChampionName).GetValue<bool>());
            }
            var qeTarget = TargetSelector.GetTarget(Spells[SpellSlot.SphereE].Range, TargetSelector.DamageType.Magical);
            var igniteTarget = TargetSelector.GetTarget(IgniteSpell.Range, TargetSelector.DamageType.True);

            return new Dictionary<SpellSlot, AIHeroClient>
            {
                { SpellSlot.Q, qTarget },
                { SpellSlot.W, wTarget },
                { SpellSlot.R, rTarget },
                { SpellSlot.SphereE, qeTarget },
                { SpellSlot.Ignite, igniteTarget }
            };
        }

        public static void ProcessSphereE(Obj_AI_Base target)
        {
            if (Spells[SpellSlot.Q].IsReady() && Spells[SpellSlot.E].IsReady() && target.IsValidTarget())
            {
                var targetPosition =
                    Prediction.GetPrediction(target, Spells[SpellSlot.Q].Delay + Spells[SpellSlot.E].Delay).UnitPosition;
                if (EntryPoint.Player.Distance(target, true) > Math.Pow(Spells[SpellSlot.E].Range, 2))
                {
                    var sphere = EntryPoint.Player.Position +
                                 (targetPosition - EntryPoint.Player.Position).Normalized() * Spells[SpellSlot.E].Range;
                    Spells[SpellSlot.SphereE].Instance.Delay = Spells[SpellSlot.Q].Delay + Spells[SpellSlot.E].Delay +
                                                               EntryPoint.Player.Distance(sphere) /
                                                               Spells[SpellSlot.E].Instance.Speed;
                    var prediction = Spells[SpellSlot.SphereE].Instance.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.Medium)
                    {
                        CastSphereE(target, sphere);
                    }
                }
                else
                {
                    var width = Spells[SpellSlot.Q].Instance.Width;
                    Spells[SpellSlot.Q].Instance.Width = 48;
                    var prediction = Spells[SpellSlot.Q].Instance.GetPrediction(target, true);
                    Spells[SpellSlot.Q].Instance.Width = width;
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        CastSphereE(target, prediction.UnitPosition);
                    }
                }
            }
        }

        private static void CastSphereE(Obj_AI_Base target, Vector3 sphere)
        {
            if (target.IsValidTarget() &&
                EntryPoint.Player.Distance(sphere, true) <= Math.Pow(Spells[SpellSlot.E].Range, 2))
            {
                var sp = sphere + (EntryPoint.Player.ServerPosition - sphere).Normalized() * 100f;
                var esp = sphere + (sphere - EntryPoint.Player.ServerPosition).Normalized() * 592f;

                Spells[SpellSlot.SphereE].Instance.Delay = Spells[SpellSlot.Q].Delay + Spells[SpellSlot.E].Delay +
                                                           EntryPoint.Player.Distance(sphere) /
                                                           Spells[SpellSlot.E].MissileSpeed;
                Spells[SpellSlot.SphereE].Instance.UpdateSourcePosition(sphere);

                var segment =
                    Spells[SpellSlot.SphereE].Instance.GetPrediction(target)
                        .UnitPosition.To2D()
                        .ProjectOn(sp.To2D(), esp.To2D());
                if (segment.IsOnSegment ||
                    (segment.SegmentPoint.Distance(target, true) <=
                     Math.Pow(Spells[SpellSlot.SphereE].Instance.Width + target.BoundingRadius, 2)))
                {
                    var delay = 280 - (int) (EntryPoint.Player.Distance(sphere) / 2.5f) +
                                EntryPoint.Menu.Item("l33t.stds.qesettings.qedelay").GetValue<Slider>().Value;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        Math.Max(0, delay), () =>
                        {
                            Spells[SpellSlot.E].Instance.Cast(sphere);
                            Spells[SpellSlot.E].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                        });
                    Spells[SpellSlot.SphereE].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    Spells[SpellSlot.Q].Instance.Cast(sphere);
                    Spells[SpellSlot.Q].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    ProcessE(target);
                }
            }
        }

        private static void ProcessQ(Obj_AI_Base target)
        {
            var prediction = Spells[SpellSlot.Q].Instance.GetPrediction(target, true);
            if (prediction.Hitchance >= HitChance.VeryHigh)
            {
                Spells[SpellSlot.Q].Instance.Cast(prediction.UnitPosition);
                Spells[SpellSlot.Q].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
            }
        }

        private static void ProcessE(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                foreach (var sphere in
                    SphereManager.GetSpheres(true)
                        .Where(
                            o =>
                                o.To2D().IsValid() &&
                                EntryPoint.Player.Distance(o, true) < Math.Pow(Spells[SpellSlot.E].Range, 2)))
                {
                    var sp = sphere.To2D() +
                             (EntryPoint.Player.ServerPosition.To2D() - sphere.To2D()).Normalized() * 100f;
                    var esp = sphere.To2D() +
                              (sphere.To2D() - EntryPoint.Player.ServerPosition.To2D()).Normalized() * 592f;

                    Spells[SpellSlot.SphereE].Instance.Delay = Spells[SpellSlot.Q].Delay + Spells[SpellSlot.E].Delay +
                                                               EntryPoint.Player.Distance(sphere) /
                                                               Spells[SpellSlot.E].MissileSpeed;
                    Spells[SpellSlot.SphereE].Instance.UpdateSourcePosition(sphere);

                    var prediction = Spells[SpellSlot.SphereE].Instance.GetPrediction(target).UnitPosition.To2D();
                    if (prediction.Distance(sp, esp, true, true) <=
                        Math.Pow(Spells[SpellSlot.SphereE].Instance.Width + target.BoundingRadius, 2))
                    {
                        Spells[SpellSlot.E].Instance.Cast(sphere);
                        Spells[SpellSlot.E].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                }
            }
        }

        private static bool UnleashedPowerCheck(AIHeroClient target)
        {
            var currentTick = (int) (Game.Time * 0x3E8);
            var tickDelay = 600 + Game.Ping;

            var attackDamage = 0d;
            if (EntryPoint.Menu.Item("l33t.stds.ups.disable.AA").GetValue<bool>())
            {
                attackDamage = EntryPoint.Player.GetAutoAttackDamage(target);
            }

            var includeQ = EntryPoint.Menu.Item("l33t.stds.ups.disable.Q").GetValue<bool>();
            var includeW = EntryPoint.Menu.Item("l33t.stds.ups.disable.W").GetValue<bool>();
            var includeE = EntryPoint.Menu.Item("l33t.stds.ups.disable.E").GetValue<bool>();
            switch (EntryPoint.Menu.Item("l33t.stds.ups.disable").GetValue<StringList>().SelectedIndex)
            {
                case 2:
                    return true;
                case 0:
                    return !(target.GetComboDamage(includeQ, includeW, includeE, false) + attackDamage >= target.Health);
                case 1:
                    return
                        !(target.GetComboDamage(includeQ, false, false, false) >= target.Health ||
                          target.GetComboDamage(false, includeW, false, false) >= target.Health ||
                          target.GetComboDamage(false, false, includeE, false) >= target.Health);
            }

            return currentTick - Spells[SpellSlot.Q].LastCastAttemptTick > tickDelay &&
                   currentTick - Spells[SpellSlot.E].LastCastAttemptTick > tickDelay &&
                   currentTick - Spells[SpellSlot.W].LastCastAttemptTick > tickDelay;
        }

        private static void ProcessW(Obj_AI_Base target, Obj_AI_Base officialTarget)
        {
            if (target.IsValidTarget() && Spells[SpellSlot.W].IsReady() &&
                Spells[SpellSlot.W].Instance.Instance.ToggleState == 1)
            {
                var gameObjectPos = GetGrabbableObjectPos(false);
                if (gameObjectPos.To2D().IsValid() &&
                    (int) (Game.Time * 0x3E8) - Spells[SpellSlot.Q].LastCastAttemptTick > Game.Ping + 150 &&
                    (int) (Game.Time * 0x3E8) - Spells[SpellSlot.W].LastCastAttemptTick > Game.Ping + 750 &&
                    (int) (Game.Time * 0x3E8) - Spells[SpellSlot.E].LastCastAttemptTick > Game.Ping + 750)
                {
                    var grab = false;
                    if (officialTarget != null)
                    {
                        var prediction = Spells[SpellSlot.W].Instance.GetPrediction(officialTarget, true);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            grab = true;
                        }
                    }
                    if (grab || target.IsStunned)
                    {
                        Spells[SpellSlot.W].Instance.Cast(gameObjectPos);
                        Spells[SpellSlot.W].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                }
            }
            else if (officialTarget.IsValidTarget() && Spells[SpellSlot.W].IsReady() &&
                     Spells[SpellSlot.W].Instance.Instance.ToggleState == 2)
            {
                var prediction = Spells[SpellSlot.W].Instance.GetPrediction(officialTarget, true);
                if (prediction.Hitchance >= HitChance.High)
                {
                    Spells[SpellSlot.W].Instance.Cast(prediction.UnitPosition);
                    Spells[SpellSlot.W].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                }
            }
        }

        private static Vector3 GetGrabbableObjectPos(bool onlyOrbs)
        {
            if (onlyOrbs)
            {
                return SphereManager.GetGrabbableSpheres((int) Spells[SpellSlot.W].Range);
            }
            foreach (var minion in
                ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(Spells[SpellSlot.W].Range)))
            {
                return minion.ServerPosition;
            }
            return SphereManager.GetGrabbableSpheres((int) Spells[SpellSlot.W].Range);
        }

        public static void ProcessFarm(bool laneClear = false)
        {
            if (!Orbwalking.CanMove(40) &&
                (laneClear &&
                 EntryPoint.Menu.Item("l33t.stds.farming.lcmana").GetValue<Slider>().Value <
                 EntryPoint.Player.ManaPercent ||
                 !laneClear &&
                 EntryPoint.Menu.Item("l33t.stds.farming.farmmana").GetValue<Slider>().Value <
                 EntryPoint.Player.ManaPercent))
            {
                return;
            }

            var rangedMinionsQ = ObjectCache.GetMinions(
                ObjectManager.Player.ServerPosition, Spells[SpellSlot.Q].Range + Spells[SpellSlot.Q].Instance.Width + 30,
                MinionTypes.Ranged);
            var allMinionsQ = ObjectCache.GetMinions(
                ObjectManager.Player.ServerPosition, Spells[SpellSlot.Q].Range + Spells[SpellSlot.Q].Instance.Width + 30);
            var rangedMinionsW = ObjectCache.GetMinions(
                ObjectManager.Player.ServerPosition, Spells[SpellSlot.W].Range + Spells[SpellSlot.W].Instance.Width + 30,
                MinionTypes.Ranged);
            var allMinionsW = ObjectCache.GetMinions(
                ObjectManager.Player.ServerPosition, Spells[SpellSlot.W].Range + Spells[SpellSlot.W].Instance.Width + 30);
            var rangedMinionsE = ObjectCache.GetMinions(
                ObjectManager.Player.ServerPosition, Spells[SpellSlot.E].Range + Spells[SpellSlot.E].Instance.Width + 30,
                MinionTypes.Ranged);
            var allMinionsE = ObjectCache.GetMinions(
                ObjectManager.Player.ServerPosition, Spells[SpellSlot.E].Range + Spells[SpellSlot.E].Instance.Width + 30);
            var useQi = EntryPoint.Menu.Item("l33t.stds.farming.qmode").GetValue<StringList>().SelectedIndex;
            var useWi = EntryPoint.Menu.Item("l33t.stds.farming.wmode").GetValue<StringList>().SelectedIndex;
            var useEi = EntryPoint.Menu.Item("l33t.stds.farming.emode").GetValue<StringList>().SelectedIndex;
            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useW = (laneClear && (useWi == 1 || useWi == 2)) || (!laneClear && (useWi == 0 || useWi == 2));
            var useE = (laneClear && (useEi == 1 || useEi == 2)) || (!laneClear && (useEi == 0 || useEi == 2));

            ProcessFarmQ(useQ, laneClear, rangedMinionsQ, allMinionsQ);
            if (laneClear)
            {
                ProcessFarmW(useW, rangedMinionsW, allMinionsW);
                ProcessFarmE(useE, rangedMinionsE, allMinionsE);
            }
        }

        private static void ProcessFarmE(bool useE, List<Obj_AI_Base> rangedMinionsE, List<Obj_AI_Base> allMinionsE)
        {
            if (useE && Spells[SpellSlot.E].IsReady() && allMinionsE.Count >= 3)
            {
                var fl1 = Spells[SpellSlot.E].Instance.GetCircularFarmLocation(
                    rangedMinionsE, Spells[SpellSlot.E].Instance.Width);
                var fl2 = Spells[SpellSlot.E].Instance.GetCircularFarmLocation(
                    allMinionsE, Spells[SpellSlot.E].Instance.Width);
                if (fl1.MinionsHit >= 3 && Spells[SpellSlot.E].Instance.IsInRange(fl1.Position.To3D()))
                {
                    Spells[SpellSlot.E].Instance.Cast(fl1.Position);
                    Spells[SpellSlot.E].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                }
                else if (fl2.MinionsHit >= 1 && Spells[SpellSlot.E].Instance.IsInRange(fl1.Position.To3D()) &&
                         fl1.MinionsHit <= 2)
                {
                    Spells[SpellSlot.E].Instance.Cast(fl2.Position);
                    Spells[SpellSlot.E].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                }
            }
        }

        private static void ProcessFarmW(bool useW, List<Obj_AI_Base> rangedMinionsW, List<Obj_AI_Base> allMinionsW)
        {
            if (useW && Spells[SpellSlot.W].IsReady() && allMinionsW.Count >= 3)
            {
                if (Spells[SpellSlot.W].Instance.Instance.ToggleState == 1)
                {
                    var gObjectPos = GetGrabbableObjectPos(false);
                    if (gObjectPos.To2D().IsValid() &&
                        Environment.TickCount - Spells[SpellSlot.W].LastCastAttemptTick > Game.Ping + 150)
                    {
                        Spells[SpellSlot.W].Instance.Cast(gObjectPos);
                        Spells[SpellSlot.W].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                }
                else if (Spells[SpellSlot.W].Instance.Instance.ToggleState == 2)
                {
                    var fl1 = Spells[SpellSlot.Q].Instance.GetCircularFarmLocation(
                        rangedMinionsW, Spells[SpellSlot.W].Instance.Width);
                    var fl2 = Spells[SpellSlot.Q].Instance.GetCircularFarmLocation(
                        allMinionsW, Spells[SpellSlot.W].Instance.Width);
                    if (fl1.MinionsHit >= 3 && Spells[SpellSlot.W].Instance.IsInRange(fl1.Position.To3D()))
                    {
                        Spells[SpellSlot.W].Instance.Cast(fl1.Position);
                        Spells[SpellSlot.W].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                    else if (fl2.MinionsHit >= 1 && Spells[SpellSlot.W].Instance.IsInRange(fl2.Position.To3D()) &&
                             fl1.MinionsHit <= 2)
                    {
                        Spells[SpellSlot.W].Instance.Cast(fl2.Position);
                        Spells[SpellSlot.W].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                }
            }
        }

        private static void ProcessFarmQ(bool useQ,
            bool laneClear,
            List<Obj_AI_Base> rangedMinionsQ,
            List<Obj_AI_Base> allMinionsQ)
        {
            if (useQ && Spells[SpellSlot.Q].IsReady())
            {
                if (laneClear)
                {
                    var fl1 = Spells[SpellSlot.Q].Instance.GetCircularFarmLocation(
                        rangedMinionsQ, Spells[SpellSlot.Q].Instance.Width);
                    var fl2 = Spells[SpellSlot.Q].Instance.GetCircularFarmLocation(
                        allMinionsQ, Spells[SpellSlot.Q].Instance.Width);
                    if (fl1.MinionsHit >= 3)
                    {
                        Spells[SpellSlot.Q].Instance.Cast(fl1.Position);
                        Spells[SpellSlot.Q].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                    else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                    {
                        Spells[SpellSlot.Q].Instance.Cast(fl2.Position);
                        Spells[SpellSlot.Q].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                }
                else
                {
                    foreach (var minion in
                        allMinionsQ.Where(
                            minion =>
                                !Orbwalking.InAutoAttackRange(minion) &&
                                minion.Health < 0.75 * Spells[SpellSlot.Q].Damage(minion)))
                    {
                        Spells[SpellSlot.Q].Instance.Cast(minion);
                        Spells[SpellSlot.Q].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                    }
                }
            }
        }

        public static void JungleFarm()
        {
            var useQ = EntryPoint.Menu.Item("l33t.stds.junglefarming.qmode").GetValue<bool>();
            var useW = EntryPoint.Menu.Item("l33t.stds.junglefarming.wmode").GetValue<bool>();
            var useE = EntryPoint.Menu.Item("l33t.stds.junglefarming.emode").GetValue<bool>();
            var mobs = ObjectCache.GetMinions(
                ObjectManager.Player.ServerPosition, Spells[SpellSlot.W].Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (mobs.Count <= 0)
            {
                return;
            }
            var mob = mobs[0];
            if (Spells[SpellSlot.Q].IsReady() && useQ)
            {
                Spells[SpellSlot.Q].Instance.Cast(mob);
                Spells[SpellSlot.Q].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
            }
            if (Spells[SpellSlot.W].IsReady() && useW &&
                Environment.TickCount - Spells[SpellSlot.Q].LastCastAttemptTick > 800)
            {
                Spells[SpellSlot.W].Instance.Cast(mob);
                Spells[SpellSlot.W].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
            }
            if (useE && Spells[SpellSlot.E].IsReady())
            {
                Spells[SpellSlot.E].Instance.Cast(mob);
                Spells[SpellSlot.E].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.SData.Name)
                {
                    case "SyndraQ":
                        Spells[SpellSlot.Q].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                        break;
                    case "SyndraW":
                    case "syndrawcast":
                        Spells[SpellSlot.W].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                        break;
                    case "SyndraE":
                    case "syndrae5":
                        Spells[SpellSlot.E].LastCastAttemptTick = (int) (Game.Time * 0x3E8);
                        break;
                }
            }

            if (EntryPoint.Menu.Item("l33t.stds.harass.useQAA").GetValue<bool>() &&
                sender.Type == EntryPoint.Player.Type && sender.IsEnemy && args.SData.Name.ToLower().Contains("attack") &&
                EntryPoint.Player.Distance(sender, true) <= Math.Pow(Spells[SpellSlot.Q].Range, 2) &&
                EntryPoint.Player.ManaPercent > EntryPoint.Menu.Item("l33t.stds.harass.mana").GetValue<Slider>().Value)
            {
                ProcessQ(sender);
            }
        }

        public static void ProcessHarass(AIHeroClient target)
        {
            if (EntryPoint.Menu.Item("l33t.stds.harass.turret").GetValue<bool>() &&
                EntryPoint.Player.Position.UnderTurret(true) ||
                EntryPoint.Player.ManaPercent < EntryPoint.Menu.Item("l33t.stds.harass.mana").GetValue<Slider>().Value)
            {
                return;
            }

            ProcessSpells(
                EntryPoint.Menu.Item("l33t.stds.harass.useQ").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.harass.useW").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.harass.useE").GetValue<bool>(), false,
                EntryPoint.Menu.Item("l33t.stds.harass.useQE").GetValue<bool>(),
                (EntryPoint.Menu.Item("l33t.stds.mode").GetValue<StringList>().SelectedIndex == 0 &&
                 target.IsValidTarget())
                    ? target
                    : null);
        }

        public static void ProcessKillSteal()
        {
            foreach (var enemy in
                ObjectCache.GetHeroes()
                    .Where(
                        e =>
                            !e.HasBuff("UndyingRage") && !e.HasBuff("JudicatorIntervention") &&
                            e.IsValidTarget(Spells[SpellSlot.SphereE].Range) &&
                            (int) (Game.Time * 0x3E8) - _flashCastTick > 650 + Game.Ping))
            {
                if (
                    enemy.GetComboDamage(
                        false, false, EntryPoint.Menu.Item("l33t.stds.ks.useQE").GetValue<bool>(), false) > enemy.Health &&
                    EntryPoint.Player.Distance(enemy, true) <= Math.Pow(Spells[SpellSlot.SphereE].Range, 2))
                {
                    ProcessSpells(
                        false, false, false, false, EntryPoint.Menu.Item("l33t.stds.ks.useQE").GetValue<bool>());
                    Audio.PlaySound();
                }
                else if (
                    enemy.GetComboDamage(
                        false, EntryPoint.Menu.Item("l33t.stds.ks.useW").GetValue<bool>(), false, false) >
                    enemy.Health &&
                    EntryPoint.Player.Distance(enemy, true) <= Math.Pow(Spells[SpellSlot.W].Range, 2))
                {
                    ProcessSpells(
                        false, EntryPoint.Menu.Item("l33t.stds.ks.useW").GetValue<bool>(), false, false, false);
                    Audio.PlaySound();
                }
                else if (
                    enemy.GetComboDamage(
                        EntryPoint.Menu.Item("l33t.stds.ks.useQ").GetValue<bool>(), false,
                        EntryPoint.Menu.Item("l33t.stds.ks.useE").GetValue<bool>(), false) > enemy.Health &&
                    EntryPoint.Player.Distance(enemy, true) <= Math.Pow(Spells[SpellSlot.Q].Range + 25f, 2))
                {
                    ProcessSpells(
                        EntryPoint.Menu.Item("l33t.stds.ks.useQ").GetValue<bool>(), false,
                        EntryPoint.Menu.Item("l33t.stds.ks.useE").GetValue<bool>(), false, false);
                    Audio.PlaySound();
                }
                else if (
                    enemy.GetComboDamage(
                        EntryPoint.Menu.Item("l33t.stds.ks.useQ").GetValue<bool>(), false, false, false) >
                    enemy.Health &&
                    EntryPoint.Player.Distance(enemy, true) <= Math.Pow(Spells[SpellSlot.Q].Range + 25f, 2))
                {
                    ProcessSpells(
                        EntryPoint.Menu.Item("l33t.stds.ks.useQ").GetValue<bool>(), false, false, false,
                        false);
                    Audio.PlaySound();
                }
                else if (
                    enemy.GetComboDamage(
                        false, false, false, EntryPoint.Menu.Item("l33t.stds.ks.useR").GetValue<bool>()) >
                    enemy.Health &&
                    EntryPoint.Player.Distance(enemy, true) <= Math.Pow(Spells[SpellSlot.R].Range, 2))
                {
                    ProcessSpells(
                        false, false, false, false,
                        EntryPoint.Menu.Item("l33t.stds.ks.useR").GetValue<bool>());
                    Audio.PlaySound();
                }
                else if (
                    enemy.GetComboDamage(
                        EntryPoint.Menu.Item("l33t.stds.ks.useQ").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.ks.useW").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.ks.useE").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.ks.useR").GetValue<bool>()) > enemy.Health &&
                    EntryPoint.Player.Distance(enemy, true) <= Math.Pow(1337f, 2))
                {
                    ProcessSpells(
                        EntryPoint.Menu.Item("l33t.stds.ks.useQ").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.ks.useW").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.ks.useE").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.ks.useR").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.ks.useQE").GetValue<bool>());
                    Audio.PlaySound();
                }

                ProcessFlashKillsteal(enemy);
            }
        }

        private static void ProcessFlashKillsteal(AIHeroClient enemy)
        {
            var useFlash = EntryPoint.Menu.Item("l33t.stds.ks.kenemies." + enemy.ChampionName) != null &&
                           EntryPoint.Menu.Item("l33t.stds.ks.kenemies." + enemy.ChampionName).GetValue<bool>();
            var useR = EntryPoint.Menu.Item("l33t.stds.ups.ignore." + enemy.ChampionName) != null &&
                       EntryPoint.Menu.Item("l33t.stds.ups.ignore." + enemy.ChampionName).GetValue<bool>() == false;
            var rflash =
                enemy.GetComboDamage(
                    EntryPoint.Menu.Item("l33t.stds.ks.useQ").GetValue<bool>(), false,
                    EntryPoint.Menu.Item("l33t.stds.ks.useE").GetValue<bool>(), false) < enemy.Health;
            var ePos = Spells[SpellSlot.R].Instance.GetPrediction(enemy);
            if ((FlashSpell.Slot == EloBuddy.SpellSlot.Unknown && FlashSpell.IsReady()) || !useFlash ||
                !(EntryPoint.Player.Distance(ePos.UnitPosition, true) <=
                  Math.Pow(Spells[SpellSlot.Q].Range + 25f + 395, 2)) ||
                !(EntryPoint.Player.Distance(ePos.UnitPosition, true) >
                  Math.Pow(Spells[SpellSlot.Q].Range + 25f + 200, 2)))
            {
                return;
            }
            if (
                (!(enemy.GetComboDamage(
                    EntryPoint.Menu.Item("l33t.stds.ks.useQ").GetValue<bool>(), false,
                    EntryPoint.Menu.Item("l33t.stds.ks.useE").GetValue<bool>(), false) > enemy.Health) ||
                 !EntryPoint.Menu.Item("l33t.stds.ks.usefqe").GetValue<bool>()) &&
                (!(enemy.GetComboDamage(false, false, false, EntryPoint.Menu.Item("l33t.stds.ks.useR").GetValue<bool>()) >
                   enemy.Health) || !EntryPoint.Menu.Item("l33t.stds.ks.usefr").GetValue<bool>() ||
                 !(EntryPoint.Player.Distance(ePos.UnitPosition, true) <= Math.Pow(Spells[SpellSlot.R].Range + 390, 2)) ||
                 (int) (Game.Time * 0x3E8) - Spells[SpellSlot.R].LastCastAttemptTick <= Game.Ping + 750 ||
                 (int) (Game.Time * 0x3E8) - Spells[SpellSlot.SphereE].LastCastAttemptTick <= Game.Ping + 750 ||
                 !(EntryPoint.Player.Distance(ePos.UnitPosition, true) > Math.Pow(Spells[SpellSlot.R].Range + 200, 2))))
            {
                return;
            }
            var manaCost = 0d;
            if (EntryPoint.Menu.Item("l33t.stds.ks.mana").GetValue<bool>())
            {
                manaCost = Spells.Where(s => s.Key != SpellSlot.SphereE)
                    .Aggregate(manaCost, (current, spell) => current + spell.Value.Instance.Instance.SData.Mana);
            }
            if (manaCost > EntryPoint.Player.Mana && EntryPoint.Menu.Item("l33t.stds.ks.mana").GetValue<bool>() &&
                EntryPoint.Menu.Item("l33t.stds.ks.mana").GetValue<bool>())
            {
                return;
            }
            var nearbyE = ePos.UnitPosition.CountEnemiesInRange(1000);
            if (nearbyE > EntryPoint.Menu.Item("l33t.stds.ks.maxenemy").GetValue<Slider>().Value)
            {
                return;
            }
            var flashPos = EntryPoint.Player.ServerPosition -
                           Vector3.Normalize(EntryPoint.Player.ServerPosition - ePos.UnitPosition) * 400;
            if (flashPos.IsWall())
            {
                return;
            }
            if (rflash)
            {
                if (useR)
                {
                    FlashSpell.Cast(flashPos);
                    ProcessSpells(
                        false, false, false, EntryPoint.Menu.Item("l33t.stds.ks.useR").GetValue<bool>(), false);
                    Audio.PlaySound();
                }
            }
            else
            {
                FlashSpell.Cast(flashPos);
            }
            _flashCastTick = (int) (Game.Time * 0x3E8);
        }

        public static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!EntryPoint.Menu.Item("l33t.stds.misc.antigapcloser").GetValue<bool>())
            {
                return;
            }

            if (!Spells[SpellSlot.E].IsReady() ||
                !(EntryPoint.Player.Distance(gapcloser.Sender, true) <= Math.Pow(Spells[SpellSlot.SphereE].Range, 2)) ||
                !gapcloser.Sender.IsValidTarget(Spells[SpellSlot.SphereE].Range))
            {
                return;
            }
            if (Spells[SpellSlot.Q].IsReady() &&
                Spells[SpellSlot.Q].Instance.Instance.SData.Mana + Spells[SpellSlot.E].Instance.Instance.SData.Mana <=
                EntryPoint.Player.Mana)
            {
                ProcessSphereE(gapcloser.Sender);
            }
            else if (EntryPoint.Player.Distance(gapcloser.Sender, true) <= Math.Pow(Spells[SpellSlot.E].Range, 2))
            {
                Spells[SpellSlot.E].Instance.Cast(gapcloser.End);
            }
        }

        public static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!EntryPoint.Menu.Item("l33t.stds.misc.interrupt").GetValue<bool>())
            {
                return;
            }

            if (Spells[SpellSlot.E].IsReady() &&
                EntryPoint.Player.Distance(sender, true) <= Math.Pow(Spells[SpellSlot.E].Range, 2) &&
                sender.IsValidTarget(Spells[SpellSlot.E].Range))
            {
                if (Spells[SpellSlot.Q].IsReady())
                {
                    ProcessSphereE(sender);
                }
                else
                {
                    Spells[SpellSlot.E].Instance.Cast(sender);
                }
            }
            else if (Spells[SpellSlot.Q].IsReady() && Spells[SpellSlot.E].IsReady() &&
                     EntryPoint.Player.Distance(sender, true) <= Math.Pow(Spells[SpellSlot.SphereE].Range, 2))
            {
                ProcessSphereE(sender);
            }
        }
    }
}