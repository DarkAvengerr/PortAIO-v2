using System;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kassadin_the_Harbinger
{
    internal class Program
    {
        public static void Main()
        {
            try
            {
                Kassadin.OnLoad();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }
    }
}