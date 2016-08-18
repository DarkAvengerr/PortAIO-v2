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
    public class Thresh : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
        
        public Thresh()
        {
            ThreshOnLoad();
        }

        private static void ThreshOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 1100f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 400f);
            R = new Spell(SpellSlot.R, 450f);

            Q.SetSkillshot(0.500f, 70f, 1900f, true, SkillshotType.SkillshotLine);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("thresh.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("thresh.w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("thresh.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("thresh.r.combo", "Use R").SetValue(true));
                    
                    var rsettings = new Menu(":: R Settings", ":: R Settings");
                    {
                        rsettings.AddItem(new MenuItem("thresh.r.count", "Min. Enemy Count").SetValue(new Slider(2, 1, 5)));
                        
                        comboMenu.AddSubMenu(rsettings);
                    }
                   
                    Config.AddSubMenu(comboMenu);
                }

                var whitelist = new Menu(":: (Q) Whitelist", ":: (Q) Whitelist");
                {
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        whitelist.AddItem(new MenuItem("thresh.q."+enemy.ChampionName, "(Q): "+enemy.ChampionName).SetValue(true));
                    }
                    
                    Config.AddSubMenu(whitelist);
                }

                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("thresh.q.harass", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("thresh.e.combo", "Use E").SetValue(true));
                    
                    Config.AddSubMenu(harassMenu);
                }

                var lanternMenu = new Menu(":: Auto Lantern Settings", ":: Auto Lantern Settings");
                {
                    lanternMenu.AddItem(new MenuItem("thresh.auto.w", "Auto W Logic").SetValue(true));
                    lanternMenu.AddItem(new MenuItem("blabla", "(W) Lantern Settings"));

                    foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
                    {
                        if (LowPriority.Contains(ally.ChampionName))
                        {
                            lanternMenu.AddItem(new MenuItem("thresh.lantern" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            lanternMenu.AddItem(new MenuItem("thresh.lantern.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(15, 1, 99)));
                        }
                        if (MediumPriority.Contains(ally.ChampionName))
                        {
                            lanternMenu.AddItem(new MenuItem("thresh.lantern" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            lanternMenu.AddItem(new MenuItem("thresh.lantern.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(20, 1, 99)));
                        }
                        if (HighChamps.Contains(ally.ChampionName))
                        {
                            lanternMenu.AddItem(new MenuItem("thresh.lantern" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            lanternMenu.AddItem(new MenuItem("thresh.lantern.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(30, 1, 99)));
                        }

                        Config.AddSubMenu(lanternMenu);
                    }
                }

                var drawing = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    drawing.AddItem(new MenuItem("thresh.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("thresh.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("thresh.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("thresh.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));
                    Config.AddSubMenu(drawing);
                }
                
                SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
                
                Config.AddItem(new MenuItem("thresh.interrupter", "Interrupter").SetValue(true)).SetTooltip("Only cast if enemy spell priorty > danger");
                Config.AddItem(new MenuItem("thresh.q.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }
            
            Config.AddToMainMenu();
            Game.OnUpdate += ThreshOnUpdate;
            Interrupter2.OnInterruptableTarget += ThreshInterrupter;
            Drawing.OnDraw += ThreshOnDraw;
        }

        private static void ThreshInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && MenuCheck("thresh.interrupter", Config) && sender.IsValidTarget(Q.Range) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                Q.Cast(sender.Position);
            }
        }

        private static void ThreshOnUpdate(EventArgs args)
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

            if (MenuCheck("thresh.auto.w", Config))
            {
                AutoLantern();
            }
        }

        private static void Combo()
        {
            if (MenuCheck("thresh.q.combo",Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && MenuCheck("thresh.q."+x.ChampionName,Config)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "thresh.q.hitchance"));
                }
            }

            if (MenuCheck("thresh.w.combo", Config) && W.IsReady())
            {
                foreach (var ally in HeroManager.Allies.Where(x => x.IsValidTarget(W.Range) && HighChamps.Contains(x.ChampionName)))
                {
                    W.Cast(ally);
                }
            }

            if (MenuCheck("thresh.e.combo", Config) && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(E.Range)))
                {
                    E.Cast(enemy);
                }
            }

            if (MenuCheck("thresh.r.combo", Config) && R.IsReady() && 
                ObjectManager.Player.CountEnemiesInRange(R.Range) >= SliderCheck("thresh.r.count",Config))
            {
                R.Cast();
            }
        }

        private static void Harass()
        {
            if (MenuCheck("thresh.q.harass", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && MenuCheck("thresh.q." + x.ChampionName, Config)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "thresh.q.hitchance"));
                }
            }

            if (MenuCheck("thresh.e.harass", Config) && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range)))
                {
                    E.Cast(enemy);
                }
            }
        }

        private static void AutoLantern()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            foreach (var lantern in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe))
            {
                if (MenuCheck("thresh.lantern" + lantern.ChampionName, Config) && lantern.HealthPercent < SliderCheck("thresh.lantern.percent" + lantern.ChampionName, Config)
                    && ObjectManager.Player.HealthPercent > SliderCheck("thresh.lantern.limit", Config) && HighChamps.Contains(lantern.ChampionName))
                {
                    W.Cast(lantern);
                }
            }
        }

        private static void ThreshOnDraw(EventArgs args)
        {
            if (Q.IsReady() && ActiveCheck("thresh.q.draw",Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range ,GetColor("thresh.q.draw",Config));
            }
            if (W.IsReady() && ActiveCheck("thresh.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("thresh.w.draw", Config));
            }
            if (E.IsReady() && ActiveCheck("thresh.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("thresh.e.draw", Config));
            }
            if (R.IsReady() && ActiveCheck("thresh.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("thresh.r.draw", Config));
            }
        }
    }
}
