using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes
{
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    public class Point : PointBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Point(Vector3 position)
        {
            if (position.IsValid())
            {
                this.Position = position;
            }
        }

        /// <summary>
        ///     Empty Constructor
        /// </summary>
        public Point()
        { }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Position of the point
        /// </summary>
        public sealed override Vector3 Position { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws a circle around the point
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="width">Width of the circle</param>
        /// <param name="color">Color of the circle</param>
        public void Draw(int radius, int width, Color color)
        {
            Render.Circle.DrawCircle(this.Position, radius, color, width);
        }

        #endregion
    }
}