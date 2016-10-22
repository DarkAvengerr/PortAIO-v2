namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Sona : IPlugin
    {
        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 850) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 1000) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 350) },
                                                                           { Spells.R, new Spell(SpellSlot.R, 1000) }
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
            this.Menu = new Menu("ElSona", "ElSona");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Sona.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Sona.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Sona.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Sona.Combo.R", "Use R").SetValue(true));
                    comboMenu.AddItem(
                        new MenuItem("ElEasy.Sona.Combo.Count.R", "Minimum hit by R").SetValue(new Slider(2, 1, 5)));
                    comboMenu.AddItem(new MenuItem("ElEasy.Sona.Combo.Ignite", "Use Ignite").SetValue(true));
                }

                this.Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Sona.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElEasy.Sona.Harass.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));
                    harassMenu.SubMenu("Auto harass")
                        .AddItem(
                            new MenuItem("ElEasy.Sona.Autoharass.Activated", "Autoharass").SetValue(
                                new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
                }

                this.Menu.AddSubMenu(harassMenu);

                var healMenu = new Menu("Heal", "Heal");
                {
                    healMenu.AddItem(new MenuItem("ElEasy.Sona.Heal.Activated", "Heal").SetValue(true));
                    healMenu.AddItem(new MenuItem("ElEasy.Sona.Heal.Player.HP", "Player HP").SetValue(new Slider(55)));
                    healMenu.AddItem(new MenuItem("ElEasy.Sona.Heal.Ally.HP", "Ally HP").SetValue(new Slider(55)));
                    healMenu.AddItem(
                        new MenuItem("ElEasy.Sona.Heal.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(healMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Sona.Interrupt.Activated", "Interrupt spells").SetValue(true));
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.SonaGapCloser.Activated", "Anti gapcloser").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Sona.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Sona.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Sona.Draw.W", "Draw W").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Sona.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Sona.Draw.R", "Draw R").SetValue(new Circle()));
                }

                this.Menu.AddSubMenu(miscellaneousMenu);
            }
            rootMenu.AddSubMenu(this.Menu);
        }

        public void Load()
        {
            Console.WriteLine("Loaded Sona");
            Ignite = this.Player.GetSpellSlot("summonerdot");

            spells[Spells.R].SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.SkillshotLine);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += this.Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (this.Menu.Item("ElEasy.Sona.GapCloser.Activated").IsActive() && spells[Spells.R].IsReady()
                && gapcloser.Sender.IsValidTarget(spells[Spells.R].Range))
            {
                spells[Spells.R].Cast(gapcloser.Sender);
            }
        }

        private void AutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (this.Menu.Item("ElEasy.Sona.Autoharass.Activated").GetValue<KeyBind>().Active
                && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }
        }

        private void HealManager()
        {
            var useHeal = this.Menu.Item("ElEasy.Sona.Heal.Activated").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Sona.Heal.Player.Mana").GetValue<Slider>().Value;
            var playerHp = this.Menu.Item("ElEasy.Sona.Heal.Player.HP").GetValue<Slider>().Value;
            var allyHp = this.Menu.Item("ElEasy.Sona.Heal.Ally.HP").GetValue<Slider>().Value;

            if (this.Player.IsRecalling() || this.Player.InFountain() || !useHeal
                || this.Player.ManaPercent < playerMana || !spells[Spells.W].IsReady())
            {
                return;
            }

            if ((this.Player.Health / this.Player.MaxHealth) * 100 <= playerHp)
            {
                spells[Spells.W].Cast();
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                if ((hero.Health / hero.MaxHealth) * 100 <= allyHp && spells[Spells.W].IsInRange(hero))
                {
                    spells[Spells.W].Cast();
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
                || sender.Distance(this.Player) > spells[Spells.R].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.R].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.R].IsReady())
            {
                spells[Spells.R].Cast(sender.Position);
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Sona.Combo.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Sona.Combo.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Sona.Combo.E").IsActive();
            var useR = this.Menu.Item("ElEasy.Sona.Combo.R").IsActive();
            var useI = this.Menu.Item("ElEasy.Sona.Combo.Ignite").IsActive();
            var hitByR = this.Menu.Item("ElEasy.Sona.Combo.Count.R").GetValue<Slider>().Value;

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (useR && spells[Spells.R].IsReady() && rTarget.IsValidTarget(spells[Spells.R].Range))
            {
                var pred = spells[Spells.R].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    var hits = HeroManager.Enemies.Where(x => x.Distance(target) <= spells[Spells.R].Width).ToList();
                    Console.WriteLine(hits.Count);
                    if (hits.Any(hit => hits.Count >= hitByR))
                    {
                        spells[Spells.R].Cast(pred.CastPosition);
                    }
                }
            }

            if (this.Player.Distance(target) <= 600 && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Sona.Draw.off").IsActive();
            var drawQ = this.Menu.Item("ElEasy.Sona.Draw.Q").GetValue<Circle>();
            var drawW = this.Menu.Item("ElEasy.Sona.Draw.W").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Sona.Draw.E").GetValue<Circle>();
            var drawR = this.Menu.Item("ElEasy.Sona.Draw.R").GetValue<Circle>();

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
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            if (this.Player.ManaPercent < this.Menu.Item("ElEasy.Sona.Harass.Player.Mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (this.Menu.Item("ElEasy.Sona.Harass.Q").IsActive() && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
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
            this.AutoHarass();
        }

        #endregion
    }
}