using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.Draw
{
    #region Using Directives

    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class DrawE : ChildBase
    {

        #region Public Properties

        public override string Name { get; set; } = "Draw [E]";

        #endregion

        #region Public Methods and Operators

        public void OnDraw(EventArgs args)
        {
            if (Variables.Player.IsDead) return;

            Render.Circle.DrawCircle(
                Variables.Player.Position,
                Variables.Spells[SpellSlot.E].Range,
                Variables.Spells[SpellSlot.E].IsReady() 
                ? Color.Cyan 
                : Color.DarkSlateGray);
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
        }

        #endregion
    }
}