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
            OnLoad(new EventArgs());
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Riven")
            {
                return;
            }

            Load.LoadAssembly();
        }

        #endregion
    }
}