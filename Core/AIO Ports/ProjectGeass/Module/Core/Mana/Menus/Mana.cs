using LeagueSharp.Common;
using _Project_Geass.Data.Champions;
using _Project_Geass.Functions;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Core.Mana.Menus
{

    internal sealed class ManaMenu
    {
        #region Public Fields

        public bool Enabled;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManaMenu" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        /// <param name="options">
        ///     The options.
        /// </param>
        /// <param name="enabled">
        ///     if set to <c> true </c> [enabled].
        /// </param>
        public ManaMenu(Menu menu, int[,] options, bool enabled)
        {
            Enabled=enabled;
            if (!enabled)
                return;

            menu.AddSubMenu(Menu(options));
            // ReSharper disable once UnusedVariable
        }

        #endregion Public Constructors

        #region Private Methods

        private Menu Menu(int[,] options)
        {
            var menu=new Menu(Names.Menu.ManaNameBase, "ManaManager");
            menu.AddItem(new MenuItem(Names.Menu.ManaItemBase+"Use.ManaManager", "Use ManaManager").SetValue(true));

            for (var index=0;index<SettingsBase.ManaModes.Length;index++)
            {
                var subMenu=new Menu($"{SettingsBase.ManaModes[index]}", $"{SettingsBase.ManaModes[index]}ManaMenu");
                for (var i=0;i<SettingsBase.ManaAbilities.Length;i++)
                    if (options[index, i]!=-1)
                        subMenu.AddItem(new MenuItem($"{Names.Menu.ManaItemBase}{SettingsBase.ManaModes[index]}.Slider.MinMana.{SettingsBase.ManaAbilities[i]}", $"Min Mana% {SettingsBase.ManaAbilities[i]}").SetValue(new Slider(options[index, i])));

                menu.AddSubMenu(subMenu);
            }

            return menu;
        }

        #endregion Private Methods
    }

}