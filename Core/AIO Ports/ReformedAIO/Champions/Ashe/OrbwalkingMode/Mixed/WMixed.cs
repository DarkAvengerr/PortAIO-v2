using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Mixed
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class WMixed : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[W]";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(
                new MenuItem("WDistance", "Max Distance").SetValue(new Slider(1100, 0, 1200)));

            Menu.AddItem(new MenuItem("WMana", "Mana %").SetValue(new Slider(80, 0, 100)));
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.W].IsReady() || ObjectManager.Player.Spellbook.IsAutoAttacking) return;

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