using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class PaleCascade : OrbwalkingChild
    {
        #region Fields

        private LogicAll logic;

        private PaleCascadeLogic rLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[R] Pale Cascade";

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
            Menu.AddItem(
                new MenuItem("Enemies", "Don't R Into >= x Enemies").SetValue(new Slider(2, 0, 5)));

            Menu.AddItem(new MenuItem("Turret", "Don't R Into Turret").SetValue(true));

            Menu.AddItem(new MenuItem("Killable", "Only If Killable").SetValue(true));

            rLogic = new PaleCascadeLogic();
            logic = new LogicAll();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            paleCascade();
        }

        private void paleCascade()
        {
            var target = TargetSelector.GetTarget(825, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid
                || target.CountEnemiesInRange(1400) >= Menu.Item("REnemies").GetValue<Slider>().Value
                || (Menu.Item("Turret").GetValue<bool>() && target.UnderTurret())
                || (Menu.Item("Killable").GetValue<bool>() && logic.ComboDmg(target) < target.Health))
            {
                return;
            }

            if (rLogic.Buff(target))
            {
                Variables.Spells[SpellSlot.R].Cast(target);
            }
        }

        #endregion
    }
}