using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ADCPackage
{
    class Program
    {
        public static void Game_OnGameLoad()
        {
            Menu.Initialize();
            PluginLoader.Load();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            OrbwalkerSwitch.Update();
        }
    }
}
