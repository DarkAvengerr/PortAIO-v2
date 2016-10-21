namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    public class Taric //: IPlugin
    {
        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 750) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 200) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 625) },
                                                                           { Spells.R, new Spell(SpellSlot.R, 200) }
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
            this.Menu = new Menu("ElTaric", "ElTaric");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Taric.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Taric.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Taric.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Taric.Combo.R", "Use R").SetValue(true));
                    comboMenu.AddItem(
                        new MenuItem("ElEasy.Taric.Combo.Count.Enemies", "Enemies in range for R").SetValue(
                            new Slider(2, 1, 5)));
                    comboMenu.AddItem(new MenuItem("ElEasy.Taric.Combo.Ignite", "Use Ignite").SetValue(true));
                }

                this.Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Taric.Harass.W", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Taric.Harass.E", "Use E").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElEasy.Taric.Harass.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(harassMenu);

                var healMenu = new Menu("Heal", "Heal");
                {
                    healMenu.AddItem(new MenuItem("ElEasy.Taric.Heal.Activated", "Heal").SetValue(true));
                    healMenu.AddItem(new MenuItem("ElEasy.Taric.Heal.Player.HP", "Player HP").SetValue(new Slider(55)));
                    healMenu.AddItem(new MenuItem("ElEasy.Taric.Heal.Ally.HP", "Ally HP").SetValue(new Slider(55)));
                    healMenu.AddItem(
                        new MenuItem("ElEasy.Taric.Heal.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(healMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Taric.Interrupt.Activated", "Interrupt spells").SetValue(true));
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Taric.GapCloser.Activated", "Anti gapcloser").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Taric.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Taric.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Taric.Draw.W", "Draw W").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Taric.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Taric.Draw.R", "Draw R").SetValue(new Circle()));
                }

                this.Menu.AddSubMenu(miscellaneousMenu);
            }
            rootMenu.AddSubMenu(this.Menu);
        }

        public void Load()
        {
            Console.WriteLine("Loaded Taric");
            Ignite = this.Player.GetSpellSlot("summonerdot");

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += this.Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (this.Menu.Item("ElEasy.Taric.GapCloser.Activated").IsActive() && spells[Spells.E].IsReady()
                && gapcloser.Sender.Distance(this.Player) < spells[Spells.E].Range)
            {
                spells[Spells.E].Cast(gapcloser.Sender);
            }
        }

        private void HealManager()
        {
            if (this.Player.IsRecalling() || this.Player.InFountain()
                || !this.Menu.Item("ElEasy.Taric.Heal.Activated").IsActive()
                || this.Player.ManaPercent < this.Menu.Item("ElEasy.Taric.Heal.Player.Mana").GetValue<Slider>().Value
                || !spells[Spells.Q].IsReady())
            {
                return;
            }

            if ((this.Player.Health / this.Player.MaxHealth) * 100
                <= this.Menu.Item("ElEasy.Taric.Heal.Player.HP").GetValue<Slider>().Value)
            {
                spells[Spells.Q].CastOnUnit(this.Player);
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                if ((hero.Health / hero.MaxHealth) * 100
                    <= this.Menu.Item("ElEasy.Taric.Heal.Ally.HP").GetValue<Slider>().Value
                    && spells[Spells.Q].IsInRange(hero))
                {
                    spells[Spells.Q].Cast(hero);
                }
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
                || sender.Distance(this.Player) > spells[Spells.E].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.E].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(sender);
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (this.Menu.Item("ElEasy.Taric.Combo.E").IsActive() && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(target);
            }

            if (this.Menu.Item("ElEasy.Taric.Combo.W").IsActive() && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].CastOnUnit(this.Player);
            }

            if (this.Menu.Item("ElEasy.Taric.Combo.R").IsActive() && spells[Spells.R].IsReady()
                && target.IsValidTarget(spells[Spells.R].Range)
                && this.Player.CountEnemiesInRange(spells[Spells.R].Range)
                >= this.Menu.Item("ElEasy.Taric.Combo.Count.Enemies").GetValue<Slider>().Value)
            {
                spells[Spells.R].CastOnUnit(this.Player);
            }

            if (this.Menu.Item("ElEasy.Taric.Combo.Ignite").IsActive() && target.IsValidTarget(600)
                && this.IgniteDamage(target) >= target.Health)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Taric.Draw.off").IsActive();
            var drawQ = this.Menu.Item("ElEasy.Taric.Draw.Q").GetValue<Circle>();
            var drawW = this.Menu.Item("ElEasy.Taric.Draw.W").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Taric.Draw.E").GetValue<Circle>();
            var drawR = this.Menu.Item("ElEasy.Taric.Draw.R").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.Q].Range, Color.White);
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
                if (spells[Spells.W].Level > 0)
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

            var useE = this.Menu.Item("ElEasy.Taric.Harass.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Taric.Harass.W").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Taric.Harass.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana)
            {
                return;
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].CastOnUnit(this.Player);
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

            this.HealManager();
        }

        #endregion
    }
}