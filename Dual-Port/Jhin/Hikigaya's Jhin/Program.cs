using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso
{
    class Program
    {
        public static void JhinOnGameLoad()
        {
            if (ObjectManager.Player.ChampionName == "Jhin")
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Jhin();
            }
            else
            {
                Console.WriteLine("sorry :roto2:");
            }
        }
    }
}
