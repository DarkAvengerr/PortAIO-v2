using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengar
{
    using LeagueSharp.Common;

    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Rengar.OnLoad;
        }

        #endregion
    }
}