using System;
using LeagueSharp.Common;

namespace Kennen
{
    internal class Program
    {

        public static void Game_OnGameLoad()
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Champion.Kennen();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load assembly: {0}", exception);
                throw;
            }
        }
    }
}
