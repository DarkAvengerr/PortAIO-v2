using System.Collections.Generic;
using System.Linq;
using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Utility;
using SoloVayne.Utility.Entities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SOLOVayne.Utility.General
{
    static class SOLOMenuExtensions
    {
        /// <summary>
        /// Adds the mode menu.
        /// </summary>
        /// <param name="mainMenu">The main menu.</param>
        /// <param name="Mode">The mode.</param>
        /// <returns></returns>
        public static Menu AddModeMenu(this Menu mainMenu, Orbwalking.OrbwalkingMode Mode)
        {
            var modeMenu = new Menu($"[SOLO] {Mode}", $"solo.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}");
            {
                mainMenu.AddSubMenu(modeMenu);
            }
            return modeMenu;
        }

        /// <summary>
        /// Adds a skill.
        /// </summary>
        /// <param name="mainMenu">The main menu.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="Mode">The mode.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <param name="defMana">The definition mana.</param>
        /// <param name="addMM">if set to <c>true</c> [add mm].</param>
        /// <returns></returns>
        public static MenuItem AddSkill(this Menu mainMenu, SpellSlot slot, Orbwalking.OrbwalkingMode Mode, bool defaultValue = true, int defMana = 20, bool addMM = true)
        {
            if (addMM)
            {
                mainMenu.AddManaManager(slot, Mode, defMana);
            }

            return mainMenu.AddBool($"solo.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.{slot.ToString().ToLower()}", $"Use {slot}", defaultValue);
        }

        /// <summary>
        /// Adds the mana manager.
        /// </summary>
        /// <param name="mainMenu">The main menu.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="Mode">The mode.</param>
        /// <param name="defMana">The definition mana.</param>
        /// <returns></returns>
        public static MenuItem AddManaManager(this Menu mainMenu, SpellSlot slot, Orbwalking.OrbwalkingMode Mode, int defMana = 0)
        {
            var mmMenu =
                mainMenu.Children.FirstOrDefault(
                    m =>
                        m.Name.ToLower().Contains(
                        $"solo.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.mm"));

            if (mmMenu == null)
            {
                mmMenu = mainMenu.AddManaManagerMenu(Mode);
            }

            return mmMenu.AddSlider(
                $"solo.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.mm.{slot.ToString().ToLower()}",
                $"{slot} Mana {Mode}", defMana, 0, 100);

        }

        /// <summary>
        /// Adds the champ menu.
        /// </summary>
        /// <param name="mainMenu">The main menu.</param>
        /// <param name="defValue">if set to <c>true</c> [definition value].</param>
        /// <returns></returns>
        public static List<MenuItem> AddChampMenu(this Menu mainMenu, bool defValue)
        {
            var champsList = new List<MenuItem>();
            var baseName = mainMenu.Name.ToLower();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var champion in GameObjects.EnemyHeroes)
            {
                var champItem = new MenuItem($"{baseName}.{champion.ChampionName.ToLower()}", champion.ChampionName).SetValue(defValue);
                mainMenu.AddItem(champItem);

                champsList.Add(champItem);
            }

            return champsList;
        }

        /// <summary>
        /// Determines whether the skills is enabled and ready.
        /// </summary>
        /// <param name="Spell">The spell.</param>
        /// <param name="checkMana">if set to <c>true</c> also checks for the mana condition.</param>
        /// <returns></returns>
        public static bool IsEnabledAndReady(this Spell Spell, bool checkMana = true)
        {
            if (!Spell.IsReady())
            {
                return false;
            }

            var Mode = Variables.Orbwalker.ActiveMode;
            var slot = Spell.Slot;
            var readyCondition = Spell.IsReady();
            var menuItem =
                Variables.Menu.Item(
                    $"solo.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.{slot.ToString().ToLower()}");
            var menuCondition = menuItem != null && menuItem.GetValue<bool>();
            var manaItem =
                Variables.Menu.Item(
                    $"solo.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.mm.{slot.ToString().ToLower()}");
            var manaCondition = manaItem != null && ObjectManager.Player.ManaPercent > manaItem.GetValue<Slider>().Value;

            return readyCondition && menuCondition && (checkMana ? manaCondition : true);
        }

        /// <summary>
        /// Adds the mana manager menu.
        /// </summary>
        /// <param name="mainMenu">The main menu.</param>
        /// <param name="Mode">The mode.</param>
        /// <returns></returns>
        private static Menu AddManaManagerMenu(this Menu mainMenu, Orbwalking.OrbwalkingMode Mode)
        {
            var manaManager = new Menu("Mana Manager", $"solo.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.mm");
            {
                mainMenu.AddSubMenu(manaManager);
            }
            return manaManager;
        }
    }
}
