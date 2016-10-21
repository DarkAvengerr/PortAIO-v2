using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Sharpy_AIO
{
    class Program
    {
        public static void Game_OnGameLoad()
        {
            PluginLoader.LoadPlugin(ObjectManager.Player.ChampionName);
        }
    }
}
