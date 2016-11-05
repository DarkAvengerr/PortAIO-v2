using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.BaseMenus
{
    #region Using Directives

    using CommonEx.Menu.Interfaces;

    using LeagueSharp.Common;

    #endregion

    internal class BaseMenuSweepingBlade : IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMenuSweepingBlade" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public BaseMenuSweepingBlade(Menu menu)
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
            this.SetupMiscMenu();
            this.SetupRootMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the misc menu.
        /// </summary>
        private void SetupMiscMenu()
        {
            this.Menu.AddItem(new MenuItem(this.Menu.Name + "NoSkillshot", "Don't E into Skillshots").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "NoTurret", "Don't E into Turret").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "NoEnemy", "Don't E into Enemies").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "NoWallDash", "Don't E over walls").SetValue(true));
        }

        /// <summary>
        ///     Adds the general menu.
        /// </summary>
        private void SetupRootMenu()
        {
            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "DashOrientation", "Dash Orientation: ").SetValue(
                    new StringList(new[] { "Mouse", "Auto" })));
        }

        #endregion
    }
}