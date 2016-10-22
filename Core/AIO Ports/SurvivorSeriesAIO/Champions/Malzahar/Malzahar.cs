// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Malzahar.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SurvivorSeriesAIO.Core;
using SurvivorSeriesAIO.SurvivorMain;
using SurvivorSeriesAIO.Utility;
using Orbwalking = SebbyLib.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Champions
{
    public class Malzahar : ChampionBase
    {
        private const float SpellQWidth = 400f;

        private readonly float Rtime = 0;

        public List<Spell> SpellList = new List<Spell>();

        public Malzahar(IRootMenu menu, Orbwalking.Orbwalker Orbwalker)
            : base(menu, Orbwalker)
        {
            // manual override - default is spell values from LeagueSharp.Data
            Q.SetSkillshot(0.75f, 80, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 80, 20, false, SkillshotType.SkillshotCircle);

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;

            Config = new Configuration(menu.Champion);
            InitHpbarOverlay();
        }

        public Configuration Config { get; }

        // Burst
        public void Oneshot()
        {
            // If player doesn't have mana don't execute the OneShot Combo
            if (Player.Mana < Q.ManaCost + W.ManaCost + E.ManaCost + R.ManaCost)
                return;

            if (Player.IsChannelingImportantSpell() || (Game.Time - Rtime < 2.5) ||
                Player.HasBuff("malzaharrsound"))
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
                SpellCast.SebbySpellMain(Q, m);

            if (E.IsReady() && E.IsInRange(m))
                E.CastOnUnit(m);

            if (W.IsReady())
                W.Cast(m);

            if (R.IsReady() && !E.IsReady() && !W.IsReady() && R.IsInRange(m))
                R.CastOnUnit(m);
        }

        private void AutoHarass()
        {
            if (Player.ManaPercent < Config.HarassMana.GetValue<Slider>().Value)
                return;

            var m = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if ((m == null) || !m.IsValidTarget())
                return;

            if ((m != null) && Config.HarassE.GetValue<bool>())
                E.CastOnUnit(m);

            if ((m != null) && Config.HarassQ.GetValue<bool>())
                SpellCast.SebbySpellMain(Q, m);
        }

        private float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
            /*if (Activator.Config.Item("UseIgnite").GetValue<bool>())
            {
                damage += (float)this.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }*/

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

        // Combo
        private void Combo()
        {
            var useQ = Config.UseQ.GetValue<bool>();
            var useW = Config.UseW.GetValue<bool>();
            var useE = Config.UseE.GetValue<bool>();
            var useR = Config.UseR.GetValue<bool>();
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if ((m == null) || !m.IsValidTarget())
                return;

            if (Player.Mana > E.ManaCost + W.ManaCost + R.ManaCost)
            {
                if (useQ && Q.IsReady() && (Player.Mana > Q.ManaCost) && Q.IsInRange(m))
                    SpellCast.SebbySpellMain(Q, m);

                if (useW && W.IsReady())
                    W.Cast(m);

                if (useE && E.IsReady() && E.IsInRange(m))
                    E.CastOnUnit(m);

                if (useR && R.IsReady() && !W.IsReady() && !E.IsReady() && (m != null) &&
                    E.IsInRange(m))
                    R.CastOnUnit(m);
            }
            else
            {
                if (useE && E.IsReady() && E.IsInRange(m))
                    E.CastOnUnit(m);

                if (useQ && Q.IsReady() && (Player.Mana > Q.ManaCost) && Q.IsInRange(m))
                    SpellCast.SebbySpellMain(Q, m);

                if (useW && W.IsReady() && (Player.Mana > W.ManaCost) && W.IsInRange(m))
                    W.Cast(m);
            }
        }

        private bool HasRBuff()
        {
            return Player.IsChannelingImportantSpell() || Player.HasBuff("AiZaharNetherGrasp") ||
                   Player.HasBuff("MalzaharR") || Player.HasBuff("MalzaharRSound") || R.IsChanneling;
        }

        private void InitHpbarOverlay()
        {
            DrawDamage.DamageToUnit = CalculateDamage;
            DrawDamage.Enabled = () => Config.DrawComboDamage.GetValue<bool>();
            DrawDamage.Fill = () => Config.FillColor.GetValue<Circle>().Active;
            DrawDamage.FillColor = () => Config.FillColor.GetValue<Circle>().Color;
        }

        // Lane
        private void Lane()
        {
            var infectedcreatures =
                Cache.GetMinions(Player.Position, Player.AttackRange, MinionTeam.All).Where(x => x.HasBuff("malzahare"));
            if (infectedcreatures != null)
                foreach (var ic in infectedcreatures)
                    if (ic != null)
                        Orbwalker.ForceTarget(ic);

            if (Player.ManaPercent < Config.laneclearMinimumMana.GetValue<Slider>().Value)
                return;

            var infectedminion =
                MinionManager.GetMinions(Player.Position, E.Range)
                    .Find(x => x.HasBuff("malzahare") && x.IsValidTarget(E.Range));
            var allMinions = Cache.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTeam.Enemy);
            var allMinionsW = Cache.GetMinions(ObjectManager.Player.ServerPosition, 450f, MinionTeam.Enemy);
            if (allMinionsW.Count > 1)
                if (infectedminion != null)
                    Orbwalker.ForceTarget(infectedminion);
                else
                    Orbwalker.ForceTarget(null);

            if (allMinions.Count > Config.LaneClearEMinMinions.GetValue<Slider>().Value)
                if (Config.laneclearE.GetValue<bool>() && E.IsReady())
                    foreach (var minion in allMinions)
                        if (minion.IsValidTarget() && !minion.HasBuff("malzahare") &&
                            (minion.Health < E.GetDamage(minion)))
                            E.CastOnUnit(minion);

            if (Config.laneclearW.GetValue<bool>() && W.IsReady())
                foreach (var minion in allMinionsW)
                    if (minion.IsValidTarget())
                        W.Cast(minion);

            if (Config.laneclearQ.GetValue<bool>() && Q.IsReady())
            {
                var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
                var farmPos = Q.GetCircularFarmLocation(allMinionsQ, 150);
                if (farmPos.MinionsHit > Config.LaneClearMinions.GetValue<Slider>().Value)
                    Q.Cast(farmPos.Position);
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (Config.drawQ.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.DarkRed, 3);

            if (Config.drawW.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, 450f, Color.LightBlue, 3);

            if (Config.drawR.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Purple, 3);

            if (Config.drawE.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.LightPink, 3);
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.IsChannelingImportantSpell() || (Game.Time - Rtime < 2.5) ||
                Player.HasBuff("malzaharrsound"))
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

            if (Config.useQAntiGapCloser.GetValue<bool>() && sender.IsValidTarget(Q.Range))
                Q.Cast(gapcloser.End);

            /*if (this.R.IsReady() && this.Menu.Item("gapcloserR" + gapcloser.Sender.ChampionName).GetValue<bool>() &&
                sender.IsValidTarget(this.R.Range) && gapcloser.End == this.Player.ServerPosition)
            {
                this.R.CastOnUnit(sender);
            }*/
        }

        private void OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.IsChannelingImportantSpell() || (Game.Time - Rtime < 2.5) ||
                Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

            if (!Config.interruptQ.GetValue<bool>() || !Q.IsReady())
                return;

            if (t.IsValidTarget(Q.Range))
                Q.Cast(t.Position);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            if (Player.IsChannelingImportantSpell() || (Game.Time - Rtime < 2.5) ||
                Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

            //new Activator().SeraphUsage();
            //new Activator().ProHexGLPUsage();
            if (E.IsReady() && Config.ksE.GetValue<bool>())
                foreach (
                    var h in
                    HeroManager.Enemies.Where(
                        h =>
                            h.IsValidTarget(E.Range) &&
                            (h.Health < OktwCommon.GetKsDamage(h, E) + OktwCommon.GetEchoLudenDamage(h))))
                    E.Cast(h);

            if (Q.IsReady() && Config.ksQ.GetValue<bool>())
                foreach (
                    var h in
                    HeroManager.Enemies.Where(
                        h =>
                            h.IsValidTarget(Q.Range) &&
                            (h.Health < OktwCommon.GetKsDamage(h, Q) + OktwCommon.GetEchoLudenDamage(h))))
                    SpellCast.SebbySpellMain(Q, h);

            // Combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Config.DontAAInCombo.GetValue<bool>())
                    Orbwalker.SetAttack(false);
                else
                    Orbwalker.SetAttack(true);

                Combo();
            }

            // Burst
            if (Config.oneshot.GetValue<KeyBind>().Active)
                Oneshot();

            // Lane
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                Lane();

            // AutoHarass
            AutoHarass();
        }

        public class Configuration
        {
            public Configuration(Menu root)
            {
                ComboMenu = MenuFactory.CreateMenu(root, "Combo");
                HarassMenu = MenuFactory.CreateMenu(root, "Harass");
                LaneClearMenu = MenuFactory.CreateMenu(root, "Lane Clear");
                MiscMenu = MenuFactory.CreateMenu(root, "Misc");
                DrawingMenu = MenuFactory.CreateMenu(root, "Drawing");

                Combos(MenuItemFactory.Create(ComboMenu));
                Harass(MenuItemFactory.Create(HarassMenu));
                LaneClear(MenuItemFactory.Create(LaneClearMenu));
                Misc(MenuItemFactory.Create(MiscMenu));
                Drawings(MenuItemFactory.Create(DrawingMenu));
            }

            public Menu ComboMenu { get; }

            public MenuItem DontAAInCombo { get; private set; }

            public MenuItem DrawComboDamage { get; private set; }

            public Menu DrawingMenu { get; }

            public MenuItem FillColor { get; private set; }

            public MenuItem HarassE { get; set; }

            public MenuItem HarassMana { get; set; }

            public Menu HarassMenu { get; }

            public MenuItem HarassQ { get; set; }

            public Menu LaneClearMenu { get; }

            public Menu MiscMenu { get; }

            public MenuItem UseE { get; private set; }

            public MenuItem UseQ { get; private set; }

            public MenuItem UseR { get; private set; }

            public MenuItem UseW { get; private set; }

            public MenuItem drawQ { get; private set; }

            public MenuItem drawW { get; private set; }

            public MenuItem drawE { get; private set; }

            public MenuItem drawR { get; private set; }

            public MenuItem laneclearE { get; private set; }

            public MenuItem laneclearQ { get; private set; }

            public MenuItem laneclearW { get; private set; }

            public MenuItem LaneClearMinions { get; private set; }

            public MenuItem LaneClearEMinMinions { get; private set; }

            public MenuItem laneclearMinimumMana { get; private set; }

            public MenuItem ksE { get; private set; }

            public MenuItem ksQ { get; private set; }

            public MenuItem interruptQ { get; private set; }

            public MenuItem useQAntiGapCloser { get; private set; }

            public MenuItem OneShotInfo { get; private set; }

            public MenuItem oneshot { get; private set; }

            private void Combos(MenuItemFactory factory)
            {
                UseQ = factory.WithName("Use Q").WithValue(true).Build();
                UseW = factory.WithName("Use W").WithValue(true).Build();
                UseE = factory.WithName("Use E").WithValue(true).Build();
                UseR = factory.WithName("Use R").WithValue(true).Build();
                DontAAInCombo = factory.WithName("Don't AA while doing Combo").WithValue(true).Build();
            }

            private void Drawings(MenuItemFactory factory)
            {
                // Drawing Menu
                drawQ = factory.WithName("Draw Q range").WithValue(false).Build();
                drawW = factory.WithName("Draw W range").WithValue(false).Build();
                drawE = factory.WithName("Draw E range").WithValue(true).Build();
                drawR = factory.WithName("Draw R range").WithValue(true).Build();
                DrawComboDamage = factory
                    .WithName("Draw Combo Damage")
                    .WithValue(true)
                    .Build();

                FillColor = factory
                    .WithName("Fill Color")
                    .WithValue(new Circle(true, Color.FromArgb(204, 255, 0, 1)))
                    .Build();
            }

            private void Harass(MenuItemFactory factory)
            {
                HarassQ = factory.WithName("Auto Harrass with Q").WithValue(false).Build();
                HarassE = factory.WithName("Auto Harrass with E").WithValue(true).Build();
                HarassMana = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to AutoHarass with Q/E.")
                    .Build();
            }

            private void LaneClear(MenuItemFactory factory)
            {
                // LaneClear Menu
                laneclearQ = factory.WithName("Use Q to LaneClear").WithValue(false).Build();
                laneclearW =
                    factory.WithName("Use W to LaneClear")
                        .WithValue(false)
                        .WithTooltip("Preferably enable after Lvl 9 :)")
                        .Build();
                laneclearE = factory.WithName("Use E to LaneClear").WithValue(true).Build();

                LaneClearMinions = factory
                    .WithName("Minimum Minions for (Q)")
                    .WithValue(new Slider(2, 0, 10))
                    .WithTooltip("Minimum Minions that Q has to hit")
                    .Build();

                LaneClearEMinMinions = factory
                    .WithName("Minimum Minions for (E)")
                    .WithValue(new Slider(2, 0, 10))
                    .WithTooltip("Minimum Minions that E has to infect")
                    .Build();

                laneclearMinimumMana = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(70))
                    .WithTooltip("Minimum Mana that you need to have to LaneClear with Q/W/E.")
                    .Build();
            }

            private void Misc(MenuItemFactory factory)
            {
                // Misc Menu
                // Todo: Add more KillSteal Variants/Spells
                // foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                // ConfigMenu.SubMenu("Misc Menu").SubMenu("GapCloser R").AddItem(new MenuItem("gapcloserR" + enemy.ChampionName, enemy.ChampionName).SetValue(false).SetTooltip("Use R on GapClosing Champions"));

                ksQ = factory.WithName("Use Q to KillSteal").WithValue(true).Build();
                ksE = factory.WithName("Use E to KillSteal").WithValue(true).Build();
                interruptQ = factory.WithName("Interrupt Spells Q").WithValue(true).Build();
                useQAntiGapCloser = factory.WithName("Use Q on GapClosers").WithValue(true).Build();

                OneShotInfo =
                    factory.WithName("OneShot Combo [Info]")
                        .WithTooltip(
                            "If you don't have mana to cast Q/W/E/R spells all together it won't cast the spells. Use Combo Instead.")
                        .Build();
                oneshot =
                    factory.WithName("Burst Combo")
                        .WithValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                        .WithTooltip("It will cast Q+E+W+R on enemy when enemy is in E range.")
                        .Build();
            }
        }

        /*private void CastQ(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!Q.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            Q.Width = GetDynamicQWidth(target);
            Q.Cast(target);
        }
        public float GetManaPercent()
        {
            return (ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100f;
        }
        private float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - (ObjectManager.Player.Distance(target) / Q.Range)) * SpellQWidth);
        }*/
    }
}