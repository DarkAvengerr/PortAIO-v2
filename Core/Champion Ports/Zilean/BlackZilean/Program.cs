using System;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BlackZilean
{
    internal class Program
    {
        public static void Main()
        {
            try
            {
                Zilean.OnLoad();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }
    }
}