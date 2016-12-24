using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;

using EloBuddy;
using LeagueSharp.Common;
namespace Diana_Masterrace
{
    class Program
    {
        public const string ChampionName = "Diana";
        public static Orbwalking.Orbwalker Orbwalker;
        public static bool LXOrb;
        public static List<Spell> SpellList = new List<Spell>();
        public static Menu Config;
        private static AIHeroClient Player = ObjectManager.Player;
        private static SpellSlot igniteSlot;

        private static Items.Item Zhonya;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;


        public static void Main()
        {
            if (Player.BaseSkinName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 820f);
            W = new Spell(SpellSlot.W, 200f);
            E = new Spell(SpellSlot.E, 420f);
            R = new Spell(SpellSlot.R, 825f);

            Q.SetSkillshot(0.35f, 200f, 1800, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Zhonya = new Items.Item(3157, 10);
            igniteSlot = Player.GetSpellSlot("SummonerDot");

            Config = new Menu("Diana Masterrace", "Diana Masterrace", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("Orbwalker Settings", "Orbwalker Settings"));
            Config.AddSubMenu(new Menu("Combo Settings", "Combo Settings"));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("qCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("wCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("eCombo", "Use E").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("rCombo", "Use R").SetValue(true));

            Config.AddSubMenu(new Menu("Harass Settings", "Harass Settings"));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("qHarass", "Use Q").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("wHarass", "Use W").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("rHarass", "Use R").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("manaHarass", "Harass Mana Percent").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("Clear Settings", "Clear Settings"));
            Config.SubMenu("Clear Settings").AddItem(new MenuItem("qClear", "Use Q").SetValue(true));
            Config.SubMenu("Clear Settings").AddItem(new MenuItem("wClear", "Use W").SetValue(true));
            Config.SubMenu("Clear Settings").AddItem(new MenuItem("qMinionHits", "Q Minion Hit Counts").SetValue(new Slider(3, 1, 5)));
            Config.SubMenu("Clear Settings").AddItem(new MenuItem("manaClear", "Clear Mana Percent").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("Jungle Settings", "Jungle Settings"));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("qJungle", "Use Q").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("wJungle", "Use W").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("jungleMana", "Jungle Mana Percent").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("Last Hit Settings", "Last Hit Settings"));
            Config.SubMenu("Last Hit Settings").AddItem(new MenuItem("qLast", "Use Q [Siege Minions]").SetValue(true));
            Config.SubMenu("Last Hit Settings").AddItem(new MenuItem("manaLast", "Last Hit Mana Percent").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("Items Settings", "Items Settings"));
            Config.SubMenu("Items Settings").AddSubMenu(new Menu("Zhonya Settings", "Zhonya Settings"));
            Config.SubMenu("Items Settings").SubMenu("Zhonya Settings").AddItem(new MenuItem("useZhonya", "Use Zhonya").SetValue(true));
            Config.SubMenu("Items Settings").SubMenu("Zhonya Settings").AddItem(new MenuItem("zhonyaMyHp", "If HP <= %").SetValue(new Slider(10, 1, 100)));

            Config.AddSubMenu(new Menu("KillSteal Settings", "KillSteal Settings"));
            Config.SubMenu("KillSteal Settings").AddItem(new MenuItem("ksQ", "KillSteal with Q").SetValue(true));
            Config.SubMenu("KillSteal Settings").AddItem(new MenuItem("ksR", "KillSteal with R").SetValue(true));

            Config.AddSubMenu(new Menu("Misc Settings", "Misc Settings"));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("agapcloser", "Anti-Gapcloser [W]!").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("ainterrupt", "Auto Interrupt [E]!").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("channelingBroker", "Channeling Spell Broker [E]!").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("immobileQ", "Auto Cast Immobile Target [Q]!").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("autoShield", "Auto Shield for Supported Skillshot [W]!").SetValue(true));

            Config.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.SpringGreen)));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.Crimson)));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.White)));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("rDraw", "R Range").SetValue(new Circle(true, Color.Yellow)));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("hitChanceDraw", "Draw HitChance on Enemy").SetValue(true));

            Config.AddItem(new MenuItem("masterracec0mb0", "                      Important Settings"));
            Config.AddItem(new MenuItem("selectOrbwalky", "Orbwalker Type").SetValue(new StringList(new[] { "Common Orbwalker", })));
            switch (Config.Item("selectOrbwalky").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
                    LXOrb = false;
                    break;
            }
            Config.AddItem(new MenuItem("cType", "Combo Method").SetValue(new StringList(new[] { "Advantage", "Misaya" })));
            Config.AddItem(new MenuItem("hType", "Harass Method").SetValue(new StringList(new[] { "Basic" })));
            Config.AddItem(new MenuItem("ignite", "Use Smart Ignite").SetValue(true));

            var drawDamageMenu = new MenuItem("RushDrawDamage", "Combo Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.Yellow));
            Config.SubMenu("Draw Settings").AddItem(drawDamageMenu);
            Config.SubMenu("Draw Settings").AddItem(drawFill);
            DamageIndicator.DamageToUnit = GetComboDamage;
            DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

            drawDamageMenu.ValueChanged +=
            delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            drawFill.ValueChanged +=
            delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            Config.AddToMainMenu();
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Config.Item("autoShield").GetValue<bool>())
            {
                string[] Spells = {"AhriSeduce"
                                          , "InfernalGuardian"
                                          , "EnchantedCrystalArrow"
                                          , "InfernalGuardian"
                                          , "EnchantedCrystalArrow"
                                          , "RocketGrab"
                                          , "BraumQ"
                                          , "CassiopeiaPetrifyingGaze"
                                          , "DariusAxeGrabCone"
                                          , "DravenDoubleShot"
                                          , "DravenRCast"
                                          , "EzrealTrueshotBarrage"
                                          , "FizzMarinerDoom"
                                          , "GnarBigW"
                                          , "GnarR"
                                          , "GragasR"
                                          , "GravesChargeShot"
                                          , "GravesClusterShot"
                                          , "JarvanIVDemacianStandard"
                                          , "JinxW"
                                          , "JinxR"
                                          , "KarmaQ"
                                          , "KogMawLivingArtillery"
                                          , "LeblancSlide"
                                          , "LeblancSoulShackle"
                                          , "LeonaSolarFlare"
                                          , "LuxLightBinding"
                                          , "LuxLightStrikeKugel"
                                          , "LuxMaliceCannon"
                                          , "UFSlash"
                                          , "DarkBindingMissile"
                                          , "NamiQ"
                                          , "NamiR"
                                          , "OrianaDetonateCommand"
                                          , "RengarE"
                                          , "rivenizunablade"
                                          , "RumbleCarpetBombM"
                                          , "SejuaniGlacialPrisonStart"
                                          , "SionR"
                                          , "ShenShadowDash"
                                          , "SonaR"
                                          , "ThreshQ"
                                          , "ThreshEFlay"
                                          , "VarusQMissilee"
                                          , "VarusR"
                                          , "VeigarBalefulStrike"
                                          , "VelkozQ"
                                          , "Vi-q"
                                          , "Laser"
                                          , "xeratharcanopulse2"
                                          , "XerathArcaneBarrage2"
                                          , "XerathMageSpear"
                                          , "xerathrmissilewrapper"
                                          , "yasuoq3w"
                                          , "ZacQ"
                                          , "ZedShuriken"
                                          , "ZiggsQ"
                                          , "ZiggsW"
                                          , "ZiggsE"
                                          , "ZiggsR"
                                          , "ZileanQ"
                                          , "ZyraQFissure"
                                          , "ZyraGraspingRoots"
                                      };
                for (int i = 0; i <= 61; i++)
                {
                    if (args.SData.Name == Spells[i])
                    {
                        if (sender is AIHeroClient && sender.IsEnemy && args.Target.IsMe && !args.SData.IsAutoAttack() && W.IsReady())
                        {
                            W.Cast();
                        }
                    }
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
                Jungle();
                LaneClear();
            }
            if (Config.Item("channelingBroker").GetValue<bool>())
            {
                brokeChannel();
            }
            if (Config.Item("immobileQ").GetValue<bool>())
            {
                immobileQ();
            }
            Items();
            KillSteal();
        }
        public static bool immobileTarget(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned)
            {
                return true;
            }
            else
                return false;
        }
        public static bool chanellingChecker(AIHeroClient target)
        {
            if (target.HasBuff("MissFortuneBulletTime") || target.HasBuff("katarinaultibroker") || target.HasBuff("missfortunebulletsound")
                || target.IsChannelingImportantSpell())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static void brokeChannel()
        {
            var enemy = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (E.IsReady() && chanellingChecker(enemy) && enemy.IsValidTarget(E.Range))
            {
                E.Cast();
            }
        }
        public static void immobileQ()
        {
            var enemy = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (Q.IsReady() && immobileTarget(enemy) && enemy.IsValidTarget(Q.Range))
            {
                if (enemy.Distance(Player.Position) <= 300 && Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                {
                    Q.Cast(enemy.Position + 150);
                }
                else if (enemy.Distance(Player.Position) >= 300 && Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                {
                    Q.Cast(enemy.Position);
                }
            }
        }
        private static void Items()
        {
            if (Config.Item("useZhonya").GetValue<bool>())
            {
                if (Player.HealthPercent <= Config.Item("zhonyaMyHp").GetValue<Slider>().Value)
                {
                    Zhonya.Cast();
                }
            }
        }
        private static void KillSteal()
        {
            foreach (var target in HeroManager.Enemies.OrderByDescending(x => x.Health))
            {
                if (Q.GetDamage(target) + R.GetDamage(target) + R.GetDamage(target) > target.Health)
                {
                    if (target.Distance(Player.Position) <= Q.Range && Q.GetPrediction(target).Hitchance >= HitChance.High)
                    {
                        Q.Cast(target);
                        if (target.HasBuff("dianamoonlight"))
                        {
                            R.Cast(target);
                        }
                        if (target.Health < R.GetDamage(target))
                        {
                            R.Cast(target);
                        }
                    }
                }
                if (Q.GetDamage(target) > target.Health && Q.GetPrediction(target).Hitchance >= HitChance.High)
                {
                    if (target.Distance(Player.Position) <= 300 && Q.GetPrediction(target).Hitchance >= HitChance.High)
                    {
                        Q.Cast(target.Position + 150);
                    }
                    else if (target.Distance(Player.Position) >= 300 && Q.GetPrediction(target).Hitchance >= HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }
                if (R.GetDamage(target) > target.Health && Player.CountEnemiesInRange(R.Range) <= 1)
                {
                    R.Cast(target);
                }
            }
        }
        private static void Combo()
        {
            var useQ = Config.Item("qCombo").GetValue<bool>();
            var useW = Config.Item("wCombo").GetValue<bool>();
            var useE = Config.Item("eCombo").GetValue<bool>();
            var useR = Config.Item("rCombo").GetValue<bool>();
            var useIgnite = Config.Item("ignite").GetValue<bool>();
            var enemy = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (Config.Item("cType").GetValue<StringList>().SelectedIndex == 1) // Misaya
            {
                if (Q.IsReady() && useQ && enemy.Distance(Player.Position) <= Q.Range)
                {
                    if (enemy.Distance(Player.Position) <= 300 && Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                    {
                        Q.Cast(enemy.Position + 150);
                    }
                    else if (enemy.Distance(Player.Position) >= 300 && Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                    {
                        Q.Cast(enemy);
                    }
                }
                if (R.IsReady() && useR && R.CanCast(enemy) && enemy.HasBuff("dianamoonlight"))
                {
                    R.Cast(enemy);
                }
                if (W.IsReady() && useW && enemy.Distance(Player.Position) <= W.Range)
                {
                    W.Cast();
                }
                if (E.IsReady() && useE && enemy.Distance(Player.Position) <= E.Range - 50)
                {
                    E.Cast();
                }
                if (R.IsReady() && useR && enemy.Distance(Player.Position) <= R.Range &&
                    Player.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health && !enemy.IsZombie)
                {
                    R.Cast(enemy);
                }
                if (enemy.Health <= GetComboDamage(enemy))
                {
                    Player.Spellbook.CastSpell(igniteSlot, enemy);
                }

            }
            if (Config.Item("cType").GetValue<StringList>().SelectedIndex == 0) // Advantage Hikigaya Combo
            {
                if (Q.IsReady() && useQ && enemy.Distance(Player.Position) <= Q.Range)
                {
                    if (enemy.Distance(Player.Position) <= 300 && Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                    {
                        Q.Cast(enemy.Position + 150);
                    }
                    if (enemy.Distance(Player.Position) >= 300 && Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                    {
                        Q.Cast(enemy);
                    }
                }
                if (W.IsReady() && useW && enemy.Distance(Player.Position) <= W.Range)
                {
                    W.Cast();
                }
                if (E.IsReady() && useE && enemy.Distance(Player.Position) <= E.Range - 50)
                {
                    E.Cast();
                }
                if (R.IsReady() && useR && enemy.Distance(Player.Position) <= R.Range &&
                    Player.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health && !enemy.IsZombie)
                {
                    R.Cast(enemy);
                }
                if (enemy.Health <= GetComboDamage(enemy))
                {
                    Player.Spellbook.CastSpell(igniteSlot, enemy);
                }
            }
        }
        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent > Config.Item("manaHarass").GetValue<Slider>().Value)
            {
                if (Config.Item("hType").GetValue<StringList>().SelectedIndex == 0) // Basic
                {
                    BasicHarass();
                }
            }
        }
        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent > Config.Item("jungleMana").GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mobs == null || (mobs != null && mobs.Count == 0))
                {
                    return;
                }
                if (Q.IsReady() && Config.Item("qJungle").GetValue<bool>())
                {
                    Q.Cast(mobs[0]);
                }
                if (W.IsReady() && Config.Item("wJungle").GetValue<bool>())
                {
                    W.Cast(mobs[0]);
                }
            }
        }
        private static void LaneClear()
        {
            var useQ = Config.Item("qClear").GetValue<bool>();
            var useW = Config.Item("wClear").GetValue<bool>();
            var minionCount = Config.Item("qMinionHits").GetValue<Slider>().Value;

            if (ObjectManager.Player.ManaPercent > Config.Item("manaClear").GetValue<Slider>().Value)
            {
                var mins = MinionManager.GetMinions(1000);

                if (mins.Count <= 0)
                {
                    return;
                }

                if (Q.IsReady() && useQ)
                {
                    var qPos = Q.GetCircularFarmLocation(mins);

                    if (qPos.MinionsHit >= minionCount)
                        Q.Cast(qPos.Position);
                }

                if (W.IsReady() && useW)
                {
                    if (mins.OrderBy(x => x.Distance(Player.Position)).FirstOrDefault().Distance(Player.Position) <= W.Range)
                    {
                        W.Cast();
                    }
                }
            }

        }
        private static void LastHit()
        {
            if (ObjectManager.Player.ManaPercent > Config.Item("manaLast").GetValue<Slider>().Value)
            {
                if (Q.IsReady() && Config.Item("qLast").GetValue<bool>())
                {
                    var qMinion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);
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
        private static void BasicHarass()
        {
            var useQ = Config.Item("qHarass").GetValue<bool>();
            var useW = Config.Item("wHarass").GetValue<bool>();
            var useR = Config.Item("rHarass").GetValue<bool>();
            var enemy = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() && useQ && enemy.Distance(Player.Position) <= Q.Range)
            {
                if (enemy.Distance(Player.Position) <= 300 && Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                {
                    Q.Cast(enemy.Position + 150);
                }
                if (enemy.Distance(Player.Position) >= 300 && Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                {
                    Q.Cast(enemy);
                }
            }

            if (R.IsReady() && useR && enemy.Distance(Player.Position) <= R.Range && enemy.HasBuff("dianamoonlight"))
            {
                R.Cast(enemy);
            }
            if (W.IsReady() && useW && enemy.Distance(Player.Position) < W.Range - 50)
            {
                W.Cast();
            }
        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Config.Item("ainterrupt").GetValue<bool>())
            {
                if (sender.IsValidTarget(1000))
                {
                    Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                    var targetpos = Drawing.WorldToScreen(sender.Position);
                    Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
                }
                if (E.CanCast(sender))
                {
                    E.Cast(sender);
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("agapcloser").GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(1000))
                {
                    Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                    var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                    Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
                }
                if (W.CanCast(gapcloser.Sender))
                {
                    W.Cast(gapcloser.Sender);
                }
            }
        }
        private static float GetComboDamage(AIHeroClient hero) // Q+W+R+AA2+LICH+PASSIVE
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
            if (R.IsReady())
            {
                damage += R.GetDamage(hero);
            }
            damage += Player.GetAutoAttackDamage(hero, true) * 2;
            if (Player.HasBuff("dianaarcready"))
            {
                damage += 15 + 5 * Player.Level;
            }
            if (Player.HasBuff("LichBane"))
            {
                damage += Player.BaseAttackDamage * 0.75 + Player.FlatMagicDamageMod * 0.5;
            }
            return (float)damage;
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = Config.Item("eDraw").GetValue<Circle>();
            var menuItem4 = Config.Item("rDraw").GetValue<Circle>();

            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.SpringGreen);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Crimson);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.White);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Yellow);
            }
            if (Config.Item("hitChanceDraw").GetValue<bool>() && Q.IsReady())
            {
                var enemy = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                {
                    var yx = Drawing.WorldToScreen(enemy.Position);
                    Drawing.DrawText(yx[0], yx[1], System.Drawing.Color.SpringGreen, "Q Hitchance >= High");
                }
            }

        }
    }
}
