using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PennyJinxReborn
{
    using System;
    using LeagueSharp.Common;

    /// <summary>
    /// The main class in the Assembly.
    /// </summary>
    internal class Program
    {

        /// <summary>
        /// The Main method.
        /// </summary>
        /// <param name="args">The method args</param>
        internal static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += GameOnLoad;
        }

        /// <summary>
        /// Called on the game load.
        /// </summary>
        /// <param name="args">The event args.</param>
        private static void GameOnLoad(EventArgs args)
        {
            PJR.OnLoad();
        }
    }
}
