using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DZLib.Logging;
using iSeriesReborn.Utility.Entities;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Utility.MenuUtility
{
    static class iSRMenuExtensions
    {
        public static Menu AddModeMenu(this Menu mainMenu, Orbwalking.OrbwalkingMode Mode)
        {
            var modeMenu = new Menu($"[iSR] {Mode}", $"iseriesr.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}");
            {
                mainMenu.AddSubMenu(modeMenu);
            }
            return modeMenu;
        }

        public static MenuItem AddSkill(this Menu mainMenu, SpellSlot slot, Orbwalking.OrbwalkingMode Mode, bool defaultValue = true, int defMana = 20, bool addMM = true)
        {
            if (addMM)
            {
                mainMenu.AddManaManager(slot, Mode, defMana);
            }

            return mainMenu.AddBool($"iseriesr.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.{slot.ToString().ToLower()}", $"Use {slot}", defaultValue);
        }

        public static MenuItem AddManaManager(this Menu mainMenu, SpellSlot slot, Orbwalking.OrbwalkingMode Mode, int defMana = 0)
        {
            var mmMenu =
                mainMenu.Children.FirstOrDefault(
                    m =>
                        m.Name.ToLower().Contains(
                        $"iseriesr.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.mm"));

            if (mmMenu == null)
            {
                mmMenu = mainMenu.AddManaManagerMenu(Mode);
            }

            return mmMenu.AddSlider(
                $"iseriesr.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.mm.{slot.ToString().ToLower()}",
                $"{slot} Mana {Mode}", defMana, 0, 100);

        }

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

        public static bool IsEnabledAndReady(this Spell Spell, bool checkMana = true)
        {
            if (!Spell.LSIsReady())
            {
                return false;
            }

            var Mode = Variables.Orbwalker.ActiveMode;
            var slot = Spell.Slot;
            var readyCondition = Spell.LSIsReady();
            var menuItem =
                Variables.Menu.Item(
                    $"iseriesr.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.{slot.ToString().ToLower()}");
            var menuCondition = menuItem != null && menuItem.GetValue<bool>();
            var manaItem =
                Variables.Menu.Item(
                    $"iseriesr.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.mm.{slot.ToString().ToLower()}");
            var manaCondition = manaItem != null && ObjectManager.Player.ManaPercent > manaItem.GetValue<Slider>().Value;

            return readyCondition && menuCondition && (checkMana ? manaCondition : true);
        }

        private static Menu AddManaManagerMenu(this Menu mainMenu, Orbwalking.OrbwalkingMode Mode)
        {
            var manaManager = new Menu("Mana Manager", $"iseriesr.{ObjectManager.Player.ChampionName.ToLower()}.{Mode.ToString().ToLower()}.mm");
            {
                mainMenu.AddSubMenu(manaManager);
            }
            return manaManager;
        }
    }
}
