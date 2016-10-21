using System;
using System.Collections.Generic;
using System.Linq;
using GeassLib.ThirdParty;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Drawing.Render
{
    public class Polygon
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the list of all points in the polygon.
        /// </summary>
        public List<Vector2> Points { get; set; } = new List<Vector2>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds a Vector2 to the points
        /// </summary>
        /// <param name="point">The Point</param>
        public void Add(Vector2 point)
        {
            this.Points.Add(point);
        }

        /// <summary>
        ///     Converts Vector3 to 2D, then adds it to the points.
        /// </summary>
        /// <param name="point">The Point</param>
        public void Add(Vector3 point)
        {
            this.Points.Add(point.To2D());
        }

        /// <summary>
        ///     Adds all of the points in the polygon to this instance.
        /// </summary>
        /// <param name="polygon">The Polygon</param>
        public void Add(Polygon polygon)
        {
            foreach (var point in polygon.Points)
            {
                this.Points.Add(point);
            }
        }

        /// <summary>
        ///     Draws all of the points in the polygon connected.
        /// </summary>
        /// <param name="color">The Color</param>
        /// <param name="width">Width of lines</param>
        public virtual void Draw(Color color, int width = 1)
        {
            for (var i = 0; i <= this.Points.Count - 1; i++)
            {
                var nextIndex = (this.Points.Count - 1 == i) ? 0 : (i + 1);
                var from = EloBuddy.Drawing.WorldToScreen(this.Points[i].To3D());
                var to = EloBuddy.Drawing.WorldToScreen(this.Points[nextIndex].To3D());

                EloBuddy.Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            }
        }

        /// <summary>
        ///     Gets if the Vector2 is inside the polygon.
        /// </summary>
        /// <param name="point">The Point</param>
        /// <returns>Whether the Vector2 is inside the polygon</returns>
        public bool IsInside(Vector2 point)
        {
            return !this.IsOutside(point);
        }

        /// <summary>
        ///     Gets if the Vector3 is inside the polygon.
        /// </summary>
        /// <param name="point">The Point</param>
        /// <returns>Whether the Vector3 is inside the polygon</returns>
        public bool IsInside(Vector3 point)
        {
            return !this.IsOutside(point.To2D());
        }

        /// <summary>
        ///     Gets if the Objects Position is inside the polygon.
        /// </summary>
        /// <param name="gameObject">Game Object</param>
        /// <returns>Whether the <see cref="GameObject" />'s position is inside the polygon</returns>
        public bool IsInside(GameObject gameObject)
        {
            return !this.IsOutside(gameObject.Position.To2D());
        }

        /// <summary>
        ///     Gets if the position is outside of the polygon.
        /// </summary>
        /// <param name="point">The Point</param>
        /// <returns>Whether the Vector2 is inside the polygon</returns>
        public bool IsOutside(Vector2 point)
        {
            var p = new IntPoint(point.X, point.Y);
            return Clipper.PointInPolygon(p, this.ToClipperPath()) != 1;
        }

        /// <summary>
        ///     Converts all the points to the Clipper Library format
        /// </summary>
        /// <returns>List of <c>IntPoint</c>'s</returns>
        public List<IntPoint> ToClipperPath()
        {
            var result = new List<IntPoint>(this.Points.Count);
            result.AddRange(this.Points.Select(point => new IntPoint(point.X, point.Y)));
            return result;
        }

        #endregion
    }
}
