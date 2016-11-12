using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Objects
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class WindWall
    {
        #region Fields

        /// <summary>
        ///     The additional width
        /// </summary>
        public float AdditionalWidth;

        /// <summary>
        ///     The allies inside
        /// </summary>
        public List<Obj_AI_Base> AlliesInside;

        /// <summary>
        ///     The cast position
        /// </summary>
        public Vector2 CastPosition;

        /// <summary>
        ///     The enemies inside
        /// </summary>
        public List<Obj_AI_Base> EnemiesInside;

        /// <summary>
        ///     The range
        /// </summary>
        public float Range;

        //public SafeZoneLogicProvider Provider;

        /// <summary>
        ///     The start
        /// </summary>
        public Vector2 Start;

        /// <summary>
        ///     The polygon
        /// </summary>
        private Geometry.Polygon polygon;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindWall" /> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="range">The range.</param>
        /// <param name="additionalWidth">Width of the additional.</param>
        public WindWall(Vector2 start, float range, float additionalWidth)
        {
            this.Start = start;

            this.Range = range;

            this.AdditionalWidth = additionalWidth;

            this.Create();

            this.CheckAllies();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        public void Draw()
        {
            this.polygon.Draw(Color.Aqua, 2);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Checks the allies.
        /// </summary>
        private void CheckAllies()
        {
            foreach (var ally in HeroManager.Allies)
            {
                if (this.polygon.IsInside(ally.ServerPosition.To2D()))
                {
                    this.AlliesInside.Add(ally);
                }
            }
        }

        /// <summary>
        ///     Creates this instance.
        /// </summary>
        private void Create()
        {
            var wwCenter = GlobalVariables.Player.ServerPosition.Extend(this.Start.To3D(), 300);
            this.CastPosition = wwCenter.To2D();

            var wwPerpend = (wwCenter - GlobalVariables.Player.ServerPosition).Normalized();
            wwPerpend.X = -wwPerpend.X;

            var leftInnerBound = wwCenter.Extend(wwPerpend,
                (GlobalVariables.Spells[SpellSlot.W].Width / 2) + this.AdditionalWidth);
            var rightInnerBound = wwCenter.Extend(wwPerpend,
                -(GlobalVariables.Spells[SpellSlot.W].Width / 2) - this.AdditionalWidth);

            var leftOuterBound = this.Start.Extend(leftInnerBound.To2D(), this.Range);
            var rightOuterBound = this.Start.Extend(rightInnerBound.To2D(), this.Range);

            var safeZone = new Geometry.Polygon();
            safeZone.Add(leftInnerBound);
            safeZone.Add(rightInnerBound);
            safeZone.Add(leftOuterBound);
            safeZone.Add(rightOuterBound);
            safeZone.Add(new Geometry.Polygon.Arc(leftOuterBound, this.Start, 250 * (float)Math.PI / 180, this.Range));

            this.polygon = safeZone;
        }

        #endregion
    }
}