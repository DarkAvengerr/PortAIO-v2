using System.Collections.Generic;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Utility.Positioning
{
    class SOLOPolygon
    {
        /// <summary>
        /// The points of the polygon
        /// </summary>
        public List<Vector2> Points;

        /// <summary>
        /// Initializes a new instance of the <see cref="SOLOPolygon"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        public SOLOPolygon(List<Vector2> p)
        {
            Points = p;
        }

        /// <summary>
        /// Adds the specified vector to the polygon.
        /// </summary>
        /// <param name="vec">The vector.</param>
        public void Add(Vector2 vec)
        {
            Points.Add(vec);
        }

        /// <summary>
        /// Counts the points in this instance.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return Points.Count;
        }

        /// <summary>
        /// Determines whether the polygon contains the point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            var result = false;
            var j = Count() - 1;
            for (var i = 0; i < Count(); i++)
            {
                if (Points[i].Y < point.Y && Points[j].Y >= point.Y || Points[j].Y < point.Y && Points[i].Y >= point.Y)
                {
                    if (Points[i].X +
                        (point.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) * (Points[j].X - Points[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Creates a rectangle with a given start vector.
        /// </summary>
        /// <param name="startVector2">The start vector2.</param>
        /// <param name="endVector2">The end vector2.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public static List<Vector2> Rectangle(Vector2 startVector2, Vector2 endVector2, float radius)
        {
            var points = new List<Vector2>();

            var v1 = endVector2 - startVector2;
            var to1Side = Vector2.Normalize(v1).Perpendicular() * radius;

            points.Add(startVector2 + to1Side);
            points.Add(startVector2 - to1Side);
            points.Add(endVector2 - to1Side);
            points.Add(endVector2 + to1Side);
            return points;
        }
    }
}
