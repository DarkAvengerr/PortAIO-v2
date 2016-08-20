using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.AssemblyDisabler.Interfaces
{
    public interface IAssembly
    {
        void DisableByMenu();

        void EnableByMenu();

        void DisableByCustom();

        void EnableByCustom();
    }
}
