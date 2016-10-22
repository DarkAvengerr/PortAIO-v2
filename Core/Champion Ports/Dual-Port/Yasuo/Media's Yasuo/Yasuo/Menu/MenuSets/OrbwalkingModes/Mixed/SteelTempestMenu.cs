using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Mixed
{
    using CommonEx.Menu.Interfaces;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.BaseMenus;

    using LeagueSharp.Common;

    internal class SteelTempestMenu : BaseMenuSteelTempest, IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastHit.SteelTempestMenu" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public SteelTempestMenu(Menu menu) : base(menu)
        { }

        #endregion
        
        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public new void Generate()
        {
            base.Generate();

            this.SetupRootMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Setups the general menu.
        /// </summary>
        private void SetupRootMenu()
        {
            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "NoQ3Count", "Don't use Q3 if >= Enemies around").SetValue(
                    new Slider(2, 0, 5)));

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "NoQ3Range", "Check for enemies in range of").SetValue(
                    new Slider(1000, 0, 5000)));
        }

        #endregion
    }
}