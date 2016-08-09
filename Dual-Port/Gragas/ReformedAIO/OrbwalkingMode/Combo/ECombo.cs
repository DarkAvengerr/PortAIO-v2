using EloBuddy; namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SPrediction;

    #endregion

    internal class ECombo : ChildBase
    {
        #region Fields

        private LogicAll logic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[E] Body Slam";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate += this.OnUpdate;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.logic = new LogicAll();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(new MenuItem(this.Name + "EKillable", "Only If Killable").SetValue(false));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "ERange", "E Range ").SetValue(new Slider(835, 0, 850)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "EMana", "Mana %").SetValue(new Slider(45, 0, 100)));
        }

        private void BodySlam()
        {
            var target = TargetSelector.GetTarget(
                this.Menu.Item(this.Menu.Name + "ERange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (this.Menu.Item(this.Menu.Name + "EKillable").GetValue<bool>()
                && this.logic.ComboDmg(target) < target.Health) return;

            var ePred = Variable.Spells[SpellSlot.E].GetSPrediction(target);

            if (target.HasBuffOfType(BuffType.Knockback)) return;

            if (ePred.HitChance < HitChance.Medium) return;

            Variable.Spells[SpellSlot.E].Cast(ePred.CastPosition);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variable.Spells[SpellSlot.E].IsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "EMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.BodySlam();
        }

        #endregion
    }
}