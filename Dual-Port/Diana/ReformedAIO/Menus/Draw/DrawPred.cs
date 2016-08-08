using EloBuddy; namespace ReformedAIO.Champions.Diana.Menus.Draw
{
    #region Using Directives

    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class DrawPred : ChildBase
    {
        #region Fields

        private CrescentStrikeLogic qLogic;

        #endregion


        #region Public Properties

        public override string Name { get; set; } = "Draw Prediction";

        #endregion

        #region Public Methods and Operators

        public void OnDraw(EventArgs args)
        {
            if (Variables.Player.IsDead) return;

            var target = TargetSelector.GetTarget(825, TargetSelector.DamageType.Magical);

            if (target != null && target.IsVisible)
            {
                Render.Circle.DrawCircle(this.qLogic.QPred(target), 50, Color.Aqua);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw -= this.OnDraw;

        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += this.OnDraw;
            
        }

        protected override void OnInitialize(object sender, Base.FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.qLogic = new CrescentStrikeLogic();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu = new Menu(this.Name, this.Name);

            Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            
        }

        #endregion
    }
}