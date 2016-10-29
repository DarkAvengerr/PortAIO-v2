using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    using SPrediction;

    #endregion

    internal class ECombo : OrbwalkingChild
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
            Menu.AddItem(new MenuItem("EKillable", "Only If Killable").SetValue(false));

            Menu.AddItem(new MenuItem("ERange", "E Range ").SetValue(new Slider(835, 0, 850)));

            Menu.AddItem(new MenuItem("EMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            logic = new LogicAll();
        }

        private void BodySlam()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item("ERange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null 
                || !target.IsValid
                || (Menu.Item("EKillable").GetValue<bool>() 
                && logic.ComboDmg(target) < target.Health))
            {
                return;
            }

            var ePred = Variable.Spells[SpellSlot.E].GetSPrediction(target);

            if (target.HasBuffOfType(BuffType.Knockback) || ePred.HitChance < HitChance.High) return;

            Variable.Spells[SpellSlot.E].Cast(ePred.CastPosition);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("EMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            BodySlam();
        }

        #endregion
    }
}