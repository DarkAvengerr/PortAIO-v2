#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Common;

using Marksman.Utils;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;


#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    using System.Threading;

    using Utils = LeagueSharp.Common.Utils;

    internal class KindredUltimate
    {
        public GameObject Object { get; set; }
        public Vector3 Position { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
    }

    internal class Kindred : Champion
    {
        public static Spell Q;
        public static Spell E;
        public static Spell W;
        public static Spell R;
        public static AIHeroClient KindredECharge;

        public static KindredUltimate KindredUltimate = new KindredUltimate();

        public Kindred()
        {
            Q = new Spell(SpellSlot.Q, 375);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 740);
            R = new Spell(SpellSlot.R, 1100);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotCircle);

            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;



            Marksman.Utils.Utils.PrintMessage("Kindred");
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(GetValue<bool>("Misc.Q.AntiMelee") && Q.IsReady()))
                return;

            if (args.Target != null && args.Target.IsMe && sender.Type == GameObjectType.AIHeroClient && sender.IsEnemy && sender.IsMelee && args.SData.IsAutoAttack())
                Q.Cast(ObjectManager.Player.Position.Extend(sender.Position, -Q.Range));
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!(GetValue<bool>("Misc.Q.Antigapcloser") && Q.IsReady())) return;

            if (!(gapcloser.End.Distance(ObjectManager.Player.Position) <= 200)) return;

            if (gapcloser.Sender.IsValidTarget())
                Q.Cast(ObjectManager.Player.Position.Extend(gapcloser.Sender.Position, -Q.Range));
        }
		
        public override void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (args.Buff.Name.ToLower() == "kindredecharge" && !sender.IsMe)
            {
                KindredECharge = sender as AIHeroClient;
            }
        }

        public override void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (args.Buff.Name.ToLower() == "kindredecharge" && !sender.IsMe)
            {
                KindredECharge = null;
            }
        }

        public override void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals("Kindred_Base_R_AOE.troy", StringComparison.InvariantCultureIgnoreCase) || sender.Name.Contains("Kindred_Base_R_NoDeath_Buff"))
            {
                KindredUltimate.Object = sender;
                KindredUltimate.Position = sender.Position;
                KindredUltimate.StartTime = Environment.TickCount;
                KindredUltimate.EndTime = Environment.TickCount + 5000;
            }
            //Kindred_Base_R_AOE.troy
            //Kindred_Base_R_NoDeath_Buff.troy
        }

        public override void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals("Kindred_Base_R_AOE.troy", StringComparison.InvariantCultureIgnoreCase) || sender.Name.Contains("Kindred_Base_R_NoDeath_Buff"))
            {
                KindredUltimate.Object = null;
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = GetValue<StringList>("DrawQ").SelectedIndex;
            switch (drawQ)
            {
                case 1:
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua);
                    break;
                case 2:
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65, Color.Aqua);
                    break;
            }
            Spell[] spellList = { W, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }
            }
        }

		
        public void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //Kindred_Base_R_AOE.troy
            //Kindred_Base_R_NoDeath_Buff.troy
            return;
            if (!R.IsReady())
            {
                return;
            }

            if (sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            if (!sender.IsValid || sender.Team == ObjectManager.Player.Team)
            {
                return;
            }

            //if (R.IsReady())
            //{
            //    if (sender.IsEnemy && sender is AIHeroClient && args.Target.IsMe)
            //    {
            //        foreach (
            //            var c in
            //                DangerousList.Where(c => ((AIHeroClient) sender).ChampionName.ToLower() == c.ChampionName)
            //                    .Where(c => args.Slot == c.SpellSlot))
            //            //.Where(c => args.SData.Name == ((AIHeroClient)sender).GetSpell(c.SpellSlot).Name))
            //        {
            //            R.Cast(ObjectManager.Player.Position);
            //        }
            //    }

            //}
						
			//TODO: Find kindred ulti object and jump in with Q
			
			if (sender != null)
				if (args.Target != null)
					if (args.Target.IsMe)
						if (sender.Type == GameObjectType.AIHeroClient)
							if (sender.IsEnemy)
								if (sender.IsMelee)
									if (args.SData.IsAutoAttack())
										//if (MenuProvider.Champion.Misc.GetBoolValue("Use Anti-Melee (Q)"))
											if (Q.IsReady())
												Q.Cast(ObjectManager.Player.Position.Extend(sender.Position, -Q.Range));


            if (R.IsReady())
            {
                var x = 0d;
                if (ObjectManager.Player.HealthPercent < 20 && ObjectManager.Player.CountEnemiesInRange(500) > 0)
                {
                    x = HeroManager.Enemies.Where(e => e.IsValidTarget(1000))
                        .Aggregate(0, (current, enemy) => (int)(current + enemy.Health));
                }
                if (ObjectManager.Player.Health < x)
                {
                    R.Cast(ObjectManager.Player.Position);
                }
                
                if (Program.Config.Item("UseRC").GetValue<bool>() &&
                    ObjectManager.Player.Health < ObjectManager.Player.MaxHealth * .2)
                {
                    if (!sender.IsMe && sender.IsEnemy && R.IsReady() && args.Target.IsMe) // for minions attack
                    {
                        R.Cast(ObjectManager.Player.Position);
                    }
                    else if (!sender.IsMe && sender.IsEnemy && (sender is AIHeroClient || sender is Obj_AI_Turret) && args.Target.IsMe && R.IsReady())
                    {
                        R.Cast(ObjectManager.Player.Position);
                    }
                }
            }
        }

        public override void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            foreach (
                var target in
                    HeroManager.Enemies.Where(
                        e =>
                            e.IsValid && e.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(null) + 65 &&
                            e.IsVisible).Where(target => target.HasBuff("kindredcharge")))
            {
                Orbwalker.ForceTarget(target);
            }
        }

        public override void GameOnUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                var x = 0d;
                if (ObjectManager.Player.HealthPercent < 20 && ObjectManager.Player.CountEnemiesInRange(500) > 0)
                {
                    x = HeroManager.Enemies.Where(e => e.IsValidTarget(1000))
                        .Aggregate(0, (current, enemy) => (int)(current + enemy.Health));
                }
                if (ObjectManager.Player.Health < x)
                {
                    R.Cast(ObjectManager.Player.Position);
                }
            }

            AIHeroClient t = null;
            if (KindredECharge != null)
            {
                t = KindredECharge;
                TargetSelector.SetTarget(KindredECharge);
            }
            else
            {
                t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            }


            if (!t.IsValidTarget())
            {
                return;
            }

            if (ComboActive && !t.HasKindredUltiBuff())
            {
                if (t.IsValidTarget(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65) && !t.HasKindredUltiBuff())
                {
                    if (GetValue<StringList>("Combo.Q.Use").SelectedIndex != 0 && Q.IsReady())
                    {
                        if (GetValue<StringList>("Combo.Q.Use").SelectedIndex == 1)
                        {
                            var x = CommonUtils.GetDashPosition(E, t, 400);
                            Q.Cast(x);
                        }

                        if (GetValue<StringList>("Combo.Q.Use").SelectedIndex == 2)
                        {
                            Q.Cast(Game.CursorPos);                        }

                        }

                    if (GetValue<bool>("Combo.E.Use") && E.IsReady() && t.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(t);
                    }

                    if (GetValue<bool>("Combo.W.Use") && W.IsReady() && t.IsValidTarget(W.Range / 2))
                    {
                        W.Cast(t.Position);
                    }
                }
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("Combo.Q.Use" + Id, "Q:").SetValue(new StringList(new []{"Off", "Smart", "Cursor Position"}, 1))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            config.AddItem(new MenuItem("Combo.W.Use" + Id, "W:").SetValue(true)).SetFontStyle(FontStyle.Regular, W.MenuColor());
            config.AddItem(new MenuItem("Combo.E.Use" + Id, "E:").SetValue(true)).SetFontStyle(FontStyle.Regular, E.MenuColor());
            config.AddItem(new MenuItem("Combo.R.Use" + Id, "R:").SetValue(true)).SetFontStyle(FontStyle.Regular, R.MenuColor());
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "Q:").SetValue(false));
            config.AddItem(new MenuItem("UseWH" + Id, "W:").SetValue(false));
            config.AddItem(new MenuItem("UseEH" + Id, "E:").SetValue(false));
            config.AddItem(
                new MenuItem("UseETH", "E (Toggle)").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle)));
            return true;
        }

        public override bool LaneClearMenu(Menu menuLane)
        {
            menuLane.AddItem(new MenuItem("UseQL" + Id, "Use Q").SetValue(true)).ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs args)
                {
                    menuLane.Item("UseQLM").Show(args.GetNewValue<bool>());
                    Program.ChampionClass.Config.Item("LaneMinMana").Show(args.GetNewValue<bool>());
                };

            menuLane.AddItem(new MenuItem("UseQLM", "Min. Minion:").SetValue(new Slider(2, 1, 3)));
            menuLane.AddItem(new MenuItem("UseWL", "Use W").SetValue(false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("DrawQ" + Id, "Q range").SetValue(new StringList(new[] { "Off", "Q Range", "Q + AA Range" }, 2)));
            config.AddItem(new MenuItem("DrawW" + Id, "W range").SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.AddItem(new MenuItem("DrawE" + Id, "E range").SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.AddItem(new MenuItem("DrawR" + Id, "R range").SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Damage After Combo").SetValue(true);

            config.AddItem(dmgAfterComboItem);

            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddItem(new MenuItem("Misc.Q.Antigapcloser" + Id, "Q: Antigapcloser").SetValue(true));
            config.AddItem(new MenuItem("Misc.Q.AntiMelee" + Id, "Q: Anti-Melee").SetValue(true));
            
            return true;
        }

        public override void PermaActive()
        {
            //KindredRNoDeathBuff
            if (KindredUltimate != null && KindredUltimate.EndTime > Environment.TickCount + 300 && !ObjectManager.Player.HasBuff("KindredRNoDeathBuff") && Q.IsReady())
            {
                Q.Cast(KindredUltimate.Position);
            }

            base.PermaActive();
        }

        public override void ExecuteLane()
        {
            var useQ = Program.Config.Item("UseQL").GetValue<StringList>().SelectedIndex;

            var minion =
                MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range)
                    .FirstOrDefault(m => m.Health < ObjectManager.Player.GetSpellDamage(m, SpellSlot.Q));

            if (minion != null)
            {
                switch (useQ)
                {
                    case 1:
                        minion =
                            MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range)
                                .FirstOrDefault(
                                    m =>
                                        m.Health < ObjectManager.Player.GetSpellDamage(m, SpellSlot.Q)
                                        && m.Health > ObjectManager.Player.TotalAttackDamage);
                        Q.Cast(minion);
                        break;

                    case 2:
                        minion =
                            MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range)
                                .FirstOrDefault(
                                    m =>
                                        m.Health < ObjectManager.Player.GetSpellDamage(m, SpellSlot.Q)
                                        && ObjectManager.Player.Distance(m)
                                        > Orbwalking.GetRealAutoAttackRange(null) + 65);
                        Q.Cast(minion);
                        break;
                }
            }
        }

        public override bool JungleClearMenu(Menu menuJungle)
        {
            menuJungle.AddItem(new MenuItem("UseQJ" + Id, "Use Q").SetValue(new StringList(new[] { "Off", "On", "Just big Monsters" }, 1)));
            menuJungle.AddItem(new MenuItem("UseWJ" + Id, "Use W").SetValue(new StringList(new[] { "Off", "On", "Just big Monsters" }, 1)));
            menuJungle.AddItem(new MenuItem("UseEJ" + Id, "Use E").SetValue(new StringList(new[] { "Off", "On", "Just big Monsters" }, 1)));

            return true;
        }

        public override void ExecuteJungle()
        {
            var jungleMobs = Marksman.Utils.Utils.GetMobs(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                Marksman.Utils.Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                switch (GetValue<StringList>("UseQJ").SelectedIndex)
                {
                    case 1:
                        {
                            if (jungleMobs.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                                Q.Cast(jungleMobs.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)
                                    ? Game.CursorPos
                                    : jungleMobs.Position);
                            break;
                        }
                    case 2:
                        {
                            jungleMobs = Marksman.Utils.Utils.GetMobs(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,Marksman.Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                Q.Cast(jungleMobs.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) ? Game.CursorPos : jungleMobs.Position);
                            }
                            break;
                        }
                }

                switch (GetValue<StringList>("UseWJ").SelectedIndex)
                {
                    case 1:
                        {
                            if (jungleMobs.IsValidTarget(W.Range))
                                W.Cast(jungleMobs.Position);
                            break;
                        }
                    case 2:
                        {
                            jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Marksman.Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                W.Cast(jungleMobs.Position);
                            }
                            break;
                        }
                }

                switch (GetValue<StringList>("UseEJ").SelectedIndex)
                {
                    case 1:
                        {
                            if (jungleMobs.IsValidTarget(E.Range))
                                E.CastOnUnit(jungleMobs);
                            break;
                        }
                    case 2:
                        {
                            jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Marksman.Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                E.CastOnUnit(jungleMobs);
                            }
                            break;
                        }
                }

            }
        }
    }
}
