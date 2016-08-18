// ReSharper disable AccessToForEachVariableInClosure

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Menu.Presets
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class ChampionSliderMenu
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
        ///     The blacklisted heroes
        /// </summary>
        public Dictionary<Obj_AI_Base, float> Values = new Dictionary<Obj_AI_Base, float>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChampionSliderMenu" /> class.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="displayName">
        ///     The menu.
        ///     The display name.
        /// </param>
        public ChampionSliderMenu(Menu menu, string displayName)
        {
            this.Menu = menu;
            this.DisplayName = displayName;

            this.SetupMenu();
            this.AddEnemies();
        }

        #endregion

        #region Public Properties

        public int Modifier { get; set; }

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
                var maxRange = (int)HeroManager.Enemies.MaxOrDefault(x => x.AttackRange).AttackRange;

                foreach (var hero in HeroManager.Enemies)
                {
                    var range = (int)hero.AttackRange;

                    this.AttachedMenu.AddItem(
                        new MenuItem(this.AttachedMenu.Name + hero.ChampionName, hero.ChampionName).SetValue(
                            new Slider(range + this.Modifier, 0, maxRange))).ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                if (!this.Values.ContainsKey(hero))
                                {
                                    this.Values.Add(hero, eventArgs.GetNewValue<Slider>().Value);
                                }
                                else
                                {
                                    this.Values[hero] = eventArgs.GetNewValue<Slider>().Value;
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

            this.AttachedMenu.AddItem(new MenuItem(this.AttachedMenu.Name + "Enabled", "Enabled").SetValue(true));

            this.AttachedMenu.AddItem(
                new MenuItem(this.AttachedMenu.Name + "Modifier", "Modifier").SetValue(new Slider(0, -2000, 2000)))
                .ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        this.Modifier = eventArgs.GetNewValue<Slider>().Value;
                    };
        }

        #endregion
    }
}