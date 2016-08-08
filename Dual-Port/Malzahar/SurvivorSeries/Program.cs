using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SharpDX;
using SPrediction;
using SebbyLib;
using Color = System.Drawing.Color;

using EloBuddy; namespace SurvivorMalzahar
{
    class Program
    {
        public const string ChampionName = "Malzahar";
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static bool IsChanneling;
        private static SebbyLib.Orbwalking.Orbwalker Orbwalker;
        //Menu
        public static Menu Menu;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        private static float Rtime = 0;
        public static Spell Q, W, E, R;
        private const float SpellQWidth = 400f;
        public static SpellSlot igniteSlot;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Malzahar") return;

            igniteSlot = Player.LSGetSpellSlot("summonerdot");
            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 760f);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R, 700f);

            Q.SetSkillshot(0.75f, 80, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 80, 20, false, SkillshotType.SkillshotCircle);

            Menu = new Menu("SurvivorMalzahar", "SurvivorMalzahar", true);
            var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new SebbyLib.Orbwalking.Orbwalker(orbwalkerMenu);
            #region Combo/Harass/LaneClear/OneShot
            //Combo Menu
            var combo = new Menu("Combo", "Combo");
            Menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("Combo", "Combo"));
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("DontAAInCombo", "Don't AA while doing Combo").SetValue(true));
            combo.AddItem(new MenuItem("useIgniteInCombo", "Use Ignite if Killable").SetValue(true));
            //Harass Menu
            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("autoharass", "Auto Harrass with E").SetValue(true));
            harass.AddItem(new MenuItem("autoharassuseQ", "Auto Harrass with Q").SetValue(false));
            harass.AddItem(new MenuItem("autoharassminimumMana", "Minimum Mana%").SetValue(new Slider(30)).SetTooltip("Minimum Mana that you need to have to AutoHarass with Q/E."));
            //LaneClear Menu
            var lc = new Menu("Laneclear", "Laneclear");
            Menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearW", "Use W to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("LaneClearMinions", "LaneClear Minimum Minions for Q").SetValue(new Slider(2, 0, 10)));
            lc.AddItem(new MenuItem("LaneClearEMinMinions", "LaneClear Minimum Minions for E").SetValue(new Slider(2, 0, 10)));
            lc.AddItem(new MenuItem("laneclearEMinimumMana", "Minimum E Mana%").SetValue(new Slider(30)).SetTooltip("Minimum Mana that you need to have to cast E on LaneClear."));
            lc.AddItem(new MenuItem("laneclearQMinimumMana", "Minimum Q Mana%").SetValue(new Slider(30)).SetTooltip("Minimum Mana that you need to have to cast Q on LaneClear."));
            lc.AddItem(new MenuItem("laneclearWMinimumMana", "Minimum W Mana%").SetValue(new Slider(30)).SetTooltip("Minimum Mana that you need to have to cast W on LaneClear."));

            // Drawing Menu
            var DrawingMenu = new Menu("Drawings", "Drawings");
            Menu.AddSubMenu(DrawingMenu);
            DrawingMenu.AddItem(new MenuItem("drawQ", "Draw Q range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawW", "Draw W range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawE", "Draw E range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawR", "Draw R range").SetValue(true));
            // Misc Menu
            var miscMenu = new Menu("Misc", "Misc");
            Menu.AddSubMenu(miscMenu);
            // Todo: Add more KillSteal Variants/Spells
            miscMenu.AddItem(new MenuItem("ksE", "Use E to KillSteal").SetValue(true));
            miscMenu.AddItem(new MenuItem("ksQ", "Use Q to KillSteal").SetValue(true));
            miscMenu.AddItem(new MenuItem("interruptQ", "Interrupt Spells Q", true).SetValue(true));
            miscMenu.AddItem(new MenuItem("useQAntiGapCloser", "Use Q on GapClosers").SetValue(true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                miscMenu.SubMenu("GapCloser R").AddItem(new MenuItem("gapcloserR" + enemy.ChampionName, enemy.ChampionName).SetValue(false).SetTooltip("Use R on GapClosing Champions"));
            miscMenu.AddItem(new MenuItem("OneShotInfo", "OneShot Combo [Info]").SetTooltip("If you don't have mana to cast Q/W/E/R spells all together it won't cast the spells. Use Combo Instead."));
            miscMenu.AddItem(new MenuItem("oneshot", "Burst Combo").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)).SetTooltip("It will cast Q+E+W+R on enemy when enemy is in E range."));
            Menu.AddToMainMenu();
            #endregion
            // Draw Damage
            #region DrawHPDamage
            var dmgAfterShave = new MenuItem("SurvivorMalzahar.DrawComboDamage", "Draw Combo Damage").SetValue(true);
            var drawFill =
                new MenuItem("SurvivorMalzahar.DrawColour", "Fill Color", true).SetValue(
                    new Circle(true, Color.FromArgb(204, 255, 0, 1)));
            DrawingMenu.AddItem(drawFill);
            DrawingMenu.AddItem(dmgAfterShave);
            DrawDamage.DamageToUnit = CalculateDamage;
            DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
            DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
            dmgAfterShave.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            #endregion
            SPrediction.Prediction.Initialize();
            #region Subscriptions
            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
            Chat.Print("<font color='#800040'>[SurvivorSeries] Malzahar</font> <font color='#ff6600'>Loaded.</font>");
            #endregion
        }
        private static void OnDraw(EventArgs args)
        {
            if (Menu.Item("drawQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.DarkRed, 3);
            }
            if (Menu.Item("drawW").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, 450f, System.Drawing.Color.LightBlue, 3);
            }
            if (Menu.Item("drawR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Purple, 3);
            }
            if (Menu.Item("drawE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.LightPink, 3);
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.LSIsRecalling())
            {
                return;
            }

            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.LSHasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            else
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }
            if (E.LSIsReady() && Menu.Item("ksE").GetValue<bool>())
            {
                foreach (var h in HeroManager.Enemies.Where(h => h.LSIsValidTarget(E.Range) && h.Health < SebbyLib.OktwCommon.GetKsDamage(h, E) + SebbyLib.OktwCommon.GetEchoLudenDamage(h)))
                {
                    E.Cast(h);
                }
            }
            if (Q.LSIsReady() && Menu.Item("ksQ").GetValue<bool>())
            {
                foreach (var h in HeroManager.Enemies.Where(h => h.LSIsValidTarget(Q.Range) && h.Health < SebbyLib.OktwCommon.GetKsDamage(h, Q) + SebbyLib.OktwCommon.GetEchoLudenDamage(h)))
                {
                    #region SebbyPrediction
                    //SebbyPrediction
                    SebbyLib.Prediction.SkillshotType PredSkillShotType = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                    bool Aoe10 = true;

                    var predictioninput = new SebbyLib.Prediction.PredictionInput
                    {
                        Aoe = Aoe10,
                        Collision = Q.Collision,
                        Speed = Q.Speed,
                        Delay = Q.Delay,
                        Range = Q.Range,
                        From = Player.ServerPosition,
                        Radius = Q.Width,
                        Unit = h,
                        Type = PredSkillShotType
                    };
                    //SebbyPrediction END
                    #endregion
                    // Input = 'var predictioninput'
                    var predpos = SebbyLib.Prediction.Prediction.GetPrediction(predictioninput);
                    if (predpos.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        Q.Cast(predpos.CastPosition);
                    }
                }
            }
            //Combo
            if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Combo)
            {
                if (Menu.Item("DontAAInCombo").GetValue<bool>())
                {
                    Orbwalker.SetAttack(false);
                }
                else
                {
                    Orbwalker.SetAttack(true);
                }
                Combo();
            }
            //Burst
            if (Menu.Item("oneshot").GetValue<KeyBind>().Active)
            {
                Oneshot();
            }
            //Lane
            if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
            }
            //AutoHarass
            AutoHarass();
        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.LSHasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            else
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }
            if (!Menu.Item("interruptQ", true).GetValue<bool>() || !Q.LSIsReady())
                return;

            if (t.LSIsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }
        }

        #region Q Range/Placement Calculations (BETA)
        /*private void CastQ(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!Q.LSIsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            Q.Width = GetDynamicQWidth(target);
            Q.Cast(target);
        }
        public static float GetManaPercent()
        {
            return (ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100f;
        }
        private static float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - (ObjectManager.Player.LSDistance(target) / Q.Range)) * SpellQWidth);
        }*/
        #endregion

        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.LSHasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            else
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }
            // Improved AntiGap Closer
            var sender = gapcloser.Sender;
            if (!gapcloser.Sender.LSIsValidTarget())
            {
                return;
            }

            if (Menu.Item("useQAntiGapCloser").GetValue<bool>() && sender.LSIsValidTarget(Q.Range))
            {
                Q.Cast(gapcloser.End);
            }
            if (R.LSIsReady() && Menu.Item("gapcloserR" + gapcloser.Sender.ChampionName).GetValue<bool>() && sender.LSIsValidTarget(R.Range) && gapcloser.End == Player.ServerPosition)
            {
                R.CastOnUnit(sender);
            }
        }
        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
            if (igniteSlot != SpellSlot.Unknown || Player.Spellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
            {
                if (Menu.Item("useIgniteInCombo").GetValue<bool>())
                {
                    damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
                }
            }
            double ultdamage = 0;

            if (Q.LSIsReady())
            {
                damage += Q.GetDamage(enemy);
            }

            if (W.LSIsReady())
            {
                damage += W.GetDamage(enemy);
            }

            if (E.LSIsReady())
            {
                damage += E.GetDamage(enemy);
            }

            if (R.LSIsReady())
            {
                ultdamage += Player.LSGetSpellDamage(enemy, SpellSlot.R);
            }
            return damage + ((float)ultdamage * 2);
        }
        private static void AutoHarass()
        {
            if (Player.ManaPercentage() < Menu.Item("autoharassminimumMana").GetValue<Slider>().Value)
                return;
            var m = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (m == null || !m.LSIsValidTarget())
                return;
            #region SebbyPrediction
            //SebbyPrediction
            SebbyLib.Prediction.SkillshotType PredSkillShotType = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
            bool Aoe10 = true;

            var predictioninput = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = Aoe10,
                Collision = Q.Collision,
                Speed = Q.Speed,
                Delay = Q.Delay,
                Range = Q.Range,
                From = Player.ServerPosition,
                Radius = Q.Width,
                Unit = m,
                Type = PredSkillShotType
            };
            //SebbyPrediction END
            #endregion
            // Input = 'var predictioninput'
            var predpos = SebbyLib.Prediction.Prediction.GetPrediction(predictioninput);
            if (m != null && Menu.Item("autoharass").GetValue<bool>())
                    E.CastOnUnit(m);
            if (m != null && Menu.Item("autoharassuseQ").GetValue<bool>())
                if (predpos.Hitchance >= SebbyLib.Prediction.HitChance.High)
                {
                    Q.Cast(predpos.CastPosition);
                }
        }
        private static bool HasRBuff()
        {
            return (Player.IsChannelingImportantSpell() || Player.LSHasBuff("AiZaharNetherGrasp") || Player.LSHasBuff("MalzaharR") || Player.LSHasBuff("MalzaharRSound") || R.IsChanneling);
        }
        //Combo
        private static void Combo()
        {
            var useQ = (Menu.Item("useQ").GetValue<bool>());
            var useW = (Menu.Item("useW").GetValue<bool>());
            var useE = (Menu.Item("useE").GetValue<bool>());
            var useR = (Menu.Item("useR").GetValue<bool>());
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (m == null || !m.LSIsValidTarget())
                return;
            #region SebbyPrediction
            //SebbyPrediction
            SebbyLib.Prediction.SkillshotType PredSkillShotType = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
            bool Aoe10 = true;

            var predictioninput = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = Aoe10,
                Collision = Q.Collision,
                Speed = Q.Speed,
                Delay = Q.Delay,
                Range = Q.Range,
                From = Player.ServerPosition,
                Radius = Q.Width,
                Unit = m,
                Type = PredSkillShotType
            };
            //SebbyPrediction END
            #endregion
            // Input = 'var predictioninput'
            var predpos = SebbyLib.Prediction.Prediction.GetPrediction(predictioninput);
            if (Player.Mana > E.ManaCost + W.ManaCost + R.ManaCost)
            {
                if (useQ && Q.LSIsReady() && Player.Mana > Q.ManaCost && Q.IsInRange(m))
                {
                    if (m.CanMove && predpos.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        Q.Cast(predpos.CastPosition);
                    }
                    else if (!m.CanMove)
                    {
                        Q.Cast(m.Position);
                    }
                }
                if (useW && W.LSIsReady()) W.Cast(m);
                if (useE && E.LSIsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (useR && R.LSIsReady() && !W.LSIsReady() && !E.LSIsReady() && m != null && E.IsInRange(m)) R.CastOnUnit(m);
            }
            else
            {
                if (useE && E.LSIsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (useQ && Q.LSIsReady() && Player.Mana > Q.ManaCost && Q.IsInRange(m))
                {
                    if (m.CanMove && predpos.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        Q.Cast(predpos.CastPosition);
                    }
                    else if (!m.CanMove)
                    {
                        Q.Cast(m.Position);
                    }
                }
                if (useW && W.LSIsReady() && Player.Mana > W.ManaCost && W.IsInRange(m)) W.Cast(m);
            }
            if (Menu.Item("useIgniteInCombo").GetValue<bool>())
            {
                if (m.Health < Player.GetSummonerSpellDamage(m, Damage.SummonerSpell.Ignite))
                {
                    Player.Spellbook.CastSpell(igniteSlot, m);
                }
            }
        }
        //Burst
        public static void Oneshot()
        {
            // If player doesn't have mana don't execute the OneShot Combo
            if (Player.Mana < Q.ManaCost + W.ManaCost + E.ManaCost + R.ManaCost)
                return;


            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.LSHasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            else
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }

            SebbyLib.Orbwalking.MoveTo(Game.CursorPos);
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!m.LSIsValidTarget())
            {
                return;
            }
            #region SebbyPrediction
            //SebbyPrediction
            SebbyLib.Prediction.SkillshotType PredSkillShotType = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
            bool Aoe10 = true;

            var predictioninput = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = Aoe10,
                Collision = Q.Collision,
                Speed = Q.Speed,
                Delay = Q.Delay,
                Range = Q.Range,
                From = Player.ServerPosition,
                Radius = Q.Width,
                Unit = m,
                Type = PredSkillShotType
            };
            //SebbyPrediction END
            #endregion
            // Input = 'var predictioninput'
            var predpos = SebbyLib.Prediction.Prediction.GetPrediction(predictioninput);
            // var pred = Q.GetSPrediction(m);
                if (Q.LSIsReady() && Q.IsInRange(m))
                {
                    if (m.CanMove && predpos.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        Q.Cast(predpos.CastPosition);
                    }
                    else if (!m.CanMove)
                    {
                        Q.Cast(m.Position);
                    }
                }
                if (E.LSIsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (W.LSIsReady()) W.Cast(m);
                Player.Spellbook.CastSpell(igniteSlot, m);
                if (R.LSIsReady() && !E.LSIsReady() && !W.LSIsReady() && R.IsInRange(m)) R.CastOnUnit(m);
        }
        //Lane
        private static void Lane()
        {
            if (Player.ManaPercentage() < Menu.Item("laneclearEMinimumMana").GetValue<Slider>().Value || Player.ManaPercentage() < Menu.Item("laneclearQMinimumMana").GetValue<Slider>().Value || Player.ManaPercentage() < Menu.Item("laneclearWMinimumMana").GetValue<Slider>().Value)
                return;

            var infectedminion = MinionManager.GetMinions(Player.Position, E.Range).Find(x => x.HasBuff("malzahare") && x.LSIsValidTarget(E.Range));
            //var allMinions = Cache.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTeam.Enemy);
            //var allMinionsW = Cache.GetMinions(ObjectManager.Player.ServerPosition, 450f, MinionTeam.Enemy);
            var allMinions = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy);
            var allMinionsW = MinionManager.GetMinions(450f, MinionTypes.All, MinionTeam.Enemy);
            if (allMinionsW.Count > 1)
            {
                if (infectedminion != null) // Replaced Sebby with Common
                {
                    Orbwalker.ForceTarget(infectedminion);
                }
                else
                {
                    Orbwalker.ForceTarget(null);
                }
            }
            if (allMinions.Count > Menu.Item("LaneClearEMinMinions").GetValue<Slider>().Value)
            {
                if (Menu.Item("laneclearE").GetValue<bool>() && E.LSIsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.LSIsValidTarget() && !minion.LSHasBuff("malzahare") && minion.Health < E.GetDamage(minion))
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
            if (Menu.Item("laneclearW").GetValue<bool>() && W.LSIsReady())
            {
                foreach (var minion in allMinionsW)
                {
                    if (minion.LSIsValidTarget())
                    {
                        W.Cast(minion);
                    }
                }
            }
            if (Menu.Item("laneclearQ").GetValue<bool>() && Q.LSIsReady())
            {
                var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
                var farmPos = Q.GetCircularFarmLocation(allMinionsQ, 150);
                if (farmPos.MinionsHit > Menu.Item("LaneClearMinions").GetValue<Slider>().Value)
                    Q.Cast(farmPos.Position);
            }
        }
    }
}
