using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

using SharpDX;

//made by Hellsing
using EloBuddy; 
 using LeagueSharp.Common;
namespace DesomondBard
{
    public static class VectorHelper
    {
        public static Vector3? GetFirstWallPoint(this Vector3 from, Vector3 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                if (NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y).HasFlag(CollisionFlags.Wall) ||
                    NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y).HasFlag(CollisionFlags.Building))
                {
                    return from + d * direction;
                }
            }

            return null;
        }

        public static Vector2? GetFirstWallPoint(this Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                if (NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y).HasFlag(CollisionFlags.Wall) ||
                    NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y).HasFlag(CollisionFlags.Building))
                {
                    return from + d * direction;
                }
            }

            return null;
        }
    }
}