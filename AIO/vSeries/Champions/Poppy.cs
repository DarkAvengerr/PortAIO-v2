using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;
using Orbwalking = vSupport_Series.Core.Plugins.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class Poppy : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        public Poppy()
        {
            PoppyOnLoad();
        }

        public static void PoppyOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 550f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1000f);

            E.SetSkillshot(0.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("poppy.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("poppy.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("poppy.r.combo", "Use R").SetValue(true));
                    Config.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("poppy.q.harass", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("poppy.e.harass", "Use E").SetValue(true));
                    harassMenu.AddItem(new MenuItem("poppy.harass.mana", "Min. Mana")).SetValue(new Slider(50,1,99));
                    Config.AddSubMenu(harassMenu);
                }

                var drawing = new Menu("Draw Settings", "Draw Settings");
                {
                    drawing.AddItem(new MenuItem("poppy.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("poppy.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("poppy.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("poppy.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));
                    Config.AddSubMenu(drawing);
                }

                Config.AddItem(new MenuItem("poppy.interrupter", "Interrupter").SetValue(true)).SetTooltip("Only cast if enemy spell priorty > danger");
                Config.AddItem(new MenuItem("poppy.gapcloser", "Gapcloser").SetValue(true));
            }

            Config.AddToMainMenu();
            Game.OnUpdate += PoppyOnUpdate;
            Drawing.OnDraw += PoppyOnDraw;
            Interrupter2.OnInterruptableTarget += PoppyInterrupter;
            AntiGapcloser.OnEnemyGapcloser += PoppyOnEnemyGapcloser;
        }

        private static void PoppyInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && MenuCheck("poppy.interrupter", Config) && sender.LSIsValidTarget(E.Range) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                E.Cast(sender);
            }
            else if (sender.IsEnemy && MenuCheck("poppy.interrupter", Config) && sender.LSIsValidTarget(125) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High && !E.LSIsReady())
            {
                R.Cast(sender.Position);
            }
        }

        private static void PoppyOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.LSIsReady() && MenuCheck("poppy.gapcloser", Config) && gapcloser.Sender.LSIsValidTarget(W.Range))
            {
                W.Cast();
            }
        }

        private static void PoppyOnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
        }

        public static bool CheckWalls(Obj_AI_Base p, Obj_AI_Base e)
        {
            var d = ObjectManager.Player.Position.LSDistance(e.Position);
            for (int i = 1; i < 6; i++)
            {
                if (ObjectManager.Player.Position.LSExtend(e.Position, d + 300 * i).LSIsWall())
                {
                    return true;
                }
            }
            return false;
        }

        private static void Combo()
        {
            if (E.LSIsReady() && MenuCheck("poppy.e.combo", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range)))
                {
                    var targetPosition = E.GetPrediction(enemy).UnitPosition;
                    var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).LSNormalized();
                    float checkDistance = 300 / 6;
                    
                    for (int i = 1; i < 6; i++)
                    {
                        Vector3 finalPos = targetPosition + (pushDirection * checkDistance * i);
                        var collFlags = NavMesh.GetCollisionFlags(finalPos);
                        var wall = CollisionFlags.Wall; var building = CollisionFlags.Building;

                        if (collFlags.HasFlag(wall) || collFlags.HasFlag(building))
                        {
                            E.Cast(enemy);
                        }
                    }
                }
            }

            if (Q.LSIsReady() && MenuCheck("poppy.q.combo", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
                {
                    Q.Cast(enemy);
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent <= SliderCheck("poppy.harass.mana",Config))
            {
                return;
            }

            if (Q.LSIsReady() && MenuCheck("poppy.q.harass", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
                {
                    Q.Cast(enemy);
                }
            }

            if (E.LSIsReady() && MenuCheck("poppy.e.harass", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range)))
                {
                    var targetPosition = E.GetPrediction(enemy).UnitPosition;
                    var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).LSNormalized();
                    float checkDistance = 300 / 6;
                    
                    for (int i = 1; i < 6; i++)
                    {
                        Vector3 finalPos = targetPosition + (pushDirection * checkDistance * i);
                        var collFlags = NavMesh.GetCollisionFlags(finalPos);
                        var wall = CollisionFlags.Wall; var building = CollisionFlags.Building;

                        if (collFlags.HasFlag(wall) || collFlags.HasFlag(building))
                        {
                            E.Cast(enemy);
                        }
                    }
                }
            }
        }
        private static void PoppyOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("poppy.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("poppy.q.draw", Config));
            }

            if (W.LSIsReady() && ActiveCheck("poppy.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("poppy.w.draw", Config));
            }

            if (E.LSIsReady() && ActiveCheck("poppy.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("poppy.e.draw", Config));
            }

            if (R.LSIsReady() && ActiveCheck("poppy.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("poppy.r.draw", Config));
            }
        }
    }
}
