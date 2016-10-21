namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    public class Malphite : IPlugin
    {
        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 625) },
                                                                           { Spells.W, new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 375) },
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
            this.Menu = new Menu("ElMalphite", "ElMalphite");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Malphite.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Malphite.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Malphite.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Malphite.Combo.Ignite", "Use Ignite").SetValue(true));
                }

                this.Menu.AddSubMenu(comboMenu);

                var rMenu = new Menu("R Settings", "R");
                {
                    rMenu.AddItem(new MenuItem("ElEasy.Malphite.Combo.R", "Use R").SetValue(true));
                    rMenu.AddItem(new MenuItem("ElEasy.Malphite.Combo.ForceR", "Force R when target can get killed").SetValue(false));
                    rMenu.AddItem(
                            new MenuItem("ElEasy.Malphite.Combo.R.Mode", "Mode ").SetValue(
                                new StringList(new[] { "Single target finisher", "Champions hit" }, 1)));
                    rMenu.AddItem(
                            new MenuItem("ElEasy.Malphite.Combo.Count.R", "Minimum champions hit by R").SetValue(
                                new Slider(2, 1, 5)));
                }

                this.Menu.AddSubMenu(rMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Malphite.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Malphite.Harass.E", "Use E").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElEasy.Malphite.Harass.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));
                    harassMenu.SubMenu("AutoHarass settings")
                        .AddItem(
                            new MenuItem("ElEasy.Malphite.AutoHarass.Activate", "Auto harass", true).SetValue(
                                new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
                    harassMenu.SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Malphite.AutoHarass.Q", "Use Q").SetValue(true));
                    harassMenu.SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Malphite.AutoHarass.E", "Use E").SetValue(true));
                    harassMenu.SubMenu("AutoHarass settings")
                        .AddItem(
                            new MenuItem("ElEasy.Malphite.AutoHarass.PlayerMana", "Minimum mana").SetValue(
                                new Slider(55)));
                }

                this.Menu.AddSubMenu(harassMenu);

                var clearMenu = new Menu("Clear", "Clear");
                {
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Malphite.LaneClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Malphite.LaneClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Malphite.LaneClear.E", "Use E").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Malphite.JungleClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Malphite.JungleClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Malphite.JungleClear.E", "Use E").SetValue(true));
                    clearMenu.SubMenu("Lasthit")
                        .AddItem(new MenuItem("ElEasy.Malphite.Lasthit.Q", "Use Q").SetValue(true));
                    clearMenu.AddItem(
                        new MenuItem("ElEasy.Malphite.Clear.Player.Mana", "Minimum Mana for clear").SetValue(
                            new Slider(55)));
                }

                this.Menu.AddSubMenu(clearMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Malphite.Interrupt.Activated", "Interrupt spells").SetValue(true));
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Malphite.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(
                       new MenuItem("ElEasy.Malphite.Castpos", "Draw R cast position").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Malphite.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Malphite.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Malphite.Draw.R", "Draw R").SetValue(new Circle()));
                }

                this.Menu.AddSubMenu(miscellaneousMenu);
            }
            rootMenu.AddSubMenu(this.Menu);
        }

        public void Load()
        {
            Console.WriteLine("Loaded Malphite");
            Ignite = this.Player.GetSpellSlot("summonerdot");
            spells[Spells.R].SetSkillshot(0.00f, 270, 700, false, SkillshotType.SkillshotCircle);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            Interrupter2.OnInterruptableTarget += this.Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

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
            if (!this.Menu.Item("ElEasy.Malphite.Interrupt.Activated").IsActive())
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.Distance(this.Player) > spells[Spells.R].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.R].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.R].IsReady())
            {
                spells[Spells.R].Cast(sender);
            }
        }

        private void OnAutoHarass()
        {
            var qTarget = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);

            if (qTarget == null || !qTarget.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Malphite.AutoHarass.Q").IsActive();
            var useE = this.Menu.Item("ElEasy.Malphite.AutoHarass.E").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Malphite.AutoHarass.PlayerMana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && qTarget.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(qTarget);
            }

            if (useE && spells[Spells.E].IsReady() && eTarget.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Malphite.Combo.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Malphite.Combo.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Malphite.Combo.E").IsActive();
            var useR = this.Menu.Item("ElEasy.Malphite.Combo.R").IsActive();
            var useI = this.Menu.Item("ElEasy.Malphite.Combo.Ignite").IsActive();
            var ultType = this.Menu.Item("ElEasy.Malphite.Combo.R.Mode").GetValue<StringList>().SelectedIndex;

            var countEnemies = this.Menu.Item("ElEasy.Malphite.Combo.Count.R").GetValue<Slider>().Value;

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }

            switch (ultType)
            {
                case 0:
                    if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range))
                    {
                        if (spells[Spells.R].GetDamage(target) > target.Health)
                        {
                            var pred = spells[Spells.R].GetPrediction(target);
                            if (pred.Hitchance >= HitChance.High)
                            {
                                spells[Spells.R].Cast(pred.CastPosition);
                            }
                        }
                    }
                    break;

                case 1:
                    if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range))
                    {
                        var pred = spells[Spells.R].GetPrediction(target);
                        if (pred.Hitchance >= HitChance.High)
                        {
                      
                            var hits = HeroManager.Enemies.Where(x => x.Distance(target) <= 300f).ToList();
                            if (hits.Any(hit => hits.Count >= countEnemies))
                            {
                                spells[Spells.R].Cast(pred.CastPosition);
                            }
                        }
                    }
                    break;
            }
            //
            if (this.Menu.Item("ElEasy.Malphite.Combo.ForceR").IsActive())
            {
                var getthabitch =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                        x.Distance(target) <= 300f && spells[Spells.R].GetDamage(x) > x.Health
                        && this.Player.CountEnemiesInRange(1000) == 1);

                var pred = spells[Spells.R].GetPrediction(getthabitch);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.R].Cast(pred.CastPosition);
                }
            }


            if (target.IsValidTarget(600) && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Malphite.Draw.off").IsActive();
            var drawQ = this.Menu.Item("ElEasy.Malphite.Draw.Q").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Malphite.Draw.E").GetValue<Circle>();
            var drawR = this.Menu.Item("ElEasy.Malphite.Draw.R").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }
            //
            if (this.Menu.Item("ElEasy.Malphite.Castpos").IsActive())
            {
                var target = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Magical);
                if (target == null) return;

                var hits = HeroManager.Enemies.Where(x => x.Distance(target) <= 300f).ToList();
                if (hits.Any(hit => hits.Count >= this.Menu.Item("ElEasy.Malphite.Combo.Count.R").GetValue<Slider>().Value))
            {
                    var pred = spells[Spells.R].GetPrediction(target);
                    Render.Circle.DrawCircle(pred.CastPosition, 300, Color.DeepSkyBlue);
                }
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
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Malphite.Harass.Q").IsActive();
            var useE = this.Menu.Item("ElEasy.Malphite.Harass.E").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Malphite.Harass.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target) && eTarget != null)
            {
                spells[Spells.E].Cast(eTarget);
            }
        }

        private void OnJungleclear()
        {
            var useQ = this.Menu.Item("ElEasy.Malphite.JungleClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Malphite.JungleClear.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Malphite.JungleClear.E").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Malphite.Clear.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.E].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                spells[Spells.Q].CastOnUnit(minions);
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(this.Player);
            }

            if (useE && spells[Spells.E].IsReady() && minions.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }
        }

        private void OnLaneclear()
        {
            var useQ = this.Menu.Item("ElEasy.Malphite.LaneClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Malphite.LaneClear.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Malphite.LaneClear.E").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Malphite.Clear.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(this.Player.ServerPosition, spells[Spells.E].Range).FirstOrDefault();
            if (minions == null)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                var allMinions = MinionManager.GetMinions(this.Player.ServerPosition, spells[Spells.E].Range);
                {
                    foreach (var minion in
                        allMinions.Where(minion => minion.Health <= this.Player.GetSpellDamage(minion, SpellSlot.Q)))
                    {
                        if (minion.IsValidTarget())
                        {
                            spells[Spells.Q].CastOnUnit(minion);
                            return;
                        }
                    }
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(this.Player);
            }

            if (useE && spells[Spells.E].IsReady() && minions.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }
        }

        private void OnLastHit()
        {
            var useQ = this.Menu.Item("ElEasy.Malphite.Lasthit.Q").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Malphite.Clear.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana || !useQ)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                this.Player.Position,
                spells[Spells.E].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].GetDamage(minion) > minion.Health && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].CastOnUnit(minion);
                }
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

                case Orbwalking.OrbwalkingMode.LaneClear:
                    this.OnLaneclear();
                    this.OnJungleclear();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    this.OnHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    this.OnLastHit();
                    break;
            }

            if (this.Menu.Item("ElEasy.Malphite.AutoHarass.Activate", true).GetValue<KeyBind>().Active)
            {
                this.OnAutoHarass();
            }
        }

        #endregion
    }
}