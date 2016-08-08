using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_Udyr
{
    static class Helper
    {
        public static bool IsTiger(this AIHeroClient unit)
        {
            return unit.LSHasBuff("udyrtigerpunch");
        }
        public static bool IsTurtle(this AIHeroClient unit)
        {
            return unit.LSHasBuff("udyrturtleactivation");
        }
        public static bool IsBear(this AIHeroClient unit)
        {
            return unit.LSHasBuff("udyrbearactivation");
        }
        public static bool IsPhoenix(this AIHeroClient unit)
        {
            return unit.LSHasBuff("udyrphoenixactivation"); // udyrbearstuncheck
        }
        public static bool HasBearPassive(this AIHeroClient unit)
        {
            return unit.LSHasBuff("udyrbearstuncheck"); // 
        }
    }
}
