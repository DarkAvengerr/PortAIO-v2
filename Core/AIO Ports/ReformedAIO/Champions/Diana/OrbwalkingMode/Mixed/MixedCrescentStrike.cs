using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Mixed
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class MixedCrescentStrike : OrbwalkingChild
    {
        #region Fields

        private CrescentStrikeLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

        #endregion

        #region Public Methods and Operators

        public void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("QMana").GetValue<Slider>().Value > Variables.Player.ManaPercent)
            {
                return;
            }

            Crescent();
        }

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
            Menu.AddItem(new MenuItem("QRange", "Q Range ").SetValue(new Slider(820, 0, 825)));

            Menu.AddItem(new MenuItem("QMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            qLogic = new CrescentStrikeLogic();
        }

        private void Crescent()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item("QRange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            Variables.Spells[SpellSlot.Q].Cast(qLogic.QPred(target));
        }

        #endregion
    }
}