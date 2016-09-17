using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElXerath
{
    using LeagueSharp.Common;

    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Xerath.Game_OnGameLoad;
        }

        #endregion
    }
}