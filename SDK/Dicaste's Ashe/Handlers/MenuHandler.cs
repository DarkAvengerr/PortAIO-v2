using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DicasteAshe.Handlers
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;

    internal static class MenuHandler
    {
        internal static Menu MainMenu { get; set; }

        private static string ChampionName { get; } = GameObjects.Player.ChampionName;

        internal static bool GetMenuBool(OrbwalkingMode mode, SpellSlot spellSlot)
            => MainMenu[mode.ToString()][string.Concat(mode, spellSlot)];

        internal static void Init()
        {
            MainMenu = new Menu($"{ChampionName}", $"{ChampionName}", true);

            var comboMenu = new Menu(nameof(OrbwalkingMode.Combo), nameof(OrbwalkingMode.Combo));
            AddMenuBool(comboMenu, OrbwalkingMode.Combo, SpellSlot.Q);
            AddMenuBool(comboMenu, OrbwalkingMode.Combo, SpellSlot.W);
            AddMenuBool(comboMenu, OrbwalkingMode.Combo, SpellSlot.R);

            MainMenu.Add(comboMenu);

            var hybridMenu = new Menu(nameof(OrbwalkingMode.Hybrid), nameof(OrbwalkingMode.Hybrid));
            AddMenuBool(hybridMenu, OrbwalkingMode.Hybrid, SpellSlot.Q);
            AddMenuBool(hybridMenu, OrbwalkingMode.Hybrid, SpellSlot.W);
            MainMenu.Add(hybridMenu);

            var drawingsMenu = new Menu("Drawings", "Drawings");
            AddDrawingsMenuBool(drawingsMenu, SpellSlot.W);
            MainMenu.Add(drawingsMenu);

            MainMenu.Attach();
        }

        private static void AddDrawingsMenuBool(Menu menu, SpellSlot spellSlot)
        {
            menu.Add(new MenuBool(string.Concat("Draw", spellSlot), $"Draw {spellSlot}", true));
        }

        private static void AddMenuBool(Menu menu, OrbwalkingMode orbwalkingMode, SpellSlot spellSlot)
        {
            menu.Add(new MenuBool(string.Concat(orbwalkingMode, spellSlot), $"Use {spellSlot}", true));
        }
    }
}