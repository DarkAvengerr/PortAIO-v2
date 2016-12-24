using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class Alistar : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        private static Obj_AI_Base Player = ObjectManager.Player;
        
        public Alistar()
        {
            AlistarOnLoad();
        }

        private static void AlistarOnLoad()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 650f);
            E = new Spell(SpellSlot.E, 575f);
            R = new Spell(SpellSlot.R);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("alistar.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("alistar.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("alistar.r.combo", "Use R").SetValue(true));

                    Config.AddSubMenu(comboMenu);
                }

                var healManager = new Menu("(E) Heal Settings", "(E) Heal Settings");
                {
                    healManager.AddItem(new MenuItem("alistar.heal.disable", "Disable Heal?").SetValue(false));
                    healManager.AddItem(new MenuItem("alistar.heal.limit", "Min. Alistar HP Percent for Heal").SetValue(new Slider(40, 1, 99)));
                    healManager.AddItem(new MenuItem("ayrac1", "                  (W) Heal Settings"));
                    foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
                    {
                        if (LowPriority.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("alistar.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("alistar.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(20, 1, 99)));
                        }
                        if (MediumPriority.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("alistar.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("alistar.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(30, 1, 99)));
                        }
                        if (HighChamps.Contains(ally.ChampionName))
                        {
                            healManager.AddItem(new MenuItem("alistar.heal" + ally.ChampionName, "Heal: " + ally.ChampionName).SetValue(true));
                            healManager.AddItem(new MenuItem("alistar.heal.percent" + ally.ChampionName, "Min. " + ally.ChampionName + " HP Percent").SetValue(new Slider(45, 1, 99)));
                        }
                    }

                    Config.AddSubMenu(healManager);
                }

                var ult = new Menu("(R) Auto Ultimate", "(R) Auto Ultimate");
                {
                    ult.AddItem(new MenuItem("alistar.auto.ult", "Auto Ult (R)")).SetValue(true);
                    ult.AddItem(new MenuItem("alistar.min.ult", "Min. HP Percent to Ult")).SetValue(new Slider(40, 1, 99));
                    ult.AddItem(new MenuItem("alistar.min.enemy", "Min. Enemies to Ult")).SetValue(new Slider(2, 1, 5));
                }

                var misc = new Menu("Miscellaneous", "Miscellaneous");
                {
                    misc.AddItem(new MenuItem("alistar.anti", "Gapcloser (W)").SetValue(true));
                    misc.AddItem(new MenuItem("alistar.inter", "Interrupt (W)").SetValue(true));

                    Config.AddSubMenu(misc);
                }

                var drawing = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    drawing.AddItem(new MenuItem("alistar.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("alistar.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    Config.AddSubMenu(drawing);
                }
            }

            Config.AddToMainMenu();
            Game.OnUpdate += AlistarOnUpdate;
            Interrupter2.OnInterruptableTarget += AlistarInterrupter;
            AntiGapcloser.OnEnemyGapcloser += AlistarGapcloser;
            Drawing.OnDraw += AlistarOnDraw;
        }

        private static void AlistarInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && MenuCheck("alistar.inter",Config) && sender.IsValidTarget(W.Range) && 
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                W.Cast(sender);
            }
        }

        private static void AlistarGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsEnemy && gapcloser.Sender.IsValidTarget(W.Range) && MenuCheck("alistar.anti", Config))
            {
                W.Cast(gapcloser.Sender);
            }
        }

        private static void AlistarOnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }

            if (!MenuCheck("alistar.heal.disable", Config))
            {
                AutoHeal();
            }

            if (MenuCheck("alistar.auto.ult", Config))
            {
                AutoUlt();
            }
        }

        private static void Combo()
        {
            if (MenuCheck("alistar.w.combo", Config) && W.IsReady() && MenuCheck("alistar.q.combo", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.Cast(enemy);
                    foreach (var qenemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                    {
                        Q.Cast(qenemy);
                    }
                }
            }
        }

        private static void AutoHeal()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            foreach (var heal in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe))
            {
                if (MenuCheck("alistar.heal" + heal.ChampionName, Config) && heal.HealthPercent < SliderCheck("alistar.heal.percent" + heal.ChampionName, Config)
                    && ObjectManager.Player.HealthPercent > SliderCheck("alistar.heal.limit", Config))
                {
                    E.CastOnUnit(heal);
                }
            }
        }

        private static void AutoUlt()
        {
            if (Player.HealthPercent < SliderCheck("alistar.min.ult", Config) &&
                Player.CountEnemiesInRange(1500f) >= SliderCheck("alistar.min.enemy", Config) && R.IsReady())
            {
                R.Cast();
            }
        }

        private static void AlistarOnDraw(EventArgs args)
        {
            if (W.IsReady() && ActiveCheck("alistar.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("alistar.w.draw", Config));
            }
            if (E.IsReady() && ActiveCheck("alistar.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("alistar.e.draw", Config));
            }
        }
    }
}
