using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Functions
{

    internal static class AssemblyTime
    {
        #region Public Methods

        /// <summary>
        ///     Return current time
        /// </summary>
        /// <returns>
        /// </returns>
        public static float CurrentTime() => (float)DateTime.Now.Subtract(StaticObjects.AssemblyLoadTime).TotalMilliseconds;

        #endregion Public Methods
    }

}