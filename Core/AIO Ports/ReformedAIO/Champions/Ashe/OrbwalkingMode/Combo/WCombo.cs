using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class WCombo : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[W]";

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

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("WDistance", "Max Distance").SetValue(new Slider(1100, 0, 1200)));

            Menu.AddItem(new MenuItem("WMana", "Mana %").SetValue(new Slider(10, 0, 100)));
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.W].IsReady()
                || Variable.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            Volley();
        }

        private void Volley()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item("WDistance").GetValue<Slider>().Value,
                TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValid) return;

            Variable.Spells[SpellSlot.W].CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        #endregion
    }
}