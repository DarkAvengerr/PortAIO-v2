using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Combo
{
    #region Using Directives

    using System.Collections.Generic;

    using CommonEx.Menu;
    using CommonEx.Menu.Interfaces;
    using CommonEx.Menu.Presets;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.BaseMenus;

    using LeagueSharp.Common;

    #endregion

    internal class SteelTempestMenu : BaseMenuSteelTempest, IMenuSet
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SteelTempestMenu" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public SteelTempestMenu(Menu menu)
            : base(menu)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public new void Generate()
        {
            base.Generate();

            this.HideBaseEntries();

            this.SetupMultiKnockupMenu();
            this.SetupStackingMenu();
        }

        #endregion

        #region Methods

        private void HideBaseEntries()
        {
            this.Menu.Item(this.Menu.Name + "MinHitAOE").Hide();
        }

        /// <summary>
        ///     Setups the Multi-KnockUp Menu
        /// </summary>
        private void SetupMultiKnockupMenu()
        {
            var selecter =
                new MenuItem("Mode", "Mode").SetValue(new StringList(new[] { "Custom", "Disabled" }, 0));

            var custom = new List<MenuItem>()
                             {
                                 new MenuItem("BuffState", "Only if: ").SetValue(
                                     new StringList(new[] { "Q3 (Stacked)", "Not Stacked", "Always" })),
                                 new MenuItem("MinHitAOECustom", "Min HitCount for AOE").SetValue(new Slider(2, 1, 5)),
                             };

            var pathBased = new List<MenuItem>()
                                {
                                    new MenuItem(
                                        "DisclaimerPathBased",
                                        "[Experimental] Assembly will try to decide based on pathing"),
                                    new MenuItem("BuffStatePathBased", "Only if: ").SetValue(
                                        new StringList(new[] { "Q3 (Stacked)", "Not Stacked", "Always" })),
                                    new MenuItem("MinHitAOEPathBased", "Min HitCount for AOE").SetValue(
                                        new Slider(2, 1, 5)),
                                    new MenuItem("SegmentAmount", "Amount of calculations: ").SetValue(
                                        new Slider(50, 1, 500)),
                                    new MenuItem("PriorityMode", "Priority/Decisison MOde: ").SetValue(
                                        new StringList(new[] { "ChampionYasuo Priority (TargetSelector)", "TODO: Killable" })),
                                };

            var dynamicMenu = new DynamicMenu(this.Menu, "Multi-Knockup Settings", selecter, new[] { custom });

            dynamicMenu.Initialize();
        }

        /// <summary>
        ///     Setups the Stack Menu.
        /// </summary>
        private void SetupStackingMenu()
        {
            var selecter =
                new MenuItem("Mode", "Mode").SetValue(
                    new StringList(new[] { "Custom", "PathBase Based", "Disabled" }));

            var custom = new List<MenuItem>()
                             {
                                 new MenuItem("MinDistance", "Don't Stack if Distance to enemy <= ").SetValue(
                                     new Slider(900, 0, 4000)),
                                 new MenuItem("MaxDistance", "Don't Stack if Distance to enemy >= ").SetValue(
                                     new Slider(1500, 0, 4000)),
                                 new MenuItem("MaxCooldownQ", "Don't Stack if Q Cooldown is >= (milliseconds)").SetValue
                                     (new Slider(1700, 1333, 5000)),
                                 new MenuItem("CarePath", "Don't kill units in SweepingBlade PathBase").SetValue(true),
                             };

            var dynamicMenu = new DynamicMenu(this.Menu, "Stack-Settings", selecter, new[] { custom });

            dynamicMenu.Initialize();
        }

        #endregion
    }
}