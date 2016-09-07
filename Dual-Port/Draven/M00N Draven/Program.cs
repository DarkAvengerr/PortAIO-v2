using EloBuddy; 
 using LeagueSharp.Common; 
 namespace MoonDraven
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        #region Methods

        public static void GameOnOnGameLoad()
        {
            if (ObjectManager.Player.BaseSkinName == "Draven")
            {
                new MoonDraven().Load();
            }
        }

        #endregion
    }
}