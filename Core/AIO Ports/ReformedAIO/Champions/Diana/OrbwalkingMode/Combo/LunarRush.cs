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

    internal class LunarRush : OrbwalkingChild
    {
        #region Fields

        private LogicAll logic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[W] Lunar Rush";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate -= OnUpdate;

        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate += OnUpdate;
            
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem("WKillable", "Use Only If Killable By Combo").SetValue(false));

            Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));

            logic = new LogicAll();
        }

        private void Lunarrush()
        {
            var target = TargetSelector.GetTarget(
                Variables.Player.AttackRange + Variables.Player.BoundingRadius,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (Menu.Item("WKillable").GetValue<bool>() && logic.ComboDmg(target) < target.Health)
            {
                return;
            }

            Variables.Spells[SpellSlot.W].Cast();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            Lunarrush();
        }

        #endregion
    }
}