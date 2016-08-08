using EloBuddy; namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class Moonfall : ChildBase
    {
        #region Fields

        private LogicAll logic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[E] Moonfall";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Interrupter2.OnInterruptableTarget -= this.Interrupt;

            AntiGapcloser.OnEnemyGapcloser -= this.Gapcloser;

            Events.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Interrupter2.OnInterruptableTarget += this.Interrupt;

            AntiGapcloser.OnEnemyGapcloser += this.Gapcloser;

            Events.OnUpdate += this.OnUpdate;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.logic = new LogicAll();

            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(new MenuItem(this.Name + "EInterrupt", "Interrupt").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Name + "EGapcloser", "Anti-Gapcloser").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Name + "EKillable", "Use Only If Killable By Combo").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Name + "ERange", "E Range").SetValue(new Slider(300, 350)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinTargets", "Min Targets In Range").SetValue(new Slider(2, 0, 5)));

            this.Menu.AddItem(new MenuItem(this.Name + "EMana", "Mana %").SetValue(new Slider(45, 0, 100)));
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!this.Menu.Item(this.Menu.Name + "EGapcloser").GetValue<bool>()) return;

            var target = gapcloser.Sender;

            if (target == null) return;

            if (!target.IsEnemy || !Variables.Spells[SpellSlot.E].LSIsReady() || !target.LSIsValidTarget()
                || target.IsZombie) return;

            if (
                target.LSIsValidTarget(
                    Variables.Spells[SpellSlot.E].Range + Variables.Player.BoundingRadius + target.BoundingRadius)) Variables.Spells[SpellSlot.E].Cast();
        }

        private void Interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!this.Menu.Item(this.Menu.Name + "EInterrupt").GetValue<bool>()) return;

            if (!sender.IsEnemy || !Variables.Spells[SpellSlot.E].LSIsReady() || !sender.LSIsValidTarget()
                || sender.IsZombie) return;

            if (sender.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range)) Variables.Spells[SpellSlot.E].Cast();
        }

        private void moonfall()
        {
            var target = TargetSelector.GetTarget(
                this.Menu.Item(this.Menu.Name + "ERange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (this.Menu.Item(this.Menu.Name + "MinTargets").GetValue<Slider>().Value
                > target.LSCountEnemiesInRange(this.Menu.Item(this.Menu.Name + "ERange").GetValue<Slider>().Value)) return;

            if (this.Menu.Item(this.Menu.Name + "EKillable").GetValue<bool>()
                && this.logic.ComboDmg(target) * 1.2 < target.Health)
            {
                return;
            }

            Variables.Spells[SpellSlot.E].Cast();
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variables.Spells[SpellSlot.E].LSIsReady()) return;

            // if (Menu.Item(Menu.Name + "EMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.moonfall();
        }

        #endregion
    }
}