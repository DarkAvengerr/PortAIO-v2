#region License

/* Copyright (c) LeagueSharp 2016
 * No reproduction is allowed in any way unless given written consent
 * from the LeagueSharp staff.
 * 
 * Author: imsosharp & Kortatu
 * Date: 2/21/2016
 * File: Geometry.cs
 */

#endregion License

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ClipperLib;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     Class that contains the geometry related methods.
    /// </summary>
    public static class Geometry
    {
        #region Constants

        private const int CircleLineSegmentN = 22;

        #endregion

        #region Public Methods and Operators

        public static Paths ClipPolygons(List<Polygon> polygons)
        {
            var subj = new Paths(polygons.Count);
            var clip = new Paths(polygons.Count);
            foreach (var polygon in polygons)
            {
                subj.Add(polygon.ToClipperPath());
                clip.Add(polygon.ToClipperPath());
            }

            var solution = new Paths();
            var c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftEvenOdd);
            return solution;
        }

        public static void DrawCircleOnMinimap(
            Vector3 center,
            float radius,
            Color color,
            int thickness = 1,
            int quality = 254)
        {
            var pointList = new List<Vector3>();
            for (var i = 0; i < quality; i++)
            {
                var angle = i * Math.PI * 2 / quality;
                pointList.Add(
                    new Vector3(
                        center.X + radius * (float)Math.Cos(angle),
                        center.Y + radius * (float)Math.Sin(angle),
                        center.Z));
            }
            for (var i = 0; i < pointList.Count; i++)
            {
                var a = pointList[i];
                var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];
                var aonScreen = Drawing.WorldToMinimap(a);
                var bonScreen = Drawing.WorldToMinimap(b);
                Drawing.DrawLine(aonScreen.X, aonScreen.Y, bonScreen.X, bonScreen.Y, thickness, color);
            }
        }

        public static bool IsOutside(this Vector3 point, Polygon poly)
        {
            var p = new IntPoint(point.X, point.Y);
            return Clipper.PointInPolygon(p, poly.ToClipperPath()) != 1;
        }

        public static bool IsOutside(this Vector2 point, Polygon poly)
        {
            var p = new IntPoint(point.X, point.Y);
            return Clipper.PointInPolygon(p, poly.ToClipperPath()) != 1;
        }

        /// <summary>
        ///     Returns the position on the path after t milliseconds at speed speed.
        /// </summary>
        public static Vector2 PositionAfter(this List<Vector2> self, int t, int speed, int delay = 0)
        {
            var distance = Math.Max(0, t - delay) * speed / 1000;
            for (var i = 0; i <= self.Count - 2; i++)
            {
                var from = self[i];
                var to = self[i + 1];
                var d = (int)to.Distance(from);
                if (d > distance)
                {
                    return from + distance * (to - from).Normalized();
                }

                distance -= d;
            }

            return self[self.Count - 1];
        }

        public static Vector3 SwitchYz(this Vector3 v)
        {
            return new Vector3(v.X, v.Z, v.Y);
        }

        /// <summary>
        ///     Converts a Vector3 to Vector2
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static Vector2 To2D(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Polygon ToPolygon(this Path v)
        {
            var polygon = new Polygon();
            foreach (var point in v)
            {
                polygon.Add(new Vector2(point.X, point.Y));
            }

            return polygon;
        }

        //Clipper
        public static List<Polygon> ToPolygons(this Paths v)
        {
            return v.Select(path => path.ToPolygon()).ToList();
        }

        #endregion

        public static class Util
        {
            #region Public Methods and Operators

            public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, Color color)
            {
                var from = Drawing.WorldToScreen(start);
                var to = Drawing.WorldToScreen(end);
                Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            }

            #endregion
        }

        public class Circle
        {
            #region Fields

            public Vector2 Center;

            public float Radius;

            #endregion

            #region Constructors and Destructors

            public Circle(Vector2 center, float radius)
            {
                this.Center = center;
                this.Radius = radius;
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                var outRadius = overrideWidth > 0
                                    ? overrideWidth
                                    : (offset + this.Radius) / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);
                for (var i = 1; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        this.Center.X + outRadius * (float)Math.Cos(angle),
                        this.Center.Y + outRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }

                return result;
            }

            #endregion
        }

        public class Polygon
        {
            #region Fields

            public List<Vector2> Points = new List<Vector2>();

            #endregion

            #region Public Methods and Operators

            public void Add(Vector2 point)
            {
                this.Points.Add(point);
            }

            public void Draw(Color color, int width = 1)
            {
                for (var i = 0; i <= this.Points.Count - 1; i++)
                {
                    var nextIndex = this.Points.Count - 1 == i ? 0 : i + 1;
                    Util.DrawLineInWorld(this.Points[i].ToVector3(), this.Points[nextIndex].ToVector3(), width, color);
                }
            }

            public bool IsOutside(Vector2 point)
            {
                var p = new IntPoint(point.X, point.Y);
                return Clipper.PointInPolygon(p, this.ToClipperPath()) != 1;
            }

            public Path ToClipperPath()
            {
                var result = new Path(this.Points.Count);
                result.AddRange(this.Points.Select(point => new IntPoint(point.X, point.Y)));

                return result;
            }

            #endregion
        }

        /// <summary>
        ///     Represents a rectangle polygon.
        /// </summary>
        public class Rectangle : Polygon
        {
            #region Fields

            /// <summary>
            ///     The end
            /// </summary>
            public Vector2 End;

            /// <summary>
            ///     The start
            /// </summary>
            public Vector2 Start;

            /// <summary>
            ///     The width
            /// </summary>
            public float Width;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Rectangle" /> class.
            /// </summary>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="width">The width.</param>
            public Rectangle(Vector3 start, Vector3 end, float width)
                : this(start.ToVector2(), end.ToVector2(), width)
            {
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Rectangle" /> class.
            /// </summary>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="width">The width.</param>
            public Rectangle(Vector2 start, Vector2 end, float width)
            {
                this.Start = start;
                this.End = end;
                this.Width = width;
                this.UpdatePolygon();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the direction.
            /// </summary>
            /// <value>
            ///     The direction.
            /// </value>
            public Vector2 Direction => (this.End - this.Start).Normalized();

            /// <summary>
            ///     Gets the perpendicular.
            /// </summary>
            /// <value>
            ///     The perpendicular.
            /// </value>
            public Vector2 Perpendicular => this.Direction.Perpendicular();

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Updates the polygon.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="overrideWidth">Width of the override.</param>
            public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
            {
                this.Points.Clear();
                this.Points.Add(
                    this.Start + (overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular
                    - offset * this.Direction);
                this.Points.Add(
                    this.Start - (overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular
                    - offset * this.Direction);
                this.Points.Add(
                    this.End - (overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular
                    + offset * this.Direction);
                this.Points.Add(
                    this.End + (overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular
                    + offset * this.Direction);
            }

            #endregion
        }

        public class Ring
        {
            #region Fields

            public Vector2 Center;

            public float Radius;

            public float RingRadius; //actually radius width.

            #endregion

            #region Constructors and Destructors

            public Ring(Vector2 center, float radius, float ringRadius)
            {
                this.Center = center;
                this.Radius = radius;
                this.RingRadius = ringRadius;
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();
                var outRadius = (offset + this.Radius + this.RingRadius)
                                / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);
                var innerRadius = this.Radius - this.RingRadius - offset;
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        this.Center.X - outRadius * (float)Math.Cos(angle),
                        this.Center.Y - outRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        this.Center.X + innerRadius * (float)Math.Cos(angle),
                        this.Center.Y - innerRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }

                return result;
            }

            #endregion
        }

        /// <summary>
        ///     Represnets a sector polygon.
        /// </summary>
        public class Sector : Polygon
        {
            #region Fields

            /// <summary>
            ///     The angle
            /// </summary>
            public float Angle;

            /// <summary>
            ///     The center
            /// </summary>
            public Vector2 Center;

            /// <summary>
            ///     The direction
            /// </summary>
            public Vector2 Direction;

            /// <summary>
            ///     The radius
            /// </summary>
            public float Radius;

            /// <summary>
            ///     The quality
            /// </summary>
            private readonly int quality;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Sector" /> class.
            /// </summary>
            /// <param name="center">The center.</param>
            /// <param name="direction">The direction.</param>
            /// <param name="angle">The angle.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="quality">The quality.</param>
            public Sector(Vector3 center, Vector3 direction, float angle, float radius, int quality = 20)
                : this(center.To2D(), direction.To2D(), angle, radius, quality)
            {
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Sector" /> class.
            /// </summary>
            /// <param name="center">The center.</param>
            /// <param name="direction">The direction.</param>
            /// <param name="angle">The angle.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="quality">The quality.</param>
            public Sector(Vector2 center, Vector2 direction, float angle, float radius, int quality = 20)
            {
                this.Center = center;
                this.Direction = (direction - center).Normalized();
                this.Angle = angle;
                this.Radius = radius;
                this.quality = quality;
                this.UpdatePolygon();
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Rotates Line by angle/radian
            /// </summary>
            /// <param name="point1"></param>
            /// <param name="point2"></param>
            /// <param name="value"></param>
            /// <param name="radian">True for radian values, false for degree</param>
            /// <returns></returns>
            public Vector2 RotateLineFromPoint(Vector2 point1, Vector2 point2, float value, bool radian = true)
            {
                var angle = !radian ? value * Math.PI / 180 : value;
                var line = Vector2.Subtract(point2, point1);
                var newline = new Vector2
                                  {
                                      X = (float)(line.X * Math.Cos(angle) - line.Y * Math.Sin(angle)),
                                      Y = (float)(line.X * Math.Sin(angle) + line.Y * Math.Cos(angle))
                                  };
                return Vector2.Add(newline, point1);
            }

            /// <summary>
            ///     Updates the polygon.
            /// </summary>
            /// <param name="offset">The offset.</param>
            public void UpdatePolygon(int offset = 0)
            {
                this.Points.Clear();
                var outRadius = (this.Radius + offset) / (float)Math.Cos(2 * Math.PI / this.quality);
                this.Points.Add(this.Center);
                var side1 = this.Direction.Rotated(-this.Angle * 0.5f);
                for (var i = 0; i <= this.quality; i++)
                {
                    var cDirection = side1.Rotated(i * this.Angle / this.quality).Normalized();
                    this.Points.Add(
                        new Vector2(this.Center.X + outRadius * cDirection.X, this.Center.Y + outRadius * cDirection.Y));
                }
            }

            #endregion
        }
    }
}