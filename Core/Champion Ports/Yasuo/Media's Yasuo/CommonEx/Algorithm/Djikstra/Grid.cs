using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    #endregion

    public class Grid<T, TV>
        where TV : ConnectionBase<T> where T : PointBase
    {
        #region Fields

        /// <summary>
        ///     Color of the grid (Drawings)
        /// </summary>
        public Color Color;

        /// <summary>
        ///     All Connections of the grid
        /// </summary>
        public List<TV> Connections = new List<TV>();

        /// <summary>
        ///     All Points inside the grid
        /// </summary>
        public List<T> Points = new List<T>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Grid(List<TV> connections, T startPoint, T endPoint)
        {
            try
            {
                if (connections != null)
                {
                    this.Connections = connections;
                }

                this.BasePoint = startPoint;

                this.EndPoint = endPoint;

                this.AddPoints(connections);

                this.Color = Color.White;

                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(
                        @"[GridInfo] Setting up new Grid. Total YasuoConnection Amount: " + this.Connections.Count);
                    Console.WriteLine(@"[GridInfo] Setting up new Grid. Total Point Amount: " + this.Points.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Exeption: " + ex);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Point where grid starts
        /// </summary>
        public T BasePoint { get; private set; }

        /// <summary>
        ///     Point where the grid ends (can be null)
        /// </summary>
        public T EndPoint { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws the Grid as lines in the world
        /// </summary>
        /// <param name="width"></param>
        public void Draw(int width = 1)
        {
            try
            {
                if (this.Connections == null || this.Connections.Count == 0)
                {
                    return;
                }

                foreach (var connection in this.Connections)
                {
                    //Console.WriteLine(@"Drawing Grid");
                    //connection.Draw(true, width, this.Color);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        ///     Searches for a connection between from and to
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public TV FindConnection(T point1, T point2)
        {
            return
                this.Connections.FirstOrDefault(
                    connection => connection.Start.Equals(point1) && connection.End.Equals(point2));
        }

        /// <summary>
        ///     Searches for all connections that are either start or end in the Point around
        /// </summary>
        /// <param name="around"></param>
        /// <returns></returns>
        public List<TV> FindConnections(T around)
        {
            return
                this.Connections.Where(connection => connection.End.Equals(around) || connection.Start.Equals(around))
                    .ToList();
        }

        #endregion

        #region Methods

        private void AddPoints(IEnumerable<TV> connections)
        {
            foreach (var connection in connections)
            {
                if (!this.Points.Contains(connection.End))
                {
                    this.Points.Add(connection.End);
                }

                if (!this.Points.Contains(connection.Start))
                {
                    this.Points.Add(connection.Start);
                }
            }
        }

        #endregion
    }
}