using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Menu.Presets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Common;

    public class DynamicMenu
    {
        #region Fields

        /// <summary>
        ///     The new menu
        /// </summary>
        public Menu AttachedMenu;

        /// <summary>
        ///     The display name
        /// </summary>
        public string DisplayName;

        /// <summary>
        ///     The main menu
        /// </summary>
        public Menu Menu;

        /// <summary>
        ///     The menu sets
        /// </summary>
        public List<List<MenuItem>> MenuSets;

        /// <summary>
        ///     The selecter
        /// </summary>
        public MenuItem Selecter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicMenu" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="selecter">The selecter.</param>
        /// <param name="menuItems">The menu items.</param>
        public DynamicMenu(Menu menu, string displayName, MenuItem selecter, List<MenuItem>[] menuItems)
        {
            this.Menu = menu;
            this.DisplayName = displayName;
            this.Selecter = selecter;
            this.MenuSets = new List<List<MenuItem>>();

            foreach (var itemSet in menuItems)
            {
                this.MenuSets.Add(itemSet);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Setups the menu.
        /// </summary>
        internal void Initialize()
        {
            if (this.Menu == null || this.MenuSets == null || !this.MenuSets.Any())
            {
                return;
            }

            this.AttachedMenu = new Menu(this.DisplayName, this.Menu.Name + this.DisplayName);

            this.Menu.AddSubMenu(this.AttachedMenu);

            if (GlobalVariables.Debug)
            {
                Console.WriteLine(@" ");
                Console.WriteLine(@"==== Setting up new DynamicMenu ====");
                Console.WriteLine(@"DisplayName: " + this.DisplayName);
                Console.WriteLine(@"Internal Menu Name: " + this.Menu.Name + this.DisplayName);
                Console.WriteLine(@"MenuSets Amount: " + this.MenuSets.Count);
                Console.WriteLine(@"Internal Selecter Name: " + this.Selecter.Name);
                Console.WriteLine(@"Displaying Selecter Name: " + this.Selecter.DisplayName);
                Console.WriteLine(@"Example Naming: " + this.DisplayName + @"SomeItemName");
                Console.WriteLine(@" ");
            }

            var value = this.Selecter.GetValue<StringList>();

            this.AttachedMenu.AddItem(
                new MenuItem(this.AttachedMenu.Name + this.Selecter.Name, this.Selecter.DisplayName).SetValue(value))
                .ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        foreach (var item in this.AttachedMenu.Items)
                        {
                            if (item.Tag != 0)
                            {
                                item.Hide();
                            }

                            if (item.Tag == eventArgs.GetNewValue<StringList>().SelectedIndex + 1)
                            {
                                item.Show();
                            }
                        }
                    };

            var tag = 1;

            foreach (var itemSet in this.MenuSets)
            {
                foreach (var item in itemSet)
                {
                    item.Name = this.AttachedMenu.Name + item.Name;
                    this.AttachedMenu.AddItem(item).SetTag(tag);
                }
                tag++;
            }

            if (GlobalVariables.Debug)
            {
                foreach (var item in this.AttachedMenu.Items)
                {
                    Console.WriteLine(@"DisplayName: {0}, Internal Name: {1}", item.DisplayName, item.Name);
                }
            }

            MenuExtensions.RefreshTagBased(
                this.AttachedMenu,
                this.AttachedMenu.Item(this.AttachedMenu.Name + this.Selecter.Name).GetValue<StringList>().SelectedIndex + 1);
        }

        #endregion
    }
}