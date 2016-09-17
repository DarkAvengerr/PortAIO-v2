using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SoloVayne.Utility.Enums;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Skills
{
    interface Skill
    {
        /// <summary>
        /// Gets the skill mode.
        /// </summary>
        /// <returns></returns>
        SkillMode GetSkillMode();

        /// <summary>
        /// Executes the logic give a specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Execute(Obj_AI_Base target);

        /// <summary>
        /// Executes the farm logic given a specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void ExecuteFarm(Obj_AI_Base target);
    }
}
