using LeagueSharp.SDK.UI;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Backbone.Menu
{
    using Menu = LeagueSharp.SDK.UI.Menu;
    internal class Spell2Menu
    {
        internal Menu Menu;
        internal Spell2Menu(string menuPreffix)
        {
            Menu = new Menu(menuPreffix + "spell2", "W Settings");

            MakeConfig(Menu, menuPreffix);

            MainMenu.Menu.Add(Menu);
        }

        #region Assembly's Particular Spell Usage Config

        internal static MenuSlider UseWOnDangerousSpell;
        internal static MenuBool UseWOnTowerShots;
        internal static MenuSlider DisableOrbwalkerDuringW;

        private void MakeConfig(Menu menu, string menuPreffix)
        {
            UseWOnDangerousSpell =
                menu.Add(new MenuSlider(menuPreffix + "wsave", "W if spell dmg > %CURRENT HP", 25, 10, 100));
            UseWOnTowerShots =
                menu.Add(new MenuBool(menuPreffix + "wtower", "Use W to block towershots", true));
            DisableOrbwalkerDuringW =
                menu.Add(new MenuSlider(menuPreffix + "wdisableorbwalker", "Force disable orbwalker after W for x seconds", 0, 0, 3));
        }

        #endregion
    }
}