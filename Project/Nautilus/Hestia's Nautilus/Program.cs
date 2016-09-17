using System;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HestiaNautilus
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Nautilus();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the assembly - {0}", exception);
                throw;
            }
        }
    }
}
