    using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Offensive2 : IPlugin
    {
        #region Fields

        private readonly List<Item> offensiveItems;

        #endregion

        #region Constructors and Destructors

        //[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public Offensive2()
        {
            this.offensiveItems =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(
                        x =>
                        x.Namespace != null && x.Namespace.Contains("OffensiveItems") && x.IsClass
                        && typeof(Item).IsAssignableFrom(x))
                    .Select(x => (Item)Activator.CreateInstance(x))
                    .ToList();
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

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            this.Menu = rootMenu.AddSubMenu(new Menu("Offensive", "omenu2"));

            foreach (var item in this.offensiveItems)
            {
                var submenu = new Menu(item.Name, (int)item.Id + item.Name);

                item.Menu = submenu;
                item.CreateMenu();

                this.Menu.AddSubMenu(submenu);
            }
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Game.OnUpdate += this.Game_OnUpdate;
        }

        #endregion

        #region Methods

        private void Game_OnUpdate(EventArgs args)
        {
            foreach (var item in this.offensiveItems.Where(x => x.ShouldUseItem() && Items.CanUseItem((int)x.Id)))
            {
                item.UseItem();
            }
        }

        #endregion
    }
}