using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Utility
{
    using System;
    using Common;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    public static class Tools
    {
        public static Menu Menu;
        public static bool EnableActivator = true;

        public static void Inject()
        {
            Menu = Program.Menu.Add(new Menu("Tools", "Tools"));

            var PlugingMenu = Menu.Add(new Menu("PlugingInject", "Pluging Inject"));
            {
                PlugingMenu.Add(new MenuBool("LoadEvade", "Inject Evade Plugings", true));
                PlugingMenu.Add(new MenuBool("LoadPotions", "Inject Potions Plugings", true));
                PlugingMenu.Add(new MenuBool("LoadOffensive", "Inject Offensive Plugings", true));
                PlugingMenu.Add(new MenuBool("LoadDefensive", "Inject Defensive Plugings", true));
                PlugingMenu.Add(new MenuBool("LoadSummoner", "Inject Summoner Plugings", true));
                PlugingMenu.Add(new MenuBool("LoadSkinChance", "Inject SkinChance Plugings", true));
                PlugingMenu.Add(new MenuBool("LoadAutoLevel", "Inject AutoLevel Plugings", true));
                PlugingMenu.Add(new MenuSeparator(" ", "  "));
                PlugingMenu.Add(new MenuSeparator("ASDASDW", "If you Change Please Press F5 ReLoad!"));
            }

            Menu.Add(new MenuSeparator(" ", "  "));

            Manager.WriteConsole("Tools Inject!");

            if (Menu["PlugingInject"]["LoadEvade"])
            {
                Common.Evade.Evade.InjectEvade();
            }

            if (Menu["PlugingInject"]["LoadPotions"])
            {
                Potions.Inject();
            }

            if (Menu["PlugingInject"]["LoadOffensive"])
            {
                Offensive.Inject();
            }

            if (Menu["PlugingInject"]["LoadDefensive"])
            {
                Defensive.Inject();
            }

            if (Menu["PlugingInject"]["LoadSummoner"])
            {
                Summoner.Inject();
            }

            if (Menu["PlugingInject"]["LoadSkinChance"])
            {
                SkinChance.Inject();
            }

            if (Menu["PlugingInject"]["LoadAutoLevel"])
            {
                AutoLevel.Inject();
            }

            if (LeagueSharp.Common.Menu.GetMenu("Activator", "activator") == null &&
                LeagueSharp.Common.Menu.GetMenu("ElUtilitySuite", "ElUtilitySuite") == null &&
                LeagueSharp.Common.Menu.GetMenu("MActivator", "masterActivator") == null)
            {
                EnableActivator = false;
            }
            else
            {
                EnableActivator = true;
            }

            DelayAction.Add(2000, () => Variables.Orbwalker.Enabled = true);
            DelayAction.Add(4000, () => Variables.Orbwalker.Enabled = true);
            DelayAction.Add(6000, () => Variables.Orbwalker.Enabled = true);
            DelayAction.Add(8000, () => Variables.Orbwalker.Enabled = true);
            DelayAction.Add(10000, () => Variables.Orbwalker.Enabled = true);
        }
    }
}