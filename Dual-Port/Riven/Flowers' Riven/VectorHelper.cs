/*
 VectorHelper.cs is part of CNLib.
*/

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Riven
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class VectorHelper
    {
        public static Vector3 GetPointByRange(float range)
        {
            var from = Program.Me.Position.To2D();
            var to = Game.CursorPos.To2D();
            var direction = (to - from).Normalized();
            return (from + range * direction).To3D();
        }

        public static Vector2? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            return GetFirstWallPoint(from.To2D(), to.To2D(), step);
        }

        public static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    return from + (d - step) * direction;
                }
            }
            return null;
        }
    }
}
