using System;
using DZLib;
using DZLib.MenuExtensions;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iDZEzreal.MenuHelper
{
    internal class MenuGenerator
    {
        public static void Generate()
        {
            Variables.Menu = new Menu("iDZEzreal 3.0", "ezreal", true);
            var rootMenu = Variables.Menu;
            var owMenu = new Menu("[Ez] Orbwalker", "ezreal.orbwalker");
            {
                Variables.Orbwalker = new Orbwalking.Orbwalker(owMenu);
                rootMenu.AddSubMenu(owMenu);
            }

            var comboMenu = new Menu("[Ez] Combo", "ezreal.combo");
            {
                comboMenu.AddBool("ezreal.combo.q", "Use Q", true);
                comboMenu.AddBool("ezreal.combo.w", "Use W", true);
                comboMenu.AddBool("ezreal.combo.r", "Use R", true);
                comboMenu.AddSlider("ezreal.combo.r.min", "Min Enemies", 2, 1, 5);
                rootMenu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu("[Ez] Harass", "ezreal.mixed");
            {
                mixedMenu.AddBool("ezreal.mixed.q", "Use Q", true);
                mixedMenu.AddBool("ezreal.mixed.w", "Use W", true);
                mixedMenu.AddSlider("ezreal.mixed.mana", "Min Mana", 45, 0, 100);
                rootMenu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu("[Ez] Farm", "ezreal.farm");
            {
                farmMenu.AddBool("ezreal.farm.q", "Use Q", true);
                rootMenu.AddSubMenu(farmMenu);
            }

            var miscMenu = new Menu("[Ez] Misc", "ezreal.misc");
            {
                miscMenu.AddStringList("ezreal.misc.hitchance", "Hitchance",
                    new[] {"Low", "Medium", "High", "Very High"}, 3);
                miscMenu.AddBool("ezreal.misc.gapcloser", "Anti Gap Closer", true);
                miscMenu.AddBool("ezreal.misc.selfWE", "Self W/E - Beta AF", true);
                miscMenu.AddKeybind("ezreal.misc.semimanualr", "Semimanual R", new Tuple<uint, KeyBindType>('U', KeyBindType.Press));

                rootMenu.AddSubMenu(miscMenu);
            }

            var moduleMenu = new Menu("[Ez] Modules", "ezreal.modules");
            {
                foreach (var module in Variables.Modules)
                {
                    moduleMenu.AddBool("ezreal.modules." + module.GetName().ToLowerInvariant(), "" + module.GetName());
                }
                rootMenu.AddSubMenu(moduleMenu);
            }

            var drawingsMenu = new Menu("[Ez] Drawings", "ezreal.drawings");
            {
                drawingsMenu.AddBool("ezreal.drawings.q", "Q Draw", true);
                drawingsMenu.AddBool("ezreal.drawings.w", "W Draw", true);

                rootMenu.AddSubMenu(drawingsMenu);
            }
            rootMenu.AddToMainMenu();
        }

        public static HitChance GetHitchance()
        {
            switch (Variables.Menu.Item("ezreal.misc.hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
            }
            return HitChance.High;
        }
    }
}