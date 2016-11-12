using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Combo
{
    #region Using Directives

    using System.Drawing;

    using CommonEx.Menu.Interfaces;

    using LeagueSharp.Common;

    #endregion

    internal class SweepingBladeMenu : IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastHitSweepingBladeMenu" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public SweepingBladeMenu(Menu menu)
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
        ///     Gets or sets the drawing menu.
        /// </summary>
        /// <value>
        ///     The drawing menu.
        /// </value>
        internal Menu DrawingMenu { get; set; }

        /// <summary>
        ///     Gets or sets the on champion menu.
        /// </summary>
        /// <value>
        ///     The on champion menu.
        /// </value>
        internal Menu OnChampionMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Generate()
        {
            this.SetupDrawingMenu();
            this.SetupUpChampionMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Setups the drawing menu.
        /// </summary>
        private void SetupDrawingMenu()
        {
            this.DrawingMenu = new Menu("Drawings", this.Menu.Name + "Drawings");

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "Enabled", "Enabled").SetValue(true)
                    .SetTooltip("The assembly will Draw the expected PathBase to the enemy"));

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "SmartDrawings", "Smart Drawings").SetValue(true)
                    .SetTooltip("Automaticall disables Drawings under certain circumstances and will do auto-coloring."));

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "PathDashColor", "Dashes").SetValue(new Circle(true, Color.DodgerBlue)));

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "PathDashWidth", "Width of lines").SetValue(new Slider(2, 1, 10)));

            this.DrawingMenu.AddItem(new MenuItem(this.Menu.Name + "Seperator1", ""));

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "PathWalkColor", "Walking").SetValue(new Circle(true, Color.White)));

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "PathWalkWidth", "Width of lines").SetValue(new Slider(2, 1, 10)));

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "CirclesColor", "Draw Circles").SetValue(
                    new Circle(true, Color.DodgerBlue)));

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "CirclesLineWidth", "Width of lines").SetValue(new Slider(2, 1, 10)));

            this.DrawingMenu.AddItem(
                new MenuItem(this.Menu.Name + "CirclesRadius", "Radius").SetValue(new Slider(40, 10, 475)));

            this.Menu.AddSubMenu(this.DrawingMenu);
        }

        /// <summary>
        ///     Adds the general menu.
        /// </summary>
        private void SetupUpChampionMenu()
        {
            this.OnChampionMenu = new Menu("Dash On ChampionYasuo", this.Menu.Name + "EOnChampionMenu");

            this.OnChampionMenu.AddItem(
                new MenuItem(this.Menu.Name + "MaxHealthDashOut", "Dash defensively if Health % <=").SetValue(
                    new Slider(30)));

            this.OnChampionMenu.AddItem(
                new MenuItem(this.Menu.Name + "OnlyKillableCombo", "Only dash on champion if killable by Combo")
                    .SetValue(true));

            this.Menu.AddSubMenu(this.OnChampionMenu);
        }

        #endregion
    }
}