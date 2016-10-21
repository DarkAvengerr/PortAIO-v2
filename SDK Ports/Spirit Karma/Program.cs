#region

using System;
using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma
{
    internal class Program
    {
        public static void Main()
        {
            Bootstrap.Init();
            Load();
        }

        private static void Load()
        {
            if (GameObjects.Player.ChampionName != "Karma")
            {
                Console.WriteLine("Could not load Karma!"); return;
            }
            Spirit_Karma.Load.Load.LoadAssembly();
        }
    }
}
