// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SurvivorMalzahar">
//      Copyright (c) SurvivorMalzahar. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using Color = SharpDX.Color;
using HitChance = SebbyLib.Prediction.HitChance;
using Orbwalking = SebbyLib.Orbwalking;
using Prediction = SebbyLib.Prediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;

using EloBuddy; 
using LeagueSharp.Common; 
namespace SurvivorMalzahar
{
    internal class Program
    {
        public const string ChampionName = "Malzahar";

        private static Orbwalking.Orbwalker Orbwalker;
        //Menu
        public static Menu Menu;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        private static readonly float Rtime = 0;
        public static Spell Q, W, E, R;
        public static SpellSlot igniteSlot;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Malzahar") return;

            igniteSlot = Player.GetSpellSlot("summonerdot");
            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 760f);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R, 700f);

            Q.SetSkillshot(0.75f, 80, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 80, 20, false, SkillshotType.SkillshotCircle);

            Menu = new Menu("SurvivorMalzahar", "SurvivorMalzahar", true);
            var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

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
            harass.AddItem(new MenuItem("autoharassenabled", "Auto Harrass Enabled?").SetValue(true));
            harass.AddItem(new MenuItem("autoharass", "Auto Harrass with E").SetValue(true));
            harass.AddItem(new MenuItem("autoharassuseQ", "Auto Harrass with Q").SetValue(false));
            harass.AddItem(
                new MenuItem("autoharassminimumMana", "Minimum Mana%").SetValue(new Slider(30))
                    .SetTooltip("Minimum Mana that you need to have to AutoHarass with Q/E."));
            //LaneClear Menu
            var lc = new Menu("Laneclear", "Laneclear");
            Menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearW", "Use W to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("LaneClearMinions", "LaneClear Minimum Minions for Q").SetValue(new Slider(2, 0, 10)));
            lc.AddItem(
                new MenuItem("LaneClearEMinMinions", "LaneClear Minimum Minions for E").SetValue(new Slider(2, 0, 10)));
            lc.AddItem(
                new MenuItem("laneclearEMinimumMana", "Minimum E Mana%").SetValue(new Slider(30))
                    .SetTooltip("Minimum Mana that you need to have to cast E on LaneClear."));
            lc.AddItem(
                new MenuItem("laneclearQMinimumMana", "Minimum Q Mana%").SetValue(new Slider(30))
                    .SetTooltip("Minimum Mana that you need to have to cast Q on LaneClear."));
            lc.AddItem(
                new MenuItem("laneclearWMinimumMana", "Minimum W Mana%").SetValue(new Slider(30))
                    .SetTooltip("Minimum Mana that you need to have to cast W on LaneClear."));

            //JungleClear Menu
            var jgc = new Menu("JungleClear", "Jungleclear");
            Menu.AddSubMenu(jgc);
            jgc.AddItem(new MenuItem("jungleclearQ", "Use Q to JungleClear").SetValue(true));
            jgc.AddItem(new MenuItem("jungleclearW", "Use W to JungleClear").SetValue(true));
            jgc.AddItem(new MenuItem("jungleclearE", "Use E to JungleClear").SetValue(true));
            jgc.AddItem(
                new MenuItem("jungleclearManaManager", "JungleClear Mana Manager").SetValue(new Slider(30, 0, 100)));

            // Drawing Menu
            var DrawingMenu = new Menu("Drawings", "Drawings");
            Menu.AddSubMenu(DrawingMenu);
            DrawingMenu.AddItem(new MenuItem("drawQ", "Draw Q range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawW", "Draw W range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawE", "Draw E range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawR", "Draw R range").SetValue(true));

            #region Skin Changer

            var SkinChangerMenu = new Menu(":: Skin Changer", "SkinChanger").SetFontStyle(FontStyle.Bold,
                Color.Chartreuse);
            Menu.AddSubMenu(SkinChangerMenu);
            var SkinChanger =
                SkinChangerMenu.AddItem(
                    new MenuItem("UseSkinChanger", ":: Use SkinChanger?").SetValue(true)
                        .SetFontStyle(FontStyle.Bold, Color.Crimson));
            var SkinID =
                SkinChangerMenu.AddItem(
                    new MenuItem("SkinID", ":: Skin").SetValue(
                        new StringList(
                            new[]
                            {
                                "Classic", "Shadow Prince Malzahar", "Djinn Malzahar", "Overlord Malzahar",
                                "Vizier Malzahar", "Snow Day Malzahar"
                            }, 3)).SetFontStyle(FontStyle.Bold, Color.Crimson));
            SkinID.ValueChanged += (sender, eventArgs) =>
            {
                if (!SkinChanger.GetValue<bool>())
                    return;

                //Player.SetSkin(Player.BaseSkinName, eventArgs.GetNewValue<StringList>().SelectedIndex);
            };

            #endregion

            // Misc Menu
            var miscMenu = new Menu("Misc", "Misc");
            Menu.AddSubMenu(miscMenu);
            // Todo: Add more KillSteal Variants/Spells
            miscMenu.AddItem(
                new MenuItem("HitChance", "HitChance?").SetValue(new StringList(new[] {"Medium", "High", "Very High"}, 1)));
            miscMenu.AddItem(new MenuItem("ksE", "Use E to KillSteal").SetValue(true));
            miscMenu.AddItem(new MenuItem("ksQ", "Use Q to KillSteal").SetValue(true));
            miscMenu.AddItem(new MenuItem("interruptQ", "Interrupt Spells Q", true).SetValue(true));
            miscMenu.AddItem(new MenuItem("useQAntiGapCloser", "Use Q on GapClosers").SetValue(true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                miscMenu.SubMenu("GapCloser R")
                    .AddItem(
                        new MenuItem("gapcloserR" + enemy.ChampionName, enemy.ChampionName).SetValue(false)
                            .SetTooltip("Use R on GapClosing Champions"));
            miscMenu.AddItem(
                new MenuItem("OneShotInfo", "OneShot Combo [Info]").SetTooltip(
                    "If you don't have mana to cast Q/W/E/R spells all together it won't cast the spells. Use Combo Instead."));
            miscMenu.AddItem(
                new MenuItem("oneshot", "Burst Combo").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                    .SetTooltip("It will cast Q+E+W+R on enemy when enemy is in E range."));

            #endregion

            // Draw Damage

            #region DrawHPDamage

            var dmgAfterShave =
                DrawingMenu.AddItem(new MenuItem("SurvivorMalzahar.DrawComboDamage", "Draw Combo Damage").SetValue(true));
            var drawFill =
                DrawingMenu.AddItem(new MenuItem("SurvivorMalzahar.DrawColour", "Fill Color", true).SetValue(
                    new Circle(true, System.Drawing.Color.FromArgb(204, 255, 0, 1))));
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

            Menu.AddToMainMenu();

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
            if (Player.IsDead)
                return;

            if (Menu.Item("drawQ").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.DarkRed, 3);
            if (Menu.Item("drawW").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, 450f, System.Drawing.Color.LightBlue, 3);
            if (Menu.Item("drawR").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Purple, 3);
            if (Menu.Item("drawE").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.LightPink, 3);
        }

        private static void SebbySpell(Spell Q, Obj_AI_Base target)
        {
            var CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            var aoe2 = false;

            if (Q.Type == SkillshotType.SkillshotCircle)
            {
                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if ((Q.Width > 80) && !Q.Collision)
                aoe2 = true;

            var predInput2 = new PredictionInput
            {
                Aoe = aoe2,
                Collision = Q.Collision,
                Speed = Q.Speed,
                Delay = Q.Delay,
                Range = Q.Range,
                From = Player.ServerPosition,
                Radius = Q.Width,
                Unit = target,
                Type = CoreType2
            };
            var poutput2 = Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if ((Q.Speed != float.MaxValue) && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
            {
                if (poutput2.Hitchance >= HitChance.Medium)
                    Q.Cast(poutput2.CastPosition);
            }
            else if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
            {
                if (poutput2.Hitchance >= HitChance.High)
                    Q.Cast(poutput2.CastPosition);
            }
            else if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
            {
                if (poutput2.Hitchance >= HitChance.VeryHigh)
                    Q.Cast(poutput2.CastPosition);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            if (Player.IsChannelingImportantSpell() || (Game.Time - Rtime < 2.5) || Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);
            if (E.IsReady() && Menu.Item("ksE").GetValue<bool>())
                foreach (
                    var h in
                    HeroManager.Enemies.Where(
                        h =>
                            h.IsValidTarget(E.Range) &&
                            (h.Health < OktwCommon.GetKsDamage(h, E) + OktwCommon.GetEchoLudenDamage(h))))
                    E.Cast(h);
            if (Q.IsReady() && Menu.Item("ksQ").GetValue<bool>())
                foreach (
                    var h in
                    HeroManager.Enemies.Where(
                        h =>
                            h.IsValidTarget(Q.Range) &&
                            (h.Health < OktwCommon.GetKsDamage(h, Q) + OktwCommon.GetEchoLudenDamage(h))))
                    SebbySpell(Q, h);
            //Combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Menu.Item("DontAAInCombo").GetValue<bool>())
                    Orbwalker.SetAttack(false);
                else
                    Orbwalker.SetAttack(true);
                Combo();
            }
            //Burst
            if (Menu.Item("oneshot").GetValue<KeyBind>().Active)
                Oneshot();
            //Lane
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear();
                Lane();
            }
            //AutoHarass
            if (Menu.Item("autoharassenabled").GetValue<bool>())
                AutoHarass();
        }

        private static void JungleClear()
        {
            if (Player.ManaPercent < Menu.Item("jungleclearManaManager").GetValue<Slider>().Value)
                return;

            var jgcq = Menu.Item("jungleclearQ").GetValue<bool>();
            var jgcw = Menu.Item("jungleclearW").GetValue<bool>();
            var jgce = Menu.Item("jungleclearE").GetValue<bool>();

            var mob =
                MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (mob == null)
                return;

            if (jgcq && Q.IsReady())
                Q.CastOnUnit(mob);
            if (jgcw && W.IsReady())
                W.Cast(mob.Position);
            if (jgce && E.IsReady())
                E.CastOnUnit(mob);
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.IsChannelingImportantSpell() || (Game.Time - Rtime < 2.5) || Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);
            if (!Menu.Item("interruptQ", true).GetValue<bool>() || !Q.IsReady())
                return;

            if (t.IsValidTarget(Q.Range))
                Q.Cast(t);
        }

        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.IsChannelingImportantSpell() || (Game.Time - Rtime < 2.5) || Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);
            // Improved AntiGap Closer
            var sender = gapcloser.Sender;
            if (!gapcloser.Sender.IsValidTarget())
                return;

            if (Menu.Item("useQAntiGapCloser").GetValue<bool>() && sender.IsValidTarget(Q.Range))
                Q.Cast(gapcloser.End);
            if (R.IsReady() && Menu.Item("gapcloserR" + gapcloser.Sender.ChampionName).GetValue<bool>() &&
                sender.IsValidTarget(R.Range) && (gapcloser.End == Player.ServerPosition))
                R.CastOnUnit(sender);
        }

        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
            if ((igniteSlot != SpellSlot.Unknown) || (ObjectManager.Player.Spellbook.CanUseSpell(igniteSlot) == SpellState.Ready))
                if (Menu.Item("useIgniteInCombo").GetValue<bool>())
                    damage += (float) Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            double ultdamage = 0;

            if (Q.IsReady())
                damage += Q.GetDamage(enemy);

            if (W.IsReady())
                damage += W.GetDamage(enemy);

            if (E.IsReady())
                damage += E.GetDamage(enemy);

            if (R.IsReady())
                ultdamage += Player.GetSpellDamage(enemy, SpellSlot.R);
            return damage + (float) ultdamage*2;
        }

        private static void AutoHarass()
        {
            if (Player.ManaPercent < Menu.Item("autoharassminimumMana").GetValue<Slider>().Value)
                return;
            var m = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if ((m == null) || !m.IsValidTarget())
                return;

            // Input = 'var predictioninput'
            if ((m != null) && Menu.Item("autoharass").GetValue<bool>())
                E.CastOnUnit(m);
            if ((m != null) && Menu.Item("autoharassuseQ").GetValue<bool>())
                SebbySpell(Q, m);
        }

        private static bool HasRBuff()
        {
            return Player.IsChannelingImportantSpell() || Player.HasBuff("AiZaharNetherGrasp") ||
                   Player.HasBuff("MalzaharR") || Player.HasBuff("MalzaharRSound") || R.IsChanneling;
        }

        //Combo
        private static void Combo()
        {
            var useQ = Menu.Item("useQ").GetValue<bool>();
            var useW = Menu.Item("useW").GetValue<bool>();
            var useE = Menu.Item("useE").GetValue<bool>();
            var useR = Menu.Item("useR").GetValue<bool>();
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if ((m == null) || !m.IsValidTarget())
                return;

            if (useE && m.IsValidTarget(E.Range) && E.IsReady())
                E.CastOnUnit(m);
            if (useW && m.IsValidTarget(Player.AttackRange) && W.IsReady())
                W.CastOnUnit(m);
            if (useQ && m.IsValidTarget(Q.Range) && Q.IsReady())
                SebbySpell(Q, m);
            if (useR && m.IsValidTarget(R.Range) && R.IsReady() && m.HasBuff("malzahare"))
                R.CastOnUnit(m);

            /*if (Player.Mana > E.ManaCost + W.ManaCost + R.ManaCost)
            {
                if (useQ && Q.IsReady() && (Player.Mana > Q.ManaCost) && Q.IsInRange(m))
                    SebbySpell(Q, m);
                if (useW && W.IsReady()) W.Cast(m);
                if (useE && E.IsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (useR && R.IsReady() && !W.IsReady() && !E.IsReady() && E.IsInRange(m))
                    R.CastOnUnit(m);
            }
            else
            {
                if (useE && E.IsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (useQ && Q.IsReady() && (Player.Mana > Q.ManaCost) && Q.IsInRange(m))
                    SebbySpell(Q, m);
                if (useW && W.IsReady() && (Player.Mana > W.ManaCost) && W.IsInRange(m)) W.Cast(m);
            }*/ //NOTE: OLD COMBO
            if (Menu.Item("useIgniteInCombo").GetValue<bool>())
                if (m.Health < Player.GetSummonerSpellDamage(m, Damage.SummonerSpell.Ignite))
                    ObjectManager.Player.Spellbook.CastSpell(igniteSlot, m);
        }

        //Burst
        public static void Oneshot()
        {
            // If player doesn't have mana don't execute the OneShot Combo
            if (Player.Mana < Q.ManaCost + W.ManaCost + E.ManaCost + R.ManaCost)
                return;


            if (Player.IsChannelingImportantSpell() || (Game.Time - Rtime < 2.5) || Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

            Orbwalking.MoveTo(Game.CursorPos);
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!m.IsValidTarget())
                return;

            if (Q.IsReady() && Q.IsInRange(m))
                SebbySpell(Q, m);
            if (E.IsReady() && E.IsInRange(m)) E.CastOnUnit(m);
            if (W.IsReady()) W.Cast(m);
            ObjectManager.Player.Spellbook.CastSpell(igniteSlot, m);
            if (R.IsReady() && !E.IsReady() && !W.IsReady() && R.IsInRange(m)) R.CastOnUnit(m);
        }

        //Lane
        private static void Lane()
        {
            if ((Player.ManaPercent < Menu.Item("laneclearEMinimumMana").GetValue<Slider>().Value) ||
                (Player.ManaPercent < Menu.Item("laneclearQMinimumMana").GetValue<Slider>().Value) ||
                (Player.ManaPercent < Menu.Item("laneclearWMinimumMana").GetValue<Slider>().Value))
                return;

            var infectedminion =
                MinionManager.GetMinions(Player.Position, E.Range)
                    .Find(x => x.HasBuff("malzahare") && x.IsValidTarget(E.Range));
            //var allMinions = Cache.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTeam.Enemy);
            //var allMinionsW = Cache.GetMinions(ObjectManager.Player.ServerPosition, 450f, MinionTeam.Enemy);
            var allMinions = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy);
            var allMinionsW = MinionManager.GetMinions(450f, MinionTypes.All, MinionTeam.Enemy);
            if (allMinionsW.Count > 1)
                if (infectedminion != null) // Replaced Sebby with Common
                    Orbwalker.ForceTarget(infectedminion);
                else
                    Orbwalker.ForceTarget(null);
            if (allMinions.Count > Menu.Item("LaneClearEMinMinions").GetValue<Slider>().Value)
                if (Menu.Item("laneclearE").GetValue<bool>() && E.IsReady())
                    foreach (var minion in allMinions)
                        if (minion.IsValidTarget() && !minion.HasBuff("malzahare") &&
                            (minion.Health < E.GetDamage(minion)))
                            E.CastOnUnit(minion);
            if (Menu.Item("laneclearW").GetValue<bool>() && W.IsReady())
                foreach (var minion in allMinionsW)
                    if (minion.IsValidTarget())
                        W.Cast(minion);
            if (Menu.Item("laneclearQ").GetValue<bool>() && Q.IsReady())
            {
                var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
                var farmPos = Q.GetCircularFarmLocation(allMinionsQ, 150);
                if (farmPos.MinionsHit > Menu.Item("LaneClearMinions").GetValue<Slider>().Value)
                    Q.Cast(farmPos.Position);
            }
        }
    }
}