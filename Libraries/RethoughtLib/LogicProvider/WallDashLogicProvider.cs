using EloBuddy; namespace RethoughtLib.LogicProvider
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     Logic Provider that contains walljump specific logics
    /// </summary>
    public class WallDashLogicProvider
    {
        #region Public Methods and Operators

        // BUG Navmesh seems broken
        /// <summary>
        ///     Gets the first wall point.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        public Vector3 GetFirstWallPoint(Vector3 start, Vector3 end, int step = 1)
        {
            if (start.LSIsValid() && end.LSIsValid())
            {
                var distance = start.LSDistance(end);
                for (var i = 0; i < distance; i = i + step)
                {
                    var newPoint = start.LSExtend(end, i);

                    if (NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall || newPoint.LSIsWall())
                    {
                        return newPoint;
                    }
                }
            }

            return Vector3.Zero;
        }

        /// <summary>
        ///     Gets the width of the wall.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="maxWallWidth">Maximum width of the wall.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        public float GetWallWidth(Vector3 start, Vector3 direction, int maxWallWidth = 1000, int step = 1)
        {
            var thickness = 0f;

            if (!start.LSIsValid() || !direction.LSIsValid())
            {
                return thickness;
            }

            for (var i = 0; i < maxWallWidth; i = i + step)
            {
                if (NavMesh.GetCollisionFlags(start.LSExtend(direction, i)) == CollisionFlags.Wall
                    || start.LSExtend(direction, i).LSIsWall())
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

        /// <summary>
        ///     Determines whether dash is walljump over a specified unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="dashRange">The dash range.</param>
        /// <param name="minWallWidth">Minimum width of the wall.</param>
        /// <returns></returns>
        public bool IsWallDash(Obj_AI_Base unit, float dashRange, float minWallWidth = 50)
        {
            return this.IsWallDash(unit.ServerPosition, dashRange, minWallWidth);
        }

        /// <summary>
        ///     Determines whether dash is walljump.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="dashRange">The dash range.</param>
        /// <param name="minWallWidth">Minimum width of the wall.</param>
        /// <returns></returns>
        public bool IsWallDash(Vector3 position, float dashRange, float minWallWidth = 50)
        {
            var dashEndPos = ObjectManager.Player.Position.LSExtend(position, dashRange);
            var firstWallPoint = this.GetFirstWallPoint(ObjectManager.Player.Position, dashEndPos);

            if (firstWallPoint.Equals(Vector3.Zero))
            {
                // No Wall
                return false;
            }
              
            if (dashEndPos.LSIsWall())
                // End Position is in Wall
            {
                var wallWidth = this.GetWallWidth(firstWallPoint, dashEndPos);

                if (wallWidth > minWallWidth && wallWidth - firstWallPoint.LSDistance(dashEndPos) < wallWidth * 0.6f)
                {
                    return true;
                }
            }
            else
            // End Position is not a Wall
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}