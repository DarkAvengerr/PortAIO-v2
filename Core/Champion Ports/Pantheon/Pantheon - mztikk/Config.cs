using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon
{
    using LeagueSharp.Common;

    internal static class Config
    {
        #region Properties

        internal static Menu Combo { get; private set; }

        internal static Menu Drawing { get; private set; }

        internal static Menu Harass { get; private set; }

        internal static Menu Jungleclear { get; private set; }

        internal static Menu Laneclear { get; private set; }

        internal static Menu LastHit { get; private set; }

        internal static Menu Misc { get; private set; }

        internal static Menu OrbwalkMenu { get; private set; }

        internal static Menu Pantheon { get; private set; }

        internal static Menu TargetSelectorMenu { get; private set; }

        #endregion

        #region Methods

        internal static void BuildMenu()
        {
            Pantheon = new Menu("Pantheon", "pantheon", true);

            TargetSelectorMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Pantheon.AddSubMenu(TargetSelectorMenu);

            OrbwalkMenu = new Menu("Orbwalker", "orbwalker");
            Mainframe.Orbwalker = new Orbwalking.Orbwalker(OrbwalkMenu);
            Pantheon.AddSubMenu(OrbwalkMenu);

            Combo = new Menu("Combo", "combo");
            Combo.AddItem(new MenuItem("combo.q", "Use Q").SetValue(true));
            Combo.AddItem(new MenuItem("combo.w", "Use W").SetValue(true));
            Combo.AddItem(new MenuItem("combo.e", "Use E").SetValue(true));
            Combo.AddItem(new MenuItem("combo.tiamat", "Use Tiamat").SetValue(true));
            Pantheon.AddSubMenu(Combo);

            Harass = new Menu("Harass", "harass");
            Harass.AddItem(new MenuItem("harass.q", "Use Q").SetValue(true));
            Harass.AddItem(new MenuItem("harass.w", "Use W").SetValue(false));
            Harass.AddItem(new MenuItem("harass.e", "Use E").SetValue(false));
            Harass.AddItem(new MenuItem("harass.mana", "Min Mana % to Harass").SetValue(new Slider(50)));
            Harass.AddItem(new MenuItem("harass.auto.q", "Auto Harass Q").SetValue(false));
            Harass.AddItem(new MenuItem("harass.auto.mana", "Min Mana % to Auto Harass").SetValue(new Slider(70)));
            Pantheon.AddSubMenu(Harass);

            Jungleclear = new Menu("JungleClear", "jungleclear");
            Jungleclear.AddItem(new MenuItem("jungleclear.q", "Use Q").SetValue(true));
            Jungleclear.AddItem(new MenuItem("jungleclear.w", "Use W").SetValue(false));
            Jungleclear.AddItem(new MenuItem("jungleclear.e", "Use E").SetValue(true));
            Jungleclear.AddItem(new MenuItem("jungleclear.mana", "Min Mana % to JungleClear").SetValue(new Slider(50)));
            Pantheon.AddSubMenu(Jungleclear);

            Laneclear = new Menu("LaneClear", "laneclear");
            Laneclear.AddItem(new MenuItem("laneclear.q", "Use Q").SetValue(true));
            Laneclear.AddItem(new MenuItem("laneclear.w", "Use W").SetValue(false));
            Laneclear.AddItem(new MenuItem("laneclear.e", "Use E").SetValue(true));
            Laneclear.AddItem(new MenuItem("laneclear.mana", "Min Mana % to LaneClear").SetValue(new Slider(50)));
            Pantheon.AddSubMenu(Laneclear);

            LastHit = new Menu("LastHit", "lasthit");
            LastHit.AddItem(new MenuItem("lasthit.q", "Use Q").SetValue(false));
            LastHit.AddItem(new MenuItem("lasthit.q.oor", "Only on out of aa range minions").SetValue(true));
            LastHit.AddItem(new MenuItem("lasthit.mana", "Min Mana % to LastHit").SetValue(new Slider(50)));
            Pantheon.AddSubMenu(LastHit);

            Misc = new Menu("Misc", "misc");
            Misc.AddItem(new MenuItem("misc.killsteal.q", "Killsteal with Q").SetValue(true));
            Misc.AddItem(
                new MenuItem("misc.stackpassive.q", "Try to get passive on enemy aa with q (experimental)").SetValue(
                    false));
            Misc.AddItem(new MenuItem("misc.interrupt.w", "Interrupt spells with W").SetValue(false));
            Pantheon.AddSubMenu(Misc);

            Drawing = new Menu("Drawing", "drawing");
            Drawing.AddItem(new MenuItem("draw.q", "Draw Q Range").SetValue(false));
            Drawing.AddItem(new MenuItem("draw.w", "Draw W Range").SetValue(false));
            Drawing.AddItem(new MenuItem("draw.e", "Draw E Range").SetValue(false));
            Drawing.AddItem(new MenuItem("draw.r", "Draw R Range").SetValue(false));
            Drawing.AddItem(new MenuItem("draw.r2", "Draw R Range on minimap").SetValue(false));
            Drawing.AddItem(new MenuItem("draw.onlyrdy", "Draw only when ready").SetValue(true));
            Drawing.AddItem(new MenuItem("draw.dmg", "Draw damage on HP bar").SetValue(false)).ValueChanged +=
                OnValueChange;
            Pantheon.AddSubMenu(Drawing);

            Pantheon.AddToMainMenu();
        }

        internal static int GetSliderValue(string itemName)
        {
            return Pantheon.Item(itemName).GetValue<Slider>().Value;
        }

        internal static int GetStringListValue(string itemName)
        {
            return Pantheon.Item(itemName).GetValue<StringList>().SelectedIndex;
        }

        internal static bool IsChecked(string itemName)
        {
            return Pantheon.Item(itemName).GetValue<bool>();
        }

        internal static bool IsKeyPressed(string itemName)
        {
            return Pantheon.Item(itemName).GetValue<KeyBind>().Active;
        }

        private static void OnValueChange(object sender, OnValueChangeEventArgs e)
        {
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = e.GetNewValue<bool>();
        }

        #endregion
    }
}