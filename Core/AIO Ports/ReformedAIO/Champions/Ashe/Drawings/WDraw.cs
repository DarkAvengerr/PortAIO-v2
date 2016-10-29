using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.Drawings
{
    #region Using Directives

    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class WDraw : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[W]";

        #endregion

        #region Public Methods and Operators

        public void OnDraw(EventArgs args)
        {
            if (Variable.Player.IsDead) return;

            if (Menu.Item("WReady").GetValue<bool>() && !Variable.Spells[SpellSlot.W].IsReady()) return;

            Render.Circle.DrawCircle(
                Variable.Player.Position,
                1275,
                Variable.Spells[SpellSlot.W].IsReady() 
                ? Color.White
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

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem("WReady", "Only If Ready").SetValue(false));
        }

        #endregion
    }
}