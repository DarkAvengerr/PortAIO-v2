using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao
{
    using LeagueSharp.Common;

    internal static class Config
    {
        #region Properties

        internal static Menu Combo { get; private set; }

        internal static Menu Draw { get; private set; }

        internal static Menu Harass { get; private set; }

        internal static Menu JungleClear { get; private set; }

        internal static Menu LaneClear { get; private set; }

        internal static Menu Misc { get; private set; }

        internal static Menu OrbwalkMenu { get; private set; }

        internal static Menu TargetSelectorMenu { get; private set; }

        internal static Menu Zhao { get; private set; }

        #endregion

        #region Methods

        internal static void BuildMenu()
        {
            Zhao = new Menu("Xin Zhao", "xinzhao", true);

            TargetSelectorMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Zhao.AddSubMenu(TargetSelectorMenu);

            OrbwalkMenu = new Menu("Orbwalker", "orbwalker");
            Mainframe.Orbwalker = new Orbwalking.Orbwalker(OrbwalkMenu);
            Zhao.AddSubMenu(OrbwalkMenu);

            Combo = new Menu("Combo", "combo");
            Combo.AddItem(new MenuItem("useQcombo", "Use Q").SetValue(true));
            Combo.AddItem(new MenuItem("useWcombo", "Use W").SetValue(true));
            Combo.AddItem(new MenuItem("useEcombo", "Use E").SetValue(true));
            Combo.AddItem(new MenuItem("combo.smite", "Use Smite").SetValue(true));
            Combo.AddItem(new MenuItem("comboETower", "Don't E under tower").SetValue(false));
            Combo.AddItem(
                new MenuItem("comboEmode", "E Usage Mode").SetValue(
                    new StringList(new[] { "Smart", "Use when out of melee range" })));
            Combo.AddItem(new MenuItem("useRcombo", "Use R").SetValue(true));
            Combo.AddItem(new MenuItem("comboMinR", "Min Targets to hit for R").SetValue(new Slider(5, 1, 5)));
            Zhao.AddSubMenu(Combo);

            Harass = new Menu("Harass", "harass");
            Harass.AddItem(new MenuItem("useQharass", "Use Q").SetValue(true));
            Harass.AddItem(new MenuItem("useWharass", "Use W").SetValue(true));
            Harass.AddItem(new MenuItem("useEharass", "Use E").SetValue(true));
            Harass.AddItem(new MenuItem("harassETower", "Don't E under tower").SetValue(true));
            Harass.AddItem(new MenuItem("harassMana", "Min Mana % to Harass").SetValue(new Slider(80)));
            Zhao.AddSubMenu(Harass);

            LaneClear = new Menu("LaneClear", "laneclear");
            LaneClear.AddItem(new MenuItem("useQLC", "Use Q").SetValue(false));
            LaneClear.AddItem(new MenuItem("useWLC", "Use W").SetValue(false));
            LaneClear.AddItem(new MenuItem("useELC", "Use E").SetValue(true));
            LaneClear.AddItem(new MenuItem("lcEtargets", "Min Targets to hit for E").SetValue(new Slider(3, 0, 10)));
            LaneClear.AddItem(new MenuItem("lcMana", "Min Mana % to LaneClear").SetValue(new Slider(80)));
            Zhao.AddSubMenu(LaneClear);

            JungleClear = new Menu("JungleClear", "jungleclear");
            JungleClear.AddItem(new MenuItem("useQJC", "Use Q").SetValue(true));
            JungleClear.AddItem(new MenuItem("useWJC", "Use W").SetValue(true));
            JungleClear.AddItem(new MenuItem("useEJC", "Use E").SetValue(false));
            JungleClear.AddItem(
                new MenuItem(
                    "jungleclear.smallmonsters", 
                    "Small Monsters first (Set this to the same as in your orbwalker)").SetValue(false));
            JungleClear.AddItem(new MenuItem("jcMana", "Min Mana % to JungleClear").SetValue(new Slider(50)));
            Zhao.AddSubMenu(JungleClear);

            Draw = new Menu("Draw", "draw");
            Draw.AddItem(new MenuItem("drawXinsec", "Draw Xinsec Target").SetValue(true));
            Draw.AddItem(new MenuItem("drawXinsecpred", "Draw Xinsec move pos").SetValue(true));
            Draw.AddItem(new MenuItem("draw.e", "Draw E Range").SetValue(false));
            Zhao.AddSubMenu(Draw);

            Misc = new Menu("Misc", "misc");
            Misc.AddItem(
                new MenuItem("xinsecKey", "Xinsec").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Misc.AddItem(new MenuItem("xinsecFlash", "Use Flash with Xinsec").SetValue(true));
            Misc.AddItem(
                new MenuItem("xinsecTargetting", "Xinsec Targetting").SetValue(
                    new StringList(new[] { "Selected Target", "Target Selector", "Lowest MaxHealth" }, 1)));
            Misc.AddItem(new MenuItem("useInterrupt", "Interrupt Spells with R").SetValue(false));
            Misc.AddItem(
                new MenuItem("dangerL", "Min DangerLevel to interrupt").SetValue(
                    new StringList(new[] { "Low", "Medium", "High" }, 2)));
            Zhao.AddSubMenu(Misc);

            Zhao.AddToMainMenu();
        }

        internal static int GetSliderValue(string itemName)
        {
            return Zhao.Item(itemName).GetValue<Slider>().Value;
        }

        internal static int GetStringListValue(string itemName)
        {
            return Zhao.Item(itemName).GetValue<StringList>().SelectedIndex;
        }

        internal static bool IsChecked(string itemName)
        {
            return Zhao.Item(itemName).GetValue<bool>();
        }

        internal static bool IsKeyPressed(string itemName)
        {
            return Zhao.Item(itemName).GetValue<KeyBind>().Active;
        }

        #endregion
    }
}