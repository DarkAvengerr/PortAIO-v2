// ReSharper disable AccessToForEachVariableInClosure

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Menu.Presets
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class WhitelistMenu
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
        ///     The whitelisted heroes
        /// </summary>
        public List<Obj_AI_Base> WhitelistedHeroes = new List<Obj_AI_Base>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WhitelistMenu" /> class.
        /// </summary>
        /// <param name="displayName">The menu.</param>
        /// <param name="displayName">The display name.</param>
        public WhitelistMenu(Menu menu, string displayName)
        {
            this.Menu = menu;
            this.DisplayName = displayName;

            this.SetupMenu();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the enemies.
        /// </summary>
        public void AddEnemies()
        {
            if (HeroManager.Enemies.Count == 0)
            {
                this.AttachedMenu.AddItem(new MenuItem(this.AttachedMenu.Name + "null", "No enemies found"));
            }
            else
            {
                foreach (var x in HeroManager.Enemies)
                {
                    this.AttachedMenu.AddItem(
                        new MenuItem(this.AttachedMenu.Name + x.ChampionName, x.ChampionName).SetValue(true)).ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                if (eventArgs.GetNewValue<bool>())
                                {
                                    if (!this.WhitelistedHeroes.Contains(x))
                                    {
                                        this.WhitelistedHeroes.Add(x);
                                    }
                                }
                                else
                                {
                                    if (this.WhitelistedHeroes.Contains(x))
                                    {
                                        this.WhitelistedHeroes.Remove(x);
                                    }
                                }
                            };
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Setups the menu.
        /// </summary>
        private void SetupMenu()
        {
            if (this.Menu == null)
            {
                return;
            }

            this.AttachedMenu = new Menu(this.DisplayName, this.Menu.Name + this.DisplayName);

            this.Menu.AddSubMenu(this.AttachedMenu);
        }

        #endregion
    }
}