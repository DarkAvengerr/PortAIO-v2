using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.BaseMenus
{
    #region Using Directives

    using CommonEx.Menu.Interfaces;

    using LeagueSharp.Common;

    #endregion

    internal class BaseMenuLastBreath : IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMenuSweepingBlade" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public BaseMenuLastBreath(Menu menu)
        {
            this.Menu = menu;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the advanced menu
        /// </summary>
        internal Menu AdvancedMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Generate()
        {
            this.SetupRootMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Method to set general settings.
        /// </summary>
        private void SetupRootMenu()
        {
            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "MinHitAOE", "Min HitCount for AOE").SetValue(new Slider(2, 1, 5)));

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "MinPlayerHealth", "Min Player Health (%)").SetValue(new Slider(10)));

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "MaxTargetsMeanHealth", "Max Target(s) Health (%)").SetValue(new Slider(80)));
        }

        #endregion
    }
}