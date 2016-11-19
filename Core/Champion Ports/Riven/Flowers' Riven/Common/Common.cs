using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Riven.Common
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public static class Common
    {
        public static float DistanceToPlayer(this Obj_AI_Base source)
        {
            return ObjectManager.Player.Distance(source);
        }

        public static float DistanceToPlayer(this Vector3 position)
        {
            return position.To2D().DistanceToPlayer();
        }

        public static float DistanceToPlayer(this Vector2 position)
        {
            return ObjectManager.Player.Distance(position);
        }

        public static bool CanWallJump(Vector3 dashEndPos, float dashRange)
        {
            var firstWallPoint = GetFirstWallPoint(ObjectManager.Player.Position, dashEndPos);

            if (firstWallPoint.Equals(Vector3.Zero))
            {
                return false;
            }

            if (dashEndPos.IsWall())
            {
                var wallWidth = GetWallWidth(firstWallPoint, dashEndPos);

                if (wallWidth > 50 && wallWidth - firstWallPoint.Distance(dashEndPos) < wallWidth * 0.4f)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end)
        {
            if (start.IsValid() && end.IsValid())
            {
                var distance = start.Distance(end);

                for (var i = 0; i < distance; i = i + 1)
                {
                    var newPoint = start.Extend(end, i);

                    if (NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall || newPoint.IsWall())
                    {
                        return newPoint;
                    }
                }
            }

            return Vector3.Zero;
        }

        public static float GetWallWidth(Vector3 start, Vector3 direction)
        {
            var thickness = 0f;

            if (!start.IsValid() || !direction.IsValid())
            {
                return thickness;
            }

            for (var i = 0; i < 1000; i = i + 1)
            {
                if (NavMesh.GetCollisionFlags(start.Extend(direction, i)) == CollisionFlags.Wall ||
                    start.Extend(direction, i).IsWall())
                {
                    thickness += 1;
                }
                else
                {
                    return thickness;
                }
            }

            return thickness;
        }
    }
}
