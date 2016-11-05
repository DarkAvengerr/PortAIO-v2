using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.LaneClear
{
    #region Using Directives

    using CommonEx.Menu.Interfaces;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.BaseMenus;

    using LeagueSharp.Common;

    #endregion

    internal class SteelTempestMenu : BaseMenuSteelTempest, IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastHit.SweepingBladeMenu" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public SteelTempestMenu(Menu menu) : base(menu)
        {
            this.Menu = menu;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public new void Generate() 
        {
            base.Generate();

            this.SetupGeneralMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the general menu.
        /// </summary>
        private void SetupGeneralMenu()
        {
            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "CenterCheck", "Check for the minions mean vector").SetValue(true)
                    .SetTooltip(
                        "if this is enabled, the assembly will try to not use stacked/charged Q inside many minions and will either wait until the buff runs out or until you are further away from the minions to hit more."));
        }

        #endregion
    }
}