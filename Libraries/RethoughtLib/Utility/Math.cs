using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Utility
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    public static class Math
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Converts a circle to a list of vectors
        /// </summary>
        /// <param name="circleRadius">The circle radius.</param>
        /// <param name="segmentNumber">The segment number.</param>
        /// <returns></returns>
        public static List<Vector2> CircleToVector2Segments(double circleRadius, int segmentNumber)
        {
            var result = new List<Vector2>();

            for (var i = 1; i < segmentNumber; i++)
            {
                var angle = i * 2 * System.Math.PI / segmentNumber;

                result.Add(
                    new Vector2(
                        (float)(circleRadius * System.Math.Cos(angle)),
                        (float)(circleRadius * System.Math.Sin(angle))));
            }

            result.Remove(result.First());

            return result;
        }

        /// <summary>
        ///     Gets the mean distance.
        /// </summary>
        /// <param name="vectors">The vectors.</param>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public static float GetMeanDistance(List<Vector3> vectors, Vector3 vector)
        {
            var result = vectors.Sum(v => v.Distance(vector));

            return result / vectors.Count;
        }

        /// <summary>
        ///     Returns the center from a given list of units
        /// </summary>
        /// <param name="units"></param>
        /// <returns>Vector2</returns>
        public static Vector2 GetMeanVector2(List<Obj_AI_Base> units)
        {
            if (units.Count == 0)
            {
                return Vector2.Zero;
            }
            float x = 0, y = 0;

            foreach (var unit in units)
            {
                x += unit.ServerPosition.X;
                y += unit.ServerPosition.Y;
            }

            return new Vector2(x / units.Count, y / units.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of vectors
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns>Vector2</returns>
        public static Vector2 GetMeanVector2(List<Vector2> vectors)
        {
            if (vectors.Count == 0)
            {
                return Vector2.Zero;
            }

            float x = 0, y = 0;

            foreach (var vector in vectors)
            {
                x += vector.X;
                y += vector.Y;
            }

            return new Vector2(x / vectors.Count, y / vectors.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of units
        /// </summary>
        /// <param name="units"></param>
        /// <returns>Vector3</returns>
        public static Vector3 GetMeanVector3(List<Obj_AI_Base> units)
        {
            if (units.Count == 0)
            {
                return Vector3.Zero;
            }
            float x = 0, y = 0, z = 0;

            foreach (var unit in units)
            {
                x += unit.ServerPosition.X;
                y += unit.ServerPosition.Y;
                z += unit.ServerPosition.Z;
            }

            return new Vector3(x / units.Count, y / units.Count, z / units.Count);
        }

        /// <summary>
        ///     Returns the center from a given list of vectors
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns>Vector2</returns>
        public static Vector3 GetMeanVector3(List<Vector3> vectors)
        {
            if (vectors.Count == 0)
            {
                return Vector3.Zero;
            }

            float x = 0, y = 0, z = 0;

            foreach (var vector in vectors)
            {
                x += vector.X;
                y += vector.Y;
                z += vector.Z;
            }

            return new Vector3(x / vectors.Count, y / vectors.Count, z / vectors.Count);
        }

        /// <summary>
        ///     Gets the path length.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static float GetPathLenght(Vector3[] path)
        {
            var result = 0f;

            for (var i = 0; i < path.Count(); i++)
            {
                if (i + 1 != path.Count())
                {
                    result += path[i].Distance(path[i + 1]);
                }
            }
            return result;
        }

        public static IEnumerable<Vector2> MoveTo(this IEnumerable<Vector2> vectors, Vector2 direction)
        {
            return vectors.Select(vec => vec.Extend(direction, direction.Length()));
        }

        /// <summary>
        ///     Converts a list of vector2's to vector3's
        /// </summary>
        /// <param name="vectorList">The vector list.</param>
        /// <returns></returns>
        public static IEnumerable<Vector2> To2D(this IEnumerable<Vector3> vectorList)
        {
            return vectorList.Select(vector => vector.To2D());
        }

        /// <summary>
        ///     Converts a list of vector3's to vector2's
        /// </summary>
        /// <param name="vectorList">The vector list.</param>
        /// <returns></returns>
        public static IEnumerable<Vector3> To3D(this IEnumerable<Vector2> vectorList)
        {
            return vectorList.Select(vector => vector.To3D());
        }

        #endregion
    }
}