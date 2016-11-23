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
 namespace RethoughtLib.LogicProvider.Interfaces
{
    #region Using Directives

    using LeagueSharp;

    using SharpDX;

    #endregion

    public interface IWallLogicProvider
    {
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
        Vector3 GetFirstWallPoint(Vector3 start, Vector3 end, int step = 1, int stepOffset = 0);

        /// <summary>
        ///     Gets the width of the wall.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        float GetWallWidth(Vector3 start, Vector3 direction, int step = 1);

        /// <summary>
        ///     Determines whether dash is wall-jump over a specified unit.
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="unit">The unit.</param>
        /// <param name="dashRange">The dash range.</param>
        bool IsWallDash(Vector3 start, Obj_AI_Base unit, float dashRange);

        /// <summary>
        ///     Determines whether dash is wall-jump.
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="direction">The direction.</param>
        /// <param name="dashRange">The dash range.</param>
        bool IsWallDash(Vector3 start, Vector3 direction, float dashRange);

        /// <summary>
        ///     Returns the Position after dash
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        /// <param name="stepOffset">The step offset.</param>
        /// <returns></returns>
        Vector3 PositionAfterDash(Vector3 start, Vector3 end, int step = 1, int stepOffset = 0);

        #endregion
    }
}