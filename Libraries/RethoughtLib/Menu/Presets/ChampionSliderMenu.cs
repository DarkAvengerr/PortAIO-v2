using EloBuddy; namespace RethoughtLib.Menu.Presets
{
    #region Using Directives

    using System;
    using System.Linq;

    using global::RethoughtLib.Menu.Interfaces;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class ChampionSliderMenu : IMenuPreset
    {
        #region Fields

        /// <summary>
        ///     The function to set the slider value
        /// </summary>
        public Func<AIHeroClient, int> FuncSliderValue = x => 0;

        /// <summary>
        ///     The function to validate a hero
        /// </summary>
        public Func<AIHeroClient, bool> FuncValidateHero = x => true;

        /// <summary>
        ///     The display name
        /// </summary>
        private readonly string displayName;

        /// <summary>
        ///     The new menu
        /// </summary>
        private Menu attachedMenu;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChampionSliderMenu" /> class.
        /// </summary>
        /// <param name="displayName">
        ///     The menu.
        ///     The display name.
        /// </param>
        public ChampionSliderMenu(string displayName)
        {
            this.displayName = displayName;
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

        public int Modifier { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Generates this instance.
        /// </summary>
        public void Generate()
        {
            this.SetupMenu();
            this.AddEnemies();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the enemies.
        /// </summary>
        private void AddEnemies()
        {
            if (HeroManager.Enemies.Count == 0)
            {
                this.attachedMenu.AddItem(new MenuItem(this.attachedMenu.Name + "null", "No enemies found"));
            }
            else
            {
                var maxValue =
                    HeroManager.Enemies.Where(hero => this.FuncValidateHero(hero))
                        .Select(hero => this.FuncSliderValue(hero))
                        .Concat(new[] { 0 })
                        .Max();

                foreach (var hero in HeroManager.Enemies)
                {
                    var value = this.FuncSliderValue(hero);

                    this.attachedMenu.AddItem(
                        new MenuItem(this.attachedMenu.Name + hero.ChampionName, hero.ChampionName).SetValue(
                            new Slider(value + this.Modifier, 0, maxValue)));
                }
            }
        }

        /// <summary>
        ///     Setups the menu.
        /// </summary>
        private void SetupMenu()
        {
            if (this.Menu == null)
            {
                return;
            }

            this.attachedMenu = new Menu(this.displayName, this.Menu.Name + this.displayName);

            this.Menu.AddSubMenu(this.attachedMenu);

            this.attachedMenu.AddItem(new MenuItem(this.attachedMenu.Name + "Enabled", "Enabled").SetValue(true));

            this.attachedMenu.AddItem(
                new MenuItem(this.attachedMenu.Name + "Modifier", "Modifier").SetValue(new Slider(0, -2000, 2000)))
                .ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        this.Modifier = eventArgs.GetNewValue<Slider>().Value;
                    };
        }

        #endregion
    }
}