using System;

using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Mundo_Sharpy
{
    class Initializer
    {
        internal static void Initialize()
        {
            Console.WriteLine("Mundo Sharpy: HelloWorld!");

            MenuProvider.initialize();

            if (PluginLoader.LoadPlugin(ObjectManager.Player.ChampionName))
            {
                MenuProvider.Champion.Drawings.addItem(" ");
            }

            AutoQuit.Load();

            Console.WriteLine("Mundo Sharpy: Initialized.");
        }
    }
}
