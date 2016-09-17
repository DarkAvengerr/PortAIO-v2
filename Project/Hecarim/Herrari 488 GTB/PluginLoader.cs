using System;

using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Herrari_488_GTB
{
    class PluginLoader
    {
        internal static bool LoadPlugin(string PluginName)
        {

            if (CanLoadPlugin(PluginName))
            {
                DynamicInitializer.NewInstance(Type.GetType("Herrari_488_GTB.Plugins." + ObjectManager.Player.ChampionName));
                return true;
            }

            return false;
        }

        internal static bool CanLoadPlugin(string PluginName)
        {
            return Type.GetType("Herrari_488_GTB.Plugins." + ObjectManager.Player.ChampionName) != null;
        }
    }
}
