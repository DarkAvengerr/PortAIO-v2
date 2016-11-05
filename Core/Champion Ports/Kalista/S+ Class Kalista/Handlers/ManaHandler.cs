using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace S_Plus_Class_Kalista.Handlers
{
    class ManaHandler : Core
    {
        //private const string _MenuNameBase = ".Mana Manager Menu";
        //private const string _MenuItemBase = ".ManaManager.";
        //private static int GetManaPercent => (int)(Player.Mana / Player.MaxMana);
        //public static void Load()
        //{
        //    SMenu.AddSubMenu(_Menu());
        //}

        //private static Menu _Menu()
        //{
        //    var menu = new Menu(_MenuNameBase, "manaMenu");

        //    menu.AddItem(new MenuItem(_MenuItemBase + "ManaManager.Boolean.Enable", "Enable ManaManager").SetValue(false));

        //    var gameMode = new Menu("Game Modes(Combo,Mixed...)", "gameModeManaMenu");
        //    gameMode.AddItem(new MenuItem(_MenuItemBase + "ManaManager.Mode.Slider.QPercent", "Q Mana%").SetValue(new Slider(35, 1, 90)));
        //    gameMode.AddItem(new MenuItem(_MenuItemBase + "ManaManager.Mode.Slider.EPercent", "E Mana%").SetValue(new Slider(15, 1, 90)));

        //    var autoMode = new Menu("Auto Events", "autoManaMenu");
        //    autoMode.AddItem(new MenuItem(_MenuItemBase + "ManaManager.Auto.Slider.EPercent", "E Mana%").SetValue(new Slider(10, 1, 90)));
        //    autoMode.AddItem(new MenuItem(_MenuItemBase + "ManaManager.Auto.Slider.RPercent", "R Mana%").SetValue(new Slider(0, 1, 90)));

        //    menu.AddSubMenu(gameMode);
        //    menu.AddSubMenu(autoMode);
        //    return menu;
        //}

        //public static bool UseModeQ()
        //{
        //    if (!SMenu.Item(_MenuItemBase + "ManaManager.Boolean.Enable").GetValue<bool>()) return true; // Dont use mana Manager

        //    return SMenu.Item(_MenuItemBase + "ManaManager.Mode.Slider.QPercent").GetValue<Slider>().Value < GetManaPercent;
        //}

        //public static bool UseModeE()
        //{
 
        //    if (!SMenu.Item(_MenuItemBase + "ManaManager.Boolean.Enable").GetValue<bool>()) return true; // Dont use mana Manager

        //    return SMenu.Item(_MenuItemBase + "ManaManager.Mode.Slider.EPercent").GetValue<Slider>().Value < GetManaPercent;
        //}
   
        //public static bool UseAutoE()
        //{
  
        //    if (!SMenu.Item(_MenuItemBase + "ManaManager.Boolean.Enable").GetValue<bool>()) return true; // Dont use mana Manager

        //    return SMenu.Item(_MenuItemBase + "ManaManager.Auto.Slider.EPercent").GetValue<Slider>().Value < GetManaPercent;
        //}


        //public static bool UseAutoR()
        //{
        //    if (!SMenu.Item(_MenuItemBase + "ManaManager.Boolean.Enable").GetValue<bool>()) return true; // Dont use mana Manager

        //    return SMenu.Item(_MenuItemBase + "ManaManager.Auto.Slider.RPercent").GetValue<Slider>().Value < GetManaPercent;
        //}
    }
}
