using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie
{
    using KoreanAnnie.Common;

    using LeagueSharp.Common;

    internal static class AnnieCustomMenu
    {
        #region Public Methods and Operators

        public static void Load(CommonMenu mainMenu)
        {
            RemoveItems(mainMenu);
            LoadLaneClear(mainMenu);
            LoadCombo(mainMenu);
            LoadMisc(mainMenu);
        }

        #endregion

        #region Methods

        private static void LoadCombo(CommonMenu mainMenu)
        {

        }

        private static void LoadLaneClear(CommonMenu mainMenu)
        {
            mainMenu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "saveqtofarm"), "Save Q to Farm").SetValue(true));
            mainMenu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "minminionstow"), "W Must Hit at Least").SetValue(
                    new Slider(3, 1, 6)));
        }

        private static void LoadMisc(CommonMenu mainMenu)
        {
            Menu passiveStunMenu =
                mainMenu.MiscMenu.AddSubMenu(
                    new Menu("Passive Control", KoreanUtils.ParamName(mainMenu, "passivestunmenu")));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "useetostack"), "Use E to Stack").SetValue(true));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "manalimitforstacking"), "Mana % Limit for Stacking")
                    .SetValue(new Slider(30, 0, 100)));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "savestunforcombo"), "Save Stun for Combo/Harass").SetValue
                    (false));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "showeeasybutton"), "Show Stun-Button").SetValue(true));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "easybuttonpositionx"), "Button Position X (Read Only)")
                    .SetValue(0));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "easybuttonpositiony"), "Button Position Y (Read Only)")
                    .SetValue(0));

            Menu flashTibbers =
                mainMenu.MiscMenu.AddSubMenu(
                    new Menu("Flash-Ultimate", KoreanUtils.ParamName(mainMenu, "flashtibbersmenu")));
            flashTibbers.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "flashtibbers"), "Key").SetValue(
                    new KeyBind('T', KeyBindType.Press)));
            flashTibbers.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "minenemiestoflashr"), "Only Use if X or More Enemies Hit")
                    .SetValue(new Slider(2, 1, 5)));
            flashTibbers.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "orbwalktoflashtibbers"), "Allow Movement with Key Pressed").SetValue(false));

            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "supportmode"), "Support Mode").SetValue(false));

            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "useqtofarm"), "Use Q to Farm").SetValue(true));
            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "useeagainstaa"), "Use E against AA").SetValue(true));
            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "antigapcloser"), "Anti-Gapcloser").SetValue(true));
            mainMenu.MiscMenu.AddItem(
                new MenuItem(
                    KoreanUtils.ParamName(mainMenu, "interruptspells"),
                    "Interrupt dangerous spells if possible").SetValue(true));
            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "autotibbers"), "Tibbers - Auto Pilot").SetValue(true));

            Menu DontUseComboMenu = mainMenu.MiscMenu.AddSubMenu(new Menu("Don't Harass/Combo Against", "dontusecomboon"));

            foreach (var enemy in HeroManager.Enemies)
            {
                DontUseComboMenu.AddItem(
                    new MenuItem(
                        KoreanUtils.ParamName(mainMenu, "combo" + enemy.ChampionName.ToLowerInvariant()),
                        enemy.ChampionName).SetValue(true));
            }

            DontUseComboMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "dontuselabel1"), "------------------------------"));
            DontUseComboMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "dontuselabel2"), "IMPORTANT: Targets set to OFF will be..."));
            DontUseComboMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "dontuselabel3"), "...attacked if alone or are at low HP"));
        }

        private static void RemoveItems(CommonMenu mainMenu)
        {
            mainMenu.HarasMenu.Items.Remove(mainMenu.HarasMenu.Item(KoreanUtils.ParamName(mainMenu, "useetoharas")));
            mainMenu.LaneClearMenu.Items.Remove(
                mainMenu.LaneClearMenu.Item(KoreanUtils.ParamName(mainMenu, "useetolaneclear")));
            mainMenu.ComboMenu.Items.Remove(mainMenu.ComboMenu.Item(KoreanUtils.ParamName(mainMenu, "useetocombo")));
        }

        #endregion
    }
}
