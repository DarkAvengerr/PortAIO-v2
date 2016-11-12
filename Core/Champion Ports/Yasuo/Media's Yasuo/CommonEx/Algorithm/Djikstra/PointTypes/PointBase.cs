using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes
{
    using SharpDX;

    /// <summary>
    ///     Base class for Points
    /// </summary>
    public abstract class PointBase
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public abstract Vector3 Position { get; set; }
    }
}
