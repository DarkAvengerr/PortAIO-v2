using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SPredictioner
{
    class Program
    {
        static void Main(string[] args)
        {
            EventHandlers.Game_OnGameLoad();
        }
    }
}