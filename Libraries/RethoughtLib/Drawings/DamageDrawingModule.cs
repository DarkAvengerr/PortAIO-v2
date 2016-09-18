using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Drawings
{
    #region Using Directives

    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.DamageCalculator;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class DamageDrawingModule : ChildBase
    {
        #region Fields

        /// <summary>
        /// The damage calculator
        /// </summary>
        private readonly IDamageCalculator damageCalculator;

        #endregion

        #region Constructors and Destructors

        public DamageDrawingModule(string name, IDamageCalculator damageCalculator)
        {
            this.damageCalculator = damageCalculator;
            this.Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnDraw -= this.DrawingOnOnDraw;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Drawing.OnDraw += this.DrawingOnOnDraw;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddItem(new MenuItem(this.Path + "." + "color", "Color").SetValue(new Circle(true, Color.OrangeRed)));
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}