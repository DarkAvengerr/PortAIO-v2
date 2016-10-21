using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Mixed
{
    using CommonEx.Menu.Interfaces;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.BaseMenus;

    using LeagueSharp.Common;

    internal class SweepingBladeMenu : BaseMenuSweepingBlade, IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SweepingBladeMenu"/> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public SweepingBladeMenu(Menu menu) : base(menu)
        { }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public new void Generate()
        {
            base.Generate();
        }

        #endregion
    }
}