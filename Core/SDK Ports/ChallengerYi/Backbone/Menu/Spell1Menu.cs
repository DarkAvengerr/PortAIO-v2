using LeagueSharp.SDK.UI;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Backbone.Menu
{
    using Menu = LeagueSharp.SDK.UI.Menu;
    internal class Spell1Menu
    {
        internal static Menu Menu;
        internal Spell1Menu(string menuPreffix)
        {
            Menu = new Menu(menuPreffix+"spell1", "Q Settings");

            MakeConfig(Menu, menuPreffix);

            MainMenu.Menu.Add(Menu);
        }

        #region Assembly's Particular Spell Usage Config

        internal static MenuList<string> QFarm;
        internal static MenuList<string> QCombo;
        internal static MenuSlider QFarmMana;
        internal static MenuList<string> QHarass;
        internal static MenuSlider QHighDamageSpell;

        private void MakeConfig(Menu menu, string menuPreffix)
        {
            QCombo =
                menu.Add(new MenuList<string>(menuPreffix + "qcombo", "Combo Mode", new[] { "SMART (OUTPLAY)", "ALWAYS" }));
            QHighDamageSpell = menu.Add(new MenuSlider(menuPreffix + "qhighdmg", "Q if spell dmg > % CURRENT HP", 10, 0, 100));
            QFarm =
                menu.Add(new MenuList<string>(menuPreffix + "qfarm", "Farm Mode", new[] {"Only when alone", "ALWAYS"}));
            QFarmMana = menu.Add(new MenuSlider(menuPreffix + "qfarmana", "Min mana % for Q Farm", 75, 0, 100));
            QHarass =
                menu.Add(new MenuList<string>(menuPreffix + "qharass", "Use Q to Harass Enemy on",
                    new[] {"DONTHARASS", "CHAMPION", "NEARBYMINION"}));
        }

        #endregion
    }
}