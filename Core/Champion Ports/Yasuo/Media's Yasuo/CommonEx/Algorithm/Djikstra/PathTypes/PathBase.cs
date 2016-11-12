using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using ConnectionTypes;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    public abstract class PathBase<T, TV>
        where TV : ConnectionBase<T>
        where T : PointBase
    {
        #region Fields

        /// <summary>
        ///     All connections
        /// </summary>
        public List<TV> Connections = new List<TV>();

        /// <summary>
        ///     Where the Path ends
        /// </summary>
        public T EndPosition;

        /// <summary>
        ///     Polygon that represents the PathBase
        /// </summary>
        public Geometry.Polygon GeometryPath;

        /// <summary>
        ///     Where the Path starts
        /// </summary>
        public T StartPosition;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PathBase{T,TV}" /> class.
        /// </summary>
        protected PathBase()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PathBase{T,TV}" /> class.
        /// </summary>
        /// <param name="connections">The connections.</param>
        protected PathBase(List<TV> connections)
        {
            this.Connections = connections;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The total time of executing the PathBase
        /// </summary>
        public float PathCost { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws the path.
        /// </summary>
        /// <param name="multicolor">if set to <c>true</c> [multicolor].</param>
        public abstract void Draw(bool multicolor = true);

        /// <summary>
        /// Removes the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public virtual void RemoveConnection(TV connection)
        {
            if (this.Connections.Contains(connection))
            {
                this.Connections.Remove(connection);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sets the PathBase time.
        /// </summary>
        protected virtual void SetPathCost()
        {
            foreach (var connection in this.Connections)
            {
                this.PathCost += connection.Cost;
            }
        }

        #endregion
    }
}