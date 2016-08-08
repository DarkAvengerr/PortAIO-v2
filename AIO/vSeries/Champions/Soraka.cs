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
    public class Soraka : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        public Soraka()
        {
            SorakaOnLoad();
        }

        public static void SorakaOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 550f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1000f);

            E.SetSkillshot(0.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("soraka.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("soraka.e.combo", "Use E").SetValue(true));
                    Config.AddSubMenu(comboMenu);
                }
                var healManager = new Menu("(W) Heal Settings", "(W) Heal Settings");
                {
                    healManager.AddItem(new MenuItem("soraka.heal.disable", "Disable Heal?").SetValue(false));
                    healManager.AddItem(new MenuItem("soraka.heal.limit", "Min. Soraka HP Percent for Heal").SetValue(new Slider(20, 1, 99)));
                    healManager.AddItem(new MenuItem("ayrac1", "                  (W) Heal Settings"));
                    foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
                    {
                        if (LowPriority.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("soraka.heal." + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("soraka.heal.percent." + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(10, 1, 99)));
                        }
                        if (MediumPriority.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("soraka.heal." + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("soraka.heal.percent." + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(20, 1, 99)));
                        }
                        if (HighChamps.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("soraka.heal." + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("soraka.heal.percent." + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(30, 1, 99)));
                        }
                    }
                    Config.AddSubMenu(healManager);
                }
                var rsettings = new Menu("(R) Heal Settings", "(R) Heal Settings");
                {
                    rsettings.AddItem(new MenuItem("soraka.r.disable", "Disable Heal?").SetValue(false));
                    rsettings.AddItem(new MenuItem("ayracx", "                  (R) Heal Settings"));
                    foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
                    {
                        if (LowPriority.Contains(ally.ChampionName))
                        {
                            rsettings.AddItem(new MenuItem("soraka.r." + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            rsettings.AddItem(new MenuItem("soraka.r.percent." + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(10, 1, 99)));
                        }
                        if (MediumPriority.Contains(ally.ChampionName))
                        {
                            rsettings.AddItem(new MenuItem("soraka.r." + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            rsettings.AddItem(new MenuItem("soraka.r.percent." + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(20, 1, 99)));
                        }
                        if (HighChamps.Contains(ally.ChampionName))
                        {
                            rsettings.AddItem(new MenuItem("soraka.r." + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            rsettings.AddItem(new MenuItem("soraka.r.percent." + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(30, 1, 99)));
                        }
                    }
                    Config.AddSubMenu(rsettings);
                }

                var harass = new Menu("Harass Settings", "Harass Settings");
                {
                    harass.AddItem(new MenuItem("soraka.q.harass", "Use Q").SetValue(true));
                    harass.AddItem(new MenuItem("soraka.harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(harass);
                }
                var misc = new Menu("Miscellaneous", "Miscellaneous");
                {
                    misc.AddItem(new MenuItem("soraka.anti", "Gapcloser (E)").SetValue(true));
                    misc.AddItem(new MenuItem("soraka.inter", "Interrupt (E)").SetValue(true));
                    Config.AddSubMenu(misc);
                }
                var drawing = new Menu("Draw Settings", "Draw Settings");
                {
                    drawing.AddItem(new MenuItem("soraka.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("soraka.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("soraka.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("soraka.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));
                    Config.AddSubMenu(drawing);
                }
                Config.AddItem(new MenuItem("soraka.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }

            SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
            Config.AddToMainMenu();
            Game.OnUpdate += SorakaOnUpdate;
            Drawing.OnDraw += SorakaOnDraw;
            AntiGapcloser.OnEnemyGapcloser += SorakaOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += SorakaOnInterruptableTarget;
        }

        private static void SorakaOnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.LSIsReady() && sender.LSIsValidTarget(1000) && MenuCheck("soraka.inter",Config))
            {
                E.CastIfHitchanceEquals(sender, HitChance.High);
            }
        }

        private static void SorakaOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.LSIsReady() && MenuCheck("soraka.anti",Config) && E.GetPrediction(gapcloser.Sender).Hitchance > HitChance.High
                && gapcloser.Sender.LSIsValidTarget(1000))
            {
                E.Cast(gapcloser.End);
            }
        }



        private static void SorakaOnUpdate(EventArgs args)
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
            if (!MenuCheck("soraka.heal.disable",Config) && W.LSIsReady())
            {
                WManager();
            }
            if (!MenuCheck("soraka.r.disable",Config) && R.LSIsReady())
            {
                RManager();
            }
        }

        private static void WManager()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            if (ObjectManager.Player.LSInFountain() || ObjectManager.Player.LSIsRecalling())
            {
                return;
            }

            foreach (var heal in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe))
            {
                if (MenuCheck("soraka.heal." + heal.ChampionName,Config) && heal.HealthPercent < SliderCheck("soraka.heal.percent." + heal.ChampionName,Config)
                    && ObjectManager.Player.HealthPercent > SliderCheck("soraka.heal.limit",Config))
                {
                    W.CastOnUnit(heal);
                }
            }
        }

        private static void RManager()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }
            foreach (var heal in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe))
            {
                if (MenuCheck("soraka.r." + heal.ChampionName,Config) && heal.HealthPercent < SliderCheck("soraka.r.percent." + heal.ChampionName,Config))
                {
                    R.CastOnUnit(heal);
                }
            }
        }


        private static void Combo()
        {
            if (MenuCheck("soraka.q.combo",Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "soraka.hitchance"));
                }
            }
            if (MenuCheck("soraka.e.combo", Config) && E.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    E.SPredictionCast(enemy, SpellHitChance(Config, "soraka.hitchance"));
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < SliderCheck("soraka.harass.mana",Config))
            {
                return;
            }

            if (MenuCheck("soraka.q.harass",Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "soraka.hitchance"));
                }
            }
        }

     

        private static void SorakaOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("soraka.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("soraka.q.draw", Config));
            }
            if (W.LSIsReady() && ActiveCheck("soraka.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("soraka.w.draw", Config));
            }
            if (E.LSIsReady() && ActiveCheck("soraka.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("soraka.e.draw", Config));
            }
            if (R.LSIsReady() && ActiveCheck("soraka.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("soraka.r.draw", Config));
            }
        }
    }
}
