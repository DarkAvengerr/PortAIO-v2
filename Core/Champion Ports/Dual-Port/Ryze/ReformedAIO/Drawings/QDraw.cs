using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ryze.Drawings
{
    #region Using Directives

    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class QDraw : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[Q] Draw";

        #endregion

        #region Public Methods and Operators

        public void OnDraw(EventArgs args)
        {
            if (Variables.Player.IsDead) return;

            Render.Circle.DrawCircle(
                Variables.Player.Position,
                Variables.Spells[SpellSlot.Q].Range,
                Variables.Spells[SpellSlot.Q].IsReady()
                ? Color.LightGray 
                : Color.DarkSlateGray);
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Console.WriteLine("ENABLING DRAWINGS!!!!!!!!!!!!!!!!!!!");
            Drawing.OnDraw += OnDraw;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));
        }

        #endregion
    }
}