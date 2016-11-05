using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie.Common
{
    using LeagueSharp.Common;

    internal class CommonMenu
    {
        #region Fields

        private readonly Menu comboMenu;

        private readonly Menu drwaingsMenu;

        private readonly Menu harasMenu;

        private readonly Menu laneClearMenu;

        private readonly Menu mainMenu;

        private readonly string menuName;

        private readonly Menu miscMenu;

        #endregion

        #region Constructors and Destructors

        public CommonMenu(string displayName, bool misc)
        {
            menuName = displayName.Replace(" ", "_").ToLowerInvariant();

            mainMenu = new Menu(displayName, menuName, true);

            AddOrbwalker(mainMenu);
            AddTargetSelector(mainMenu);

            Menu modes = new Menu("Modes", string.Format("{0}.modes", MenuName));
            mainMenu.AddSubMenu(modes);

            harasMenu = AddHarasMenu(modes);
            laneClearMenu = AddLaneClearMenu(modes);
            comboMenu = AddComboMenu(modes);

            if (misc)
            {
                miscMenu = AddMiscMenu(mainMenu);
            }

            drwaingsMenu = AddDrawingMenu(mainMenu);

            mainMenu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public Menu ComboMenu
        {
            get
            {
                return comboMenu;
            }
        }

        public Menu DrawingMenu
        {
            get
            {
                return drwaingsMenu;
            }
        }

        public Menu HarasMenu
        {
            get
            {
                return harasMenu;
            }
        }

        public Menu LaneClearMenu
        {
            get
            {
                return laneClearMenu;
            }
        }

        public Menu MainMenu
        {
            get
            {
                return mainMenu;
            }
        }

        public Menu MiscMenu
        {
            get
            {
                return miscMenu;
            }
        }

        public Orbwalking.Orbwalker Orbwalker { get; private set; }

        #endregion

        #region Properties

        private string MenuName
        {
            get
            {
                return menuName;
            }
        }

        #endregion

        #region Methods

        private Menu AddComboMenu(Menu rootMenu)
        {
            Menu newMenu = new Menu("Combo", string.Format("{0}.combo", MenuName));
            rootMenu.AddSubMenu(newMenu);

            newMenu.AddItem(new MenuItem(string.Format("{0}.useqtocombo", MenuName), "Use Q").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.usewtocombo", MenuName), "Use W").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.useetocombo", MenuName), "Use E").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.usertocombo", MenuName), "Use R").SetValue(true));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.minenemiestor", MenuName), "Use R if it Will Hit at Least").SetValue(
                    new Slider(3, 1, 5)));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.disableaa", MenuName), "Disable AA When").SetValue(
                    new StringList(
                        new[] { "Never", "Always", "Some Skills Ready", "Harass-Combo Ready", "Full-Combo Ready" })));
            newMenu.AddItem(
                new MenuItem(
                    string.Format("{0}.forceultusingmouse", MenuName),
                    "Force Ultimate Using Mouse-buttons (Cursor Sprite)").SetValue(true));

            return newMenu;
        }

        private Menu AddDrawingMenu(Menu rootMenu)
        {
            Menu newMenu = new Menu("Drawings", string.Format("{0}.drawings", MenuName));
            rootMenu.AddSubMenu(newMenu);

            newMenu.AddItem(new MenuItem(string.Format("{0}.drawskillranges", MenuName), "Skill Ranges").SetValue(true));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.damageindicator", MenuName), "Damage Indicator").SetValue(true));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.damageindicatorcolor", MenuName), "Color Scheme").SetValue(
                    new StringList(new string[] { "Normal", "Colorblind" })));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.killableindicator", MenuName), "Killable Indicator").SetValue(true));

            return newMenu;
        }

        private Menu AddHarasMenu(Menu rootMenu)
        {
            Menu newMenu = new Menu("Harass", string.Format("{0}.haras", MenuName));
            rootMenu.AddSubMenu(newMenu);

            newMenu.AddItem(new MenuItem(string.Format("{0}.useqtoharas", MenuName), "Use Q").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.usewtoharas", MenuName), "Use W").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.useetoharas", MenuName), "Use E").SetValue(true));

            MenuItem manaLimitToHaras =
                new MenuItem(string.Format("{0}.manalimittoharas", MenuName), "Mana % Limit").SetValue(
                    new Slider(0, 0, 100));
            newMenu.AddItem(manaLimitToHaras);

            return newMenu;
        }

        private Menu AddLaneClearMenu(Menu rootMenu)
        {
            Menu newMenu = new Menu("Lane Clear", string.Format("{0}.laneclear", MenuName));
            rootMenu.AddSubMenu(newMenu);

            newMenu.AddItem(new MenuItem(string.Format("{0}.useqtolaneclear", MenuName), "Use Q").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.usewtolaneclear", MenuName), "Use W").SetValue(true));
            newMenu.AddItem(new MenuItem(string.Format("{0}.useetolaneclear", MenuName), "Use E").SetValue(true));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.manalimittolaneclear", MenuName), "Mana % Limit").SetValue(
                    new Slider(50, 0, 100)));
            newMenu.AddItem(
                new MenuItem(string.Format("{0}.harasonlaneclear", MenuName), "Harass Enemies in Lane Clear").SetValue(true));

            return newMenu;
        }

        private Menu AddMiscMenu(Menu rootMenu)
        {
            Menu newMenu = new Menu("Options", string.Format("{0}.misc", MenuName));
            rootMenu.AddSubMenu(newMenu);

            return newMenu;
        }

        private void AddOrbwalker(Menu rootMenu)
        {
            Menu orbwalkerMenu = new Menu("Orbwalker", string.Format("{0}.orbwalker", MenuName));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            rootMenu.AddSubMenu(orbwalkerMenu);
        }

        private void AddTargetSelector(Menu rootMenu)
        {
            Menu targetselectorMenu = new Menu("Target Selector", string.Format("{0}.targetselector", MenuName));
            TargetSelector.AddToMenu(targetselectorMenu);
            rootMenu.AddSubMenu(targetselectorMenu);
        }

        #endregion
    }
}
