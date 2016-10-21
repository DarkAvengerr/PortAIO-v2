using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate
{
    using LeagueSharp.Common;

    internal static class Config
    {
        #region Properties

        internal static Menu CardMenu { get; private set; }

        internal static Menu ComboMenu { get; private set; }

        internal static Menu DrawMenu { get; private set; }

        internal static Menu HarassMenu { get; private set; }

        internal static Menu JungleClearMenu { get; private set; }

        internal static Menu LaneClearMenu { get; private set; }

        internal static Menu MiscMenu { get; private set; }

        internal static Menu OrbwalkMenu { get; private set; }

        internal static Menu TargetSelectorMenu { get; private set; }

        internal static Menu TwistedFateMenu { get; private set; }

        #endregion

        #region Methods

        internal static void BuildConfig()
        {
            TwistedFateMenu = new Menu("Twisted Fate", "twistedfate", true);

            TargetSelectorMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            TwistedFateMenu.AddSubMenu(TargetSelectorMenu);

            OrbwalkMenu = new Menu("Orbwalker", "orbwalker");
            Mainframe.Orbwalker = new Orbwalking.Orbwalker(OrbwalkMenu);
            TwistedFateMenu.AddSubMenu(OrbwalkMenu);

            ComboMenu = new Menu("Combo", "combo");
            ComboMenu.AddItem(new MenuItem("useQCombo", "Use Q").SetValue(true));
            ComboMenu.AddItem(new MenuItem("useWCombo", "Use W").SetValue(true));
            ComboMenu.AddItem(new MenuItem("yellowIntoQ", "Only Q after Yellow Card").SetValue(false));
            ComboMenu.AddItem(
                new MenuItem("wModeC", "W Mode").SetValue(
                    new StringList(new[] { "Smart Mode", "Always Yellow", "Always Blue", "Always Red" })));
            ComboMenu.AddItem(new MenuItem("disableAAselectingC", "Disable AA while selecting Card").SetValue(false));
            ComboMenu.AddItem(new MenuItem("combo.w.extrarange", "Extra W Range").SetValue(new Slider(0, 0, 250)));
            TwistedFateMenu.AddSubMenu(ComboMenu);

            HarassMenu = new Menu("Harass", "harass");
            HarassMenu.AddItem(new MenuItem("useQHarass", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("useWHarass", "Use W").SetValue(true));
            HarassMenu.AddItem(
                new MenuItem("wModeH", "W Mode").SetValue(
                    new StringList(new[] { "Smart Mode", "Always Yellow", "Always Blue", "Always Red" })));
            HarassMenu.AddItem(new MenuItem("disableAAselectingH", "Disable AA while selecting Card").SetValue(false));
            HarassMenu.AddItem(new MenuItem("manaToHarass", "Min Mana % to Harass").SetValue(new Slider(50)));
            HarassMenu.AddItem(new MenuItem("autoharass", "Auto Harass"));
            HarassMenu.AddItem(new MenuItem("autoQ", "Auto Q Harass").SetValue(false));
            HarassMenu.AddItem(new MenuItem("manaToAHarass", "Min Mana % to Auto Harass").SetValue(new Slider(70)));
            TwistedFateMenu.AddSubMenu(HarassMenu);

            LaneClearMenu = new Menu("Lane Clear", "laneclear");
            LaneClearMenu.AddItem(new MenuItem("useQinLC", "Use Q").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("qTargetsLC", "Min Targets to hit for Q").SetValue(new Slider(3, 1, 10)));
            LaneClearMenu.AddItem(new MenuItem("useWinLC", "Use W").SetValue(true));
            LaneClearMenu.AddItem(
                new MenuItem("wModeLC", "W Mode").SetValue(
                    new StringList(new[] { "Smart Mode", "Always Yellow", "Always Blue", "Always Red" })));
            LaneClearMenu.AddItem(
                new MenuItem("disableAAselectingLC", "Disable AA while selecting Card").SetValue(false));
            LaneClearMenu.AddItem(new MenuItem("manaToLC", "Min Mana % to LaneClear").SetValue(new Slider(30)));
            TwistedFateMenu.AddSubMenu(LaneClearMenu);

            JungleClearMenu = new Menu("Jungle Clear", "jungleclear");
            JungleClearMenu.AddItem(new MenuItem("useQinJC", "Use Q").SetValue(true));
            JungleClearMenu.AddItem(new MenuItem("useWinJC", "Use W").SetValue(true));
            JungleClearMenu.AddItem(
                new MenuItem("wModeJC", "W Mode").SetValue(
                    new StringList(new[] { "Smart Mode", "Always Yellow", "Always Blue", "Always Red" })));
            JungleClearMenu.AddItem(
                new MenuItem("disableAAselectingJC", "Disable AA while selecting Card").SetValue(false));
            JungleClearMenu.AddItem(new MenuItem("manaToJC", "Min Mana % to JungleClear").SetValue(new Slider(10)));
            TwistedFateMenu.AddSubMenu(JungleClearMenu);

            CardMenu = new Menu("Card Selector", "cardselector");
            CardMenu.AddItem(
                new MenuItem("csYellow", "Select Yellow Card").SetValue(
                    new KeyBind("W".ToCharArray()[0], KeyBindType.Press)));
            CardMenu.AddItem(
                new MenuItem("csBlue", "Select Blue Card").SetValue(
                    new KeyBind("E".ToCharArray()[0], KeyBindType.Press)));
            CardMenu.AddItem(
                new MenuItem("csRed", "Select Red Card").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            CardMenu.AddItem(new MenuItem("ignoreFirst", "Ignore First Card").SetValue(true));
            CardMenu.AddItem(new MenuItem("humanizePicks", "Humanize Card picks").SetValue(true));
            CardMenu.AddItem(new MenuItem("humanizeLower", "Lower randomize bound").SetValue(new Slider(50, 0, 250)))
                .ValueChanged += OnLowerChange;
            CardMenu.AddItem(new MenuItem("humanizeUpper", "Upper randomize bound").SetValue(new Slider(90, 10, 300)))
                .ValueChanged += OnUpperChange;
            TwistedFateMenu.AddSubMenu(CardMenu);

            MiscMenu = new Menu("Misc", "misc");
            MiscMenu.AddItem(new MenuItem("AutoYAG", "Auto Yellow on R teleport").SetValue(true));
            MiscMenu.AddItem(new MenuItem("qKillsteal", "Killsteal with Q").SetValue(true));
            MiscMenu.AddItem(new MenuItem("autoYellowIntoQ", "Auto Q after Yellow Card").SetValue(false));
            MiscMenu.AddItem(new MenuItem("autoQonCC", "Auto Q on immobile targets").SetValue(false));
            TwistedFateMenu.AddSubMenu(MiscMenu);

            DrawMenu = new Menu("Drawings", "drawings");
            DrawMenu.AddItem(new MenuItem("drawQrange", "Draw Q Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("drawRrange", "Draw R Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("drawOnlyReady", "Draw only when Spells are Ready").SetValue(true));
            TwistedFateMenu.AddSubMenu(DrawMenu);

            TwistedFateMenu.AddToMainMenu();
        }

        internal static int GetSliderValue(string itemName)
        {
            return TwistedFateMenu.Item(itemName).GetValue<Slider>().Value;
        }

        internal static int GetStringListValue(string itemName)
        {
            return TwistedFateMenu.Item(itemName).GetValue<StringList>().SelectedIndex;
        }

        internal static bool IsChecked(string itemName)
        {
            return TwistedFateMenu.Item(itemName).GetValue<bool>();
        }

        internal static bool IsKeyPressed(string itemName)
        {
            return TwistedFateMenu.Item(itemName).GetValue<KeyBind>().Active;
        }

        private static void OnLowerChange(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<Slider>().Value >= GetSliderValue("humanizeUpper"))
            {
                e.Process = false;
            }
        }

        private static void OnUpperChange(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<Slider>().Value <= GetSliderValue("humanizeLower"))
            {
                e.Process = false;
            }
        }

        #endregion
    }
}