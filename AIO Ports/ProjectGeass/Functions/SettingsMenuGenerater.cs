using System.Collections.Generic;
using System.Linq;
using LeagueSharp.Common;
using _Project_Geass.Module.Champions.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Functions
{

    internal sealed class SettingsMenuGenerater
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsMenuGenerater" /> class.
        /// </summary>
        public SettingsMenuGenerater()
        {
            StaticObjects.SettingsMenu.AddItem(new MenuItem($"{Names.Menu.BaseItem}.PredictionMethod", "PredictionMethod")).SetValue(new StringList(Names.PredictionMethods.ToArray(), 2));

            foreach (var champ in Bootloader.ChampionBundled.Where(x => x.Value))
            {
                var temp=new Menu(champ.Key, Names.Menu.BaseItem+champ);

                foreach (var element in GenerateSettingsList(Names.Menu.BaseItem+champ.Key))
                    temp.AddItem(element);

                StaticObjects.SettingsMenu.AddSubMenu(temp);
            }
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        ///     Generates the settings list.
        /// </summary>
        /// <param name="basename">
        ///     The basename.
        /// </param>
        /// <returns>
        /// </returns>
        private IEnumerable<MenuItem> GenerateSettingsList(string basename)
        {
            var items=new List<MenuItem>
            {
                new MenuItem($"{basename}.Enable", "Enable Champion").SetValue(true), new MenuItem($"{basename}.ManaMenu", "Mana Menu").SetValue(true), new MenuItem($"{basename}.ItemMenu", "Item Menu").SetValue(true),
                new MenuItem($"{basename}.OnLevelMenu", "OnLevel Menu").SetValue(true), new MenuItem($"{basename}.TrinketMenu", "Trinket Menu").SetValue(true), new MenuItem($"{basename}.DrawingMenu", "Drawing Menu").SetValue(true)
            };

            return items;
        }

        #endregion Private Methods
    }

}