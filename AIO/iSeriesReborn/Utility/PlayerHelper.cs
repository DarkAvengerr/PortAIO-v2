using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Utility
{
    static class PlayerHelper
    {
        private static float LastMoveC;

        public static float GetRealAutoAttackRange(AIHeroClient attacker, AttackableUnit target)
        {
            var result = attacker.AttackRange + attacker.BoundingRadius;
            if (target.LSIsValidTarget())
            {
                return result + target.BoundingRadius;
            }
            return result;
        }

        public static void MoveToLimited(Vector3 where)
        {
            if (Environment.TickCount - LastMoveC < 80)
            {
                return;
            }
            LastMoveC = Environment.TickCount;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }
        
        public static bool IsSummonersRift()
        {
            var map = LeagueSharp.Common.Utility.Map.GetMap();
            if (map != null && map.Type == LeagueSharp.Common.Utility.Map.MapType.SummonersRift)
            {
                return true;
            }
            return false;
        }

        public static Vector3 GetPositionInFront(this AIHeroClient target, float distance, bool addHitBox = true)
        {
            return (target.ServerPosition.LSTo2D() + target.Direction.LSTo2D().LSPerpendicular() * (distance + (addHitBox ? 0 : 65f))).To3D();
        }

        public static bool IsRunningAway(this AIHeroClient target)
        {
            return ObjectManager.Player.LSDistance(target.GetPositionInFront(300)) >
                   ObjectManager.Player.LSDistance(target.ServerPosition) && !target.LSIsFacing(ObjectManager.Player);
        }
    }
}
