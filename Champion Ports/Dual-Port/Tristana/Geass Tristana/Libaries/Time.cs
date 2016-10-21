using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace S__Class_Tristana.Libaries
{
    class Time
    {
        private readonly DateTime AssemblyLoadTime = DateTime.Now;

        public float LastTick { get; set; }

        public float TickCount()
        {
            return (float)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
        }

        public bool CheckLast()
        {
            return TickCount() - LastTick > (1000 + Game.Ping / 2);
        }
    }
}
