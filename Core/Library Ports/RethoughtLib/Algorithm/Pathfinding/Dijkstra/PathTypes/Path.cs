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
//     Last Edited: 04.10.2016 1:43 PM

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
            if (this.Connections.Contains(connection)) this.Connections.Remove(connection);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sets the Path time.
        /// </summary>
        protected virtual void SetPathCost()
        {
            foreach (var connection in this.Connections) this.PathCost += connection.Cost;
        }

        #endregion

        #region Constructors

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
    }
}