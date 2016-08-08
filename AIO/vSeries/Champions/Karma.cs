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
    public class Karma : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        private static AIHeroClient Player = ObjectManager.Player;

        public Karma()
        {
            KarmaOnLoad();
        }

        public static void KarmaOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 800f);
            R = new Spell(SpellSlot.R);

            E.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            W.SetTargetted(0.25f, 2200f);
            E.SetTargetted(0.25f, float.MaxValue);

            Config = new Menu("vSupport Series:  " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu(":: Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("karma.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("karma.w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("karma.r.combo", "Use R").SetValue(true));
                    var comborsettings = new Menu(":: R Settings", ":: R Settings");
                    {
                        comborsettings.AddItem(new MenuItem("combo.r.q", "Empower Q?").SetValue(true));
                        comborsettings.AddItem(new MenuItem("combo.r.w", "Empower W?").SetValue(true));
                        comborsettings.AddItem(new MenuItem("combo.r.w.health", "Min. Health to Empower W").SetValue(new Slider(40, 1, 99)));

                        comboMenu.AddSubMenu(comborsettings);
                    }

                    Config.AddSubMenu(comboMenu);
                }

                var healManager = new Menu("(E) Shield Settings", "(E) Shield Settings");
                {
                    healManager.AddItem(new MenuItem("karma.shield.disable", "Disable shielding?").SetValue(false));
                    healManager.AddItem(new MenuItem("karma.shield.limit", "Min. Karma HP Percent for Shield").SetValue(new Slider(40, 1, 99)));
                    healManager.AddItem(new MenuItem("blabla", "(W) Shield Settings"));

                    var wsettings = new Menu("(E) Priority", "(E) Priority").SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold);
                    {
                        foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
                        {
                            if (LowPriority.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("karma.shield" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("karma.shield.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(15, 1, 99)));
                            }
                            if (MediumPriority.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("karma.shield" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("karma.shield.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(20, 1, 99)));
                            }
                            if (HighChamps.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("karma.shield" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("karma.shield.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(30, 1, 99)));
                            }
                        }
                        healManager.AddSubMenu(wsettings);
                    }
                    Config.AddSubMenu(healManager);
                }

                var harass = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    harass.AddItem(new MenuItem("karma.q.harass", "Use Q").SetValue(true));
                    harass.AddItem(new MenuItem("karma.w.harass", "Use W").SetValue(true));
                    var harassrsettings = new Menu(":: R Settings", ":: R Settings");
                    {
                        harassrsettings.AddItem(new MenuItem("karma.rq.harass", "Empower Q?").SetValue(true));
                        harassrsettings.AddItem(new MenuItem("karma.rw.harass", "Empower W?").SetValue(true));
                        harassrsettings.AddItem(new MenuItem("karma.rw.health", "Min. Health to Empower W").SetValue(new Slider(40, 1, 99)));

                        harass.AddSubMenu(harassrsettings);
                    }
                    harass.AddItem(new MenuItem("karma.harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));

                    Config.AddSubMenu(harass);
                }

                var misc = new Menu(":: Miscellaneous", ":: Miscellaneous");
                {
                    misc.AddItem(new MenuItem("karma.anti.q", "Gapcloser (Q)").SetValue(true));
                    misc.AddItem(new MenuItem("karma.anti.e", "Gapcloser (E)").SetValue(true));

                    Config.AddSubMenu(misc);
                }

                var drawing = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    drawing.AddItem(new MenuItem("karma.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("karma.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("karma.e.draw", "E Range").SetValue(new Circle(true, Color.White)));

                    Config.AddSubMenu(drawing);
                }

                Config.AddItem(new MenuItem("karma.q.hitchance", ":: Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }

            SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
            Config.AddToMainMenu();
            Game.OnUpdate += KarmaOnUpdate;
            Drawing.OnDraw += KarmaOnDraw;
            AntiGapcloser.OnEnemyGapcloser += KarmaOnEnemyGapcloser;
        }

        private static void KarmaOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.LSIsReady() && MenuCheck("karma.anti.q", Config) && gapcloser.Sender.LSIsValidTarget(Q.Range))
            {
                Q.Cast(gapcloser.Sender);
            }
            else if (E.LSIsReady() && MenuCheck("karma.anti.e", Config) && gapcloser.Sender.LSIsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private static void KarmaOnUpdate(EventArgs args)
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
            
            EManager();
        }

        private static void EManager()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && !x.IsDead
                && x.LSDistance(ObjectManager.Player.Position) < E.Range))
            {
                if (MenuCheck("karma.shield" + ally.ChampionName, Config) && ally.HealthPercent < SliderCheck("karma.ally.percent" + ally.ChampionName, Config)
                    && ObjectManager.Player.HealthPercent > SliderCheck("karma.ally.limit", Config))
                {
                    E.Cast(ally);
                }
            }
        }

        private static void Combo()
        {
            if (MenuCheck("karma.w.combo", Config) && W.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(W.Range)))
                {
                    if (MenuCheck("combo.r.w", Config) && R.LSIsReady())
                    {
                        if (ObjectManager.Player.HealthPercent <= SliderCheck("combo.r.w.health", Config))
                        {
                            R.Cast();
                            W.Cast(enemy);
                        }
                    }

                    if (!MenuCheck("combo.r.w", Config) || !R.LSIsReady())
                    {
                        W.Cast(enemy);
                    }
                }
            }

            if (MenuCheck("karma.q.combo", Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
                {
                    if (MenuCheck("combo.r.q", Config) && R.LSIsReady())
                    {
                        R.Cast();
                        Q.SPredictionCast(enemy, SpellHitChance(Config, "karma.q.hitchance"));
                    }

                    if (!MenuCheck("combo.r.q", Config) || !R.LSIsReady())
                    {
                        Q.SPredictionCast(enemy, SpellHitChance(Config, "karma.q.hitchance"));
                    }
                }
            }
        }

        private static void Harass()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(W.Range)))
            {
                if (MenuCheck("karma.rw.harass", Config) && W.LSIsReady() && R.LSIsReady())
                {
                    if (ObjectManager.Player.HealthPercent <= SliderCheck("harass.rw.health", Config))
                    {
                        R.Cast();
                        W.Cast(enemy);
                    }
                }

                if (!MenuCheck("karma.rw.harass", Config) || !R.LSIsReady())
                {
                    W.Cast(enemy);
                }
            }

            foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
            {
                if (MenuCheck("karma.rq.harass", Config) && Q.LSIsReady() && R.LSIsReady())
                {
                    R.Cast();
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "karma.q.hitchance"));
                }

                if (MenuCheck("karma.q.harass", Config) && Q.LSIsReady())
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "karma.q.hitchance"));
                }

            }
        }

        private static void KarmaOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("karma.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("karma.q.draw", Config));
            }

            if (W.LSIsReady() && ActiveCheck("karma.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("karma.w.draw", Config));
            }

            if (E.LSIsReady() && ActiveCheck("karma.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("karma.e.draw", Config));
            }
        }
    }
}
