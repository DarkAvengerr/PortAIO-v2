// ReSharper disable AccessToForEachVariableInClosure

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Menu.MenuSets.Modules
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using CommonEx.Menu;
    using CommonEx.Menu.Interfaces;
    using CommonEx.Menu.Presets;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class SimplePathfinderMenu : IMenuSet
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
        ///     The blacklist
        /// </summary>
        private BlacklistMenu blacklistMenu;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePathfinderMenu"/> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="displayName">The display name.</param>
        public SimplePathfinderMenu(Menu menu, string displayName)
        {
            this.Menu = menu;
            this.DisplayName = displayName;

            this.Settings = new Menu(this.DisplayName, "Pathfinder");
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds all menu items.
        /// </summary>
        public void Generate()
        {
            if (this.Menu == null)
            {
                return;
            }

            var menuItems = new List<MenuItem>()
                        {
                            new MenuItem("DontDashUnderTurret", "Don't dash under turret").SetValue(false),
                            new MenuItem("AutoWalkToDash", "[WIP > Disabled] Auto-Walk to dash").SetValue(true)
                                .SetTooltip(
                                    "If this is enabled the assembly will auto-walk behind a unit to dash over it."),
                            new MenuItem("AutoDashing", "Auto-Dash dashable PathBase (Dashing-PathBase)")
                                .SetValue(true)
                                .SetTooltip(
                                    "If this is enabled the assembly will automatic pathfind and walk to the end of the PathBase. This is a basic feature of pathfinding."),
                            new MenuItem("AutoWalking", "[WIP > Disabled] Auto-Walk non-dashable PathBase (Walking-PathBase)")
                                .SetValue(false)
                                .SetTooltip(
                                    "If this is enabled the assembly will automatic pathfind and walk to the end of the PathBase. If you like to have maximum control or your champion disable this."),
                            new MenuItem("PathAroundSkillShots", "[WIP > Disabled] Try to PathBase around Skillshots")
                                .SetValue(true)
                                .SetTooltip(
                                    "if this is enabled, the assembly will PathBase around a skillshot if a PathBase is given."),
                            new MenuItem("Enabled", "Enabled").SetValue(true),
                        };

            this.blacklistMenu = new BlacklistMenu(this.Settings, "Blacklist");

            foreach (var item in menuItems)
            {
                item.Name = this.Menu.Name + item.Name;

                this.Settings.AddItem(item);
            }

            foreach (var item in this.Settings.Items)
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

            this.Blacklist = this.blacklistMenu.AttachedMenu;
            this.BlacklistedHeroes = this.blacklistMenu.BlacklistedHeroes;

            this.Menu.AddSubMenu(this.Settings);
        }

        #endregion
    }
}