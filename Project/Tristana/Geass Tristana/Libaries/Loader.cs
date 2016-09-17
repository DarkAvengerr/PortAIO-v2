using Geass_Tristana.Misc;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Libaries
{
    internal class Loader : Core
    {
        // ReSharper disable once NotAccessedField.Local
        private static GeassLib.Loader _loader;
        public static void LoadAssembly()
        {
            //load delays
            DelayHandler.Load();


            //Initilize Menus
            var champMenu = new Menus.ChampionMenu();
            var orbwalkerMenu = new Menus.OrbwalkerMenu();
            var antiMenu = new Menus.AutoMenu();

            //Load Data
            Champion.Player = ObjectManager.Player;
            //Load Menus into SMenu
            antiMenu.Load();
            champMenu.Load();
            orbwalkerMenu.Load();

            _loader = new GeassLib.Loader($"{Champion.Player.ChampionName}", true, true, Data.Level.AbilitySequence, true, true);

            //Add SMenu to main menu
            SMenu.AddToMainMenu();
        }
    }
}