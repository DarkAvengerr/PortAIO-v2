using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheKalista.Commons
{
    public static class TickLimiter
    {
        private static readonly Dictionary<int, float> Time = new Dictionary<int, float>();

        public static bool Limit(int ms, int id = 0)
        {
            if (!Time.ContainsKey(id))
                Time.Add(id, Game.Time);

            var diff = (Game.Time - Time[id]) * 1000 > ms;
            if (diff) Time[id] = Game.Time;
            return diff;
        }

    }
}
