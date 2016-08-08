using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Helpers.Positioning
{
    static class PositioningHelper
    {
        internal static bool IsSafe(this Vector3 Position)
        {
            if ((Position.LSUnderTurret(true) 
                && !ObjectManager.Player.LSUnderTurret(true)) 
                || (PositioningVariables.EnemiesClose.Count() > 1 && DZAIOGeometry.GetEnemyPoints().Contains(Position.LSTo2D())))
            {
                return false;
            }

            return true;

        }

        internal static bool DoPositionsCrossWall(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i).LSTo2D();
                if (tempPosition.LSIsWall())
                {
                    return true;
                }
            }

            return false;
        }

        internal static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i);
                if (tempPosition.LSIsWall())
                {
                    return tempPosition.LSExtend(start, -35);
                }
            }

            return Vector3.Zero;
        }

        internal static float GetWallLength(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            var firstPosition = Vector3.Zero;
            var lastPosition = Vector3.Zero;

            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i);
                if (tempPosition.LSIsWall() && firstPosition == Vector3.Zero)
                {
                    firstPosition = tempPosition;
                }
                lastPosition = tempPosition;
                if (!lastPosition.LSIsWall() && firstPosition != Vector3.Zero)
                {
                    break;
                }
            }

            return Vector3.Distance(firstPosition, lastPosition);
        }

        public static IEnumerable<List<Vector2>> GetCombinations(IReadOnlyCollection<Vector2> allValues)
        {
            var collection = new List<List<Vector2>>();
            for (var counter = 0; counter < (1 << allValues.Count); ++counter)
            {
                var combination = allValues.Where((t, i) => (counter & (1 << i)) == 0).ToList();

                collection.Add(combination);
            }

            return collection;
        }
    }
}
