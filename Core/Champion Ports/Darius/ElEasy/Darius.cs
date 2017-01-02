namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;

    public class Darius : IPlugin
    {
        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 420) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 145) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 540) },
                                                                           { Spells.R, new Spell(SpellSlot.R, 460) }
                                                                       };

        private static SpellSlot ignite;

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
            this.Menu = new Menu("ElDarius", "ElDarius");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Darius.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Darius.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Darius.Combo.E", "Use E").SetValue(true));
                    comboMenu.SubMenu("R").AddItem(new MenuItem("ElEasy.Darius.Combo.R", "Use SlamDUNK").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Darius.Combo.Ignite", "Use Ignite").SetValue(true));
                }

                this.Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Darius.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElEasy.Darius.Harass.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(harassMenu);

                var clearMenu = new Menu("Clear", "Clear");
                {
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Darius.LaneClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Darius.LaneClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Darius.JungleClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Darius.JungleClear.W", "Use W").SetValue(true));
                    clearMenu.AddItem(
                        new MenuItem("ElEasy.Darius.Clear.Player.Mana", "Minimum Mana for clear").SetValue(
                            new Slider(55)));
                }

                this.Menu.AddSubMenu(clearMenu);

                var itemMenu = new Menu("Items", "Items");
                {
                    itemMenu.AddItem(
                        new MenuItem("ElEasy.Darius.Items.Youmuu", "Use Youmuu's Ghostblade").SetValue(true));
                    itemMenu.AddItem(new MenuItem("ElEasy.Darius.Items.Cutlass", "Use Cutlass").SetValue(true));
                    itemMenu.AddItem(
                        new MenuItem("ElEasy.Darius.Items.Blade", "Use Blade of the Ruined King").SetValue(true));
                    itemMenu.AddItem(new MenuItem("ElEasy.Darius.Harasssfsddass.E", ""));
                    itemMenu.AddItem(
                        new MenuItem("ElEasy.Darius.Items.Blade.EnemyEHP", "Enemy HP Percentage").SetValue(
                            new Slider(80, 100, 0)));
                    itemMenu.AddItem(
                        new MenuItem("ElEasy.Darius.Items.Blade.EnemyMHP", "My HP Percentage").SetValue(
                            new Slider(80, 100, 0)));
                }

                this.Menu.AddSubMenu(itemMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Darius.Interrupt.Activated", "Interrupt spells").SetValue(true));
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Darius.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Darius.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Darius.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Darius.Draw.R", "Draw R").SetValue(new Circle()));

                    var dmgAfterE = new MenuItem("ElEasy.Darius.DrawComboDamage", "Draw combo damage").SetValue(true);
                    var drawFill =
                        new MenuItem("ElEasy.Darius.DrawColour", "Fill colour", true).SetValue(
                            new Circle(true, Color.FromArgb(204, 204, 0, 0)));
                    miscellaneousMenu.AddItem(drawFill);
                    miscellaneousMenu.AddItem(dmgAfterE);

                    //DrawDamage.DamageToUnit = this.GetComboDamage;
                    //DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
                    //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                    //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

                    dmgAfterE.ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                            };

                    drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                        {
                            //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                            //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                        };
                }

                this.Menu.AddSubMenu(miscellaneousMenu);
            }
            rootMenu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            ignite = this.Player.GetSpellSlot("summonerdot");
            spells[Spells.E].SetSkillshot(0.30f, 80, int.MaxValue, false, SkillshotType.SkillshotCone);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            Interrupter2.OnInterruptableTarget += this.Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (spells[Spells.Q].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (spells[Spells.R].IsReady())
            {
                damage +=
                    enemy.Buffs.Where(buff => buff.Name == "dariushemo")
                        .Sum(buff => this.Player.GetSpellDamage(enemy, SpellSlot.R, 1) * (1 + buff.Count / 5) - 1);
            }

            return (float)damage;
        }

        private float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || this.Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)this.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!this.Menu.Item("ElEasy.Darius.Interrupt.Activated").IsActive())
            {
                return;
            }

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

        private void Items(Obj_AI_Base target)
        {
            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            var useYoumuu = this.Menu.Item("ElEasy.Darius.Items.Youmuu").IsActive();
            var useCutlass = this.Menu.Item("ElEasy.Darius.Items.Cutlass").IsActive();
            var useBlade = this.Menu.Item("ElEasy.Darius.Items.Blade").IsActive();

            var useBladeEhp = this.Menu.Item("ElEasy.Darius.Items.Blade.EnemyEHP").GetValue<Slider>().Value;
            var useBladeMhp = this.Menu.Item("ElEasy.Darius.Items.Blade.EnemyMHP").GetValue<Slider>().Value;

            if (botrk.IsReady() && botrk.IsOwned(this.Player) && botrk.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && useBlade)
            {
                botrk.Cast(target);
            }

            if (botrk.IsReady() && botrk.IsOwned(this.Player) && botrk.IsInRange(target)
                && this.Player.HealthPercent <= useBladeMhp && useBlade)
            {
                botrk.Cast(target);
            }

            if (cutlass.IsReady() && cutlass.IsOwned(this.Player) && cutlass.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && useCutlass)
            {
                cutlass.Cast(target);
            }

            if (ghost.IsReady() && ghost.IsOwned(this.Player) && target.IsValidTarget(spells[Spells.Q].Range)
                && useYoumuu)
            {
                ghost.Cast();
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Darius.Combo.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Darius.Combo.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Darius.Combo.E").IsActive();
            var useR = this.Menu.Item("ElEasy.Darius.Combo.R").IsActive();
            var useI = this.Menu.Item("ElEasy.Darius.Combo.Ignite").IsActive();

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target)
                && !target.HasBuff("BlackShield") || !target.HasBuff("SivirShield") || !target.HasBuff("BansheesVeil")
                || !target.HasBuff("ShroudofDarkness"))
            {
                spells[Spells.E].Cast(target);
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                spells[Spells.Q].Cast();
            }

            this.Items(target);

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }

            if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range))
            {
                foreach (var hero in
                    HeroManager.Enemies.Where(hero => hero.IsValidTarget(spells[Spells.R].Range)))
                {
                    if (this.Player.GetSpellDamage(target, SpellSlot.R) > hero.Health)
                    {
                        spells[Spells.R].CastOnUnit(target);
                    }

                    else if (this.Player.GetSpellDamage(target, SpellSlot.R) < hero.Health)
                    {
                        foreach (var buff in hero.Buffs.Where(buff => buff.Name == "dariushemo"))
                        {
                            if (this.Player.GetSpellDamage(target, SpellSlot.R, 1) * (1 + buff.Count / 5) - 1
                                > target.Health)
                            {
                                spells[Spells.R].CastOnUnit(target);
                            }
                        }
                    }
                }
            }

            if (this.Player.Distance(target) <= 600 && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Darius.Draw.off").GetValue<bool>();
            var drawQ = this.Menu.Item("ElEasy.Darius.Draw.Q").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Darius.Draw.E").GetValue<Circle>();
            var drawR = this.Menu.Item("ElEasy.Darius.Draw.R").GetValue<Circle>();

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
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Darius.Harass.Q").GetValue<bool>();

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                spells[Spells.Q].Cast();
            }
        }

        private void OnJungleclear()
        {
            var useQ = this.Menu.Item("ElEasy.Darius.JungleClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Darius.JungleClear.W").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Darius.Clear.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.Q].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast();
            }
        }

        private void OnLaneclear()
        {
            var useQ = this.Menu.Item("ElEasy.Darius.LaneClear.Q").GetValue<bool>();
            var useW = this.Menu.Item("ElEasy.Darius.LaneClear.W").GetValue<bool>();
            var playerMana = this.Menu.Item("ElEasy.Darius.Clear.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(this.Player.ServerPosition, spells[Spells.Q].Range).FirstOrDefault();
            if (minions == null)
            {
                return;
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast();
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
            }
        }

        #endregion
    }
}