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
namespace RethoughtLib.Algorithm.Graphs
{
    #region Using Directives

    using RethoughtLib.Algorithm.Pathfinding;

    #endregion

    public interface IGraphGenerator<TNode, TEdge>
        where TEdge : EdgeBase<TNode> where TNode : NodeBase
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Generates the specified start to end graph
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        Graph<TNode, TEdge> Generate(TNode start, TNode end);

        #endregion
    }
}