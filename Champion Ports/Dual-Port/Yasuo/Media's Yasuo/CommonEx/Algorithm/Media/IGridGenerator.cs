using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Media
{
    #region Using Directives

    using System.Collections.Generic;

    using Djikstra;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    using LeagueSharp;

    using SharpDX;

    #endregion

    internal interface IGridGenerator<T, TV> where TV : ConnectionBase<T> where T : PointBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// start.
        /// </value>
        Vector3 From { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>
        /// end.
        /// </value>
        Vector3 To { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Generates this instance.
        /// </summary>
        /// <returns></returns>
        Grid<T, ConnectionBase<T>> Generate();

        /// <summary>
        ///     Updates the specified settings.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <param name="from">start.</param>
        /// <param name="to">end.</param>
        void Update(List<Obj_AI_Base> units, Vector3 from, Vector3 to);

        #endregion
    }
}