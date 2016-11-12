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
    public class Leona : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        private static AIHeroClient Player = ObjectManager.Player;

        public Leona()
        {
            LeonaOnLoad();
        }

        public static void LeonaOnLoad()
        {
            Q = new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(Player) + 100);
            W = new Spell(SpellSlot.W, 200f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 1200f);

            E.SetSkillshot(0.25f, 120f, 2000f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Config = new Menu("vSupport Series:  " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("leona.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("leona.w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("leona.e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("leona.r.combo", "Use R").SetValue(true));
                    var rsettings = new Menu(":: R Settings", ":: R Settings");
                    {
                        rsettings.AddItem(new MenuItem("leona.r.count", "Min. Enemy Count").SetValue(new Slider(3, 1, 5)));
                        comboMenu.AddSubMenu(rsettings);
                    }
                    Config.AddSubMenu(comboMenu);
                }

                var harass = new Menu("Harass Settings", "Harass Settings");
                {
                    harass.AddItem(new MenuItem("leona.q.harass", "Use Q").SetValue(true));
                    harass.AddItem(new MenuItem("leona.w.harass", "Use W for Resist?").SetValue(true));
                    harass.AddItem(new MenuItem("leona.e.harass", "Use E").SetValue(true));
                    harass.AddItem(new MenuItem("leona.harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));

                    Config.AddSubMenu(harass);
                }

                var misc = new Menu("Miscellaneous", "Miscellaneous");
                {
                    misc.AddItem(new MenuItem("leona.anti", "Gapcloser (Q)").SetValue(true));
                    misc.AddItem(new MenuItem("leona.inter", "Interrupt (E)").SetValue(true));

                    Config.AddSubMenu(misc);
                }

                var drawing = new Menu("Draw Settings", "Draw Settings");
                {
                    drawing.AddItem(new MenuItem("leona.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("leona.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("leona.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("leona.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));

                    Config.AddSubMenu(drawing);
                }

                Config.AddItem(new MenuItem("leona.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
            }

            SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
            Config.AddToMainMenu();
            Game.OnUpdate += LeonaOnUpdate;
            Drawing.OnDraw += LeonaOnDraw;
            AntiGapcloser.OnEnemyGapcloser += LeonaOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += LeonaOnInterruptableTarget;
           // Obj_AI_Base.OnSpellCast += OnSpellCast;
        }
        
        /*private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Q.IsReady() || Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
            {
                var ward =
                    ObjectManager
                        .Get<Obj_AI_Base>(
                        )
                        .FirstOrDefault(
                            x =>
                                x.IsEnemy && x.IsHPBarRendered && x.Distance(Player) < Player.AttackRange + x.BoundingRadius + Player.BoundingRadius &&
                                (x.Name.ToLower().Contains("ward") || x.Name.ToLower().Contains("sight") ||
                                 x.Name.ToLower().Contains("vision"))); 

                if (ward == null) return;

                if (ward.Health == 3 || ward.Health == 2)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, ward);
                }
                if (Orbwalking.IsAutoAttack(args.SData.Name) && (ward.Health == 2 || ward.Health == 1))
                {
                    Q.Cast();
                }

                if (Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, ward);
                }
            }
        }*/

        private static void LeonaOnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Q.IsReady() && sender.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) + 100) && MenuCheck("leona.inter", Config))
            {
                Q.CastIfHitchanceEquals(sender, HitChance.High);
            }
            else if (E.IsReady() && sender.IsValidTarget(700) && MenuCheck("leona.inter", Config))
            {
                E.CastIfHitchanceEquals(sender, HitChance.High);
            }
        }

        private static void LeonaOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.IsReady() && MenuCheck("leona.anti", Config) && gapcloser.Sender.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) + 100))
            {
                Q.Cast(gapcloser.Sender);
            }
            else if (E.IsReady() && MenuCheck("leona.anti", Config) && E.GetPrediction(gapcloser.Sender).Hitchance > HitChance.High
                && gapcloser.Sender.IsValidTarget(700))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private static void LeonaOnUpdate(EventArgs args)
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
            if (MenuCheck("leona.q.combo", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    Q.Cast(enemy);
                }
            }

            if (MenuCheck("leona.w.combo", Config) && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }
            }

            if (MenuCheck("leona.e.combo", Config) && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.IsDead && !x.IsZombie))
                {
                    E.SPredictionCast(enemy, SpellHitChance(Config, "leona.hitchance"));
                }
            }

            if (MenuCheck("leona.r.combo", Config) && R.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    R.SPredictionCastAoe(SliderCheck("leona.r.count", Config));
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < SliderCheck("leona.harass.mana", Config))
            {
                return;
            }

            if (MenuCheck("leona.q.harass", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    Q.Cast(enemy);
                }
            }

            if (MenuCheck("leona.w.harass", Config) && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }
            }

            if (MenuCheck("leona.e.harass", Config) && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    E.SPredictionCast(enemy, SpellHitChance(Config, "leona.hitchance"));
                }
            }
        }

        private static void LeonaOnDraw(EventArgs args)
        {
            if (Q.IsReady() && ActiveCheck("leona.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("leona.q.draw", Config));
            }

            if (W.IsReady() && ActiveCheck("leona.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("leona.w.draw", Config));
            }

            if (E.IsReady() && ActiveCheck("leona.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("leona.e.draw", Config));
            }

            if (R.IsReady() && ActiveCheck("leona.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("leona.r.draw", Config));
            }
        }
    }
}
