using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElVeigar
{
    using System;

    using LeagueSharp.Common;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ElVeigar
    {
        #region Methods

        private static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += Entry.OnLoad;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}