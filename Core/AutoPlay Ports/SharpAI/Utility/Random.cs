using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace SharpAI.Utility
{
    public static class Random
    {
        private static System.Random _rand = new System.Random();

        public static int GetRandomInteger(int min, int max)
        {
            return _rand.Next(min, max);
        }
    }
}
