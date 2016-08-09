using EloBuddy; namespace ReformedAIO.Champions.Ryze.OrbwalkingMode.Mixed
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ryze.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class WMixed : ChildBase
    {
        #region Fields

        private ELogic eLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[W] Rune Prison";

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
            this.eLogic = new ELogic();
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(new MenuItem(this.Menu.Name + "WRange", "W Range ").SetValue(new Slider(600, 0, 600)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "WMana", "Mana %").SetValue(new Slider(45, 0, 100)));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed
                || !Variable.Spells[SpellSlot.W].IsReady()) return;

            var target = TargetSelector.GetTarget(
                this.Menu.Item(this.Menu.Name + "WRange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null) return;

            if (!target.IsValid || this.eLogic.RyzeE(target)
                || !(this.Menu.Item(this.Menu.Name + "WMana").GetValue<Slider>().Value < Variable.Player.ManaPercent)) return;

            Variable.Spells[SpellSlot.W].Cast(target);
        }

        #endregion
    }
}