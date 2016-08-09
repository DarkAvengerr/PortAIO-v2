using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace PandaTeemo
{
    internal class StateManager
    {
        /// <summary>
        /// Does the Combo
        /// </summary>
        public static void Combo()
        {
            var checkCamo = Essentials.Config.SubMenu("Combo").Item("checkCamo").GetValue<bool>();

            if (checkCamo && Essentials.Player.HasBuff("CamouflageStealth"))
            {
                return;
            }

            var enemies =
                HeroManager.Enemies.FirstOrDefault(t => t.IsValidTarget() && Essentials.Orbwalker.InAutoAttackRange(t));
            var rtarget = TargetSelector.GetTarget(Essentials.R.Range, TargetSelector.DamageType.Magical);
            var useW = Essentials.Config.SubMenu("Combo").Item("wcombo").GetValue<bool>();
            var useR = Essentials.Config.SubMenu("Combo").Item("rcombo").GetValue<bool>();
            var wCombat = Essentials.Config.SubMenu("Combo").Item("wCombat").GetValue<bool>();
            var rCount = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            var rCharge = Essentials.Config.SubMenu("Combo").Item("rCharge").GetValue<Slider>().Value;

            if (Essentials.W.IsReady() && useW && !wCombat)
            {
                Essentials.W.Cast();
            }

            if (enemies == null)
            {
                return;
            }

            if (useW && wCombat)
            {
                if (Essentials.W.IsReady())
                {
                    Essentials.W.Cast();
                }
            }

            if (Essentials.R.IsReady() && useR && Essentials.R.IsInRange(rtarget) && rCharge <= rCount &&
                rtarget.IsValidTarget() &&
                !Essentials.IsShroomed(rtarget.Position))
            {
                Essentials.R.CastIfHitchanceEquals(rtarget, HitChance.VeryHigh);
            }
            else if (Essentials.R.IsReady() && useR && rCharge <= rCount)
            {
                var shroom = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(t => t.Name == "Noxious Trap");

                if (shroom != null)
                {
                    var shroomPosition = shroom.Position;
                    var predictionPosition = shroomPosition.Extend(rtarget.Position,
                        Essentials.Player.CharData.AcquisitionRange*Essentials.R.Level + 2);

                    if (Essentials.R.IsInRange(rtarget, Essentials.Player.CharData.AcquisitionRange*Essentials.R.Level + 2) &&
                        Essentials.IsShroomed(shroomPosition))
                    {
                        Essentials.R.Cast(predictionPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Kill Steal
        /// </summary>
        public static void KillSteal()
        {
            var ksq = Essentials.Config.SubMenu("KSMenu").Item("KSQ").GetValue<bool>();
            var ksr = Essentials.Config.SubMenu("KSMenu").Item("KSR").GetValue<bool>();

            if (ksq)
            {
                var target = HeroManager.Enemies.Where(t => t.IsValidTarget()
                                                            && Essentials.Q.IsInRange(t)
                                                            && Essentials.Q.GetDamage(t) >= t.Health)
                    .OrderBy(t => t.Health)
                    .FirstOrDefault();

                if (target != null && Essentials.Q.IsReady())
                {
                    Essentials.Q.Cast(target);
                }
                else
                {
                    return;
                }
            }

            if (ksr)
            {
                var target = HeroManager.Enemies.Where(t => t.IsValidTarget()
                                                            && Essentials.R.IsInRange(t)
                                                            && Essentials.R.GetDamage(t) >= t.Health)
                    .OrderBy(t => t.Health)
                    .FirstOrDefault();

                if (target != null && Essentials.R.IsReady())
                {
                    Essentials.R.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }
            }
        }

        /// <summary>
        /// Does the LaneClear
        /// </summary>
        public static void LaneClear()
        {
            var useQ = Essentials.Config.SubMenu("LaneClear").Item("qclear").GetValue<bool>();
            var qMinion = MinionManager.GetMinions(ObjectManager.Player.Position, Essentials.Q.Range);
            var qManaManager = Essentials.Config.SubMenu("LaneClear").Item("qManaManager").GetValue<Slider>().Value;

            if (useQ && Essentials.Q.IsReady() && qMinion != null)
            {
                foreach (var m in qMinion)
                {
                    if (Essentials.Q.IsInRange(m) && Essentials.Q.GetDamage(m) >= m.Health &&
                        (int) Essentials.Player.ManaPercent >= qManaManager)
                    {
                        Essentials.Q.CastOnUnit(m);
                    }
                }
            }

            var allMinionsR = MinionManager.GetMinions(ObjectManager.Player.Position, Essentials.R.Range, MinionTypes.Melee);
            var rangedMinionsR = MinionManager.GetMinions(ObjectManager.Player.Position, Essentials.R.Range,
                MinionTypes.Ranged);
            var rLocation = Essentials.R.GetCircularFarmLocation(allMinionsR, Essentials.R.Range);
            var r2Location = Essentials.R.GetCircularFarmLocation(rangedMinionsR, Essentials.R.Range);
            var useR = Essentials.Config.SubMenu("LaneClear").Item("rclear").GetValue<bool>();
            var userKill = Essentials.Config.SubMenu("LaneClear").Item("userKill").GetValue<bool>();
            var minionR = Essentials.Config.SubMenu("LaneClear").Item("minionR").GetValue<Slider>().Value;

            if (minionR <= rLocation.MinionsHit && useR
                || minionR <= r2Location.MinionsHit && useR
                || minionR <= rLocation.MinionsHit + r2Location.MinionsHit && useR)
            {
                if (userKill)
                {
                    foreach (var minion in allMinionsR)
                    {
                        if (minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.R)
                            && Essentials.R.IsReady()
                            && Essentials.R.IsInRange(rLocation.Position.To3D())
                            && !Essentials.IsShroomed(rLocation.Position.To3D())
                            && minionR <= rLocation.MinionsHit)
                        {
                            Essentials.R.Cast(rLocation.Position);
                            return;
                        }

                        if (minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.R)
                            && Essentials.R.IsReady()
                            && Essentials.R.IsInRange(r2Location.Position.To3D())
                            && !Essentials.IsShroomed(r2Location.Position.To3D())
                            && minionR <= r2Location.MinionsHit)
                        {
                            Essentials.R.Cast(r2Location.Position);
                            return;
                        }
                    }
                }
                else
                {
                    if (Essentials.R.IsReady()
                        && Essentials.R.IsInRange(rLocation.Position.To3D())
                        && !Essentials.IsShroomed(rLocation.Position.To3D())
                        && minionR <= rLocation.MinionsHit)
                    {
                        Essentials.R.Cast(rLocation.Position);
                    }
                    else if (Essentials.R.IsReady()
                             && Essentials.R.IsInRange(r2Location.Position.To3D())
                             && !Essentials.IsShroomed(r2Location.Position.To3D())
                             && minionR <= r2Location.MinionsHit)
                    {
                        Essentials.R.Cast(r2Location.Position);
                    }
                }
            }
        }

        /// <summary>
        /// Does the JungleClear
        /// </summary>
        public static void JungleClear()
        {
            var useQ = Essentials.Config.SubMenu("JungleClear").Item("qclear").GetValue<bool>();
            var useR = Essentials.Config.SubMenu("JungleClear").Item("rclear").GetValue<bool>();
            var ammoR = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            var qManaManager = Essentials.Config.SubMenu("LaneClear").Item("qManaManager").GetValue<Slider>().Value;

            if (Essentials.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var jungleMobQ =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(t => Essentials.Q.IsInRange(t) && t.Team == GameObjectTeam.Neutral && t.IsValidTarget())
                        .OrderBy(t => t.MaxHealth)
                        .FirstOrDefault();
                var jungleMobR =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(t => Essentials.R.IsInRange(t) && t.Team == GameObjectTeam.Neutral && t.IsValidTarget())
                        .OrderBy(t => t.MaxHealth)
                        .FirstOrDefault();

                if (useQ && jungleMobQ != null)
                {
                    if (Essentials.Q.IsReady() && qManaManager <= (int) Essentials.Player.ManaPercent)
                    {
                        Essentials.Q.CastOnUnit(jungleMobQ);
                    }
                }

                if (useR && jungleMobR != null)
                {
                    if (Essentials.R.IsReady() && ammoR >= 1)
                    {
                        Essentials.R.Cast(jungleMobR.Position);
                    }
                }
            }
        }


        /// <summary>
        /// Does the Flee
        /// </summary>
        public static void Flee()
        {
            // Checks if toggle is on
            var useW = Essentials.Config.SubMenu("Flee").Item("w").GetValue<bool>();
            var useR = Essentials.Config.SubMenu("Flee").Item("r").GetValue<bool>();
            var rCharge = Essentials.Config.SubMenu("Flee").Item("rCharge").GetValue<Slider>().Value;

            // Force move to player's mouse cursor
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            // Uses W if avaliable and if toggle is on
            if (useW && Essentials.W.IsReady())
            {
                Essentials.W.Cast(Essentials.Player);
            }

            // Uses R if avaliable and if toggle is on
            if (useR && Essentials.R.IsReady() && rCharge <= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
            {
                Essentials.R.Cast(Essentials.Player.Position);
            }
        }
    }
}