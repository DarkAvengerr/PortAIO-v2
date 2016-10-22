using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.Dijkstra.PathTypes
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    public abstract class Path<T, TV>
        where TV : EdgeBase<T>
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
        ///     Where the Path starts
        /// </summary>
        public T StartPosition;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Path{T, TV}" /> class.
        /// </summary>
        protected Path()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Path{T, TV}" /> class.
        /// </summary>
        /// <param name="connections">The connections.</param>
        protected Path(List<TV> connections)
        {
            this.Connections = connections;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The total time of executing the Path
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
        ///     Removes the connection.
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
        ///     Sets the Path time.
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