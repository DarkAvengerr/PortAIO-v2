using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.Logic
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal sealed class GnarWallDetection
    {
        public Vector3 GetFirstWallPoint(Vector3 start, Vector3 end, float range)
        {
            if (!start.IsValid() || !end.IsValid() || start.Distance(end) > range)
            {
                return Vector3.Zero;
            }

            var newPoint = start.Extend(end, range);

            if (NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall || newPoint.IsWall())
            {
                return newPoint;
            }

            return Vector3.Zero;
        }

        public Vector3 Wall(Vector3 position, float range)
        {
            var firstWallPoint = GetFirstWallPoint(ObjectManager.Player.Position, position, range);

            return !firstWallPoint.IsWall() 
                ? Vector3.Zero
                : firstWallPoint;
        }
    }
}
