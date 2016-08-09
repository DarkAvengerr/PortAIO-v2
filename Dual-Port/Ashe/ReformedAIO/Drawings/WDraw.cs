using EloBuddy; namespace ReformedAIO.Champions.Ashe.Drawings
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

        public override string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        public WDraw(string name)
        {
            this.Name = name;
        }

        public void OnDraw(EventArgs args)
        {
            if (Variable.Player.IsDead) return;

            if (this.Menu.Item(this.Menu.Name + "WReady").GetValue<bool>() && !Variable.Spells[SpellSlot.W].IsReady()) return;

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
            Drawing.OnDraw -= this.OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += this.OnDraw;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(new MenuItem(this.Name + "WReady", "Only If Ready").SetValue(false));
        }

        #endregion
    }
}