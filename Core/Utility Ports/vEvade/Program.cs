using EloBuddy; 
using LeagueSharp.Common; 
 namespace vEvade
{
    #region

    using LeagueSharp.Common;

    using vEvade.Core;
    using vEvade.Helpers;

    #endregion

    public class Program
    {
        #region Methods

        public static void Main()
        {
            Configs.Debug = false;
            Evade.OnGameLoad(new System.EventArgs());
        }

        #endregion
    }
}