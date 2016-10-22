using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Combo
{
    using CommonEx.Menu.Interfaces;

    using LeagueSharp.Common;

    internal class LastBreathMenu : BaseMenus.BaseMenuLastBreath, IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LastBreathMenu"/> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public LastBreathMenu(Menu menu) : base(menu)
        { }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public new void Generate()
        {
            base.Generate();

            this.SetupAdvancedMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Method to set advanced settings.
        /// </summary>
        private void SetupAdvancedMenu()
        {
            var advanced = new Menu("Advanced", this.Menu.Name + "Advanced");

            advanced.AddItem(
                new MenuItem(advanced.Name + "EvaluationLogic", "Evaluation Logic").SetValue(
                    new StringList(new[] { "Damage", "Count", "Priority", "Auto" })));

            advanced.AddItem(
                new MenuItem(advanced.Name + "MaxHealthPercDifference", "Max Health (%) Difference").SetValue(
                    new Slider(40)));

            advanced.AddItem(
                new MenuItem(advanced.Name + "OverkillCheck", "Overkill Check").SetValue(true)
                    .SetTooltip(
                        "If Combo is enough to finish the target it won't execute. Only works on single targets."));

            advanced.AddItem(
                new MenuItem(advanced.Name + "Disclaimer", "[i] Disclaimer").SetTooltip(
                    "Changing Values here might destroy the assembly logic, only change values if you know what you are doing!"));

            this.Menu.AddSubMenu(advanced);
        }

        #endregion
    }
}