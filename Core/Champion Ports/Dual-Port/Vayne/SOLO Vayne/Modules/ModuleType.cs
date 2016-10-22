using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules
{
    /// <summary>
    /// The Module Type
    /// </summary>
    enum ModuleType
    {
        /// <summary>
        /// The Module is Executed Every Tick
        /// </summary>
        OnUpdate,

        /// <summary>
        /// The module is executed after an AA
        /// </summary>
        OnAfterAA,

        /// <summary>
        /// The module is executed under other conditions.
        /// </summary>
        Other
    }
}
