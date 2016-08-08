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
    public class Blitzcrank : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
        
        public Blitzcrank()
        {
            BlitzcrankOnLoad();
        }

        private static void BlitzcrankOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 155f);
            R = new Spell(SpellSlot.R, 545f);

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("blitzcrank.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("blitzcrank.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("blitzcrank.r.combo", "Use R").SetValue(true));
                    var rsettings = new Menu(":: R Settings", ":: R Settings");
                    {
                        rsettings.AddItem(new MenuItem("blitz.r.count", "Min. Enemy Count").SetValue(new Slider(3, 1, 5)));
                        comboMenu.AddSubMenu(rsettings);
                    }
                    Config.AddSubMenu(comboMenu);
                }
                var whitelist = new Menu(":: (Q) Whitelist", ":: (Q) Whitelist");
                {
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        whitelist.AddItem(new MenuItem("blitzcrank.q."+enemy.ChampionName, "(Q): "+enemy.ChampionName).SetValue(true));
                    }
                    Config.AddSubMenu(whitelist);
                }
                var drawing = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    drawing.AddItem(new MenuItem("blitzcrank.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("blitzcrank.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("blitzcrank.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("blitzcrank.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));
                    Config.AddSubMenu(drawing);
                }
                SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
                Config.AddItem(new MenuItem("blitzcrank.interrupter", "Interrupter").SetValue(true)).SetTooltip("Only cast if enemy spell priorty > danger");
                Config.AddItem(new MenuItem("blitzcrank.q.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }
            Config.AddToMainMenu();
            Game.OnUpdate += BlitzcrankOnUpdate;
            Interrupter2.OnInterruptableTarget += BlitzcrankInterrupter;
            Drawing.OnDraw += BlitzcrankOnDraw;
        }

        private static void BlitzcrankInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && MenuCheck("blitzcrank.interrupter",Config) && sender.LSIsValidTarget(R.Range) && 
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                R.Cast();
            }
            if (sender.IsEnemy && MenuCheck("blitzcrank.interrupter", Config) && sender.LSIsValidTarget(Q.Range) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                Q.Cast(sender.Position);
            }
        }

        private static void BlitzcrankOnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
        }

        private static void Combo()
        {
            if (MenuCheck("blitzcrank.q.combo",Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && MenuCheck("blitzcrank.q."+x.ChampionName,Config)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "blitzcrank.q.hitchance"));
                }
            }

            if (MenuCheck("blitzcrank.e.combo", Config) && E.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.LSIsValidTarget(E.Range)))
                {
                    E.Cast(enemy);
                }
            }

            if (MenuCheck("blitzcrank.r.combo", Config) && R.LSIsReady() && 
                ObjectManager.Player.LSCountEnemiesInRange(R.Range) >= SliderCheck("blitz.r.count",Config))
            {
                R.Cast();
            }
        }
        private static void BlitzcrankOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("blitzcrank.q.draw",Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range ,GetColor("blitzcrank.q.draw",Config));
            }
            if (W.LSIsReady() && ActiveCheck("blitzcrank.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("blitzcrank.w.draw", Config));
            }
            if (E.LSIsReady() && ActiveCheck("blitzcrank.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("blitzcrank.e.draw", Config));
            }
            if (R.LSIsReady() && ActiveCheck("blitzcrank.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("blitzcrank.r.draw", Config));
            }
        }
    }
}
