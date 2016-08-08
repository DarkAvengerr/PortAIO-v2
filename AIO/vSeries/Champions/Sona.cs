using System;
using System.Drawing;
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
    public class Sona : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        public static AIHeroClient Player = ObjectManager.Player;

        public Sona()
        {
            SonaOnLoad();
        }

        public static void SonaOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 850f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 350f);
            R = new Spell(SpellSlot.R, 1000f);

            R.SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.SkillshotLine);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("sona.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("sona.r.combo", "Use R").SetValue(true));

                    Config.AddSubMenu(comboMenu);
                }

                var healManager = new Menu("(W) Heal Settings", "(W) Heal Settings");
                {
                    healManager.AddItem(new MenuItem("sona.heal.disable", "Disable healing?").SetValue(false));
                    healManager.AddItem(new MenuItem("sona.heal.limit", "Min. sona HP Percent for Heal").SetValue(new Slider(40, 1, 99)));
                    healManager.AddItem(new MenuItem("blabla", "(W) heal Settings"));

                    var wsettings = new Menu("(W) Priority", "(W) Priority").SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold);
                    {
                        foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
                        {
                            if (LowPriority.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("sona.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("sona.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(15, 1, 99)));
                            }
                            if (MediumPriority.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("sona.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("sona.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(20, 1, 99)));
                            }
                            if (HighChamps.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("sona.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("sona.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(30, 1, 99)));
                            }
                        }
                        healManager.AddSubMenu(wsettings);
                    }
                    Config.AddSubMenu(healManager);
                    
                }

                var rSettings = new Menu("(R) Ult Settings", "(R) Ult Settings");
                {
                    rSettings.AddItem(new MenuItem("sona.r.killsteal", "Killsteal using R").SetValue(false));

                    Config.AddSubMenu(rSettings);
                }

                var harass = new Menu("Harass Settings", "Harass Settings");
                {
                    harass.AddItem(new MenuItem("sona.q.harass", "Use Q").SetValue(true));
                    harass.AddItem(new MenuItem("sona.harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));

                    Config.AddSubMenu(harass);
                }

                var misc = new Menu("Miscellaneous", "Miscellaneous");
                {
                    misc.AddItem(new MenuItem("sona.anti", "Gapcloser (Q)").SetValue(true));
                    misc.AddItem(new MenuItem("sona.inter", "Interrupt (R)").SetValue(true));

                    Config.AddSubMenu(misc);
                }

                var drawing = new Menu("Draw Settings", "Draw Settings");
                {
                    drawing.AddItem(new MenuItem("sona.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("sona.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("sona.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("sona.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));

                    Config.AddSubMenu(drawing);
                }

                Config.AddItem(new MenuItem("sona.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }

            SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
            Config.AddToMainMenu();
            Game.OnUpdate += SonaOnUpdate;
            Drawing.OnDraw += SonaOnDraw;
            AntiGapcloser.OnEnemyGapcloser += SonaOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += SonaOnInterruptableTarget;
        }

        private static void SonaOnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.LSIsReady() && sender.LSIsValidTarget(R.Range) && MenuCheck("sona.inter", Config))
            {
                R.CastIfHitchanceEquals(sender, HitChance.High);
            }
        }

        private static void SonaOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.LSIsReady() && MenuCheck("sona.anti", Config) && R.GetPrediction(gapcloser.Sender).Hitchance > HitChance.High
                && gapcloser.Sender.LSIsValidTarget(1000))
            {
                R.Cast(gapcloser.Sender);
            }
        }

        private static void SonaOnUpdate(EventArgs args)
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

            if (!MenuCheck("sona.heal.disable", Config) && W.LSIsReady())
            {
                WManager();
            }

            if (MenuCheck("sona.r.killsteal", Config) && R.LSIsReady())
            {
                Killsteal();
            }
        }

        private static void WManager()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            if (ObjectManager.Player.LSIsRecalling() || ObjectManager.Player.LSInFountain())
            {
                return;
            }

            foreach (var shield in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && !x.IsDead 
                && x.LSDistance(ObjectManager.Player.Position) < W.Range))
            {
                if (MenuCheck("sona.heal" + shield.ChampionName, Config) && shield.HealthPercent < SliderCheck("sona.heal.percent" + shield.ChampionName, Config)
                    && ObjectManager.Player.HealthPercent > SliderCheck("sona.heal.limit", Config))
                {
                    W.Cast(shield);
                }
            }
        }

        private static void Killsteal()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            if (MenuCheck("sona.r.killsteal", Config) && R.LSIsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (R.GetDamage(target) > target.Health)
                    {
                        R.SPredictionCast(target, SpellHitChance(Config, "sona.hitchance"));
                    }
                }
            }
        }

        private static void Combo()
        {
            if (MenuCheck("sona.q.combo", Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    Q.Cast(enemy);
                }
            }

            if (MenuCheck("sona.r.combo", Config) && R.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    R.SPredictionCast(enemy, SpellHitChance(Config, "sona.hitchance"));
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < SliderCheck("sona.harass.mana", Config))
            {
                return;
            }

            if (MenuCheck("sona.q.harass", Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsDead 
                    && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    Q.Cast(enemy);
                }
            }
        }

        private static void SonaOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("sona.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("sona.q.draw", Config));
            }

            if (W.LSIsReady() && ActiveCheck("sona.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("sona.w.draw", Config));
            }

            if (E.LSIsReady() && ActiveCheck("sona.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("sona.e.draw", Config));
            }

            if (R.LSIsReady() && ActiveCheck("sona.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("sona.r.draw", Config));
            }
        }
    }
}
