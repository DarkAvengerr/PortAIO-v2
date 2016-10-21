using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Graphs
{
    #region Using Directives

    using System.Collections.Generic;

    using RethoughtLib.Algorithm.Pathfinding;

    #endregion

    public class Graph<TNode, TEdge> where TNode : NodeBase where TEdge : EdgeBase<TNode>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Graph{TNode, TEdge}"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="edges">The edges.</param>
        public Graph(List<TNode> nodes, List<TEdge> edges)
        {
            this.Edges = edges;
            this.Nodes = nodes;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the edges.
        /// </summary>
        /// <value>
        /// The edges.
        /// </value>
        public List<TEdge> Edges { get; set; }

        /// <summary>
        /// Gets or sets the nodes.
        /// </summary>
        /// <value>
        /// The nodes.
        /// </value>
        public List<TNode> Nodes { get; set; }

        public TNode Start { get; set; }

        public TNode End { get; set; }

        #endregion
    }
}