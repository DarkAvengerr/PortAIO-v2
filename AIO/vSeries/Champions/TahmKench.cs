using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using SharpDX;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;
using Orbwalking = vSupport_Series.Core.Plugins.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class TahmKench : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        private static AIHeroClient Player = ObjectManager.Player;

        public static string Passive = "TahmKenchPDebuffCounter";

        public enum Swallowed
        {
            Ally,
            Enemy,
            Minion,
            None
        }

        public TahmKench()
        {
            TahmKenchOnLoad();
        }

        public static void TahmKenchOnLoad()
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
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("tahm.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("tahm.w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("tahm.w.minion", "Use W on Minion to Stack").SetValue(true));
                    Config.AddSubMenu(comboMenu);
                }

                var harass = new Menu("Harass Settings", "Harass Settings");
                {
                    harass.AddItem(new MenuItem("tahm.q.harass", "Use Q").SetValue(true));
                    harass.AddItem(new MenuItem("tahm.w.harass", "Use W (uses Minions)").SetValue(true));
                    harass.AddItem(new MenuItem("tahm.harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));

                    Config.AddSubMenu(harass);
                }

                var shield = new Menu("Auto Shield", "Auto Shielding (E)");
                {
                    // Needs adding
                }

                var misc = new Menu("Miscellaneous", "Miscellaneous");
                {
                    misc.AddItem(new MenuItem("tahm.anti.q", "Gapcloser (Q)").SetValue(true));

                    Config.AddSubMenu(misc);
                }

                var drawing = new Menu("Draw Settings", "Draw Settings");
                {
                    drawing.AddItem(new MenuItem("tahm.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("tahm.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("tahm.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));

                    Config.AddSubMenu(drawing);
                }

                Config.AddItem(new MenuItem("tahm.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }

            SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
            Config.AddToMainMenu();
            Game.OnUpdate += TahmKenchOnUpdate;
            Drawing.OnDraw += TahmKenchOnDraw;
            AntiGapcloser.OnEnemyGapcloser += TahmKenchOnEnemyGapcloser;
        }

        private static void TahmKenchOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.LSIsReady() && MenuCheck("tahm.anti.q", Config) && gapcloser.Sender.LSIsValidTarget(Q.Range))
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        private static void TahmKenchOnUpdate(EventArgs args)
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
            // Switch cases need adding for smarter passive control
        }

        private static void Harass()
        {
            if (Player.ManaPercent < SliderCheck("tahm.harass.mana", Config))
            {
                return;
            }

            if (MenuCheck("tahm.q.harass", Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "tahm.hitchance"));
                }
            }

            if (MenuCheck("tahm.w.harass", Config) && W.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(650)))
                {
                    // Swallowed logic needs adding
                    var minion = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.LSDistance(Player, true) < 250).FirstOrDefault();
                    W.CastOnUnit(minion);

                    LeagueSharp.Common.Utility.DelayAction.Add(
                        100,
                        () =>
                        {
                            W.SPredictionCast(enemy, SpellHitChance(Config, "tahm.hitchance"));
                        }
                    );
                }
            }
        }

        private static void AutoShield()
        {
            // Auto shielding to give infinite health :kappa:
        }

        private static void TahmKenchOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("tahm.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("tahm.q.draw", Config));
            }

            if (W.LSIsReady() && ActiveCheck("tahm.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("tahm.w.draw", Config));
            }

            if (R.LSIsReady() && ActiveCheck("tahm.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("tahm.r.draw", Config));
            }
        }
    }
}
