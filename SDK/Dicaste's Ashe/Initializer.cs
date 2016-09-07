using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DicasteAshe
{
    using System;
    using System.Reflection;

    using LeagueSharp.SDK;

    using static LeagueSharp.SDK.GameObjects;

    internal static class Initializer
    {
        private static string AssemblyName { get; } = Assembly.GetExecutingAssembly().GetName().Name;

        internal static void Init()
        {
            Events.OnLoad += OnLoad;
        }

        private static void OnLoad(object sender, EventArgs eventArgs)
        {
            if (Player.ChampionName != AssemblyName)
            {
                return;
            }

            Core.Init();
        }
    }
}