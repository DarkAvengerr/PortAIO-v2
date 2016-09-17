#region
using System;
using System.Collections.Generic;
using Colors = System.Drawing.Color;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace EzrealDreamCatcher
{
	internal class Program
	{
		public static string ChampionName = "Ezreal";
		public static Orbwalking.Orbwalker Orbwalker;
		public static AIHeroClient Player { get { return ObjectManager.Player; } }
		public static List<Spell> SpellList = new List<Spell>();
		public static Spell Q;
		public static Spell W;
		public static Spell E;
		public static Spell R;
		public static float QMana;
		public static float WMana;
		public static float EMana;
		public static Menu Config;

		private static void Main()
		{
			CustomEvents.Game.OnGameLoad += OnGameLoad;
		}

		private static void OnGameLoad(EventArgs args)
		{
			if (Player.ChampionName != ChampionName) return;
			
			Q = new Spell(SpellSlot.Q, 1150f);
			W = new Spell(SpellSlot.W, 1000f);
			E = new Spell(SpellSlot.E, 475f);
			R = new Spell(SpellSlot.R, 3000f);

			Q.SetSkillshot(0.25f, 70f, 2000f, true, SkillshotType.SkillshotLine);
			W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
			R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

			SpellList.Add(Q);
			SpellList.Add(W);
			SpellList.Add(E);
			SpellList.Add(R);

			SetMenu();
			SetEvents();
			SetMana();
		}

		private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
		{
			var useE = Config.Item("AntiGapcloser").GetValue<bool>();

			if (useE && Player.Mana >= EMana + QMana)
			{
				var target = gapcloser.Sender;
				if (target == null || !target.IsValidTarget(E.Range)) return;

				E.Cast(Player.Position.Extend(Game.CursorPos, E.Range));
			}
		}

		private static void OnDraw(EventArgs args)
		{
			var drawQ = Config.Item("qDraw").GetValue<bool>();
			if (drawQ && Q.IsReady())
				Render.Circle.DrawCircle(Player.Position, Q.Range, Colors.MistyRose);

			var drawW = Config.Item("wDraw").GetValue<bool>();
			if (drawW && W.IsReady())
				Render.Circle.DrawCircle(Player.Position, W.Range, Colors.MistyRose);

			var drawE = Config.Item("eDraw").GetValue<bool>();
			if (drawE && E.IsReady())
				Render.Circle.DrawCircle(Player.Position, E.Range, Colors.MistyRose);
		}

		private static void OnGameUpdate(EventArgs args)
		{
			if (Player != null && Player.BaseSkinName != null)
			{
				//Player.SetSkin(model: Player.BaseSkinName, id: Config.Item("SkinChangerTogggle").GetValue<bool>() ? Config.Item("SkinChangerID").GetValue<StringList>().SelectedIndex : Player.SkinId);
			}

			switch (Orbwalker.ActiveMode)
			{
				case Orbwalking.OrbwalkingMode.Combo:
					Combo();
					break;
				case Orbwalking.OrbwalkingMode.Mixed:
					Harass();
					break;
				case Orbwalking.OrbwalkingMode.LaneClear:
					LaneClear();
					JungleClear();
					break;
			}

			if (Config.Item("harassToggle").GetValue<KeyBind>().Active)
				HarassToggle();
			
			if (Config.Item("farmKey").GetValue<KeyBind>().Active)
				Farm();

			Killsteal();
		}

		private static void Combo()
		{
			var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
			var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
			var eTarget = TargetSelector.GetTarget(E.Range + W.Range, TargetSelector.DamageType.Magical);

			if (qTarget == null || wTarget == null || eTarget == null) return;

			var useQ = Config.Item("UseQCombo").GetValue<bool>();
			var useW = Config.Item("UseWCombo").GetValue<bool>();

			if (Player.Mana >= WMana + QMana)
			{
				switch (Config.Item("UseECombo").GetValue<StringList>().SelectedIndex)
				{
					case 0:
						if (E.IsReady() && Q.IsReady() || W.IsReady())
							E.Cast(Game.CursorPos);
						break;
					case 1:
						if (E.IsReady() && Q.IsReady() || W.IsReady())
							E.Cast(eTarget.ServerPosition);
						break;
					case 2:
						break;
				}
			}

			if (useQ && Q.IsReady())
				Q.CastIfHitchanceEquals(qTarget, HitChanceChooser());
			if (useW && W.IsReady())
				W.CastIfHitchanceEquals(wTarget, HitChanceChooser());
		}

		private static void Harass()
		{
			if (Config.Item("harassMana").GetValue<Slider>().Value >= (Player.Mana / Player.MaxMana) * 100) return;

			var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
			var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

			if (qTarget == null || wTarget == null) return;

			var useQ = Config.Item("UseQHarass").GetValue<bool>();
			var useW = Config.Item("UseWHarass").GetValue<bool>();

			if (useQ && Q.IsReady())
				Q.CastIfHitchanceEquals(qTarget, HitChanceChooser());
			if (useW && W.IsReady())
				W.CastIfHitchanceEquals(wTarget, HitChanceChooser());
		}

		private static void HarassToggle()
		{
			if (Config.Item("harassMana").GetValue<Slider>().Value >= (Player.Mana / Player.MaxMana) * 100) return;

			var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
			if (target == null) return;

			var useQ = Config.Item("UseQHarass").GetValue<bool>();
			var useW = Config.Item("UseWHarass").GetValue<bool>();
			var validTarget = Config.Item("harass" + target.ChampionName).GetValue<bool>();

			if (!validTarget)
			{
				return;
			}

			if (useQ && Q.IsReady())
				Q.CastIfHitchanceEquals(target, HitChanceChooser());
			if (useW && W.IsReady())
				W.CastIfHitchanceEquals(target, HitChanceChooser());
		}
		
		private static void Farm()
		{
			if (Config.Item("farmMana").GetValue<Slider>().Value >= (Player.Mana / Player.MaxMana) * 100) return;

			var useQ = Config.Item("UseQFarm").GetValue<bool>();
			var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

			if (minions.Count <= 0) return;

			var minion = minions[0];

			if (useQ && Q.IsReady() && Q.GetDamage(minion) > minion.Health)
				Q.CastIfHitchanceEquals(minion, HitChanceChooser());
		}
		
		private static void JungleClear()
		{
			if (Config.Item("jungleMana").GetValue<Slider>().Value >= (Player.Mana / Player.MaxMana) * 100) return;

			var useQ = Config.Item("UseQJungle").GetValue<bool>();
			var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

			if (mobs.Count <= 0) return;

			var mob = mobs[0];

			if (useQ && Q.IsReady())
				Q.CastIfHitchanceEquals(mob, HitChanceChooser());
		}

		private static void LaneClear()
		{
			if (Config.Item("laneMana").GetValue<Slider>().Value >= (Player.Mana / Player.MaxMana) * 100) return;

			var useQ = Config.Item("UseQLane").GetValue<bool>();
			var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

			if (minions.Count <= 0) return;

			var minion = minions[0];

			if (useQ && Q.IsReady())
				Q.CastIfHitchanceEquals(minion, HitChanceChooser());
		}

		private static void Killsteal()
		{
			var useQ = Config.Item("UseQKillsteal").GetValue<bool>();
			var useW = Config.Item("UseWKillsteal").GetValue<bool>();
			var useR = Config.Item("UseRKillsteal").GetValue<bool>();

			foreach (
				var target in
					ObjectManager.Get<AIHeroClient>()
						.Where(target => !target.IsMe && target.Team != Player.Team))
			{
				if (useQ && Q.IsReady() && Q.GetDamage(target) > target.Health
					&& Player.Distance(target) <= Q.Range)
					Q.CastIfHitchanceEquals(target, HitChanceChooser());

				if (useW && W.IsReady() && W.GetDamage(target) > target.Health
					&& Player.Distance(target) <= W.Range)
					W.CastIfHitchanceEquals(target, HitChanceChooser());

				if (useR && R.IsReady() && R.GetDamage(target) > target.Health
					&& Player.Distance(target) <= R.Range && Player.Distance(target) >= Orbwalking.GetRealAutoAttackRange(Player))
					R.Cast(target);
			}
		}
		
		private static HitChance HitChanceChooser()
		{
			switch (Config.Item("HitChanceChooser").GetValue<StringList>().SelectedIndex)
			{
				case 0:
					return HitChance.Low;
				case 1:
					return HitChance.Medium;
				case 2:
					return HitChance.High;
				case 3:
					return HitChance.VeryHigh;
				default:
					return HitChance.VeryHigh;
			}
		}

		private static void SetMenu()
		{
			Config = new Menu("Ezreal - the Dream Chaser", "Ezreal", true);

			var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
			TargetSelector.AddToMenu(targetSelectorMenu);
			Config.AddSubMenu(targetSelectorMenu);

			Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
			Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

			Config.AddSubMenu(new Menu("Combo", "Combo"));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q in Combo").SetValue(true));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W in Combo").SetValue(true));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E in Combo").SetValue(new StringList(new[] { "To mouse", "To enemy", "No" })));

			Config.AddSubMenu(new Menu("Harass", "Harass"));
			Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q in Harass").SetValue(true));
			Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "Use W in Harass").SetValue(false));
			Config.SubMenu("Harass").AddItem(new MenuItem("harassMana", "Min. Mana Percent").SetValue(new Slider(50, 100, 0)));

			Config.SubMenu("Harass").AddSubMenu(new Menu("Auto-Harass", "AutoHarass"));
			Config.SubMenu("Harass").SubMenu("AutoHarass").AddItem(new MenuItem("harassToggle", "Toggle Key").SetValue(new KeyBind(84, KeyBindType.Toggle)));
			foreach (var enemyChampion in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
			{
				Config.SubMenu("Harass")
					.SubMenu("AutoHarass")
					.AddItem(
						new MenuItem("harass" + enemyChampion.ChampionName, enemyChampion.ChampionName).SetValue(false));
			}

			Config.AddSubMenu(new Menu("Farm", "Farm"));
			Config.SubMenu("Farm").AddItem(new MenuItem("farmKey", "Farm Key").SetValue(new KeyBind(67, KeyBindType.Press)));
			Config.SubMenu("Farm").AddItem(new MenuItem("UseQFarm", "Use Q in Farm").SetValue(true));
			Config.SubMenu("Farm").AddItem(new MenuItem("farmMana", "Min. Mana Percent").SetValue(new Slider(50, 100, 0)));

			Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
			Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQLane", "Clear with Q").SetValue(true));
			Config.SubMenu("LaneClear").AddItem(new MenuItem("laneMana", "Min. Mana Percent").SetValue(new Slider(50, 100, 0)));

			Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
			Config.SubMenu("JungleClear").AddItem(new MenuItem("UseQJungle", "Clear with Q").SetValue(true));
			Config.SubMenu("JungleClear").AddItem(new MenuItem("jungleMana", "Min. Mana Percent").SetValue(new Slider(50, 100, 0)));

			Config.AddSubMenu(new Menu("Killsteal", "Killsteal"));
			Config.SubMenu("Killsteal").AddItem(new MenuItem("UseQKillsteal", "Killsteal with Q").SetValue(true));
			Config.SubMenu("Killsteal").AddItem(new MenuItem("UseWKillsteal", "Killsteal with W").SetValue(true));
			Config.SubMenu("Killsteal").AddItem(new MenuItem("UseRKillsteal", "Killsteal with R").SetValue(true));

			Config.AddSubMenu(new Menu("Drawing", "Drawing"));
			Config.SubMenu("Drawing").AddItem(new MenuItem("qDraw", "Draw Q Range").SetValue(true));
			Config.SubMenu("Drawing").AddItem(new MenuItem("wDraw", "Draw W Range").SetValue(true));
			Config.SubMenu("Drawing").AddItem(new MenuItem("eDraw", "Draw E Range").SetValue(true));

			Config.AddSubMenu(new Menu("Misc", "Misc"));
			Config.SubMenu("Misc").AddItem(new MenuItem("AntiGapcloser", "Use E against Gapclosers").SetValue(true));
			Config.SubMenu("Misc").AddItem(new MenuItem("HitChanceChooser", "Hitchance").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" })));
			Config.SubMenu("Misc").AddItem(new MenuItem("SkinChangerTogggle", "Use Skin Changer").SetValue(false));
			Config.SubMenu("Misc").AddItem(new MenuItem("SkinChangerID", "Skins").SetValue(new StringList(new[] { "Classic", "Nottingham", "Striker", "Frosted", "Explorer", "Pulsefire", "TPA", "Debonair", "Ace of Spades" })));

			Config.AddToMainMenu();
		}

		private static void SetEvents()
		{
			Game.OnUpdate += OnGameUpdate;
			Drawing.OnDraw += OnDraw;
			AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
		}

		private static void SetMana()
		{
			QMana = Q.Instance.SData.Mana;
			WMana = W.Instance.SData.Mana;
			EMana = E.Instance.SData.Mana;
		}
	}
}
