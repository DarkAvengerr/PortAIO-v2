using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ClipperLib;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using FontStyle = System.Drawing.FontStyle;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Viktor
{
    public static class Program
    {
        public static Menu Config;
        public static readonly string CName = "Viktor";
        public static Orbwalking.Orbwalker Orbwalker;
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };
        
        public static Spell Q,W,E,R;

        public static int ERange = 525;
        public static SpellSlot Ignite;

        public static void Game_OnGameLoad()
        {
            if (Player.CharData.BaseSkinName != CName)
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 550 + ERange / 4f); 
            R = new Spell(SpellSlot.R, 700);

            Q.SetTargetted(0.25f, 2000);
            W.SetSkillshot(0.25f, 300, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.0f, 90, 1200, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 250, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Ignite = Player.GetSpellSlot("summonerdot");

            Config = new Menu("HikiCarry - Viktor", "HikiCarry - Viktor", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold);
            TargetSelector.AddToMenu(Config.SubMenu("Target Selector Settings"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("qCombo", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("wCombo", "Use W").SetValue(false));
                comboMenu.AddItem(new MenuItem("eCombo", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("rCombo", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("minHitR", "Minimum Hit R").SetValue(new Slider(2, 1, 5)));
                Config.AddSubMenu(comboMenu);
            }
            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("qHarass", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("eHarass", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("hMana", "Mana Manager").SetValue(new Slider(50, 0, 100)));
                Config.AddSubMenu(harassMenu);
            }

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("eClear", "Use E").SetValue(true));
                //clearMenu.AddItem(new MenuItem("eMinionCount", "E Minion Hit Count").SetValue(new Slider(3, 1, 5)));
                clearMenu.AddItem(new MenuItem("cMana", "Mana Manager").SetValue(new Slider(50, 0, 100)));
                Config.AddSubMenu(clearMenu);
            }

            var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                jungleMenu.AddItem(new MenuItem("qJungle", "Use Q").SetValue(true));
                jungleMenu.AddItem(new MenuItem("wJungle", "Use W").SetValue(true));
                jungleMenu.AddItem(new MenuItem("eJungle", "Use E").SetValue(true));
                jungleMenu.AddItem(new MenuItem("jMana", "Mana Manager").SetValue(new Slider(50, 0, 100)));
                Config.AddSubMenu(jungleMenu);
            }

            var lastMenu = new Menu("Lasthit Settings", "Lasthit Settings");
            {
                lastMenu.AddItem(new MenuItem("qLast", "Use Q [Siege Minions]").SetValue(true));
                lastMenu.AddItem(new MenuItem("lMana", "Mana Manager").SetValue(new Slider(50, 0, 100)));
                Config.AddSubMenu(lastMenu);
            }

            var miscMenu = new Menu("Misc Settings", "Misc Settings");
            {
                miscMenu.AddItem(new MenuItem("aGapcloser", "AntiGapcloser[W]").SetValue(true));
                miscMenu.AddItem(new MenuItem("wInterrupter", "Interrupter[W]").SetValue(true));
                miscMenu.AddItem(new MenuItem("eKS", "Killsteal[E]").SetValue(true));
                Config.AddSubMenu(miscMenu);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                drawMenu.AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.White)));
                drawMenu.AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.DarkOrange)));
                drawMenu.AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.Green)));
                drawMenu.AddItem(new MenuItem("rDraw", "R Range").SetValue(new Circle(true, Color.Gold)));
                Config.AddSubMenu(drawMenu);
            }
            Config.AddItem(new MenuItem("useIgnite", "Smart Ignite").SetValue(true));
            Config.AddItem(new MenuItem("hChance", "Hit Chance").SetValue<StringList>(new StringList(HitchanceNameArray, 2)));
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.Gold));

            drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
            drawMenu.SubMenu("Damage Draws").AddItem(drawFill);


            //DamageIndicator.DamageToUnit = CDamage;
            //DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            //DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            //DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

            drawDamageMenu.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("aGapcloser").GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(1000))
                {
                    Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                }
                if (W.CanCast(gapcloser.Sender))
                {
                    W.Cast(gapcloser.Sender);
                }
            }
        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Config.Item("ainterrupt").GetValue<bool>())
            {
                if (sender.IsValidTarget(1000))
                {
                    Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                }
                if (W.CanCast(sender))
                {
                    W.Cast(sender);
                }
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Clear();
                JungleClear();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }
            if (R.Instance.Name != "ViktorChaosStorm")
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range + 500)))
                {
                    R.Cast(enemy);
                }
            }
            EKs();
            
        }
        private static void Combo()
        {
            byte minHit = (byte)Config.Item("minHitR").GetValue<Slider>().Value;
            HitChance HikiChance = HitchanceArray[Config.Item("hChance").GetValue<StringList>().SelectedIndex];
            if (Q.IsReady() && Config.Item("qCombo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange)))
                {
                    Q.Cast(enemy);
                }
            }

            if (W.IsReady() && Config.Item("wCombo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && W.GetPrediction(x).Hitchance > HikiChance))
                {
                    W.Cast(enemy);
                }
            }

            if (E.IsReady() && Config.Item("eCombo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range + ERange)))
                {
                    DeathRay(enemy, HikiChance);
                }
            }

            if (R.IsReady() && Config.Item("rCombo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(R.Range) &&
                    R.GetPrediction(x, true).Hitchance > HikiChance))
                {
                    if (enemy.Health < CDamage(enemy))
                    {
                        R.Cast(enemy);
                    }
                    if (Player.CountEnemiesInRange(R.Range) > minHit)
                    {
                        R.CastIfWillHit(enemy, minHit);
                    }
                }
            }

            if (Ignite.IsReady() && Config.Item("useIgnite").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsDead && !x.IsZombie
                    && x.IsValidTarget(550)))
                {
                    if (Player.GetSpellDamage(enemy, Ignite) + CDamage(enemy) > enemy.Health)
                    {
                        Player.Spellbook.CastSpell(Ignite, enemy);
                    }
                }
            }
        }
        private static void DeathRay(Obj_AI_Base enemy, HitChance hitChance)
        {
            if (Player.ServerPosition.Distance(enemy.ServerPosition) < ERange)
            {
                E.UpdateSourcePosition(enemy.ServerPosition, enemy.ServerPosition);
                var prediction = E.GetPrediction(enemy, true);
                if (prediction.Hitchance >= hitChance)
                    E.Cast(enemy.ServerPosition, prediction.CastPosition);
            }
            else if (Player.ServerPosition.Distance(enemy.ServerPosition) < E.Range + ERange)
            {
                var castStartPos = Player.ServerPosition.Extend(enemy.ServerPosition, ERange);
                E.UpdateSourcePosition(castStartPos, castStartPos);
                var prediction = E.GetPrediction(enemy, true);
                if (prediction.Hitchance >= hitChance)
                    E.Cast(castStartPos, prediction.CastPosition);
            }
        }
        private static float CDamage(Obj_AI_Base enemy)
        {
            var useQ = Config.Item("qCombo").GetValue<bool>();
            var useE = Config.Item("eCombo").GetValue<bool>();
            var useR = Config.Item("rCombo").GetValue<bool>();

            var qDAA = new Double[] { 20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210 };
            float damage = 0;

            if (Q.IsReady() && useQ)
            {
                damage += Q.GetDamage(enemy);
            }
            if (Q.IsReady() || ObjectManager.Player.HasBuff("viktorpowertransferreturn") && useQ)
            {
                damage += (float)Player.CalcDamage(enemy, Damage.DamageType.Magical,
                    qDAA[Player.Level >= 18 ? 18 - 1 : Player.Level - 1] +
                    (Player.TotalMagicalDamage * .5) + Player.TotalAttackDamage());
            }
            if (E.IsReady() && useE)
            {
                if (Player.HasBuff("viktoreaug") || Player.HasBuff("viktorqeaug") || Player.HasBuff("viktorqweaug"))
                {
                    damage += E.GetDamage(enemy, 1);
                }
                else
                {
                    damage += E.GetDamage(enemy, 0);
                }
            }
            if (R.IsReady() && useR)
            {
                damage += R.GetDamage(enemy);
                damage += R.GetDamage(enemy, 2);
            }

            return (float)damage;
        }
        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("hMana").GetValue<Slider>().Value)
            {
                return;
            }
            HitChance HikiChance = HitchanceArray[Config.Item("hChance").GetValue<StringList>().SelectedIndex];
            if (Q.IsReady() && Config.Item("qHarass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange)))
                {
                    Q.Cast(enemy);
                }
            }

            if (E.IsReady() && Config.Item("eHarass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range + ERange)))
                {
                    DeathRay(enemy, HikiChance);
                }
            }
            
        }
        /*private static void EToggle()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("hMana").GetValue<Slider>().Value)
            {
                return;
            }

            if (E.IsReady() && Config.Item("eToggle").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range + ERange)))
                {
                    DeathRay(enemy, ToggleChance);
                }
            }
                
            
        }*/
        private static void Clear()
        {
            if (Player.ManaPercent < Config.Item("cMana").GetValue<Slider>().Value)
            {
                return;
            }
            if (E.IsReady() && Config.Item("eClear").GetValue<bool>())
            {
                var firstMinion = ObjectManager.Get<Obj_AI_Minion>().Where(a => a.IsEnemy).Where(a => !a.IsDead).Where(a => a.Distance(Player) < E.Range+ERange).FirstOrDefault();
                var lasttMinion = ObjectManager.Get<Obj_AI_Minion>().Where(a => a.IsEnemy).Where(a => !a.IsDead).Where(a => a.Distance(Player) < E.Range).LastOrDefault();
                if (firstMinion == null || lasttMinion == null)
                {
                    return;
                }
                if (firstMinion.Distance(Player) < ERange)
                {
                    E.Cast(firstMinion.Position, lasttMinion.Position);
                } 
            }
        }
        private static void EKs()
        {
            if (E.IsReady() && Config.Item("eKS").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsDead && !x.IsZombie
                    && x.IsValidTarget(E.Range) && E.GetDamage(x) > x.Health))
                {
                    HitChance HikiChance = HitchanceArray[Config.Item("hChance").GetValue<StringList>().SelectedIndex];
                    DeathRay(enemy,HikiChance);
                }
            }
        }
        private static void JungleClear()
        {
            var mob = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral,MinionOrderTypes.MaxHealth);
            HitChance HikiChance = HitchanceArray[Config.Item("hChance").GetValue<StringList>().SelectedIndex];
            //public static HitChance ToggleChance = HitchanceArray[Config.Item("tChance").GetValue<StringList>().SelectedIndex];
            if (ObjectManager.Player.ManaPercent < Config.Item("hMana").GetValue<Slider>().Value ||mob == null || (mob != null && mob.Count == 0))
            {
                return;
            }
            if (Q.IsReady() && Config.Item("qJungle").GetValue<bool>())
            {
                Q.Cast(mob[0]);
            }
            if (W.IsReady() && Config.Item("wJungle").GetValue<bool>())
            {
                W.Cast(mob[0].Position);
            }
            if (E.IsReady() && Config.Item("eJungle").GetValue<bool>())
            {
                DeathRay(mob[0], HikiChance);
            }
                
        }
        private static void LastHit()
        {
            var lMana = Config.Item("lMana").GetValue<Slider>().Value;
            var useQ = Config.Item("qLast").GetValue<bool>();
            var qMinion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);
            if (ObjectManager.Player.ManaPercent > lMana)
            {
                if (Q.IsReady() && useQ)
                {
                    foreach (var minyon in qMinion)
                    {
                        if (minyon.CharData.BaseSkinName.Contains("MinionSiege"))
                        {
                            if (Q.IsKillable(minyon))
                            {
                                Q.CastOnUnit(minyon);
                            }
                        }
                    }
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = Config.Item("eDraw").GetValue<Circle>();
            var menuItem4 = Config.Item("rDraw").GetValue<Circle>();
            

            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), Q.Range, menuItem1.Color, 5);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), W.Range, menuItem2.Color, 5);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), E.Range + ERange , menuItem3.Color, 5);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), R.Range, menuItem4.Color, 5);
            }

            
        }
    }
}