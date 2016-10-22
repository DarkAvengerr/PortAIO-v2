using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Libaries;
using EloBuddy;

namespace S_Plus_Class_Kalista.Handlers
{
    class DebugHandler : Core
    {
        private const string _MenuNameBase = ".Debug Menu";
        private const string _MenuItemBase = ".Debug.";

        public static void Load()
        {
            SMenu.AddSubMenu(_Menu());
            Game.OnUpdate += OnUpdate;
        }

        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "debugMenu");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.Debug", "Enable debugging").SetValue(false));
            return menu;
        }
        private static void OnUpdate(EventArgs args)
        {
            if (!SMenu.Item(_MenuItemBase + "Boolean.Debug").GetValue<bool>()) return;
            foreach (var delay in Humanizer.Limiter.Delays)
            {
                Chat.Print(delay.Key + ":Delay:" + delay.Value.Delay + ":Last Tick:" + delay.Value.LastTick);
            }
        }
    }
}
