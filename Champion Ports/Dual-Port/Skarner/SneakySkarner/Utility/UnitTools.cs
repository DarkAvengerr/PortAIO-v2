using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SneakySkarner.Utility
{
    static class UnitTools
    {
        public static bool TargetRunningAway(AIHeroClient hero, AIHeroClient player)
        {
            var isfacingplayer = hero.IsFacing(player);
            var ismoving = hero.IsMoving;
            return !isfacingplayer && ismoving && !hero.IsMovementImpaired();
        }

        public static int NumInRange(this List<Vector2> unitPositions, Obj_AI_Base centerUnit, float range)
        {
            return unitPositions.Count(pos => pos.Distance(centerUnit) < (range - 10));
        }
    }
}
