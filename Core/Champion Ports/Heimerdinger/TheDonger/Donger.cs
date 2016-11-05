using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

/* Name: Donger
 * Author: Brandon Woolworth
 * Desc: Donger assembly */

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace The_Donger {
    internal class Donger
    {
        public const string Champion = "Heimerdinger";
        public static Orbwalking.Orbwalker Orbwalker;
        private static SpellSlot _ignite;

        private static Items.Item Zhonyas;

        public static AIHeroClient Player {
            get { return ObjectManager.Player; }
        }

        internal enum Spells {
            Q, W, E, R, E1, E2
        }

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>() {
            {Spells.Q, new Spell(SpellSlot.Q, 325)},
            {Spells.W, new Spell(SpellSlot.W, 1100)},
            {Spells.E, new Spell(SpellSlot.E, 925)},
            {Spells.R, new Spell(SpellSlot.R, 1000)},
            {Spells.E1, new Spell(SpellSlot.E, 1125)},
            {Spells.E2, new Spell(SpellSlot.E, 1325)},
        };

        #region OnLoad

        #region Hit Chance

        private static HitChance CustomHitChance {
            get { return GetHitchance(); }
        }

        private static HitChance GetHitchance() {
            switch (DongerMenu.Config.Item("hitChance").GetValue<StringList>().SelectedIndex) {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                default:
                    return HitChance.Medium;
            }
        }

        #endregion

        private static void OnLoad() {

            DongerMenu.Init();
            if (ObjectManager.Player.CharData.BaseSkinName != Champion)
                return;

            Notifications.AddNotification("The Donger by TheOBJop");

            // Set skillshots
            spells[Spells.Q].SetSkillshot(0.5f, 40f, 1100f, true, SkillshotType.SkillshotLine);
            spells[Spells.W].SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            spells[Spells.E].SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);
            spells[Spells.E1].SetSkillshot(0.25f + spells[Spells.E].Delay, 120f, 1200f, false,
                SkillshotType.SkillshotLine);
            spells[Spells.E2].SetSkillshot(0.3f + spells[Spells.E1].Delay, 120f, 1200f, false,
                SkillshotType.SkillshotLine);

            _ignite = SpellSlot.Summoner2;
            Zhonyas = new Items.Item(3157, 1f);

            // Interruption
            Interrupter2.OnInterruptableTarget += (source, eventArgs) => {
                var eSlot = spells[Spells.E];
                if (DongerMenu.Config.Item("Interrupt").GetValue<bool>()
                    && eSlot.IsReady()
                    && eSlot.Range >= Player.Distance(source, false))
                {
                    eSlot.Cast(source.Position);
                }
            };

            AntiGapcloser.OnEnemyGapcloser += (activeGapcloser) => {
                if (DongerMenu.Config.Item("AntiGap").GetValue<bool>()
                    && spells[Spells.E].IsReady()
                    && activeGapcloser.Sender.IsValidTarget(spells[Spells.E].Range))
                {
                    spells[Spells.E].Cast(activeGapcloser.End);
                }
            };

            Game.OnUpdate += OnGameUpdate;
        }

        #endregion

        #region OnUpdate
        private static void OnGameUpdate(EventArgs args) {
            if (Player.IsDead)
                return;

            switch (Orbwalker.ActiveMode) {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
        }
        #endregion

        #region LaneClear

        private static void LaneClear() {
            var lanemana = DongerMenu.Config.Item("Laneclear.Mana").GetValue<Slider>().Value;
            var laneclear = (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear);
            var MinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                spells[Spells.W].Range + spells[Spells.W].Width);
            var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                spells[Spells.E].Range + spells[Spells.E].Width);
            var Wfarmpos = spells[Spells.W].GetLineFarmLocation(MinionsW, spells[Spells.W].Width);
            var Efarmpos = spells[Spells.E].GetCircularFarmLocation(MinionsE, spells[Spells.E].Width);

            if (Wfarmpos.MinionsHit >= 3 && DongerMenu.Config.Item("Laneclear.W").GetValue<bool>() && Player.ManaPercent >= lanemana)
                spells[Spells.W].Cast(Wfarmpos.Position);
            if (Efarmpos.MinionsHit >= 3 && MinionsE.Count >= 1 && DongerMenu.Config.Item("Laneclear.E").GetValue<bool>() && Player.ManaPercent >= lanemana)
                spells[Spells.E].Cast(Efarmpos.Position);
        }

        #endregion

        #region Combo
        private static void Combo() {
            var target = TargetSelector.GetTarget(spells[Spells.W].Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            var qtarget = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            if (qtarget == null)
                return;

            var useQ = DongerMenu.Config.Item("Combo.Q").GetValue<bool>();
            var useW = DongerMenu.Config.Item("Combo.W").GetValue<bool>();
            var useE = DongerMenu.Config.Item("Combo.E").GetValue<bool>();
            var useR = DongerMenu.Config.Item("Combo.R").GetValue<bool>();

            var upgradeQ = DongerMenu.Config.Item("Mode").GetValue<StringList>().SelectedIndex == 1;
            var upgradeW = DongerMenu.Config.Item("Mode").GetValue<StringList>().SelectedIndex == 0;
            var upgradeE = DongerMenu.Config.Item("Mode").GetValue<StringList>().SelectedIndex == 2;

            var useIgnite = DongerMenu.Config.Item("Combo.Ignite").GetValue<bool>();

            // Place down turret
            if (useQ && spells[Spells.Q].IsReady() && qtarget.IsValidTarget(650f) && Player.Position.CountEnemiesInRange(650) >= 1) {

                if (DongerMenu.Config.Item("Combo.Zhonyas").GetValue<bool>()) {
                    // If Zho Ult
                    var HP = Player.Health;
                    var critHP = Player.MaxHealth/4;
                    if (HP <= critHP) {
                        spells[Spells.R].Cast();
                        LeagueSharp.Common.Utility.DelayAction.Add(1010, () => spells[Spells.Q].Cast(Player.Position));
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => spells[Spells.Q].Cast(Player.Position));
                        LeagueSharp.Common.Utility.DelayAction.Add(1200, () => Zhonyas.Cast());
                    }
                }


                if (spells[Spells.R].IsReady() && upgradeQ)
                    spells[Spells.R].Cast();
                
				if (Player.HasBuff("HeimerdingerR") && !upgradeQ) { }
				else
					spells[Spells.Q].Cast(Player.Position.Extend(target.Position, +300));
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget()) {
                if (upgradeE) {
                    var prediction = spells[Spells.E].GetPrediction(target, true);
                    float dist = Vector3.Distance(target.Position, Player.Position);
					
					if (dist > spells[Spells.E].Range) {
						prediction = spells[Spells.E1].GetPrediction(target, true);
					}
					
					if (dist > spells[Spells.E1].Range) {
						prediction = spells[Spells.E2].GetPrediction(target, true);
					}
					
					if (prediction.Hitchance >= CustomHitChance) {
						spells[Spells.R].Cast();
						Vector3 point = Player.Position.Extend(target.Position, spells[Spells.E].Range);
						spells[Spells.E].Cast(point, true);
					}
				} else {
					var prediction = spells[Spells.E].GetPrediction(target, true);
					if (prediction.Hitchance >= CustomHitChance) {
						if (Player.HasBuff("HeimerdingerR")) { }
						else
							spells[Spells.E].Cast(target);
					}
				}
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target)) {
                var pred = spells[Spells.W].GetPrediction(target);
                if (pred.Hitchance >= CustomHitChance) {
                    if (spells[Spells.R].IsReady() && upgradeW) {
                        spells[Spells.R].Cast();

                        // Packetcast to immediately cast afterwards
                        spells[Spells.W].Cast(target, true);
                    }
					
					if (Player.HasBuff("HeimerdingerR") && !upgradeW) { }
					else
						spells[Spells.W].Cast(target);
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useIgnite) {
                Player.Spellbook.CastSpell(_ignite, target);
            }
        }
        #endregion

        #region Harass
        private static void Harass() {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValid)
                return;

            var useQ = DongerMenu.Config.Item("Harass.Q").GetValue<bool>();
            var useW = DongerMenu.Config.Item("Harass.W").GetValue<bool>();
            var useE = DongerMenu.Config.Item("Harass.E").GetValue<bool>();
            var checkMana = DongerMenu.Config.Item("Harass.Mana").GetValue<Slider>().Value;

            if (Player.ManaPercent < checkMana)
                return;

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target)) {
                spells[Spells.Q].Cast(Player.Position.Extend(target.Position, +300));
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target)) {
                var pred = spells[Spells.W].GetPrediction(target);
                if (pred.Hitchance >= CustomHitChance)
                    spells[Spells.W].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target)) {
                var pred = spells[Spells.E].GetPrediction(target);
                if (pred.Hitchance >= CustomHitChance)
                    spells[Spells.E].Cast();
            }
        }
        #endregion

        #region IgniteDamage

        private static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        #endregion

        #region ComboDamage

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += spells[Spells.W].GetDamage(enemy);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                damage += (float) Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }

        #endregion

        #region Main
        public static void Main()
        {
            OnLoad();
        }
        #endregion
    }
}