using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class WCombo : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[W] Drunken Rage";

        #endregion
        
        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("WRange", "W Range ").SetValue(new Slider(500, 0, 800)));

            Menu.AddItem(new MenuItem("WMana", "Mana %").SetValue(new Slider(45, 0, 100)));
        }

        private void DrunkenRage()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item("WRange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            Variable.Spells[SpellSlot.W].Cast();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("WMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            DrunkenRage();
        }

        #endregion
    }
}