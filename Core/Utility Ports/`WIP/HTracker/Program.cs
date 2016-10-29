using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.SDK;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HTrackerSDK
{
    class Program
    {
        static void Main(string[] args)
        {
            OnLoad();
        }
        private static void OnLoad()
        {
            Tracker.OnLoad();
            WardTracker.OnLoad();
            PathTracker.OnLoad();
        }
    }
}
