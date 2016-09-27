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
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            PJR.OnLoad();
        }
    }
}
