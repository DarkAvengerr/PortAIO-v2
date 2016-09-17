using System;
using LeagueSharp;
using LeagueSharp.Common;
using CNLib;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Font = SharpDX.Direct3D9.Font;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Aurelion_Sol_As_the_Star_Forger {
	internal class AurelionSol {
		public static Spell Q { get; set; }
		public static Spell W { get; set; }
		public static Spell E { get; set; }
		public static Spell R { get; set; }
		public static Orbwalking.Orbwalker Orbwalker { get; set; }
		public static Menu Config { get; set; }
		public static AIHeroClient Player => HeroManager.Player;
		public static bool IsWActive => Player.HasBuff("AurelionSolWActive");
		public static Font font { get; set; }
		
		public static Vector3 BestPosition { get; set; }

		internal static void Game_OnGameLoad(EventArgs args) {
			if (Player.ChampionName != "AurelionSol")
			{
				return;
			}
			font = new Font(Drawing.Direct3DDevice,new SharpDX.Direct3D9.FontDescription {
				 Height = 25,
				 FaceName = "微软雅黑"
			});

			LoadSpell();
			LoadMenu();
			LoadEvent();
		}

		private static void LoadEvent() {
			Game.OnUpdate += Game_OnUpdate;
			Drawing.OnDraw += Drawing_OnDraw;
			AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
			Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
			Spellbook.OnCastSpell += Spellbook_OnCastSpell;
		}

		private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args) {
			if (sender.Owner.IsMe && args.Slot == SpellSlot.E && Config.GetBool("标识E") && args.EndPosition.CountAlliesInRange(600)>0)
			{
				TacticalMap.SendPing(PingCategory.OnMyWay,args.EndPosition);
			}
		}

		private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args) {
			if (Config.GetBool("打断Q") && !sender.HasSpellShield() && Q.CanCast(sender))
			{
				Q.Cast(sender);

			}

			if (Config.GetBool("R打断") && args.MovementInterrupts && !sender.HasSpellShield() && R.CanCast(sender))
			{
				R.Cast(sender);
			}
		}

		private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser) {
			if (gapcloser.Sender.ChampionName == "MasterYi" && gapcloser.Slot == SpellSlot.Q || !Config.GetBool("防突名单"+gapcloser.Sender.ChampionName))
			{
				return;
			}

			if (Config.GetBool("防突Q") && !gapcloser.Sender.HasSpellShield())
			{
				Q.Cast(gapcloser.Sender);
			}

			if (Config.GetBool("防突E") && E.IsReady())
			{
				var allies = HeroManager.Allies.Where(a => !a.IsDead && a.Distance(Player) < 1000);
				if (allies?.Count() > 0)
				{
					var pose = E.GetCircularFarmLocation(allies.Cast<Obj_AI_Base>().ToList()).Position;
					if (pose.Distance(Player.Position) > 500)
					{
						E.Cast(pose);
					}
				}
				else
				{
					
				}
			}

			if (Config.GetBool("R防突进") 
				&& !gapcloser.Sender.HasSpellShield())
			{
				R.Cast(gapcloser.Sender);
				//CastSpell(R,gapcloser.Sender);
			}
		}

		private static void Drawing_OnDraw(EventArgs args) {
			Q.DrawRange(Config.GetCircle("Q范围"));
			W.DrawRange(Config.GetCircle("W范围"));
			R.DrawRange(Config.GetCircle("R范围"));

			var EShow = Config.GetCircle("E范围");
			if (EShow.Active && E.IsReady())
			{
				var eRange = new[] { 3000, 4000 , 5000 , 6000 , 7000 }[E.Level - 1];
				Render.Circle.DrawCircle(Player.Position, eRange,EShow.Color);
			}

			var EMiniShow = Config.GetCircle("E小地图");
			if (EMiniShow.Active && E.IsReady())
			{
				var eRange = new[] { 3000, 4000, 5000, 6000, 7000 }[E.Level - 1];
				LeagueSharp.Common.Utility.DrawCircle(Player.Position, eRange, EMiniShow.Color,2,25,true);
			}

			var ManaShow = Config.GetCircle("蓝量显示");
			if (ManaShow.Active && W.Level >=1)
			{
				var ws = (int)((Player.Mana - (IsWActive ? 0 : 40)) / new []{16,22,28,34,40 }[W.Level-1]);
				if (CNLib.MultiLanguage.IsCN)
				{
					font.DrawTextCentered($"可以W {ws} 秒", Player, ManaShow.Color);
				}
				else
				{
					font.DrawTextCentered($"W {ws} Sec", Player, ManaShow.Color);
				}
				
			}

			//Render.Circle.DrawCircle(BestPosition,50,Color.White);
		}

		private static void Game_OnUpdate(EventArgs args) {

			Orbwalker.SetOrbwalkingPoint(Vector3.Zero);

			QLogic();
			WLogic();
			RLogic();

			//Combo();
		}

		private static void QLogic() {
			
			if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Config.GetBool("连招Q") 
				|| Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Config.GetBool("消耗Q"))
			{
				var target = TargetSelector.GetTarget(Q.Range + Q.Width, TargetSelector.DamageType.Magical);
				if (target != null && target.IsValid)
				{
					//Q.Cast(target);
					if (Q.CanCast(target))
					{
						Q.Cast(target);
						//CastSpell(Q, target);
					}
				}
			}
			else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
			{
				var minions = new List<Obj_AI_Base>();
				if (Config.GetBool("清线Q") && Config.GetBool("清野Q"))
				{
					minions = MinionManager.GetMinions(Q.Range,MinionTypes.All, MinionTeam.NotAlly);
				}
				else if (Config.GetBool("清线Q"))
				{
					minions = MinionManager.GetMinions(Q.Range, MinionTypes.All,MinionTeam.Enemy);
				}
				else if (Config.GetBool("清野Q"))
				{
					minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral);
				}
				else
				{
					return;
				}
				var farmlocation = Q.GetCircularFarmLocation(minions);
				if (farmlocation.MinionsHit>=3 || minions.Any(m=>m.IsHPBarRendered))
				{
					Q.Cast(farmlocation.Position);
				}
			}
		}

		private static void WLogic2() {
			var target = TargetSelector.GetTarget(Q.Range + Q.Width, TargetSelector.DamageType.Magical);
			if (target == null || !target.IsValid) return;
			var distence = target.Distance(Player);

			if (W.IsReady() && Config.GetBool("自动W") && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed))
			{

				//走向W2
				if (distence < 650 && distence > 610 &&  target.MoveSpeed > Player.MoveSpeed && target.IsFacing(Player) && !IsWActive)
				{
					W.Cast();
					//350+(600-350)/2 = 350 + 250/2 = 350 + 125 = 475
					//350	475		600
					//DeBug.WriteChatBox("走向W2");
				}
				//处于W2
				else if (distence > 590 && distence <610 && !IsWActive)
				{
					W.Cast();
					//DeBug.WriteChatBox("处于W2");
				}
				//从里走向W2
				else if (distence < 590 && distence > 475 && !target.IsFacing(Player) && !IsWActive)
				{
					
					W.Cast();
					//DeBug.WriteChatBox("从里走向W2");
				}
				//从外走向W1
				else if (distence < 475 && distence > 360 && target.IsFacing(Player) && IsWActive)
				{
					W.Cast();
					//DeBug.WriteChatBox("从外走向W1");
				}
				//处于W1内
				else if (distence < 360 && IsWActive)
				{
					W.Cast();
					//DeBug.WriteChatBox("处于W1内");
				}
			}

		
		}

		private static void WLogic() {
			var target = TargetSelector.GetTarget(Q.Range + Q.Width, TargetSelector.DamageType.Magical);
			if (target == null || !target.IsValid || target.IsDead || target.IsZombie) return;
			var distence = target.Distance(Player);

			if (Config.GetBool("自动W") && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed))
			{

				if (Player.GetEnemiesInRange(1000).Where(enemy => !enemy.IsDead).Count() == 0 && IsWActive)
				{
					W.Cast();
				}
				else if (target.Distance(Player) < 400 && IsWActive)
				{
					W.Cast();
				}
				else if (target.Distance(Player) > 320 && !IsWActive)
				{
					W.Cast();
				}
			}

			if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
					&& Config.GetBool("禁止平A")
					&& W.IsInRange(target,100)
					&& Player.MoveSpeed + 25 < target.MoveSpeed)
			{
				Orbwalking.Attack = false;
			}
			else
			{
				Orbwalking.Attack = true;
			}

			
			if (Config.GetBool("调整走位")
				&& distence <= W.Range
				&& Player.MoveSpeed >= target.MoveSpeed
				&& Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
			{
				OrbwalkerBestPosition(target);
			}
			
		}

		private static void ELogic() {
			if (Config.GetKeyActive("E加Q") && Player.Mana > Q.ManaCost + E.ManaCost && Q.IsReady() && E.IsReady())
			{
				var pos = Game.CursorPos;
				Q.Cast(pos);
				E.Cast(pos);
			}
		}

		private static void RLogic() {

			if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
			{
				var target = TargetSelector.GetTarget(Q.Range + Q.Width, TargetSelector.DamageType.Magical);
				if (target == null || !target.IsValid) return;
				var distence = target.Distance(Player);

				if (R.IsReady() && Config.GetBool("连招R") && distence < (target.IsMelee ? 340 : W.Range - 100))
				{
					R.Cast(target, false, true);
				}
			}

			if (Config.GetBool("抢人头R"))
			{
				foreach (var enemy in HeroManager.Enemies.Where(e=>e.IsValid && !e.IsDead && GetRDmg(e)> e.Health).OrderByDescending(e=>TargetSelector.GetPriority(e)))
				{
					R.Cast(enemy);
				}
			}
			
		}

		private static float GetQDmg(Obj_AI_Base target) {
			if (Q.Level >=1)
			{
				var amount = new[] { 70, 110, 150, 190, 230 }[Q.Level - 1] + 0.65f * Player.TotalMagicalDamage;
				return (float)GetDmg(target, amount);
			}
			return 0;
		}

		private static float GetRDmg(Obj_AI_Base target) {
			if (R.Level >=1)
			{
				var amount = new[] { 200, 400, 600 }[R.Level - 1] + 0.70f * Player.TotalMagicalDamage;
				return (float)GetDmg(target, amount);
			}
			return 0;
			
		}

		private static double GetDmg(Obj_AI_Base target,double amount) {
			amount = Player.CalcDamage(target, Damage.DamageType.Magical, amount) - target.HPRegenRate;
			if (Player.HasBuff("summonerexhaust"))
				amount = amount * 0.6f;

			if (target.HasBuff("ferocioushowl"))
				amount = amount * 0.7f;

			if (target.Type == GameObjectType.AIHeroClient && (target as AIHeroClient).ChampionName == "Blitzcrank" && !target.HasBuff("BlitzcrankManaBarrierCD") && !target.HasBuff("ManaBarrier"))
			{
				amount -= target.Mana / 2f;
			}
			return amount;
		}

		private static void LoadMenu() {
			CNLib.MultiLanguage.AddLanguage(new Dictionary<string, Dictionary<string, string>> {
				{"English",Languages.EnglishDictionary },
			});

			Chat.Print("Aurelion Sol".ToHtml(32) + "  迎接星辰之力吧，凡人 !".ToHtml("#D15FEE", FontStlye.Bold));
				
			Config = MenuExtensions.CreatMainMenu("Aurelion Sol As the Star Forger", "铸星龙王 - 索尔");

			Orbwalker = Config.AddOrbwalker("走砍设置", "走砍设置");

			var QMenu = Config.AddMenu("Q设置", "Q设置");
			QMenu.AddBool("连招Q", "连招使用Q",true);
			QMenu.AddBool("消耗Q", "消耗使用Q", true);
			QMenu.AddBool("清线Q", "清线使用Q", true);
			QMenu.AddBool("清野Q", "清野使用Q", true);
			QMenu.AddBool("打断Q", "打断技能", true);
			QMenu.AddBool("防突Q", "防突进", true);

			var WMenu = Config.AddMenu("W设置", "W设置");
			WMenu.AddBool("自动W", "自动W", true);

			var EMenu = Config.AddMenu("E设置", "E设置");
			//EMenu.AddKeyBind("E加Q","QE到目标地点",'G',KeyBindType.Press);
			EMenu.AddBool("标识E", "飞行时打标识给队友", true);
			EMenu.AddBool("防突E","E可用时防突进",true);

			var RMenu = Config.AddMenu("R设置", "R设置");
			RMenu.AddBool("抢人头R","抢人头使用R",false);
			RMenu.AddBool("连招R", "连招使用R", true);
			RMenu.AddBool("R打断", "打断技能", true);
			//RMenu.AddSeparator();
			RMenu.AddBool("R防突进", "防突进", true);
			

			var MMenu = Config.AddMenu("其它设置", "其它设置");
			MMenu.AddBool("禁止平A", "连招时不使用平A", true);
			MMenu.AddBool("调整走位","调整走位配合被动",true);
			MMenu.AddSeparator();
			MMenu.AddLabel("防突进名单");
			foreach (var enemy in HeroManager.Enemies)
			{
				MMenu.AddBool("防突名单" + enemy.ChampionName, enemy.CnName(), true);
			}

			var DMenu = Config.AddMenu("显示设置", "显示设置");
			DMenu.AddCircle("Q范围", "显示Q范围",true,Color.AliceBlue);
			DMenu.AddCircle("W范围", "显示W范围", true, Color.AliceBlue);
			DMenu.AddCircle("E范围", "显示E范围", true, Color.AliceBlue);
			DMenu.AddCircle("E小地图", "小地图显示E范围", true, Color.AliceBlue);
			DMenu.AddCircle("R范围", "显示R范围", true, Color.AliceBlue);
			DMenu.AddCircle("蓝量显示", "显示蓝量可以用几秒W", true, Color.AliceBlue);

			AutoLevelUp.Initialize(Config,new []{R,W,Q,E});
			CheckVersion.Initialize(Config, "https://raw.githubusercontent.com/VivianGit/LeagueSharp/master/Aurelion%20Sol%20As%20the%20Star%20Forger/Aurelion%20Sol%20As%20the%20Star%20Forger/Properties/AssemblyInfo.cs");
		}

		private static void LoadSpell() {
			Q = new Spell(SpellSlot.Q, 650f);
			W = new Spell(SpellSlot.W, 600f);
			E = new Spell(SpellSlot.E, 400f);
			R = new Spell(SpellSlot.R, 1420f);

			Q.SetSkillshot(0.25f, 180, 850, false, SkillshotType.SkillshotLine);
			R.SetSkillshot(0.25f, 300, 4500, false, SkillshotType.SkillshotLine);
		}

		private static Wstatus GetWStatus() {
			if (!W.IsReady())
			{
				return Wstatus.Cooldown;
			}
			else
			{
				return Player.HasBuff("AurelionSolWActive") ? Wstatus.Active : Wstatus.Passive;
			}
		}
		enum Wstatus {
			Cooldown,
			Passive,
			Active
		}

		private static List<Vector3> CirclePoints(Vector3 position, float radius, float circleLineSegmentN = 20) {
			List<Vector3> points = new List<Vector3>();
			for (var i = 1; i <= circleLineSegmentN; i++)
			{
				var angle = i * 2 * Math.PI / circleLineSegmentN;
				var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
				if (point.Distance(position) == radius)
				{
					points.Add(point);
				}
			}
			return points;
		}

		private static void OrbwalkerBestPosition(AIHeroClient target) {
			if (target == null || !target.IsValid || target.IsDead || target.IsZombie || target.IsDashing())
			{
				Orbwalker.SetOrbwalkingPoint(Vector3.Zero);
				return;
			}
			var BestPosition = Vector3.Zero;

			float Wrange = IsWActive ? W.Range - 20 : 350;

			var points = CirclePoints(target.Position, Wrange);
		
			foreach (var point in points.Where(p => !p.IsWall()))
			{
				if (BestPosition == Vector3.Zero || Game.CursorPos.Distance(point) < Game.CursorPos.Distance(BestPosition))
				{
					BestPosition = point;
				}
			}

			if (BestPosition == Vector3.Zero)
			{
				Orbwalker.SetOrbwalkingPoint(Vector3.Zero);
				
			}
			else
			{
				Orbwalker.SetOrbwalkingPoint(BestPosition);
			}
		}
	}
}