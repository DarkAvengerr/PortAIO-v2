using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia
{
    using LeagueSharp.Common;

    internal static class Config
    {
        #region Properties

        internal static Menu AutoHarass { get; private set; }

        internal static Menu Cassop { get; private set; }

        internal static Menu Combo { get; private set; }

        internal static Menu Gapclose { get; private set; }

        internal static Menu Harass { get; private set; }

        internal static Menu Interrupter { get; private set; }

        internal static Menu JungleClear { get; private set; }

        internal static Menu LaneClear { get; private set; }

        internal static Menu LastHit { get; private set; }

        internal static Menu Misc { get; private set; }

        internal static Menu OrbwalkMenu { get; private set; }

        internal static Menu TargetSelectorMenu { get; private set; }

        #endregion

        #region Methods

        internal static void BuildMenu()
        {
            Cassop = new Menu("CassOp", "cassop", true);

            TargetSelectorMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Cassop.AddSubMenu(TargetSelectorMenu);

            OrbwalkMenu = new Menu("Orbwalker", "orbwalker");
            Mainframe.Orbwalker = new Orbwalking.Orbwalker(OrbwalkMenu);
            Cassop.AddSubMenu(OrbwalkMenu);

            Combo = new Menu("Combo", "combo");
            Combo.AddItem(new MenuItem("useQInCombo", "Use Q").SetValue(true));
            Combo.AddItem(new MenuItem("useWInCombo", "Use W").SetValue(true));
            Combo.AddItem(new MenuItem("useEInCombo", "Use E").SetValue(true));
            Combo.AddItem(new MenuItem("useRInCombo", "Use R").SetValue(true));
            Combo.AddItem(new MenuItem("comboEonP", "E only on poisoned").SetValue(true));
            Combo.AddItem(new MenuItem("humanEInCombo", "Humanize E casts").SetValue(true));
            Combo.AddItem(new MenuItem("comboWonlyCD", "W only on Q CD and no Poison").SetValue(true));
            Combo.AddItem(new MenuItem("comboMinR", "Min Targets to hit for R").SetValue(new Slider(3, 1, 5)));
            Combo.AddItem(new MenuItem("comboNoAA", "Disable AA on Heroes in Combo").SetValue(false));
            Combo.AddItem(new MenuItem("comboFlashR", "Flash R Combo on killable").SetValue(false));
            Combo.AddItem(
                new MenuItem("maxEnFlash", "Max Enemies around Target to Flash R").SetValue(new Slider(2, 0, 4)));
            Cassop.AddSubMenu(Combo);

            Harass = new Menu("Harass", "harass");
            Harass.AddItem(new MenuItem("useQInHarass", "Use Q").SetValue(true));
            Harass.AddItem(new MenuItem("useWInHarass", "Use W").SetValue(false));
            Harass.AddItem(new MenuItem("useEInHarass", "Use E").SetValue(true));
            Harass.AddItem(new MenuItem("harassEOnP", "E only on poisoned").SetValue(true));
            Harass.AddItem(new MenuItem("humanEInHarass", "Humanize E casts").SetValue(true));
            Harass.AddItem(new MenuItem("harassWonlyCD", "W only on Q CD and no Poison").SetValue(true));
            Harass.AddItem(new MenuItem("manaToHarass", "Min Mana % to Harass").SetValue(new Slider(40)));
            Cassop.AddSubMenu(Harass);

            AutoHarass = new Menu("Auto Harass", "autoharass");
            AutoHarass.AddItem(new MenuItem("autoQHarass", "Auto Q").SetValue(false));
            AutoHarass.AddItem(new MenuItem("autoWHarass", "Auto W").SetValue(false));
            AutoHarass.AddItem(new MenuItem("autoEHarass", "Auto E").SetValue(false));
            AutoHarass.AddItem(new MenuItem("autoHarassEonP", "E only on poisoned").SetValue(true));
            AutoHarass.AddItem(new MenuItem("humanEInAutoHarass", "Humanize E casts").SetValue(true));
            AutoHarass.AddItem(new MenuItem("dontAutoHarassInBush", "Don't Auto Harass in Bush").SetValue(true));
            AutoHarass.AddItem(new MenuItem("dontAutoHarassTower", "Don't Auto Harass under Tower").SetValue(true));
            AutoHarass.AddItem(new MenuItem("manaToAutoHarass", "Min Mana % to Auto Harass").SetValue(new Slider(60)));
            Cassop.AddSubMenu(AutoHarass);

            LaneClear = new Menu("LaneClear", "laneclera");
            LaneClear.AddItem(new MenuItem("useQInLC", "Use Q").SetValue(true));
            LaneClear.AddItem(new MenuItem("useWInLC", "Use W").SetValue(true));
            LaneClear.AddItem(new MenuItem("useEInLC", "Use E").SetValue(true));
            LaneClear.AddItem(new MenuItem("laneEonP", "E only on poisoned").SetValue(false));
            LaneClear.AddItem(new MenuItem("minQInLC", "Min Targets to hit for Q").SetValue(new Slider(3, 1, 9)));
            LaneClear.AddItem(new MenuItem("minWInLC", "Min Targets to hit for W").SetValue(new Slider(3, 1, 9)));
            LaneClear.AddItem(new MenuItem("useManaEInLC", "Use Mana Threshhold").SetValue(false));
            LaneClear.AddItem(
                new MenuItem("manaEInLC", "If Mana below this ignore posion for E LastHit").SetValue(new Slider(30, 1)));
            LaneClear.AddItem(new MenuItem("manaToLC", "Min Mana % to LaneClear").SetValue(new Slider(20)));
            Cassop.AddSubMenu(LaneClear);

            JungleClear = new Menu("JungleClear", "jungleclear");
            JungleClear.AddItem(new MenuItem("useQInJC", "Use Q").SetValue(true));
            JungleClear.AddItem(new MenuItem("useWInJC", "Use W").SetValue(true));
            JungleClear.AddItem(new MenuItem("useEInJC", "Use E").SetValue(true));
            JungleClear.AddItem(new MenuItem("jungEonP", "E only on poisoned").SetValue(true));
            JungleClear.AddItem(new MenuItem("manaToJC", "Min Mana % to JungleClear").SetValue(new Slider(10)));
            Cassop.AddSubMenu(JungleClear);

            LastHit = new Menu("Last Hit", "lasthit");
            LastHit.AddItem(new MenuItem("useEInLH", "Use E").SetValue(true));
            LastHit.AddItem(new MenuItem("lastEonP", "E only on poisoned").SetValue(false));
            Cassop.AddSubMenu(LastHit);

            Interrupter = new Menu("Interrupter", "interrupter");
            Interrupter.AddItem(new MenuItem("bInterruopt", "Interrupt Spells with R").SetValue(false));
            Interrupter.AddItem(
                new MenuItem("dangerL", "Min DangerLevel to Interrupt").SetValue(
                    new StringList(new[] { "Low", "Medium", "High" }, 2)));
            Cassop.AddSubMenu(Interrupter);

            Gapclose = new Menu("Anti Gapclose", "antigapclose");
            Gapclose.AddItem(new MenuItem("qGapclose", "Anti Gapclose with Q").SetValue(false));
            Cassop.AddSubMenu(Gapclose);

            Misc = new Menu("Misc", "misc");
            Misc.AddItem(new MenuItem("antiMissR", "Block R Casts if they miss/don't face").SetValue(true));
            Misc.AddItem(
                new MenuItem("assistedR", "Assisted R").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
            Misc.AddItem(new MenuItem("eLastHit", "Use E on unkillable Minion").SetValue(true));
            Misc.AddItem(new MenuItem("eKillSteal", "Use E to Killsteal").SetValue(true));
            Misc.AddItem(new MenuItem("clearE", "Automatically kill poisoned minions with E").SetValue(true));
            Misc.AddItem(new MenuItem("manaClearE", "Min Mana % to Auto E").SetValue(new Slider(10)));
            Misc.AddItem(new MenuItem("tearStackQ", "Use Q to stack Tear passively").SetValue(false));
            Misc.AddItem(new MenuItem("manaTearStack", "Min Mana % to stack Tear").SetValue(new Slider(50)));
            Misc.AddItem(new MenuItem("humanDelay", "Humanize").SetValue(new Slider(30, 1, 500)));
            Cassop.AddSubMenu(Misc);

            Cassop.AddToMainMenu();
        }

        internal static int GetSliderValue(string itemName)
        {
            return Cassop.Item(itemName).GetValue<Slider>().Value;
        }

        internal static int GetStringListValue(string itemName)
        {
            return Cassop.Item(itemName).GetValue<StringList>().SelectedIndex;
        }

        internal static bool IsChecked(string itemName)
        {
            return Cassop.Item(itemName).GetValue<bool>();
        }

        internal static bool IsKeyPressed(string itemName)
        {
            return Cassop.Item(itemName).GetValue<KeyBind>().Active;
        }

        #endregion
    }
}