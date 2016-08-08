using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen
{
    public static class Program
    {
        public static void Main()
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Kennen();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the assembly - {0}", exception);
                throw;
            }
        }
    }
}

