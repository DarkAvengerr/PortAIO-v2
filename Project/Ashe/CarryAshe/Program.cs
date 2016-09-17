using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CarryAshe
{
    class Program
    {

        internal static Ashe Champion = new Ashe();
        internal static Menu RootMenu;
        internal static Orbwalking.Orbwalker Orbwalker;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;

        }

        static void OnLoad(EventArgs args)
        {
            RootMenu = new Menu("CarryAshe", "CarryAsheMenu", true);
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            RootMenu.AddSubMenu(orbwalkerMenu);
            RootMenu.AddSubMenu(ItemManager.Menu);
            Champion.OnLoad(args);
        }
    }
}
