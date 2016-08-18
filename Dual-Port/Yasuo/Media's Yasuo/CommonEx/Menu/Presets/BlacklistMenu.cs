// ReSharper disable AccessToForEachVariableInClosure

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Menu.Presets
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class BlacklistMenu
    {
        #region Fields

        /// <summary>
        ///     The new menu
        /// </summary>
        public Menu AttachedMenu;

        /// <summary>
        ///     The blacklisted heroes
        /// </summary>
        public List<Obj_AI_Base> BlacklistedHeroes = new List<Obj_AI_Base>();

        /// <summary>
        ///     The display name
        /// </summary>
        public string DisplayName;

        /// <summary>
        ///     The main menu
        /// </summary>
        public Menu Menu;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlacklistMenu" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu
        /// </param>
        /// <param name="displayName">
        ///     The display name.
        /// </param>
        public BlacklistMenu(Menu menu, string displayName)
        {
            this.Menu = menu;
            this.DisplayName = displayName;

            this.SetupMenu();
            this.AddEnemies();
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
                        new MenuItem(this.AttachedMenu.Name + x.ChampionName, x.ChampionName).SetValue(false)).ValueChanged
                        += delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                if (eventArgs.GetNewValue<bool>())
                                {
                                    if (!this.BlacklistedHeroes.Contains(x))
                                    {
                                        this.BlacklistedHeroes.Add(x);
                                    }
                                }
                                else
                                {
                                    if (this.BlacklistedHeroes.Contains(x))
                                    {
                                        this.BlacklistedHeroes.Remove(x);
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