using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Library.WallExtension
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class WallExtension
    {
       /// <summary>
       /// Gets the first wall point/node, w/e. 
       /// </summary>
       /// <param name="playerPosition"></param>
       /// <param name="endPosition"></param>
       /// <param name="step"></param>
       /// <returns></returns>
        public Vector3 FirstWallPoint(Vector3 playerPosition, Vector3 endPosition, int step = 1)
        {
            if (!playerPosition.IsValid() || !endPosition.IsValid())
            {
                return Vector3.Zero;
            }

            var distance = playerPosition.Distance(endPosition);

            for (var i = 0; i < distance; i = i + step)
            {
                var newPoint = playerPosition.Extend(endPosition, i);

                if (NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall || newPoint.IsWall())
                {
                    return newPoint;
                }
            }

            return Vector3.Zero;
        }

        public bool IsWallDash(Vector3 position, float dashRange, float minWallWidth = 100)
        {
            var dashEndPos = ObjectManager.Player.Position.Extend(position, dashRange);

            var firstWallPoint = FirstWallPoint(ObjectManager.Player.Position, dashEndPos);

            if (firstWallPoint == Vector3.Zero)
            {
                return false;
            }

            if (!dashEndPos.IsWall())
            {
                return false;
            }

            var wallWidth = GetWallWidth(firstWallPoint, dashEndPos);

            return wallWidth > minWallWidth && wallWidth < dashRange;
        }

        private static float GetWallWidth(Vector3 start, Vector3 direction, int maxWallWidth = 350, int step = 1)
        {
            var thickness = 0f;

            if (!start.IsValid() || !direction.IsValid())
            {
                return thickness;
            }

            for (var i = 0; i < maxWallWidth; i = i + step)
            {
                if (NavMesh.GetCollisionFlags(start.Extend(direction, i)) == CollisionFlags.Wall || start.Extend(direction, i).IsWall())
                {
                    thickness += step;
                }
            }
            return thickness;
        }
    }
}
