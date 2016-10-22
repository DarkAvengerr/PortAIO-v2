using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Sharpy_AIO
{
    class PluginLoader
    {
        internal static bool LoadPlugin(string PluginName)
        {
            if (CanLoadPlugin(PluginName))
            {
                DynamicInitializer.NewInstance(Type.GetType("Sharpy_AIO.Plugins." + ObjectManager.Player.ChampionName));
                return true;
            }

            return false;
        }

        internal static bool CanLoadPlugin(string PluginName)
        {
            return Type.GetType("Sharpy_AIO.Plugins." + ObjectManager.Player.ChampionName) != null;
        }
    }
}
