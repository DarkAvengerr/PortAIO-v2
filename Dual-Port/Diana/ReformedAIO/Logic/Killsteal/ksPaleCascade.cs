using EloBuddy; namespace ReformedAIO.Champions.Diana.Logic.Killsteal
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class KsPaleCascade : ChildBase
    {
        #region Fields

        private PaleCascadeLogic rLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[R] PaleCascade";

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
            this.rLogic = new PaleCascadeLogic();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
            // TODO Add Dmg Multiplier(?)
        {
            this.Menu.AddItem(new MenuItem(this.Menu.Name + "RRange", "R Range ").SetValue(new Slider(825, 0, 825)));
        }

        private void OnUpdate(EventArgs args)
        {
            var target =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    !x.IsDead && x.LSIsValidTarget(this.Menu.Item(this.Menu.Name + "RRange").GetValue<Slider>().Value));

            if (target == null || target.Health > this.rLogic.GetDmg(target)) return;

            Variables.Spells[SpellSlot.R].Cast(target);
        }

        #endregion
    }
}