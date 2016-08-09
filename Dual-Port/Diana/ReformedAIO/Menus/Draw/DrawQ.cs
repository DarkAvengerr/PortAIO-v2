using EloBuddy; namespace ReformedAIO.Champions.Diana.Menus.Draw
{
    #region Using Directives

    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class DrawQ : ChildBase
    {

        #region Public Properties

        public override string Name { get; set; } = "Draw [Q]";

        #endregion

        #region Public Methods and Operators

        public void OnDraw(EventArgs args)
        {
            if (Variables.Player.IsDead) return;

            Render.Circle.DrawCircle(
                Variables.Player.Position,
                Variables.Spells[SpellSlot.Q].Range,
                Variables.Spells[SpellSlot.Q].IsReady() ? Color.FromArgb(120, 0, 170, 255) : Color.IndianRed);
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

        protected sealed override void OnLoad(object sender, Base.FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu = new Menu(this.Name, this.Name);

            Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            
        }

        #endregion
    }
}