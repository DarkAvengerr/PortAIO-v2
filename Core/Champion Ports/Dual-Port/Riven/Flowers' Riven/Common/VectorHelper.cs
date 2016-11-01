using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Riven.Common
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public static class VectorHelper
    {
        public static Vector3 GetFirstWallPoint(Vector3 end, int step = 1, int stepOffset = 0)
        {
            if (ObjectManager.Player.ServerPosition.IsValid() && end.IsValid())
            {
                var distance = ObjectManager.Player.ServerPosition.Distance(end);

                for (var i = 0; i < distance; i = i + step)
                {
                    var newPoint = ObjectManager.Player.ServerPosition.Extend(end, i - stepOffset * step);

                    if ((NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall) || newPoint.IsWall())
                    {
                        return newPoint;
                    }
                }
            }

            return Vector3.Zero;
        }

        public static float GetWallWidth(Vector3 start, Vector3 direction, int step = 1)
        {
            var thickness = 0f;

            if (!start.IsValid() || !direction.IsValid())
            {
                return thickness;
            }

            for (var i = 0; i < 350; i = i + step)
            {
                if ((NavMesh.GetCollisionFlags(start.Extend(direction, i)) == CollisionFlags.Wall)
                    || start.Extend(direction, i).IsWall())
                {
                    thickness += step;
                }
                else
                {
                    return thickness;
                }
            }

            return thickness;
        }

        public static bool IsWallDash(Vector3 start)
        {
            var dashEndPos = start.Extend(Game.CursorPos, 350);
            var firstWallPoint = GetFirstWallPoint(dashEndPos);

            if (firstWallPoint.Equals(Vector3.Zero))
            {
                return false;
            }

            if (dashEndPos.IsWall())
            {
                var wallWidth = GetWallWidth(firstWallPoint, dashEndPos);

                if ((wallWidth > 350) && (wallWidth - firstWallPoint.Distance(dashEndPos) < wallWidth*0.6f))
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
    }
}
