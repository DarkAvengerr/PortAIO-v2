using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ConnectionTypes;
    using PointTypes;

    #endregion

    /// <summary>
    ///     Generic Shortest Path Algorithm
    /// </summary>
    internal class Dijkstra<T, TV>
        where TV : ConnectionBase<T> where T : PointBase
    {
        #region Fields

        public List<T> Base = new List<T>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dijkstra{T, TV}" /> class.
        /// </summary>
        /// <param name="tlist">The list of nodes.</param>
        /// <param name="tvlist">The list of edges.</param>
        public Dijkstra(List<T> tlist, List<TV> tvlist)
        {
            try
            {
                this.Connections = tvlist;
                this.Base = tlist;

                foreach (var point in this.Base.Where(x => !this.Costs.ContainsKey(x)))
                {
                    this.Costs.Add(point, float.MaxValue);
                    this.Previous.Add(point, default(T));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[Error while using Djikstra Algorithm]: " + ex);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the connections.
        /// </summary>
        /// <value>
        ///     The connections.
        /// </value>
        public List<TV> Connections { get; set; } = new List<TV>();

        /// <summary>
        ///     Gets or sets the costs.
        /// </summary>
        /// <value>
        ///     The costs.
        /// </value>
        public Dictionary<T, float> Costs { get; set; } = new Dictionary<T, float>();

        /// <summary>
        ///     Gets or sets the previous point.
        /// </summary>
        /// <value>
        ///     The previous.
        /// </value>
        public Dictionary<T, T> Previous { get; set; } = new Dictionary<T, T>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the distance between 2 Points
        /// </summary>
        /// <param name="t1">Start</param>
        /// <param name="t2">End</param>
        /// <returns></returns>
        public float GetCostBetween(T t1, T t2)
        {
            return (from connection in this.Connections
                    where connection.Start.Equals(t1) && connection.End.Equals(t2)
                    select connection.Cost).FirstOrDefault();
        }

        /// <summary>
        ///     Gives all neighbors that are still in the base
        /// </summary>
        /// <param name="point">Points</param>
        /// <returns></returns>
        public List<T> GetNeighbors(T point)
        {
            var neighbors = new List<T>();

            foreach (var connection in this.Connections)
            {
                if (connection.Start.Equals(point) && this.Base.Contains(point)
                    && this.Base.Contains(connection.End))
                {
                    neighbors.Add(connection.End);
                }
            }

            return neighbors;
        }

        /// <summary>
        ///     Gets the Point with the shortest distance
        /// </summary>
        /// <returns></returns>
        public T GetPointSmallestDistance()
        {
            var speed = float.MaxValue;
            var smallest = default(T);

            foreach (var n in this.Base)
            {
                if (!(this.Costs[n] < speed)) continue;

                speed = this.Costs[n];
                smallest = n;
            }

            return smallest;
        }

        /// <summary>
        ///     Calculates the Path to the Point d (d = target)
        /// </summary>
        /// <param name="point">Targeted Point</param>
        /// <returns>point PathBase</returns>
        public List<T> GetPointsTo(T point)
        {
            try
            {
                var path = new List<T>();

                path.Insert(0, point);

                while (this.Previous.ContainsKey(point) && this.Previous[point] != null)
                {
                    point = this.Previous[point];
                    path.Insert(0, point);
                }

                return path;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        ///     Calculates the shortest distance from the Start Point to all other Points
        /// </summary>
        /// <param name="start">Startknoten</param>
        public void SetStart(T start)
        {
            Console.WriteLine("Searching for " + start.Position);

            foreach (var entry in this.Costs)
            {
                Console.WriteLine(entry.Key.Position);
            }

            if (this.Costs.ContainsKey(start))
            {
                Console.WriteLine(string.Format("Match found!"));
            }

            this.Costs[start] = 0;

            Console.WriteLine(string.Format("Start set to 0"));

            // while we have points to process
            while (this.Base.Count > 0)
            {
                var point = this.GetPointSmallestDistance();

                if (point == null)
                {
                    this.Base.Clear();
                }

                else
                {
                    foreach (var neighbour in this.GetNeighbors(point))
                    {
                        var tempCost = this.Costs[point] + this.GetCostBetween(point, neighbour);

                        Console.WriteLine(string.Format("Another Point found"));

                        if (tempCost > this.Costs[neighbour]) continue;

                        Console.WriteLine(string.Format("2"));

                        this.Costs[neighbour] = tempCost;
                        Console.WriteLine(string.Format("3"));
                        this.Previous[neighbour] = point;
                        Console.WriteLine(string.Format("4"));
                    }
                    this.Base.Remove(point);
                }
            }
        }

        #endregion
    }
}