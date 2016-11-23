//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.LogicProvider.Modules
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.LogicProvider.Interfaces;

    using SharpDX;

    #endregion

    public class WallLogicProviderModule : ChildBase, IWallLogicProvider
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Wall Logic Provider";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the first wall point. Walks from start to end in steps and stops at the first wall point and goes stepOffset
        ///     steps backwards.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        /// <param name="stepOffset">The offset in steps</param>
        /// <returns></returns>
        public Vector3 GetFirstWallPoint(Vector3 start, Vector3 end, int step = 1, int stepOffset = 0)
        {
            if (start.IsValid() && end.IsValid())
            {
                var distance = start.Distance(end);

                for (var i = 0; i < distance; i = i + step)
                {
                    var newPoint = start.Extend(end, i - stepOffset * step);

                    if ((NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall) || newPoint.IsWall()) return newPoint;
                }
            }

            return Vector3.Zero;
        }

        /// <summary>
        ///     Gets the width of the wall.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        public float GetWallWidth(Vector3 start, Vector3 direction, int step = 1)
        {
            var thickness = 0f;

            if (!start.IsValid() || !direction.IsValid()) return thickness;

            for (var i = 0; i < this.Menu.Item("MaxWallWidth").GetValue<Slider>().Value; i = i + step)
                if ((NavMesh.GetCollisionFlags(start.Extend(direction, i)) == CollisionFlags.Wall)
                    || start.Extend(direction, i).IsWall()) thickness += step;
                else return thickness;

            return thickness;
        }

        /// <summary>
        ///     Determines whether dash is wall-jump over a specified unit.
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="unit">The unit.</param>
        /// <param name="dashRange">The dash range.</param>
        public bool IsWallDash(Vector3 start, Obj_AI_Base unit, float dashRange)
        {
            return this.IsWallDash(start, unit.ServerPosition, dashRange);
        }

        /// <summary>
        ///     Determines whether dash is wall-jump.
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="direction">The direction.</param>
        /// <param name="dashRange">The dash range.</param>
        public bool IsWallDash(Vector3 start, Vector3 direction, float dashRange)
        {
            var dashEndPos = start.Extend(direction, dashRange);
            var firstWallPoint = this.GetFirstWallPoint(start, dashEndPos);

            if (firstWallPoint.Equals(Vector3.Zero)) return false;

            if (dashEndPos.IsWall())
                // End Position is in Wall
            {
                var wallWidth = this.GetWallWidth(firstWallPoint, dashEndPos);

                if ((wallWidth > this.Menu.Item("MinWallWidth").GetValue<Slider>().Value)
                    && (wallWidth - firstWallPoint.Distance(dashEndPos) < wallWidth * 0.6f)) return true;
            }
            else
                // End Position is not a Wall
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns the Position after dash
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        /// <param name="stepOffset">The step offset.</param>
        /// <returns></returns>
        public Vector3 PositionAfterDash(Vector3 start, Vector3 end, int step = 1, int stepOffset = 0)
        {
            if (this.IsWallDash(start, end, start.Distance(end))) return end;

            return this.GetFirstWallPoint(start, end, step, stepOffset);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            this.Menu.AddItem(
                new MenuItem("MinWallWidth", "Min. Wall-width before a wall is recognized").SetValue(
                    new Slider(50, 0, 400)));

            this.Menu.AddItem(
                new MenuItem("MaxWallWidth", "Max. Wall-width after a wall is recognized").SetValue(
                    new Slider(500, 400, 1000)));
        }

        #endregion
    }
}