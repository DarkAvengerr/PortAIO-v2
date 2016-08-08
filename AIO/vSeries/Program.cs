using System;
using LeagueSharp;
using LeagueSharp.Common;
using vSupport_Series.Champions;
using vSupport_Series.Core.Activator;
using vSupport_Series.Core.Plugins;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series
{
    class Program
    {
        public static void Game_OnGameLoad()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Alistar":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Alistar();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Blitzcrank":
                    // ReSharper disable once ObjectCreationAsStatement
                    new vSupport_Series.Champions.Blitzcrank();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "FiddleSticks":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Fiddlesticks();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Janna":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Janna();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Karma":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Karma();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Leona":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Leona();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Lux":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Lux();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Morgana":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Morgana();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Nami":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Nami();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Nautilus":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Nautilus();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Poppy":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Poppy();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Sona":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Sona();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Soraka":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Soraka();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Syndra":
                    // ReSharper disable once ObjectCreationAsStatement
                    new vSupport_Series.Champions.Syndra();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Taric":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Taric();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Thresh":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Thresh();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
                case "Trundle":
                    // ReSharper disable once ObjectCreationAsStatement
                    new Trundle();
                    new ActivatorBase();
                    VersionCheck.UpdateCheck();
                    break;
            }
            
        }
    }
}
