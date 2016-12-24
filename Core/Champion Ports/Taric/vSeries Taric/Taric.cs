using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class Taric : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        private static AIHeroClient Player = ObjectManager.Player;

        private static bool hasPassive = false;
        private readonly static string buffName = "taricgemcraftbuff";

        public Taric()
        {
            TaricOnLoad();
        }

        public static void TaricOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 750f);
            W = new Spell(SpellSlot.W, 200f);
            E = new Spell(SpellSlot.E, 625f);
            R = new Spell(SpellSlot.R, 200f);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("taric.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("taric.w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("taric.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("taric.r.combo", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("taric.r.count", "Min. Enemies in range for R").SetValue(new Slider(2, 1, 5)));

                    Config.AddSubMenu(comboMenu);
                }

                var healManager = new Menu("(Q) Heal Settings", "(W) Heal Settings");
                {
                    healManager.AddItem(new MenuItem("taric.heal.disable", "Disable Heal?").SetValue(false));
                    healManager.AddItem(new MenuItem("taric.heal.limit", "Min. Taric HP Percent for Heal").SetValue(new Slider(20, 1, 99)));
                    healManager.AddItem(new MenuItem("blabla", "(Q) Heal Settings"));

                    foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
                    {
                        if (LowPriority.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("taric.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("taric.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(10, 1, 99)));
                        }
                        if (MediumPriority.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("taric.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("taric.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(20, 1, 99)));
                        }
                        if (HighChamps.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("taric.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("taric.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(30, 1, 99)));
                        }
                    }
                    Config.AddSubMenu(healManager);
                }

                var harass = new Menu("Harass Settings", "Harass Settings");
                {
                    harass.AddItem(new MenuItem("taric.w.harass", "Use W for Resist?").SetValue(true));
                    harass.AddItem(new MenuItem("taric.e.harass", "Use E").SetValue(true));
                    harass.AddItem(new MenuItem("taric.harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));

                    Config.AddSubMenu(harass);
                }

                var misc = new Menu("Miscellaneous", "Miscellaneous");
                {
                    misc.AddItem(new MenuItem("taric.inter", "Interrupt (E)").SetValue(true));

                    Config.AddSubMenu(misc);
                }

                var drawing = new Menu("Draw Settings", "Draw Settings");
                {
                    drawing.AddItem(new MenuItem("taric.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("taric.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("taric.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("taric.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));

                    Config.AddSubMenu(drawing);
                }

                // Config.AddItem(new MenuItem("taric.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 1)));
            }

            SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
            Config.AddToMainMenu();
            Game.OnUpdate += TaricOnUpdate;
            Drawing.OnDraw += TaricOnDraw;
            AntiGapcloser.OnEnemyGapcloser += TaricOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += TaricOnInterruptableTarget;
        }

        private static void TaricOnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(625) && MenuCheck("taric.inter", Config))
            {
                E.Cast(sender);
            }
        }

        private static void TaricOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && MenuCheck("taric.anti", Config) && gapcloser.Sender.IsValidTarget(625))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private static void TaricOnUpdate(EventArgs args)
        {
            hasPassive = Player.HasBuff(buffName);

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }

            if (!MenuCheck("taric.heal.disable", Config) && Q.IsReady())
            {
                QManager();
            }
        }

        private static bool GetPassive()
        {
            return hasPassive;
        }

        private static void QManager()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            foreach (var heal in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe))
            {
                if (MenuCheck("taric.heal" + heal.ChampionName, Config) && heal.HealthPercent < SliderCheck("taric.heal.percent." + heal.ChampionName, Config)
                    && ObjectManager.Player.HealthPercent > SliderCheck("taric.heal.limit", Config))
                {
                    Q.CastOnUnit(heal);
                }
            }
        }

        private static void Combo()
        {
            if (MenuCheck("taric.w.combo", Config) && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }
            }

            if (MenuCheck("taric.e.combo", Config) && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.IsDead && !x.IsZombie))
                {
                    E.Cast(enemy);
                }
            }

            if (MenuCheck("taric.r.combo", Config) && R.IsReady() && Player.CountEnemiesInRange(R.Range) >= SliderCheck("taric.r.count", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    R.Cast();
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < SliderCheck("taric.harass.mana", Config))
            {
                return;
            }

            if (MenuCheck("taric.e.harass", Config) && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    E.Cast(enemy);
                }
            }
        }

        private static void SpellWeaving()
        {
            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && t != null)
            {
                if (E.IsReady() && !GetPassive())
                {
                    E.Cast(t);
                }
                else if (R.IsReady() && t.IsValidTarget(R.Range) && !E.IsReady() && !GetPassive())
                {
                    R.Cast();
                }
                else if (W.IsReady() && t.IsValidTarget(W.Range) && !R.IsReady() && E.IsReady() && !GetPassive())
                {
                    W.Cast();
                }
                else if (Q.IsReady() && t.IsValidTarget(Q.Range) && !W.IsReady() && !R.IsReady() && !E.IsReady() && !GetPassive())
                {
                    Q.CastOnUnit(Player);
                }
            }
        }

        private static void TaricOnDraw(EventArgs args)
        {
            if (Q.IsReady() && ActiveCheck("taric.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("taric.q.draw", Config));
            }

            if (W.IsReady() && ActiveCheck("taric.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("taric.w.draw", Config));
            }

            if (E.IsReady() && ActiveCheck("taric.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("taric.e.draw", Config));
            }

            if (R.IsReady() && ActiveCheck("taric.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("taric.r.draw", Config));
            }
        }
    }
}
