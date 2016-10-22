using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Utility.ModuleHelper
{
    interface IModule
    {
        string GetName();

        ModuleType GetModuleType();

        bool ShouldRun();

        void Run();
    }
}
