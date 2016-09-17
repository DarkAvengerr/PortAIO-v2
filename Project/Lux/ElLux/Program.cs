using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElLux
{
    using LeagueSharp.Common;

    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Lux.OnLoad;
        }

        #endregion
    }
}