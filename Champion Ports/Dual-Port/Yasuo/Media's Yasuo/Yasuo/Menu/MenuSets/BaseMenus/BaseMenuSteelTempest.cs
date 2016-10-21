using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.BaseMenus
{
    #region Using Directives

    using CommonEx.Menu.Interfaces;

    using LeagueSharp.Common;

    #endregion

    public class BaseMenuSteelTempest : IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMenuSteelTempest"/> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        protected BaseMenuSteelTempest(Menu menu)
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
        ///     Gets the advanced menu.
        /// </summary>
        /// <value>
        ///     The advanced menu.
        /// </value>
        internal Menu AdvancedMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Generate()
        {
            this.SetupAdvancedMenu();
            this.SetupRootMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Setups the advanced menu.
        /// </summary>
        private void SetupAdvancedMenu()
        {
            this.AdvancedMenu = new Menu("Advanced", this.Menu.Name + "Advanced");

            this.AdvancedMenu.AddItem(
                new MenuItem(this.AdvancedMenu.Name + "TurretCheck", "Don't use spells under enemy turret").SetValue(
                    true)
                    .SetTooltip(
                        "if this is enabled, the assembly won't use stacked/charged Q under the enemy turret. But it will use them if the turret is focusing something else!"));

            this.Menu.AddSubMenu(this.AdvancedMenu);
        }

        /// <summary>
        ///     Setups the general menu.
        /// </summary>
        private void SetupRootMenu()
        {
            this.Menu.AddItem(new MenuItem(this.Menu.Name + "MinHitAOE", "Min Hit (AOE)").SetValue(new Slider(2, 1, 5)));
        }

        #endregion
    }
}