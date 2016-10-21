using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Switches
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    #endregion

    public class UnreversibleSwitch : SwitchBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SwitchBase" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public UnreversibleSwitch(Menu menu)
            : base(menu)
        {
            this.Enabled = true;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        public override void Setup()
        {
            return;
        }

        #endregion
    }
}