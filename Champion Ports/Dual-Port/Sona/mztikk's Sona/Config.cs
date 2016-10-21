using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona
{
    using System.Linq;

    using LeagueSharp.Common;

    internal static class Config
    {
        #region Properties

        internal static Menu AutoRMenu { get; private set; }
        internal static Menu Misc { get; private set; }

        internal static Menu AutoWMenu { get; private set; }
        internal static Menu AutoWMenuWhitelist { get; private set; }

        internal static Menu ComboMenu { get; private set; }

        internal static Menu DrawMenu { get; private set; }

        internal static Menu GapcloseMenu { get; private set; }

        internal static Menu HarassMenu { get; private set; }

        internal static Menu InterrupterMenu { get; private set; }

        internal static Menu LaneClearMenu { get; private set; }

        internal static Menu OrbwalkMenu { get; private set; }

        internal static Menu SonaMenu { get; private set; }

        internal static Menu TargetSelectorMenu { get; private set; }

        internal static Menu FleeMenu { get; private set; }
        #endregion

        #region Methods

        internal static void BuildMenu()
        {
            SonaMenu = new Menu("Sona", "sona", true);

            TargetSelectorMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            SonaMenu.AddSubMenu(TargetSelectorMenu);

            OrbwalkMenu = new Menu("Orbwalker", "orbwalker");
            Mainframe.Orbwalker = new Orbwalking.Orbwalker(OrbwalkMenu);
            SonaMenu.AddSubMenu(OrbwalkMenu);

            ComboMenu = new Menu("Combo", "combo");
            ComboMenu.AddItem(new MenuItem("Combo.bQ", "Use Q").SetValue(true));
            ComboMenu.AddItem(new MenuItem("Combo.bE", "Use E").SetValue(true));
            ComboMenu.AddItem(new MenuItem("Combo.bR", "Use R").SetValue(true));
            ComboMenu.AddItem(new MenuItem("Combo.minR", "Min Targets to hit for R").SetValue(new Slider(3, 1, 5)));
            ComboMenu.AddItem(new MenuItem("Combo.bFlashR", "Use Flash R").SetValue(true));
            ComboMenu.AddItem(
                new MenuItem("Combo.minFlashR", "Min Targets to hit for Flash R").SetValue(new Slider(5, 1, 5)));
            ComboMenu.AddItem(new MenuItem("Combo.bSmartAA", "AA only after Q or on 3rd passive").SetValue(false));
            SonaMenu.AddSubMenu(ComboMenu);

            HarassMenu = new Menu("Harass", "harass");
            HarassMenu.AddItem(new MenuItem("Harass.bQ", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("Harass.minMana", "Min Mana % to Harass").SetValue(new Slider(50)));
            HarassMenu.AddItem(new MenuItem("Harass.aaMins", "Disable AA on minions when allies nearby").SetValue(true));
            SonaMenu.AddSubMenu(HarassMenu);

            AutoWMenu = new Menu("Auto W", "autow");
            AutoWMenu.AddItem(new MenuItem("bW", "Use W").SetValue(true));
            AutoWMenu.AddItem(new MenuItem("autow.incoming", "Use W on incoming damage").SetValue(false));
            AutoWMenu.AddItem(new MenuItem("autow.cancelbase", "Cancel Recall to Auto W").SetValue(false));
            AutoWMenu.AddItem(new MenuItem("allyWhp", "Ally HP % to W").SetValue(new Slider(50, 1)));
            AutoWMenu.AddItem(new MenuItem("playerWhp", "Player HP % to W").SetValue(new Slider(50, 1)));
            AutoWMenu.AddItem(new MenuItem("AutoW.minMana", "Min Mana % to W").SetValue(new Slider(20)));
            var allAllies = HeroManager.Allies.ToList();
            AutoWMenuWhitelist = new Menu("Whitelist", "whitelist");
            foreach (var ally in allAllies)
            {
                AutoWMenuWhitelist.AddItem(
                    new MenuItem("autoW_" + ally.ChampionName, "Auto Heal " + ally.ChampionName + " with W").SetValue(
                        true));
            }
            AutoWMenu.AddSubMenu(AutoWMenuWhitelist);
            SonaMenu.AddSubMenu(AutoWMenu);

            FleeMenu = new Menu("Flee", "flee");
            FleeMenu.AddItem(
                new MenuItem("fleeBind", "Flee Key").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press)));
            FleeMenu.AddItem(new MenuItem("fleeE", "Use E").SetValue(true));
            FleeMenu.AddItem(new MenuItem("slowAttack", "Use AA with slow passive").SetValue(true));
            SonaMenu.AddSubMenu(FleeMenu);

            Misc = new Menu("Misc", "misc");
            Misc.AddItem(
                new MenuItem("assistedR", "Assisted R").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Misc.AddItem(
                new MenuItem("assistedRTargetting", "Targetting Mode").SetValue(
                    new StringList(new[] { "Nearest to Mouse", "Target Selector" })));
            //Misc.AddItem(new MenuItem("assRminTargets", "Min Targets").SetValue(new Slider(1, 1, 5)));
            SonaMenu.AddSubMenu(Misc);

            GapcloseMenu = new Menu("Anti Gapclose", "antigapclose");
            GapcloseMenu.AddItem(new MenuItem("gapclose.bE", "Use E").SetValue(false));
            SonaMenu.AddSubMenu(GapcloseMenu);

            InterrupterMenu = new Menu("Interrupter", "interrupter");
            InterrupterMenu.AddItem(new MenuItem("intr.bR", "Use R").SetValue(false));
            InterrupterMenu.AddItem(
                new MenuItem("dangerL", "Min DangerLevel to interrupt").SetValue(
                    new StringList(new[] { "Low", "Medium", "High" }, 2)));
            SonaMenu.AddSubMenu(InterrupterMenu);

            DrawMenu = new Menu("Drawings", "drawings");
            DrawMenu.AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("drawW", "Draw W Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("drawE", "Draw E Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("drawR", "Draw R Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("onlyRdy", "Draw only when spells can be cast").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawFR", "Draw possible FlashUlt Targets").SetValue(false));
            SonaMenu.AddSubMenu(DrawMenu);

            SonaMenu.AddToMainMenu();
        }

        internal static int GetSliderValue(string itemName)
        {
            return SonaMenu.Item(itemName).GetValue<Slider>().Value;
        }

        internal static int GetStringListValue(string itemName)
        {
            return SonaMenu.Item(itemName).GetValue<StringList>().SelectedIndex;
        }

        internal static bool IsChecked(string itemName)
        {
            return SonaMenu.Item(itemName).GetValue<bool>();
        }

        internal static bool IsKeyPressed(string itemName)
        {
            return SonaMenu.Item(itemName).GetValue<KeyBind>().Active;
        }

        #endregion
    }
}