using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HikiCarry.Core.Plugins;
using HikiCarry.Core.Predictions;
using HikiCarry.Core.Utilities;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Champions
{
    class Draven
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public static List<Axe> AxeSpots = new List<Axe>();
        public static List<string> AxesList = new List<string>
        {
            "Draven_Base_Q_reticle.troy" ,
            "Draven_Skin01_Q_reticle.troy" ,
            "Draven_Skin03_Q_reticle.troy"
        };

        public static List<string> BuffList = new List<string>
        {
            "Draven_Base_Q_buf.troy",
            "Draven_Skin01_Q_buf.troy",
            "Draven_Skin02_Q_buf.troy",
            "Draven_Skin03_Q_buf.troy"
        };

        private static int CurrentAxes { get; set; }
        private static int LastQTime { get; set; }

        public Draven()
        {
            Q = new Spell(SpellSlot.Q, ObjectManager.Player.AttackRange);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 950);
            R = new Spell(SpellSlot.R, 3000f);

            E.SetSkillshot(0.25f, 100, 1400, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.4f, 160, 2000, false, SkillshotType.SkillshotLine);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("draven.q.combo", "Use Q",true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("draven.q.combo.axe.count", "Min. Axe Count",true).SetValue(new Slider(2, 1, 2)));
                comboMenu.AddItem(new MenuItem("draven.e.combo", "Use E",true).SetValue(true));
                comboMenu.AddItem(new MenuItem("draven.r.combo", "Use R",true).SetValue(true));
                Initializer.Config.AddSubMenu(comboMenu);
            }

            var harassmenu = new Menu("Harass Settings","Harass Settings");
            {
                harassmenu.AddItem(new MenuItem("draven.q.harass", "Use Q", true).SetValue(true));
                harassmenu.AddItem(
                    new MenuItem("draven.q.harass.axe.count", "Min. Axe Count", true).SetValue(new Slider(2, 1, 2)));
                harassmenu.AddItem(new MenuItem("draven.e.harass", "Use E", true).SetValue(true));
                harassmenu.AddItem(
                    new MenuItem("draven.harass.mana", "Min. Harass Mana", true).SetValue(new Slider(60, 1, 99)));
                Initializer.Config.AddSubMenu(harassmenu);
            }

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("draven.q.clear", "Use Q",true).SetValue(true));
                clearMenu.AddItem(
                    new MenuItem("draven.q.lane.clear.axe.count", "Min. Axe Count",true).SetValue(new Slider(1, 1, 2)));
                clearMenu.AddItem(
                    new MenuItem("draven.q.minion.count", "(Q) Min. Minion Count",true).SetValue(new Slider(4, 1, 10)));
                clearMenu.AddItem(new MenuItem("draven.clear.mana", "Min. Mana",true).SetValue(new Slider(50, 1, 99)));
                Initializer.Config.AddSubMenu(clearMenu);
            }

            var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                jungleMenu.AddItem(new MenuItem("draven.q.jungle", "Use Q",true).SetValue(true));
                jungleMenu.AddItem(
                    new MenuItem("draven.q.jungle.clear.axe.count", "Min. Axe Count",true).SetValue(new Slider(1, 1, 2)));
                jungleMenu.AddItem(new MenuItem("draven.jungle.mana", "Min. Mana",true).SetValue(new Slider(50, 1, 99)));
                Initializer.Config.AddSubMenu(jungleMenu);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
                var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, System.Drawing.Color.Gold));

                drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                DamageIndicator.DamageToUnit = TotalDamage;
                DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                Initializer.Config.AddSubMenu(drawMenu);
            }

            Initializer.Config.AddItem(
                    new MenuItem("draven.axe.underturret", "Dont Catch Axe Under Turret", true).SetValue(true));
            Initializer.Config.AddItem(
                new MenuItem("draven.axe.in.enemies", "Dont Catch Axe In Enemies", true).SetValue(true));
            Initializer.Config.AddItem(new MenuItem("catch.axes", "Auto Catch Axes ?", true).SetValue(false));
            Initializer.Config.AddItem(new MenuItem("draw.catch.modes", "Draw Catch Sector/Circle", true).SetValue(false));
            Initializer.Config.AddItem(new MenuItem("draw.axe.positions", "Draw Axe Positions & Duration", true).SetValue(false));

            Initializer.Config.AddItem(
                new MenuItem("catch.logic", "Axe Catch Mode", true).SetValue(new StringList(new[] {"Sector", "Circle"})))
                .SetFontStyle(FontStyle.Bold, SharpDX.Color.Yellow);
            Initializer.Config.AddItem(
                new MenuItem("catch.radius", "Axe Catch Radius", true).SetValue(new Slider(600, 1, 1500)))
                .SetFontStyle(FontStyle.Bold);

            Initializer.Config.AddItem(
                new MenuItem("info.draven", "                                 Prediction Settings", true).SetFontStyle(FontStyle.Bold));

            Obj_AI_Base.OnProcessSpellCast += DravenOnProcess;
            GameObject.OnCreate += DravenOnCreate;
            GameObject.OnDelete += DravenOnDelete;
            Drawing.OnDraw += DravenOnDraw;
            Game.OnUpdate += DravenOnUpdate;
        }

        private float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
            if (Q.IsReady())
            {
                damage += Q.GetDamage(hero);
            }
            
            if (E.IsReady())
            {
                damage += W.GetDamage(hero);
            }
            if (R.IsReady())
            {
                damage += R.GetDamage(hero);
            }
            return (float)damage;
        }

        private void DravenOnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.SData.Name == "dravenspinning")
            {
                LastQTime = Environment.TickCount;
            }
        }

        private void DravenOnDraw(EventArgs args)
        {
            if (Utilities.Enabled("draw.catch.modes"))
            {
                switch (Initializer.Config.Item("catch.logic", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0: //Sector
                        var sectorpoly = new Geometry.Polygon.Sector(
                                ObjectManager.Player.Position.To2D(),
                                Game.CursorPos.To2D(),
                                100 * (float)Math.PI / 180,
                                Utilities.Slider("catch.radius"));
                        sectorpoly.Draw(Color.Gold);
                        break;
                    case 1: // Circle
                        var circlepoly = new Geometry.Polygon.Circle(ObjectManager.Player.Position.Extend(Game.CursorPos, Utilities.Slider("catch.radius")), 
                            Utilities.Slider("catch.radius"));
                        circlepoly.Draw(Color.Gold);
                        break;
                }
            }

            if (Utilities.Enabled("draw.axe.positions"))
            {
                foreach (var axe in AxeSpots)
                {
                    if (CatchableAxes(axe))
                    {
                        Render.Circle.DrawCircle(axe.Object.Position, 100, Color.GreenYellow);
                        Drawing.DrawText(Drawing.WorldToScreen(axe.Object.Position).X - 40, Drawing.WorldToScreen(axe.Object.Position).Y,
                            Color.Gold, (((float)(axe.EndTick - Environment.TickCount))) + " ms");
                    }
                }
            }
        }

        private void DravenOnUpdate(EventArgs args)
        {
            switch (Initializer.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnClear();
                    OnJungle();
                    break;
            }

            CatchAxe();
        }

        private void OnHarass()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("harass.mana"))
            {
                return;
            }
            var target = TargetSelector.GetTarget(3500f, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && LastQTime + 100 < Environment.TickCount && target.IsValidTarget(Q.Range)
                    && Utilities.Enabled("draven.q.harass") &&
                    CurrentAxes < Utilities.Slider("draven.q.harass.axe.count"))
                {
                    Q.Cast();
                }

                if (E.IsReady() && Utilities.Enabled("draven.e.harass") && target.IsValidTarget(E.Range))
                {
                    E.Do(target, Utilities.HikiChance("hitchance"));
                }

            }
        }

        public static void CatchAxe()
        {
            if (Utilities.Enabled("catch.axes"))
            {
                if (Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var axe = AxeSpots.OrderBy(ax3 => ax3.EndTick).FirstOrDefault(CatchableAxes);
                    if (axe != null)
                    {
                        Initializer.Orbwalker.SetOrbwalkingPoint(axe.Object.Position);
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            500, () => Initializer.Orbwalker.SetOrbwalkingPoint(Game.CursorPos));
                    }
                    else
                    {
                        Initializer.Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                    }
                }
                if (Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var axe = AxeSpots.OrderBy(ax3 => ax3.EndTick).FirstOrDefault(CatchableAxes);
                    if (axe != null)
                    {
                        Initializer.Orbwalker.SetOrbwalkingPoint(axe.Object.Position);
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => Initializer.Orbwalker.SetOrbwalkingPoint(Game.CursorPos));
                    }
                    else
                    {
                        Initializer.Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                    }
                }
            }
            

        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(3500f, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && LastQTime + 100 < Environment.TickCount && target.IsValidTarget(Q.Range)
                && Utilities.Enabled("draven.q.combo") && CurrentAxes < Utilities.Slider("draven.q.combo.axe.count"))
                {
                    Q.Cast();
                }

                if (E.IsReady() && Utilities.Enabled("draven.e.combo") && target.IsValidTarget(E.Range))
                {
                    E.Do(target, Utilities.HikiChance("hitchance"));
                }

                if (R.IsReady() && Utilities.Enabled("draven.r.combo") && target.IsValidTarget(R.Range)
                    && R.GetDamage(target) > target.Health)
                {
                    R.Do(target, Utilities.HikiChance("hitchance"));
                }
            }
        }
        private void OnClear()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("draven.clear.mana"))
            {
                return;
            }

            if (Q.IsReady() && LastQTime + 100 < Environment.TickCount
               && Utilities.Enabled("draven.q.clear") && CurrentAxes < Utilities.Slider("draven.q.lane.clear.axe.count"))
            {
                var minionlist = MinionManager.GetMinions(ObjectManager.Player.AttackRange);
                if (minionlist.Count() >= Utilities.Slider("draven.q.minion.count") && minionlist != null)
                {
                    Q.Cast();
                }
            }
        }
        
        private void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("draven.jungle.mana"))
            {
                return;
            }

            if (Q.IsReady() && LastQTime + 100 < Environment.TickCount
               && Utilities.Enabled("draven.q.jungle") && CurrentAxes < Utilities.Slider("draven.q.jungle.clear.axe.count"))
            {
                var target = MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                   .FirstOrDefault(x => x.IsValidTarget(Q.Range));

                if (target != null)
                {
                    Q.Cast();
                }
            }
        }

        private void DravenOnDelete(GameObject sender, EventArgs args)
        {
            for (var i = 0; i < AxeSpots.Count; i++)
            {
                if (AxeSpots[i].Object.NetworkId == sender.NetworkId)
                {
                    AxeSpots.RemoveAt(i);
                    return;
                }
            }

            if ((BuffList.Contains(sender.Name)) && 
                sender.Position.Distance(ObjectManager.Player.Position) < 300)
            {
                if (CurrentAxes == 0)
                {
                    CurrentAxes = 0;
                }

                if (CurrentAxes <= 2)
                {
                    CurrentAxes = CurrentAxes - 1;
                }
                else
                {
                    CurrentAxes = CurrentAxes - 1;
                }
            }
        }

        private void DravenOnCreate(GameObject sender, EventArgs args)
        {
            if (AxesList.Contains(sender.Name) && sender.Position.Distance(ObjectManager.Player.Position) /
                ObjectManager.Player.MoveSpeed <= 2)
            {
                AxeSpots.Add(new Axe(sender));
            }
            if (BuffList.Contains(sender.Name) && sender.Position.Distance(ObjectManager.Player.Position) < 100)
            {
                CurrentAxes += 1;
            }
        }

        public static bool CatchableAxes(Axe axe)
        {
            switch (Initializer.Config.Item("catch.logic", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    var sectorpoly = new Geometry.Polygon.Sector(
                        ObjectManager.Player.Position.To2D(),
                        Game.CursorPos.To2D(),
                        100 * (float)Math.PI / 180,
                        600).IsInside(axe.Object.Position);
                    return sectorpoly;
                default:
                    var circlepoly = new Geometry.Polygon.Circle(Game.CursorPos, Utilities.Slider("catch.radius"))
                        .IsInside(axe.Object.Position);
                    return circlepoly;
            }
        }
    }

    class Axe
    {
        public Axe(GameObject obj)
        {
            Object = obj;
            EndTick = Environment.TickCount + 1500;
        }

        public int EndTick;
        public GameObject Object;
    }
}

