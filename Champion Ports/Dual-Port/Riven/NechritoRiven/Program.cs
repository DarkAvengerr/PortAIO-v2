using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven
{
    #region

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class Program
    {
        #region Methods

        public static void Main()
        {
            OnLoad();
        }

        private static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Riven")
            {
                return;
            }

            Console.WriteLine("Loading...");
            Load.LoadAssembly();
        }

        #endregion
    }
}