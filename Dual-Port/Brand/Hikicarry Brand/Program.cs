using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Brand
{
    static class Program
    {
        public static Spell Q,W,E,R;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;
        public static readonly AIHeroClient Brand = ObjectManager.Player;
        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static void Game_OnGameLoad()
        {
            if (Brand.ChampionName != "Brand") return;

            Q = new Spell(SpellSlot.Q, 1050, TargetSelector.DamageType.Magical);
            W = new Spell(SpellSlot.W, 900, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 625, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 750, TargetSelector.DamageType.Magical);

            Q.SetSkillshot(0.625f, 50f, 1600f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.0f, 240f, int.MaxValue, false, SkillshotType.SkillshotCircle);

            Config = new Menu("HikiCarry - Brand", "HikiCarry - Brand", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("q.combo.style", "(Q) Combo Style").SetValue(new StringList(new[] { "Only Enemy If Stunnable", "Always" })));
                    comboMenu.AddItem(new MenuItem("w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.combo", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.combo.killable", "If Enemy Killable Use (R)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.hit.x.target", "Min. Enemy Count For (R)").SetValue(new Slider(4, 1, 5)));
                    Config.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("w.harass", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "Use E").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(harassMenu);
                }

                var clearMenu = new Menu("Clear Settings", "Clear Settings");
                {
                    clearMenu.AddItem(new MenuItem("w.clear", "Use W").SetValue(true));
                    clearMenu.AddItem(new MenuItem("w.minion.count", "Min. Minion (W)").SetValue(new Slider(4, 1, 5)));
                    clearMenu.AddItem(new MenuItem("clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(clearMenu);
                }

                var ksMenu = new Menu("Killsteal Settings", "Killsteal Settings");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "Use Q").SetValue(true));
                    ksMenu.AddItem(new MenuItem("w.ks", "Use W").SetValue(true));
                    ksMenu.AddItem(new MenuItem("e.ks", "Use E").SetValue(true));
                    Config.AddSubMenu(ksMenu);
                }

                var miscMenu = new Menu("Misc Settings", "Misc Settings");
                {
                    var antigapcloser = new Menu("Anti Gapcloser", "Anti Gapcloser");
                    {
                        antigapcloser.AddItem(
                            new MenuItem("e.q.antigapcloser", "(E) + (Q) Anti-Gapcloser").SetValue(true));

                        miscMenu.AddSubMenu(antigapcloser);
                    }
                    var interrupterSet = new Menu("Interrupter Settings", "Interrupter Settings");
                    {
                        interrupterSet.AddItem(new MenuItem("e.q.interrupter", "(E) + (Q) Interrupter").SetValue(true));
                        interrupterSet.AddItem(
                            new MenuItem("min.interrupter.danger.level", "Interrupter Danger Level").SetValue(
                                new StringList(new[] { "HIGH", "MEDIUM", "LOW" })));
                        miscMenu.AddSubMenu(interrupterSet);
                    }
                    Config.AddSubMenu(miscMenu);
                }
                var drawMenu = new Menu("Draw Settings", "Draw Settings");
                {
                    var skillDraw = new Menu("Skill Draws", "Skill Draws");
                    {
                        skillDraw.AddItem(new MenuItem("q.draw", "Draw E Range").SetValue(new Circle(false, Color.White)));
                        skillDraw.AddItem(new MenuItem("w.draw", "Draw W Range").SetValue(new Circle(true, Color.White)));
                        skillDraw.AddItem(new MenuItem("e.draw", "Draw E Range").SetValue(new Circle(false, Color.White)));
                        skillDraw.AddItem(new MenuItem("r.draw", "Draw R Range").SetValue(new Circle(false, Color.White)));
                        drawMenu.AddSubMenu(skillDraw);
                    }

                    Config.AddSubMenu(drawMenu);

                }
                var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
                var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.Gold));

                drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                DamageIndicator.DamageToUnit = TotalDamage;
                DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                Config.AddItem(new MenuItem("keysinfo", "                 Hit Chance Settings").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Gold));
                Config.AddItem(new MenuItem("hikiChance", "Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
                Config.AddToMainMenu();
            }

            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }
            KillSteal();
        }

        public static void KillSteal()
        {
            if (Q.IsReady() && Config.Item("q.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Q.Range) &&
                    Q.GetPrediction(x).Hitchance >= HikiChance("hikiChance") && Q.GetDamage(x) > x.Health))
                {
                    Q.Cast(enemy);
                }
            }
            if (W.IsReady() && Config.Item("w.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) &&
                    W.GetPrediction(x).Hitchance >= HikiChance("hikiChance") && W.GetDamage(x) > x.Health))
                {
                    W.Cast(enemy);
                }
            }
            if (E.IsReady() && Config.Item("e.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && 
                    E.GetDamage(x) > x.Health))
                {
                    E.CastOnUnit(enemy);
                }
            }
        }

        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[Config.Item(menuName).GetValue<StringList>().SelectedIndex];
        }
        public static void QCast()
        {
            if (Config.Item("q.combo.style").GetValue<StringList>().SelectedIndex == 0)
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(Q.Range) && Q.GetPrediction(x).Hitchance >= HikiChance("hikiChance")
                                 && x.HasBuff("brandablaze") && Q.GetPrediction(x).CollisionObjects.Count == 0))
                {
                    Q.Cast(enemy);
                }
            }
            else if (Config.Item("q.combo.style").GetValue<StringList>().SelectedIndex == 1)
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(Q.Range) && Q.GetPrediction(x).Hitchance >= HikiChance("hikiChance")))
                {
                    Q.Cast(enemy);
                }
            }
        }
        private static void Combo()
        {
            if (Q.IsReady() && Config.Item("q.combo").GetValue<bool>())
            {
                QCast();
            }
            if (W.IsReady() && Config.Item("w.combo").GetValue<bool>() && !E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && W.GetPrediction(x).Hitchance >= HikiChance("hikiChance")))
                {
                    W.Cast(enemy);
                }
            }
            if (E.IsReady() && Config.Item("e.combo").GetValue<bool>() && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range)))
                {
                    E.CastOnUnit(enemy);
                }
            }
            if (R.IsReady() && Config.Item("r.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)))
                {
                    if (Brand.CountEnemiesInRange(R.Range) >= Config.Item("r.hit.x.target").GetValue<Slider>().Value)
                    {
                        R.CastOnUnit(enemy);
                    }
                    if (R.GetDamage(enemy) > enemy.Health)
                    {
                        R.CastOnUnit(enemy);
                    }
                }
            }
        }
        private static void Harass()
        {
            if (Brand.ManaPercent < Config.Item("harass.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (W.IsReady() && Config.Item("w.harass").GetValue<bool>() && !E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && W.GetPrediction(x).Hitchance >= HikiChance("hikiChance")))
                {
                    W.Cast(enemy);
                }
            }
            if (E.IsReady() && Config.Item("e.harass").GetValue<bool>() && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range)))
                {
                    E.CastOnUnit(enemy);
                }
            }

        }
        private static void Clear()
        {
            if (Brand.ManaPercent < Config.Item("clear.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (W.IsReady() && Config.Item("w.clear").GetValue<bool>())
            {
                var wminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
                var xx = W.GetCircularFarmLocation(wminion);
                if (xx.MinionsHit >= Config.Item("w.minion.count").GetValue<Slider>().Value)
                {
                    W.Cast(xx.Position);
                }
            }

        }
        private static float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
            if (Q.IsReady())
            {
                damage += Q.GetDamage(hero);
            }
            if (W.IsReady())
            {
                damage += W.GetDamage(hero);
            }
            if (E.IsReady())
            {
                damage += W.GetDamage(hero);
            }
            if (R.IsReady())
            {
                damage += R.GetDamage(hero);
            }
            return (float)damage;
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var qdraw = Config.Item("q.draw").GetValue<Circle>();
            var wdraw = Config.Item("w.draw").GetValue<Circle>();
            var edraw = Config.Item("e.draw").GetValue<Circle>();
            var rdraw = Config.Item("r.draw").GetValue<Circle>();

            if (qdraw.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(Brand.Position, Q.Range, qdraw.Color);
            }
            if (wdraw.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(Brand.Position, W.Range, qdraw.Color);
            }
            if (edraw.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(Brand.Position, E.Range, edraw.Color);
            }
            if (rdraw.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(Brand.Position, R.Range, rdraw.Color);
            }
        }
        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.HasBuff("brandablaze") && Q.IsReady())
            {
                Q.Cast(gapcloser.Sender);
            }
            else
            {
                if (E.IsReady() && Q.IsReady())
                {
                    E.Cast(gapcloser.Sender);
                }
            }
        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.Item("e.q.interrupter").GetValue<bool>() || !sender.IsValidTarget()) return;
            Interrupter2.DangerLevel a;
            switch (Config.Item("min.interrupter.danger.level").GetValue<StringList>().SelectedValue)
            {
                case "HIGH":
                    a = Interrupter2.DangerLevel.High;
                    break;
                case "MEDIUM":
                    a = Interrupter2.DangerLevel.Medium;
                    break;
                default:
                    a = Interrupter2.DangerLevel.Low;
                    break;
            }

            if (args.DangerLevel == Interrupter2.DangerLevel.High ||
                args.DangerLevel == Interrupter2.DangerLevel.Medium && a != Interrupter2.DangerLevel.High ||
                args.DangerLevel == Interrupter2.DangerLevel.Medium && a != Interrupter2.DangerLevel.Medium &&
                a != Interrupter2.DangerLevel.High)
            {
                if (sender.HasBuff("brandablaze") && Q.IsReady())
                {
                    Q.Cast(sender);
                }
                else
                {
                    if (E.IsReady() && Q.IsReady())
                    {
                        E.Cast(sender);
                    }
                }
            }
        }
        
    }
}
