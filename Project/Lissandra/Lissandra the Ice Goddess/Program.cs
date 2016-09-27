using System;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Lissandra_the_Ice_Goddess
{
    internal class Program
    {
        public static void Main()
        {
            try
            {
                Lissandra.OnLoad();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }
    }
}