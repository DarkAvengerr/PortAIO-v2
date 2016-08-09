using EloBuddy; namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Mixed
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class WMixed : ChildBase
    {
        #region Constructors and Destructors

        public WMixed(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        public override string Name { get; set; }

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

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "WDistance", "Max Distance").SetValue(new Slider(1100, 0, 1200)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "WMana", "Mana %").SetValue(new Slider(80, 0, 100)));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed
                || !Variable.Spells[SpellSlot.W].IsReady() || Variable.Player.Spellbook.IsAutoAttacking) return;

            this.Volley();
        }

        private void Volley()
        {
            var target = TargetSelector.GetTarget(
                this.Menu.Item(this.Menu.Name + "WDistance").GetValue<Slider>().Value,
                TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValid) return;

            Variable.Spells[SpellSlot.W].CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        #endregion
    }
}