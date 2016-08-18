using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Point = global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes.Point;

    #region Using Directives

    using ConnectionTypes;
    using PointTypes;

    #endregion

    #endregion

    public class YasuoPath<T, TV> : PathBase<T, TV>
        where TV : ConnectionBase<T> where T : PointBase
    {
        #region Constructors and Destructors

        public YasuoPath(List<TV> connections)
            : base(connections)
        {
            this.Units = new List<Obj_AI_Base>();

            var dashConnections = this.Connections.OfType<YasuoDashConnection>();

            foreach (var connection in dashConnections)
            {
                this.Units.Add(connection.Unit);
            }

            this.SetDashLength();
            this.SetDashTime();

            this.SetWalkLength();
            this.SetDashLength();
        }

        #endregion

        #region Public Properties

        // TODO: PRIORITY LOW
        /// <summary>
        ///     Bool that represents if the PathBase will build up the Yasuo Shield if it is finished
        /// </summary>
        public bool BuildsUpShield { get; set; }

        /// <summary>
        ///     The average danger value of the PathBase
        /// </summary>
        public int DangerValue { get; set; }

        /// <summary>
        ///     The total lenght of all dashes
        /// </summary>
        public float DashLenght { get; set; }

        /// <summary>
        ///     Time that is needed to execute all dashes
        /// </summary>
        public float DashTime { get; set; }

        /// <summary>
        ///     Gets the units.
        /// </summary>
        /// <value>
        ///     The units.
        /// </value>
        public List<Obj_AI_Base> Units { get; set; }

        /// <summary>
        ///     The total walk lenght
        /// </summary>
        public float WalkLenght { get; set; }

        /// <summary>
        ///     The total time of walking
        /// </summary>
        public float WalkTime { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Draw(bool multicolor = true)
        {
            if (multicolor)
            {
                var dashConnections = this.Connections.OfType<YasuoDashConnection>();

                foreach (var connection in this.Connections)
                {
                    if (connection.GetType() == typeof(YasuoDashConnection)) continue;

                    var start = Drawing.WorldToScreen(connection.Start.Position);
                    var end = Drawing.WorldToScreen(connection.End.Position);

                    Drawing.DrawLine(start, end, 2, Color.White);
                }

                foreach (var connection in dashConnections)
                {
                    var start = Drawing.WorldToScreen(connection.Start.Position);
                    var end = Drawing.WorldToScreen(connection.End.Position);

                    Drawing.DrawLine(start, end, 2, Color.DodgerBlue);
                }
            }
            else
            {
                foreach (var connection in this.Connections)
                {
                    var start = Drawing.WorldToScreen(connection.Start.Position);
                    var end = Drawing.WorldToScreen(connection.End.Position);

                    Drawing.DrawLine(start, end, 2, Color.DodgerBlue);
                }
            }
        }

        /// <summary>
        ///     Gets a position after time following the PathBase
        /// </summary>
        /// <param name="time"></param>
        /// <returns>A vector</returns>
        public Vector3 GetPosition(float time)
        {
            var result = new Vector3();

            var time2 = time;

            foreach (var connection in this.Connections)
            {
                if (time2 > 0)
                {
                    time2 -= connection.Cost;
                }

                if (time <= 0)
                {
                    if (connection is YasuoDashConnection)
                    {
                        return connection.End.Position;
                    }
                    else
                    {
                        var minusTime = time * -1;

                        return connection.Start.Position.Extend(
                            connection.End.Position,
                            GlobalVariables.Player.MoveSpeed * 1000 / minusTime);
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Removes the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public override void RemoveConnection(TV connection)
        {
            base.RemoveConnection(connection);

            var connectionCopy = connection as YasuoDashConnection;

            if (connectionCopy != null && this.Units.Contains(connectionCopy.Unit))
            {
                this.Units.Remove(connectionCopy.Unit);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sets the length of the dash.
        /// </summary>
        private void SetDashLength()
        {
            foreach (var connection in this.Connections.OfType<YasuoDashConnection>())
            {
                this.DashLenght += connection.Start.Position.Distance(connection.End.Position);
            }
        }

        /// <summary>
        ///     Sets the dash time.
        /// </summary>
        private void SetDashTime()
        {
            foreach (var connection in this.Connections.OfType<YasuoDashConnection>())
            {
                this.DashTime += connection.Cost;
            }
        }

        /// <summary>
        ///     Sets the length of the walk.
        /// </summary>
        private void SetWalkLength()
        {
            foreach (var connection in this.Connections.OfType<SimpleConnection<Point>>())
            {
                this.WalkLenght += connection.Start.Position.Distance(connection.End.Position);
            }
        }

        /// <summary>
        ///     Sets the walk time.
        /// </summary>
        private void SetWalkTime()
        {
            foreach (var connection in this.Connections.OfType<SimpleConnection<Point>>())
            {
                this.WalkTime += connection.Cost;
            }
        }

        #endregion
    }
}