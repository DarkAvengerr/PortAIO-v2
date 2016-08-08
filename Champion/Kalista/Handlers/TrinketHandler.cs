using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using EloBuddy;

namespace S_Plus_Class_Kalista.Handlers
{
    internal class TrinketHandler : Core
    {
        private const string _MenuNameBase = ".Trinket Menu";
        private const string _MenuItemBase = ".Trinket.";

        public static void Load()
        {
            SMenu.AddSubMenu(_Menu());
            Game.OnUpdate += OnUpdate;
        }

        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "trinketOptions");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.BuyOrb", "Auto Buy Orb At Level >= 9").SetValue(true));
            return menu;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.TrinketDelay")) return;

            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.TrinketDelay");
            if (!SMenu.Item(_MenuItemBase + "Boolean.BuyOrb").GetValue<bool>() || Player.Level < 9) return;
            if (!Shop.IsOpen || Items.HasItem(Structures.Items.Trinkets.Orb.Id))
                return;

            Structures.Items.Trinkets.Orb.Buy();
        }
    }
}
