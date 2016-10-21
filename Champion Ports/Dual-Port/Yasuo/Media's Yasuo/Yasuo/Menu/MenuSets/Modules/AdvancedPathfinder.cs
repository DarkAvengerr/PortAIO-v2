// ReSharper disable AccessToForEachVariableInClosure

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.Modules
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using CommonEx.Menu.Interfaces;
    using CommonEx.Menu.Presets;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class AdvancedPathfinderMenu : IMenuSet
    {
        #region Fields

        /// <summary>
        ///     The blacklist
        /// </summary>
        public Menu Blacklist;

        /// <summary>
        ///     The blacklisted heroes
        /// </summary>
        public List<Obj_AI_Base> BlacklistedHeroes;

        /// <summary>
        ///     The display name
        /// </summary>
        public string DisplayName;

        /// <summary>
        ///     The main menu
        /// </summary>
        public Menu Menu { get; set; }

        /// <summary>
        ///     The settings
        /// </summary>
        public Menu Settings;

        /// <summary>
        ///     The dynamic menu
        /// </summary>
        private DynamicMenu dynamicMenu;

        /// <summary>
        ///     The blacklist
        /// </summary>
        private BlacklistMenu blacklistMenu;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPathfinderMenu"/> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="displayName">The display name.</param>
        public AdvancedPathfinderMenu(Menu menu, string displayName)
        {
            this.Menu = menu;
            this.DisplayName = displayName;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds all menu items.
        /// </summary>
        public void Generate()
        {
            var selecter = new MenuItem("Mode", "Dash to: ").SetValue(new StringList(new[] { "Mouse", "Enemy" }));

            var mouse = new List<MenuItem>() { };

            var enemy = new List<MenuItem>()
                            {
                                new MenuItem("Prediction", "Use Prediction").SetValue(true)
                                    .SetTooltip("The assembly will try to E to the enemy predicted position."),
                                new MenuItem("PredictionEnhanced", "Prediction -> Two PathBase System").SetValue(true)
                                    .SetTooltip("The assembly will try to E to the enemy predicted position."),
                                new MenuItem("MinCursorDistance", "Min Cursor Distance to target").SetValue(
                                    new Slider(600, 50, 2000)),
                            };

            this.dynamicMenu = new DynamicMenu(this.Menu, this.DisplayName, selecter, new[] { mouse, enemy });

            this.dynamicMenu.Initialize();

            var both = new List<MenuItem>()
                           {
                               new MenuItem("DontDashUnderTurret", "Don't dash under turret").SetValue(true),
                               new MenuItem("AutoWalkToDash", "[Experimental] Auto-Walk to dash").SetValue(true)
                                   .SetTooltip(
                                       "If this is enabled the assembly will auto-walk behind a unit to dash over it."),
                               new MenuItem("AutoDashing", "[Experimental] Auto-Dash dashable PathBase (Dashing-PathBase)")
                                   .SetValue(true)
                                   .SetTooltip(
                                       "If this is enabled the assembly will automatic pathfind and walk to the end of the PathBase. This is a basic feature of pathfinding."),
                               new MenuItem("AutoWalking", "[Experimental] Auto-Walk non-dashable PathBase (Walking-PathBase)")
                                   .SetValue(false)
                                   .SetTooltip(
                                       "If this is enabled the assembly will automatic pathfind and walk to the end of the PathBase. If you like to have maximum control or your champion disable this."),
                               new MenuItem("PathAroundSkillShots", "[Experimental] Try to PathBase around Skillshots")
                                   .SetValue(true)
                                   .SetTooltip(
                                       "if this is enabled, the assembly will PathBase around a skillshot if a PathBase is given."),
                               new MenuItem("Enabled", "Enabled").SetValue(true),
                           };

            this.blacklistMenu = new BlacklistMenu(this.dynamicMenu.AttachedMenu, "Blacklist");

            foreach (var item in both)
            {
                item.Name = this.dynamicMenu.AttachedMenu.Name + item.Name;

                this.dynamicMenu.AttachedMenu.AddItem(item);
            }

            foreach (var item in this.dynamicMenu.AttachedMenu.Items)
            {
                if (item.Name == "Mode")
                {
                    var stringarray = item.GetValue<StringList>().SList;

                    var id = 0;

                    for (var i = 0; i < stringarray.Count(); i++)
                    {
                        if (stringarray[i] == "Enemy")
                        {
                            id = i + 1;
                        }
                    }

                    foreach (var item2 in this.blacklistMenu.AttachedMenu.Items)
                    {
                        item2.SetTag(id);
                    }

                    break;
                }
            }

            this.Settings = this.dynamicMenu.AttachedMenu;
            this.Blacklist = this.blacklistMenu.AttachedMenu;
            this.BlacklistedHeroes = this.blacklistMenu.BlacklistedHeroes;
        }

        #endregion
    }
}