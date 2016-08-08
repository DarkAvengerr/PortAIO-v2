namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    public class Leona : IPlugin
    {
        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 200) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 700) },
                                                                           { Spells.R, new Spell(SpellSlot.R, 1200) }
                                                                       };

        private static SpellSlot Ignite;

        private static Orbwalking.Orbwalker Orbwalker;

        #endregion

        #region Enums

        public enum Spells
        {
            Q,

            W,

            E,

            R
        }

        #endregion

        #region Properties

        private HitChance CustomHitChance
        {
            get
            {
                return this.GetHitchance();
            }
        }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            this.Menu = new Menu("ElLeona", "ElLeona");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Leona.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Leona.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Leona.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Leona.Combo.R", "Use R").SetValue(false));
                    comboMenu.AddItem(
                        new MenuItem("ElEasy.Leona.Combo.Count.Enemies", "Enemies in range for R").SetValue(
                            new Slider(2, 1, 5)));
                    comboMenu.AddItem(
                        new MenuItem("ElEasy.Leona.Hitchance", "Hitchance").SetValue(
                            new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));
                    comboMenu.AddItem(new MenuItem("ElEasy.Leona.Combo.Ignite", "Use Ignite").SetValue(true));
                }

                this.Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Leona.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Leona.Harass.E", "Use E").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElEasy.Leona.Harass.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(harassMenu);

                var settingsMenu = new Menu("Settings", "Settings");
                {
                    settingsMenu.AddItem(
                        new MenuItem("ElEasy.Leona.Interrupt.Activated", "Interrupt spells").SetValue(true));
                    settingsMenu.AddItem(
                        new MenuItem("ElEasy.Leona.GapCloser.Activated", "Anti gapcloser").SetValue(true));
                }

                this.Menu.AddSubMenu(settingsMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Leona.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Leona.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Leona.Draw.W", "Draw W").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Leona.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Leona.Draw.R", "Draw R").SetValue(new Circle()));
                }

                this.Menu.AddSubMenu(miscellaneousMenu);
            }
            rootMenu.AddSubMenu(this.Menu);
        }

        public void Load()
        {
            Console.WriteLine("Loaded Leona");
            Ignite = this.Player.LSGetSpellSlot("summonerdot");
            spells[Spells.E].SetSkillshot(0.25f, 120f, 2000f, false, SkillshotType.SkillshotLine);
            spells[Spells.R].SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += this.Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var gapCloserActive = this.Menu.Item("ElEasy.Leona.Interrupt.Activated").IsActive();

            if (gapCloserActive && spells[Spells.Q].LSIsReady()
                && gapcloser.Sender.LSDistance(this.Player) < spells[Spells.Q].Range)
            {
                spells[Spells.Q].Cast();
            }
        }

        private HitChance GetHitchance()
        {
            switch (this.Menu.Item("ElEasy.Leona.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        private float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || this.Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)this.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.LSDistance(this.Player) > spells[Spells.R].Range)
            {
                return;
            }

            if (sender.LSIsValidTarget(spells[Spells.R].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.R].LSIsReady())
            {
                spells[Spells.R].Cast(sender);
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Leona.Combo.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Leona.Combo.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Leona.Combo.E").IsActive();
            var useR = this.Menu.Item("ElEasy.Leona.Combo.R").IsActive();
            var useI = this.Menu.Item("ElEasy.Leona.Combo.Ignite").IsActive();
            var countEnemies = this.Menu.Item("ElEasy.Leona.Combo.Count.Enemies").GetValue<Slider>().Value;

            if (useQ)
            {
                //check aa range
                if (spells[Spells.Q].LSIsReady() && !target.HasBuff("BlackShield") || !target.HasBuff("SivirShield")
                    || !target.HasBuff("BansheesVeil") || !target.HasBuff("ShroudofDarkness"))
                {
                    spells[Spells.Q].Cast();
                }
            }

            if (useW && spells[Spells.W].LSIsReady() && target.LSIsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].LSIsReady() && target.LSIsValidTarget(spells[Spells.E].Range))
            {
                var pred = spells[Spells.E].GetPrediction(target).Hitchance;
                if (pred >= HitChance.VeryHigh)
                {
                    spells[Spells.E].Cast(target);
                }
            }

            if (useR && spells[Spells.R].LSIsReady() && target.LSIsValidTarget(spells[Spells.R].Range)
                && this.Player.LSCountEnemiesInRange(spells[Spells.R].Range) >= countEnemies)
            {
                var pred = spells[Spells.R].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.R].CastIfHitchanceEquals(target, HitChance.Immobile);
                }
            }

            if (target.LSIsValidTarget(600) && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Leona.Draw.off").IsActive();
            var drawQ = this.Menu.Item("ElEasy.Leona.Draw.Q").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Leona.Draw.E").GetValue<Circle>();
            var drawW = this.Menu.Item("ElEasy.Leona.Draw.W").GetValue<Circle>();
            var drawR = this.Menu.Item("ElEasy.Leona.Draw.R").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawR.Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Leona.Harass.Q").IsActive();
            var useE = this.Menu.Item("ElEasy.Leona.Harass.E").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Leona.Harass.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].LSIsReady())
            {
                spells[Spells.Q].Cast();
            }

            if (useE && spells[Spells.E].LSIsReady() && target.LSIsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(target);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    this.OnCombo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    this.OnHarass();
                    break;
            }
        }

        #endregion
    }
}