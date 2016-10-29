using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.Draw
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
                Render.Circle.DrawCircle(qLogic.QPred(target), 50, Color.Aqua);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Drawing.OnDraw += OnDraw;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            qLogic = new CrescentStrikeLogic();
        }

        #endregion
    }
}