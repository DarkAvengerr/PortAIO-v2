using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;
using Orbwalking = vSupport_Series.Core.Plugins.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class Nautilus : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
        
        public Nautilus()
        {
            NautilusOnLoad();
        }

        private static void NautilusOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 755);

            Q.SetSkillshot(0.25f, 90f, 2000f, true, SkillshotType.SkillshotLine);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("nautilus.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("nautilus.w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("nautilus.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("nautilus.r.combo", "Use R").SetValue(true));
                    
                    var rsettings = new Menu(":: R Settings", ":: R Settings");
                    {
                        rsettings.AddItem(new MenuItem("nautilus.r.count", "Min. Enemy Count").SetValue(new Slider(2, 1, 5)));
                        
                        comboMenu.AddSubMenu(rsettings);
                    }
                   
                    Config.AddSubMenu(comboMenu);
                }

                var whitelist = new Menu(":: (Q) Whitelist", ":: (Q) Whitelist");
                {
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        whitelist.AddItem(new MenuItem("nautilus.q."+enemy.ChampionName, "(Q): "+enemy.ChampionName).SetValue(true));
                    }
                    
                    Config.AddSubMenu(whitelist);
                }

                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("nautilus.q.harass", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("nautilus.e.combo", "Use E").SetValue(true));
                    
                    Config.AddSubMenu(harassMenu);
                }

                var drawing = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    drawing.AddItem(new MenuItem("nautilus.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    // drawing.AddItem(new MenuItem("nautilus.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("nautilus.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("nautilus.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));
                    Config.AddSubMenu(drawing);
                }
                
                SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
                
                Config.AddItem(new MenuItem("nautilus.interrupter", "Interrupter").SetValue(true)).SetTooltip("Only cast if enemy spell priorty > danger");
                Config.AddItem(new MenuItem("nautilus.q.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }
            
            Config.AddToMainMenu();
            Game.OnUpdate += NautilusOnUpdate;
            Interrupter2.OnInterruptableTarget += NautilusInterrupter;
            Drawing.OnDraw += NautilusOnDraw;
        }

        private static void NautilusInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && MenuCheck("nautilus.interrupter",Config) && sender.IsValidTarget(R.Range) && 
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                R.Cast(sender);
            }

            if (sender.IsEnemy && MenuCheck("nautilus.interrupter", Config) && sender.IsValidTarget(Q.Range) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                Q.Cast(sender.Position);
            }
        }

        private static void NautilusOnUpdate(EventArgs args)
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
            if (MenuCheck("nautilus.q.combo", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && 
                    MenuCheck("nautilus.q." + x.ChampionName, Config)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "nautilus.q.hitchance"));
                }
            }

            if (MenuCheck("nautilus.w.combo", Config) && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }
            }

            if (MenuCheck("nautilus.e.combo", Config) && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange)))
                {
                    E.Cast();
                }
            }
            if (MenuCheck("nautilus.r.combo", Config) && R.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)))
                {
                    if (ObjectManager.Player.CountEnemiesInRange(R.Range) >= SliderCheck("nautilus.r.count", Config))
                    {
                        R.Cast(enemy);
                    }
                    
                }
            }
        }

        private static void Harass()
        {
            if (MenuCheck("nautilus.q.harass", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && MenuCheck("nautilus.q." + x.ChampionName, Config)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "nautilus.q.hitchance"));
                }
            }

            if (MenuCheck("nautilus.e.harass", Config) && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range)))
                {
                    E.Cast(enemy);
                }
            }
        }

        private static void NautilusOnDraw(EventArgs args)
        {
            if (Q.IsReady() && ActiveCheck("nautilus.q.draw",Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range ,GetColor("nautilus.q.draw",Config));
            }

            if (E.IsReady() && ActiveCheck("nautilus.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("nautilus.e.draw", Config));
            }

            if (R.IsReady() && ActiveCheck("nautilus.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("nautilus.r.draw", Config));
            }
        }
    }
}
