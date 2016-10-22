using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElSinged
{

    public class ElSingedMenu
    {
        public static Menu Menu;

        public static void Initialize()
        {
            Menu = new Menu("ElSinged", "menu", true);

            //ElSinged.Orbwalker
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Singed.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            //ElSinged.TargetSelector
            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);

            var cMenu = new Menu("Combo", "Combo");
            {
                cMenu.AddItem(new MenuItem("ElSinged.Combo.Q", "Use Q").SetValue(true));
                cMenu.AddItem(new MenuItem("ElSinged.Combo.W", "Use W").SetValue(true));
                cMenu.AddItem(new MenuItem("ElSinged.Combo.E", "Use E").SetValue(true));
                cMenu.AddItem(new MenuItem("ElSinged.Combo.R", "Use R").SetValue(true));
                cMenu.AddItem(new MenuItem("ElSinged.Coffasfsafsambo.R", ""));
                cMenu.AddItem(new MenuItem("ElSinged.Combo.R.Count", "Use R enemies >= ")).SetValue(new Slider(2, 1, 5));
                cMenu.AddItem(new MenuItem("ElSinged.Combo.Ignite", "Use Ignite").SetValue(true));
                cMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            }
            Menu.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "Harass");
            {
                hMenu.AddItem(new MenuItem("ElSinged.Harass.Q", "Use Q").SetValue(true));
                hMenu.AddItem(new MenuItem("ElSinged.Harass.W", "Use W").SetValue(true));
                hMenu.AddItem(new MenuItem("ElSinged.Harass.E", "Use E").SetValue(true));
            }
            Menu.AddSubMenu(hMenu);

            var lcMenu = new Menu("Laneclear", "Laneclear");
            {
                lcMenu.AddItem(new MenuItem("ElSinged.Laneclear.Q", "Use Q").SetValue(true));
                lcMenu.AddItem(new MenuItem("ElSinged.Laneclear.E", "Use E").SetValue(true));
            }
            Menu.AddSubMenu(lcMenu);

            //ElSinged.Misc
            var miscMenu = new Menu("Drawings", "Misc");
            {
                miscMenu.AddItem(new MenuItem("ElSinged.Draw.off", "Turn drawings off").SetValue(false));
                miscMenu.AddItem(new MenuItem("ElSinged.Draw.Q", "Draw Q").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElSinged.Draw.W", "Draw W").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElSinged.Draw.E", "Draw E").SetValue(new Circle()));
            }
            Menu.AddSubMenu(miscMenu);

            //Here comes the moneyyy, money, money, moneyyyy
            var credits = new Menu("Credits", "jQuery");
            {
                credits.AddItem(new MenuItem("ElSinged.Paypal", "if you would like to donate via paypal:"));
                credits.AddItem(new MenuItem("ElSinged.Email", "info@zavox.nl"));
            }
            Menu.AddSubMenu(credits);

            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(new MenuItem("422442fsaafsf", "Version: 1.0.0.4"));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            Menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }
    }
}