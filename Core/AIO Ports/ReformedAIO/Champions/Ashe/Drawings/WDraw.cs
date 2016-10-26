namespace ReformedAIO.Champions.Ashe.Drawings
{
    #region Using Directives

    using System;
    using System.Drawing;

    using EloBuddy;
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
            Name = name;
        }

        public void OnDraw(EventArgs args)
        {
            if (Variable.Player.IsDead) return;

            if (Menu.Item(Menu.Name + "WReady").GetValue<bool>() && !Variable.Spells[SpellSlot.W].IsReady()) return;

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
            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem(Name + "WReady", "Only If Ready").SetValue(false));
        }

        #endregion
    }
}