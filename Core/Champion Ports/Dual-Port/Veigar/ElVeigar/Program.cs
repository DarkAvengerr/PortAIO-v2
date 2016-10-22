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

        public static void Main()
        {
            try
            {
                Entry.OnLoad();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}