using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Handlers;

namespace S_Plus_Class_Kalista
{
    class Program : Core
    {

        public static void OnLoad()
        {
            if (Player.ChampionName != "Kalista")
                return;

            Console.WriteLine(@"S+ Class Kalista Loading Core...");
            Core.Champion.Load();
            Console.WriteLine(@"S+ Class Kalista Loading Humanizer...");
            Humanizer.Load();
            Console.WriteLine(@"S+ Class Kalista Loading Drawing...");
            DrawingHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Loading Orbwalker...");
            OrbwalkHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Loading Auto Events...");
            RendHandler.Load();
            SentinelHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Loading ManaManager...");
           // ManaHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Loading Trinkets...");
            TrinketHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Loading Items...");
            ItemHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Loading Levels...");
            LevelHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Loading SoulBound...");
            SoulBoundHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Loading Debugger...");
            DebugHandler.Load();
            Console.WriteLine(@"S+ Class Kalista Finalizing Menu...");
            SMenu.AddSubMenu(new Menu("Credits: By Kallen", "doesnotMatterMenu"));
            Core.SMenu.AddToMainMenu();

            Console.WriteLine(@"S+ Class Kalista Load Completed");
        }
    }
}
