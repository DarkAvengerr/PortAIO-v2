using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SAssemblies.Champions
{
    class Champion
    {
        public static Menu.MenuItemSettings Champions = new Menu.MenuItemSettings();

        private Champion()
        {

        }

        ~Champion()
        {
            
        }

        private static void SetupMainMenu()
        {
            var menu = new LeagueSharp.Common.Menu("SChampions", "SAssembliesSChampions", true);
            SetupMenu(menu);
            menu.AddToMainMenu();
        }

        public static Menu.MenuItemSettings SetupMenu(LeagueSharp.Common.Menu menu, bool useExistingMenu = false)
        {
            Language.SetLanguage();
            if (!useExistingMenu)
            {
                Champions.Menu = Menu.GetSubMenu(menu, "SAssembliesChampions") ?? menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_MAIN"), "SAssembliesChampions"));
            }
            else
            {
                Champions.Menu = menu;
            }
            if (!useExistingMenu)
            {
                Champions.CreateActiveMenuItem("SAssembliesChampionsActive");
            }
            return Champions;
        }
    }
}
