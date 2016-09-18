using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.Logic.Killsteal
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

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
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    rLogic = new PaleCascadeLogic();
        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem(Menu.Name + "RRange", "R Range ").SetValue(new Slider(825, 0, 825)));

            rLogic = new PaleCascadeLogic();
        }

        private void OnUpdate(EventArgs args)
        {
            var target =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    !x.IsDead && x.IsValidTarget(Menu.Item(Menu.Name + "RRange").GetValue<Slider>().Value));

            if (target == null || target.Health > rLogic.GetDmg(target)) return;

            Variables.Spells[SpellSlot.R].Cast(target);
        }

        #endregion
    }
}