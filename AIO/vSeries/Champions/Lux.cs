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
    public class Lux : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
        public static GameObject LuxE;
        public static AIHeroClient Player = ObjectManager.Player;

        public Lux()
        {
            LuxOnLoad();
        }

        public static void LuxOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 1175f);
            W = new Spell(SpellSlot.W, 1075f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 3340f);

            Q.SetSkillshot(0.25f, 70f, 1200f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 275f, 1300f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1f, 190f, int.MaxValue, false, SkillshotType.SkillshotCircle);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("lux.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("lux.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("lux.r.combo", "Use R").SetValue(true));

                    Config.AddSubMenu(comboMenu);
                }

                var shieldManager = new Menu("(W) Shield Settings", "(W) Shield Settings");
                {
                    shieldManager.AddItem(new MenuItem("lux.shield.disable", "Disable shielding?").SetValue(false));
                    shieldManager.AddItem(new MenuItem("lux.shield.limit", "Min. Lux HP Percent for Shield").SetValue(new Slider(20, 1, 99)));

                    var wsettings = new Menu("(W) Priority", "(W) Priority").SetFontStyle(FontStyle.Bold,SharpDX.Color.Gold);
                    {
                        foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
                        {
                            if (LowPriority.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("lux.shield" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("lux.shield.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(15, 1, 99)));
                            }
                            if (MediumPriority.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("lux.shield" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("lux.shield.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(20, 1, 99)));
                            }
                            if (HighChamps.Contains(ally.ChampionName))
                            {
                                wsettings.AddItem(new MenuItem("lux.shield" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                                wsettings.AddItem(new MenuItem("lux.shield.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(30, 1, 99)));
                            }
                        }
                        shieldManager.AddSubMenu(wsettings);
                    }
                    Config.AddSubMenu(shieldManager);

                }

                var rSettings = new Menu("(R) Ult Settings", "(R) Ult Settings");
                {
                    rSettings.AddItem(new MenuItem("lux.r.killsteal", "Killsteal using R").SetValue(false));

                    Config.AddSubMenu(rSettings);
                }

                var harass = new Menu("Harass Settings", "Harass Settings");
                {
                    harass.AddItem(new MenuItem("lux.q.harass", "Use Q").SetValue(true));
                    harass.AddItem(new MenuItem("lux.e.harass", "Use E").SetValue(true));
                    harass.AddItem(new MenuItem("lux.harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));

                    Config.AddSubMenu(harass);
                }

                var misc = new Menu("Miscellaneous", "Miscellaneous");
                {
                    misc.AddItem(new MenuItem("lux.anti", "Gapcloser (Q)").SetValue(true));

                    Config.AddSubMenu(misc);
                }

                var drawing = new Menu("Draw Settings", "Draw Settings");
                {
                    drawing.AddItem(new MenuItem("lux.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("lux.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("lux.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("lux.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));

                    Config.AddSubMenu(drawing);
                }

                Config.AddItem(new MenuItem("lux.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }

            SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
            Config.AddToMainMenu();
            Game.OnUpdate += LuxOnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Drawing.OnDraw += LuxOnDraw;
            AntiGapcloser.OnEnemyGapcloser += LuxOnEnemyGapcloser;
        }

        private static void OnCreate(GameObject objects, EventArgs args)
        {
            if (objects.Name == "Lux_Base_E_mis.troy")
            {
                LuxE = objects;

            }
        }

        private static void OnDelete(GameObject objects, EventArgs args)
        {
            // if (objects.Name == "Lux_Base_E_tar_nova.troy")
            if (objects.Name == "Lux_Base_E_mis.troy")
            {
                LuxE = null;
            }
        }

        private static void LuxOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.LSIsReady() && MenuCheck("lux.anti", Config) && Q.GetPrediction(gapcloser.Sender).Hitchance > HitChance.High
                && gapcloser.Sender.LSIsValidTarget(1175))
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        private static void LuxOnUpdate(EventArgs args)
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

            if (!MenuCheck("lux.shield.disable", Config) && W.LSIsReady())
            {
                WManager();
            }

            if (MenuCheck("lux.r.killsteal", Config) && R.LSIsReady())
            {
                Killsteal();
            }

            if (LuxE.Position.LSCountEnemiesInRange(350) >= 1 && E.Instance.Name != "LuxLightStrikeKugel")
            {
                E.Cast();
            }

        }

        private static void WManager()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            foreach (var shield in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && x.LSDistance(ObjectManager.Player.Position) < W.Range && !x.IsDead && x.IsVisible && !x.IsZombie))
            {
                if (MenuCheck("lux.shield" + shield.ChampionName, Config) && shield.HealthPercent < SliderCheck("lux.shield.percent" + shield.ChampionName, Config)
                    && ObjectManager.Player.HealthPercent > SliderCheck("lux.shield.limit", Config))
                {
                    W.Cast(shield.Position);
                }
            }
        }

        private static void Killsteal()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            if (MenuCheck("lux.r.killsteal", Config) && R.LSIsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (R.GetDamage(target) > target.Health)
                    {
                        R.SPredictionCast(target, SpellHitChance(Config, "lux.hitchance"));
                    }    
                }
            }
        }

        private static void Combo()
        {
            if (MenuCheck("lux.q.combo", Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "lux.hitchance"));
                }
            }

            if (MenuCheck("lux.e.combo", Config) && E.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range) && !x.IsDead && !x.IsZombie))
                {
                    E.SPredictionCast(enemy, SpellHitChance(Config, "lux.hitchance"));
                }
            }

            if (MenuCheck("lux.r.combo", Config) && R.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    R.SPredictionCast(enemy, SpellHitChance(Config, "lux.hitchance"));
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < SliderCheck("lux.harass.mana", Config))
            {
                return;
            }

            if (MenuCheck("lux.q.harass", Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "lux.hitchance"));
                }
            }

            if (MenuCheck("lux.e.harass", Config) && E.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    E.SPredictionCast(enemy, SpellHitChance(Config, "lux.hitchance"));
                }
            }
        }

        private static void LuxOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("lux.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("lux.q.draw", Config));
            }

            if (W.LSIsReady() && ActiveCheck("lux.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("lux.w.draw", Config));
            }

            if (E.LSIsReady() && ActiveCheck("lux.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("lux.e.draw", Config));
            }

            if (R.LSIsReady() && ActiveCheck("lux.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("lux.r.draw", Config));
            }
        }
    }
}
