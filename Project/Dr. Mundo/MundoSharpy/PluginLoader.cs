using System;

using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Mundo_Sharpy
{
    class PluginLoader
    {
        internal static bool LoadPlugin(string PluginName)
        {

            if (CanLoadPlugin(PluginName))
            {
                DynamicInitializer.NewInstance(Type.GetType("Mundo_Sharpy.Plugins." + ObjectManager.Player.ChampionName));
                return true;
            }

            return false;
        }

        internal static bool CanLoadPlugin(string PluginName)
        {
            return Type.GetType("Mundo_Sharpy.Plugins." + ObjectManager.Player.ChampionName) != null;
        }
    }
}
