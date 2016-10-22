using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Orbwalker.Implementations
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    /// <summary>
    ///     A simple FeatureSystem implementation containing an instance of an Orbwalker.
    /// </summary>
    /// <seealso cref="RethoughtLib.FeatureSystem.Abstract_Classes.Base" />
    public class OrbwalkerModule : Base
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Orbwalker";

        /// <summary>
        ///     Gets or sets the orbwalker instance.
        /// </summary>
        /// <value>
        ///     The orbwalker instance.
        /// </value>
        public Orbwalking.Orbwalker OrbwalkerInstance { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            this.OrbwalkerInstance.SetAttack(false);
            this.OrbwalkerInstance.SetMovement(false);
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            this.OrbwalkerInstance.SetAttack(true);
            this.OrbwalkerInstance.SetMovement(true);
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);
        }

        /// <summary>
        ///     Sets the menu
        /// </summary>
        protected override void SetMenu()
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.OrbwalkerInstance = new Orbwalking.Orbwalker(this.Menu);

            this.Menu.DisplayName = this.Name;
        }

        #endregion
    }
}