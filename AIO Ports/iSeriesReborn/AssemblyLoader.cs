using iSeriesReborn.External.Activator;
using iSeriesReborn.Utility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn
{
    class AssemblyLoader
    {
        public static void OnLoad()
        {
            BuildDefaultMenu();
            LoadChampion();

            Variables.Menu.AddToMainMenu();
        }

        private static void BuildDefaultMenu()
        {
            var defaultMenu = Variables.Menu;

            var OWMenu = new Menu("[iSR] Orbwalker", "iseriesr.orbwalker");
            {
                Variables.Orbwalker = new Orbwalking.Orbwalker(OWMenu);
                defaultMenu.AddSubMenu(OWMenu);
            }

            var TSMenu = new Menu("[iSR] TS", "iseriesr.ts");
            {
                TargetSelector.AddToMenu(TSMenu);
                defaultMenu.AddSubMenu(TSMenu);
            }
        }

        private static void LoadChampion()
        {
            var ChampionToLoad = ObjectManager.Player.ChampionName;

            if (Variables.ChampList.ContainsKey(ChampionToLoad))
            {
                Variables.ChampList[ChampionToLoad]();

                iSeriesReborn.External.Activator.Activator.LoadMenu();
                iSeriesReborn.External.Activator.Activator.OnLoad();

                Chat.Print($"<b><font color='#FF0000'>[iSR]</font></b> {ChampionToLoad} Loaded!");
            }
        }
    }
}
