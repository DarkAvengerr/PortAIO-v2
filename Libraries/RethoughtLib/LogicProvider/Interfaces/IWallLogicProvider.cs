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