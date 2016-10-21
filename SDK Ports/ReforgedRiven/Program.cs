#region

using System;
using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven
{
    internal class Program : Core
    {
        public static void Main()
        {
            Bootstrap.Init();
            Load();
        }

        private static void Load()
        {
            if (GameObjects.Player.ChampionName != "Riven")
            {
                return;
            }
            Reforged_Riven.Load.LoadAssembly();
        }
    }
}
