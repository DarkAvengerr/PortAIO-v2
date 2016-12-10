using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Standalone_Orbwalker
{
    class Program
    {
        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu _menu;

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            _menu = new Menu("Standalone Orbwalker","Orb",true);
            var orbMenu = new Menu("Orbwalker", "orbStand");
            _orbwalker = new Orbwalking.Orbwalker(orbMenu);
            _menu.AddSubMenu(orbMenu);
            _menu.AddToMainMenu();
        }
    }
}
