using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
    #region

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    internal class FleeLogic
    {
        #region Public Methods and Operators

        public static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end, int step = 1)
        {
            if (!start.IsValid() || !end.IsValid())
            {
                return Vector3.Zero;
            }

            var distance = start.Distance(end);

            for (var i = 0; i < distance; i = i + step)
            {
                var newPoint = start.Extend(end, i);

                if (NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall || newPoint.IsWall())
                {
                    return newPoint;
                }
            }

            return Vector3.Zero;
        }

        public static float GetWallWidth(Vector3 start, Vector3 direction, int maxWallWidth = 350, int step = 1)
        {
            var thickness = 0f;

            if (start.IsValid() && direction.IsValid())
            {
                for (var i = 0; i < maxWallWidth; i = i + step)
                {
                    if (NavMesh.GetCollisionFlags(start.Extend(direction, i)) == CollisionFlags.Wall
                        || start.Extend(direction, i).IsWall())
                    {
                        // Console.WriteLine("Thickness: " + thickness);
                        thickness += step;
                    }
                    else
                    {
                        return thickness;
                    }
                }
            }
            return thickness;
        }

        public static bool IsWallDash(Obj_AI_Base unit, float dashRange, float minWallWidth = 100)
        {
            return IsWallDash(unit.ServerPosition, dashRange, minWallWidth);
        }

        public static bool IsWallDash(Vector3 position, float dashRange, float minWallWidth = 100)
        {
            var dashEndPos = ObjectManager.Player.Position.Extend(position, dashRange);

            var firstWallPoint = GetFirstWallPoint(ObjectManager.Player.Position, dashEndPos);

            if (firstWallPoint.Equals(Vector3.Zero))
            {
                // No Wall
                return false;
            }

            if (dashEndPos.IsWall())
            {
                // End Position is in Wall
                var wallWidth = GetWallWidth(firstWallPoint, dashEndPos);

                if (wallWidth > minWallWidth && wallWidth < dashRange)
                {
                    return true;
                }
            }
            else
            {
                // End Position is not a Wall
                return true;
            }

            return false;
        }

        #endregion
    }
}