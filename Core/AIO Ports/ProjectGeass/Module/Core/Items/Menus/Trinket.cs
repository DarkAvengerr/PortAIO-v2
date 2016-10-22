using LeagueSharp.Common;
using _Project_Geass.Functions;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Core.Items.Menus
{

    internal sealed class Trinket
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Trinket" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        /// <param name="enabled">
        ///     if set to <c> true </c> [enabled].
        /// </param>
        public Trinket(Menu menu, bool enabled)
        {
            if (!enabled)
                return;

            menu.AddSubMenu(Menu());
            // ReSharper disable once UnusedVariable
            var trinket=new Events.Trinket();
            StaticObjects.ProjectLogger.WriteLog("Trinket Menu and events loaded.");
        }

        #endregion Public Constructors

        #region Private Methods

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Menu Menu()
        {
            var menu=new Menu(Names.Menu.TrinketNameBase, "trinketOptions");
            menu.AddItem(new MenuItem(Names.Menu.TrinketItemBase+"Boolean.BuyOrb", "Auto Buy Orb At Level >= 9").SetValue(true));
            return menu;
        }

        #endregion Private Methods
    }

}