using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FastTrundle
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Trundle.Game_OnGameLoad;
        }

        #endregion
    }
}