using LeagueSharp.SDK.UI;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Backbone.Menu
{
    using Menu = LeagueSharp.SDK.UI.Menu;
    internal class Spell3Menu
    {
        internal Menu Menu;
        internal Spell3Menu(string menuPreffix)
        {
            Menu = new Menu(menuPreffix + "spell3", "E Settings");

            MakeConfig(Menu, menuPreffix);

            MainMenu.Menu.Add(Menu);
        }

        #region Assembly's Particular Spell Usage Config

        internal static MenuBool UseEFarm;
        internal static MenuBool UseECombo;

        private void MakeConfig(Menu menu, string menuPreffix)
        {
            UseECombo =
                menu.Add(new MenuBool(menuPreffix + "ecombo", "Use E in Combo", true));
            UseEFarm =
                menu.Add(new MenuBool(menuPreffix + "ejg", "Use E in Jungle", true));
        }

        #endregion
    }
}