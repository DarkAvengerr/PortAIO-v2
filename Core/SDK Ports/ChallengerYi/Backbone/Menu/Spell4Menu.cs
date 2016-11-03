using LeagueSharp.SDK.UI;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Backbone.Menu
{
    using Menu = LeagueSharp.SDK.UI.Menu;
    internal class Spell4Menu
    {
        internal Menu Menu;
        internal Spell4Menu(string menuPreffix)
        {
            Menu = new Menu(menuPreffix + "spell4", "R Settings");

            MakeConfig(Menu, menuPreffix);

            MainMenu.Menu.Add(Menu);
        }

        #region Assembly's Particular Spell Usage Config

        internal static MenuBool UseR;

        private void MakeConfig(Menu menu, string menuPreffix)
        {
            UseR =
                menu.Add(new MenuBool(menuPreffix + "autor", "Auto R + Youmuu's?", true));
        }

        #endregion
    }
}