using System;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ODarius
{
    internal class Program
    {
        public static void Main()
        {
            try
            {
                Darius.Load();
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"The Exception Error Is: " + ex);
            }
        }
    }
}
