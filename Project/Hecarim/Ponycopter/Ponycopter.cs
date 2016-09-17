using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ponycopter {	
	class Ponycopter {
		
        public const string Champion = "Hecarim";
		public static Orbwalking.Orbwalker Orbwalker;
	    private static SpellSlot _ignite;
		
		public static AIHeroClient Player {
            get { return ObjectManager.Player; }
        }

		#region Hit Chance
        private static HitChance CustomHitChance {
            get { return GetHitchance(); }
        }

        private static HitChance GetHitchance() {
            switch (PonyMenu.Config.Item("hitChance").GetValue<StringList>().SelectedIndex) {
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
		
		#region OnLoad
		public static void OnLoad(EventArgs e) {
			PonyMenu.Init();
			if (Player.CharData.BaseSkinName != Champion)
				return;
			
			Notifications.AddNotification("PonyCopter by TheOBJop", 8000);
			
			Spells.Q.SetSkillshot(0.5f, 700f, 1200f, true, SkillshotType.SkillshotCircle);
			Spells.W.SetSkillshot(0.5f, 1050f, 2000f, true, SkillshotType.SkillshotCircle);
			Spells.E.SetTargetted(0.5f, 800f);
			Spells.R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);
		
            _ignite = Player.GetSpellSlot("summonerdot");

            Interrupter2.OnInterruptableTarget += (source, eventArgs) => {
                if (Spells.E.IsReady() && source.IsValidTarget(Spells.E.Range) && PonyMenu.Config.Item("InterruptE").GetValue<bool>()) {
                    Spells.E.Cast();
                }
                if (Spells.R.IsReady() && source.IsValidTarget(Spells.R.Range) && PonyMenu.Config.Item("InterruptR").GetValue<bool>()) {
                    var pred = Spells.R.GetPrediction(source).Hitchance;
                    if (pred >= HitChance.High)
                        Spells.R.Cast(source);
                }

            };

            AntiGapcloser.OnEnemyGapcloser += (activeGapcloser) => {
                if (Spells.E.IsReady() && activeGapcloser.Sender.IsValidTarget(Player.AttackRange) && PonyMenu.Config.Item("Antigap").GetValue<bool>())
                    Spells.E.Cast();
            };

			Game.OnUpdate += OnUpdate;
		}
		#endregion
		
		#region OnUpdate
		public static void OnUpdate(EventArgs e) {
			if (Player.IsDead)
				return;
			
			switch (Orbwalker.ActiveMode) {
			case Orbwalking.OrbwalkingMode.Combo:
				Combo();
				break;
			case Orbwalking.OrbwalkingMode.LaneClear:
				JungleClear();
				LaneClear();
				break;
			case Orbwalking.OrbwalkingMode.Mixed:
				Harass();
				break;
            case Orbwalking.OrbwalkingMode.LastHit:
			    LastHit();
                break;
			}

            if (PonyMenu.Config.Item("AutoHarass").GetValue<KeyBind>().Active) {
                AutoHarass();
            }
		}
		#endregion
		
		#region LaneClear and JungleClear
		public static void LaneClear() {
		    var useQ = PonyMenu.Config.Item("LaneClear.Q").GetValue<bool>();
		    var useW = PonyMenu.Config.Item("LaneClear.W").GetValue<bool>();
		    var minMinions = PonyMenu.Config.Item("LaneClear.MinMinions").GetValue<Slider>().Value;
            var lanemana = PonyMenu.Config.Item("LaneClear.MinMana").GetValue<Slider>().Value;
			var minionObj = MinionManager.GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            if (!minionObj.Any())
                return;

            if (useQ && Spells.Q.IsReady() && Player.ManaPercent >= lanemana)
                Spells.Q.Cast();

            if (useW && Spells.W.IsReady() && minionObj.Count > minMinions && Player.ManaPercent >= lanemana)
                Spells.W.Cast();
		}
		
		public static void JungleClear() {
		    var useQ = PonyMenu.Config.Item("JungleClear.Q").GetValue<bool>();
		    var useW = PonyMenu.Config.Item("JungleClear.W").GetValue<bool>();
		    var useE = PonyMenu.Config.Item("JungleClear.W").GetValue<bool>();
			var minionObj = MinionManager.GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (!minionObj.Any())
                return;

		    if (useQ && Spells.Q.IsReady())
		        Spells.Q.Cast();

            if (useW && Spells.W.IsReady())
		        Spells.W.Cast();

            if (useE && Spells.E.IsReady())
		        Spells.E.Cast();

		}
		#endregion

        #region LastHit
        private static void LastHit() {
            var minions = MinionManager.GetMinions(Player.ServerPosition, Spells.Q.Range);
            if (minions.Count <= 0)
                return;

            if (Spells.Q.IsReady() && PonyMenu.Config.Item("LastHit.Q").GetValue<bool>() && Player.ManaPercent >= PonyMenu.Config.Item("LastHit.MinMana").GetValue<Slider>().Value) {
                var qtarget = minions.Where(x => x.Distance(Player) < Spells.Q.Range && (x.Health < Player.GetSpellDamage(x, SpellSlot.Q) && !(x.Health < Player.GetAutoAttackDamage(x))))
                        .OrderByDescending(x => x.Health)
                        .FirstOrDefault();
                if (HealthPrediction.GetHealthPrediction(qtarget, (int)0.5) <=
                    Player.GetSpellDamage(qtarget, SpellSlot.Q))
                    Spells.Q.Cast();
            }
        }
        #endregion

        #region Combo
        public static void Combo() {
			var target = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
                return;

            var enemys = target.CountEnemiesInRange(Spells.R.Range);

            var useQ = PonyMenu.Config.Item("Combo.Q").GetValue<bool>();
            var useW = PonyMenu.Config.Item("Combo.W").GetValue<bool>();
            var useE = PonyMenu.Config.Item("Combo.E").GetValue<bool>();
            var useR = PonyMenu.Config.Item("Combo.R").GetValue<bool>();
            var rEnemies = PonyMenu.Config.Item("Combo.RCount").GetValue<Slider>().Value;

            if (useE && Spells.E.IsReady() && target.IsValidTarget(3000))
                Spells.E.Cast();

            if (useW && Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range))
                Spells.W.Cast();
           
            if (useQ && Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range))
                Spells.Q.Cast();

            if (useR && Spells.R.IsReady() && target.IsValidTarget(Spells.R.Range))
                if (rEnemies <= enemys && Spells.R.GetPrediction(target).Hitchance >= CustomHitChance)
                    Spells.R.Cast(target.Position.Shorten(Player.Position, 150));
		}
		#endregion

        #region AutoHarass
        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);
            if (!Spells.Q.IsReady() || !PonyMenu.Config.Item("Harass.Q").GetValue<bool>() || Player.IsRecalling() || target == null || !target.IsValidTarget())
                return;

            if (Spells.Q.IsReady() && PonyMenu.Config.Item("Harass.Q").GetValue<bool>() && target.IsValidTarget(Spells.Q.Range - 10)) {
                Spells.Q.Cast();
            }
        }
        #endregion

        #region Harass
        public static void Harass() {
			var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical);
            var harassmana = PonyMenu.Config.Item("Harass.MinMana").GetValue<Slider>().Value;
            if (target == null || !target.IsValidTarget())
                return;
            
            if (Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range) && Player.ManaPercent >= harassmana && PonyMenu.Config.Item("Harass.W").GetValue<bool>())
                Spells.W.Cast();
            
            if (PonyMenu.Config.Item("Harass.Q").GetValue<bool>() && target.IsValidTarget(Spells.Q.Range) && Player.ManaPercent >= harassmana)
                Spells.Q.Cast();
            
		}
		#endregion
		
		#region IgniteDamage
        private static float IgniteDamage(AIHeroClient target) {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;

            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }
        #endregion

        #region ComboDamage
        public static float GetComboDamage(Obj_AI_Base enemy) {
            float damage = 0;

            damage += Spells.Q.IsReady() ? Spells.Q.GetDamage(enemy) : 0;
            damage += Spells.W.IsReady() ? Spells.W.GetDamage(enemy) : 0;
            damage += Spells.E.IsReady() ? Spells.E.GetDamage(enemy) : 0;
            damage += Spells.R.IsReady() ? Spells.R.GetDamage(enemy) : 0;

            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                damage += (float) Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

            return damage;
        }
        #endregion		
		
		#region Main
		public static void Main(String[] args) {
			Spells.Q = new Spell(SpellSlot.Q, 350);
			Spells.W = new Spell(SpellSlot.W, 525);
			Spells.E = new Spell(SpellSlot.E, 600);
			Spells.R = new Spell(SpellSlot.R, 800);
			CustomEvents.Game.OnGameLoad += OnLoad;
		}
		#endregion
	}
}