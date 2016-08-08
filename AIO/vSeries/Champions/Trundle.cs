using System;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using vSupport_Series.Core.Database;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;
using Orbwalking = vSupport_Series.Core.Plugins.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class Trundle : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        public Trundle()
        {
            TrundleOnLoad();
        }

        public static void TrundleOnLoad()
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
                    comboMenu.AddItem(new MenuItem("trundle.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("trundle.w.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("trundle.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("trundle.r.combo", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("trundle.r.slider", "If Trundle health > slider amount, cast (R)")).SetValue(new Slider(50, 1, 99));
                    Config.AddSubMenu(comboMenu);
                    var rwhitelist = new Menu(":: (R) Whitelist", ":: (R) Whitelist");
                    {
                        foreach (var enemy in HeroManager.Enemies)
                        {
                            rwhitelist.AddItem(new MenuItem("trundle.r." + enemy.ChampionName, "(R): " + enemy.ChampionName).SetValue(true));
                        }
                        comboMenu.AddSubMenu(rwhitelist);
                    }
                }

                var whitelist = new Menu(":: (R) Whitelist", ":: (R) Whitelist");
                {
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        whitelist.AddItem(new MenuItem("trundle.q." + enemy.ChampionName, "(Q): " + enemy.ChampionName).SetValue(true));
                    }
                    Config.AddSubMenu(whitelist);
                }

                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("trundle.q.harass", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("trundle.w.harass", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("trundle.e.harass", "Use E").SetValue(true));
                    harassMenu.AddItem(new MenuItem("trundle.harass.mana", "Min. Mana")).SetValue(new Slider(50, 1, 99));
                    Config.AddSubMenu(harassMenu);
                }

                var trickMenu = new Menu(":: Trick Settings", ":: Trick Settings");
                {
                    foreach (var spell in HeroManager.Allies.SelectMany(ally => SpellDatabase.TrundleSpells.Where(p => p.ChampionName == ally.ChampionName)))
                    {
                        trickMenu.AddItem(new MenuItem(string.Format("trick.{0}", spell.SpellName), string.Format("Trick: {0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                    }
                    Config.AddSubMenu(trickMenu);
                }

                var drawing = new Menu("Draw Settings", "Draw Settings");
                {
                    drawing.AddItem(new MenuItem("trundle.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("trundle.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("trundle.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("trundle.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));
                    Config.AddSubMenu(drawing);
                }
                Config.AddItem(new MenuItem("trundle.pillar.block", "Use (E) for Blitzcrank Q").SetValue(true));
                Config.AddItem(new MenuItem("trundle.interrupter", "Interrupter").SetValue(true)).SetTooltip("Only cast if enemy spell priorty > danger");
            }
            Config.AddToMainMenu();
            Game.OnUpdate += TrundleOnUpdate;
            Drawing.OnDraw += TrundleOnDraw;
            Interrupter2.OnInterruptableTarget += TrundleInterrupter;
            Obj_AI_Base.OnProcessSpellCast += TrundleOnProcessSpellCast;
        }

        private static void TrundleInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && MenuCheck("trundle.interrupter", Config) && sender.LSIsValidTarget(E.Range) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                E.Cast(sender.Position);
            }
        }

        private static void TrundleOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient && sender is AIHeroClient &&
                ((Config.Item("trick." + args.SData.Name).GetValue<bool>() && Config.Item("trick." + args.SData.Name) != null)))
            {
                if (args.Target.IsMe)
                {
                    if (args.SData.Name == "CaitlynEntrapment" && args.End.LSDistance(ObjectManager.Player.Position) < E.Range - 50
                        && sender.LSDistance(ObjectManager.Player.Position) < E.Range - 50)
                    {
                        E.Cast(args.End.LSExtend(ObjectManager.Player.Position, -50));
                    }
                    else if (args.SData.Name == "BlindMonkQOne" && ObjectManager.Player.LSHasBuff("BlindMonkQOne") && sender.LSHasBuff("lee sin q fly buff. just i need"))
                    {
                        E.Cast(ObjectManager.Player.Position.LSExtend(sender.Position, 100));
                    }
                }

                if (args.SData.Name == "RocketJump" && sender.LSDistance(ObjectManager.Player.Position) < E.Range - 50)
                {
                    E.Cast(ObjectManager.Player.Position.LSExtend(args.End, 50));
                }

            }
        }

        private static void TrundleOnUpdate(EventArgs args)
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


        private static void Combo()
        {
            if (Q.LSIsReady() && MenuCheck("trundle.q.combo", Config))
            {
                // ReSharper disable once UnusedVariable
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
                {
                    Q.Cast();
                }
            }

            if (W.LSIsReady() && MenuCheck("trundle.w.combo", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(W.Range)))
                {
                    W.Cast(enemy);
                }
            }

            if (E.LSIsReady() && MenuCheck("trundle.e.combo", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range)))
                {
                    var pred = E.GetPrediction(enemy);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        E.Cast(PillarPos(enemy));
                    }
                }
            }

            if (R.LSIsReady() && MenuCheck("trundle.r.combo", Config) && SliderCheck("trundle.r.slider", Config) > ObjectManager.Player.Health)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && MenuCheck("trundle.r." + x.ChampionName, Config)))
                {
                    R.Cast(enemy);
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent <= SliderCheck("trundle.harass.mana", Config))
            {
                return;
            }
            if (Q.LSIsReady() && MenuCheck("trundle.q.harass", Config))
            {
                // ReSharper disable once UnusedVariable
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
                {
                    Q.Cast();
                }
            }

            if (W.LSIsReady() && MenuCheck("trundle.w.harass", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(W.Range)))
                {
                    W.Cast(enemy);
                }
            }

            if (E.LSIsReady() && MenuCheck("trundle.e.harass", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range)))
                {
                    E.Cast(PillarPos(enemy));
                }
            }
        }

        private static Vector3 PillarPos(AIHeroClient enemy)
        {
            return enemy.Position.LSTo2D().LSExtend(ObjectManager.Player.Position.LSTo2D(), -E.Width / 2).To3D();
        }

        private static void TrundleOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("trundle.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("trundle.q.draw", Config));
            }
            if (W.LSIsReady() && ActiveCheck("trundle.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("trundle.w.draw", Config));
            }
            if (E.LSIsReady() && ActiveCheck("trundle.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("trundle.e.draw", Config));
            }
            if (R.LSIsReady() && ActiveCheck("trundle.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("trundle.r.draw", Config));
            }
            foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(1000)))
            {
                Render.Circle.DrawCircle(PillarPos(enemy), 50, Color.Gold);
            }
        }
    }
}