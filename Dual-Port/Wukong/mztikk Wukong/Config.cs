using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong
{
    using LeagueSharp.Common;

    internal static class Config
    {
        #region Properties

        internal static Menu Combo { get; private set; }

        internal static Menu DrawMenu { get; private set; }

        internal static Menu ForceUltCombo { get; private set; }

        internal static Menu Harass { get; private set; }

        internal static Menu InterrupterMenu { get; private set; }

        internal static Menu JungleClear { get; private set; }

        internal static Menu Kong { get; private set; }

        internal static Menu LaneClear { get; private set; }

        internal static Menu OrbwalkMenu { get; private set; }

        internal static Menu TargetSelectorMenu { get; private set; }

        #endregion

        #region Methods

        internal static void BuildMenu()
        {
            Kong = new Menu("Wukong", "wukong", true);

            TargetSelectorMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Kong.AddSubMenu(TargetSelectorMenu);

            OrbwalkMenu = new Menu("Orbwalker", "orbwalker");
            Mainframe.Orbwalker = new Orbwalking.Orbwalker(OrbwalkMenu);
            Kong.AddSubMenu(OrbwalkMenu);

            Combo = new Menu("Combo", "combo");
            Combo.AddItem(new MenuItem("combo.q", "Use Q").SetValue(true));
            Combo.AddItem(new MenuItem("combo.e", "Use E").SetValue(true));
            Combo.AddItem(new MenuItem("combo.e.tower", "Don't E under Tower").SetValue(false));
            Combo.AddItem(
                new MenuItem("combo.e.mode", "E Mode").SetValue(
                    new StringList(new[] { "Smart Mode", "Out of melee range" })));
            Combo.AddItem(new MenuItem("combo.r", "Use R").SetValue(true));
            Combo.AddItem(new MenuItem("combo.r.targets", "Min Targets to hit for R").SetValue(new Slider(5, 1, 5)));
            ForceUltCombo = new Menu("Force Ult Whitelist", "forceult");
            foreach (var enemy in HeroManager.Enemies)
            {
                ForceUltCombo.AddItem(
                    new MenuItem("combo.r.force." + enemy.ChampionName, "Force Ult on: " + enemy.ChampionName).SetValue(
                        false));
            }

            Combo.AddSubMenu(ForceUltCombo);
            Combo.AddItem(new MenuItem("combo.r.force.waitq", "Wait for Q Debuff on Force Ult").SetValue(true));
            Kong.AddSubMenu(Combo);

            Harass = new Menu("Harass", "harass");
            Harass.AddItem(new MenuItem("harass.q", "Use Q").SetValue(true));
            Harass.AddItem(new MenuItem("harass.e", "Use E").SetValue(true));
            Harass.AddItem(new MenuItem("harass.e.tower", "Don't E under Tower").SetValue(true));
            Harass.AddItem(
                new MenuItem("harass.e.mode", "E Mode").SetValue(
                    new StringList(new[] { "Smart Mode", "Out of melee range" })));
            Harass.AddItem(new MenuItem("harass.q.auto", "Auto Q Harass").SetValue(false));
            Harass.AddItem(new MenuItem("harass.mana", "Min Mana % to Harass").SetValue(new Slider(50)));
            Kong.AddSubMenu(Harass);

            LaneClear = new Menu("LaneClear", "laneclear");
            LaneClear.AddItem(new MenuItem("laneclear.q", "Use Q").SetValue(true));
            LaneClear.AddItem(new MenuItem("laneclear.e", "Use E").SetValue(true));
            LaneClear.AddItem(new MenuItem("laneclear.mana", "Min Mana % to LaneClear").SetValue(new Slider(50)));
            Kong.AddSubMenu(LaneClear);

            JungleClear = new Menu("JungleClear", "jungleclear");
            JungleClear.AddItem(new MenuItem("jungleclear.q", "Use Q").SetValue(true));
            JungleClear.AddItem(new MenuItem("jungleclear.e", "Use E").SetValue(true));
            JungleClear.AddItem(new MenuItem("jungleclear.mana", "Min Mana % to JungleClear").SetValue(new Slider(50)));
            Kong.AddSubMenu(JungleClear);

            InterrupterMenu = new Menu("Interrupt", "interrupt");
            InterrupterMenu.AddItem(new MenuItem("interrupt.r", "Use R").SetValue(false));
            InterrupterMenu.AddItem(
                new MenuItem("interrupt.danger", "Min DangerLevel to interrupt").SetValue(
                    new StringList(new[] { "Low", "Medium", "High" }, 2)));
            Kong.AddSubMenu(InterrupterMenu);

            DrawMenu = new Menu("Drawings", "drawings");
            DrawMenu.AddItem(new MenuItem("draw.q", "Draw Q Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("draw.e", "Draw E Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("draw.r", "Draw R Range").SetValue(false));
            DrawMenu.AddItem(new MenuItem("draw.onlyrdy", "Draw only when ready").SetValue(true));
            Kong.AddSubMenu(DrawMenu);

            Kong.AddToMainMenu();
        }

        internal static int GetSliderValue(string itemName)
        {
            return Kong.Item(itemName).GetValue<Slider>().Value;
        }

        internal static int GetStringListValue(string itemName)
        {
            return Kong.Item(itemName).GetValue<StringList>().SelectedIndex;
        }

        internal static bool IsChecked(string itemName)
        {
            return Kong.Item(itemName).GetValue<bool>();
        }

        internal static bool IsKeyPressed(string itemName)
        {
            return Kong.Item(itemName).GetValue<KeyBind>().Active;
        }

        #endregion
    }
}