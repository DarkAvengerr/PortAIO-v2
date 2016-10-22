using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Media
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Objects;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using SharpDX;

    using Geometry = LeagueSharp.Common.Geometry;
    using Point = global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes.Point;

    #endregion

    /// <summary>
    ///     Class to generate grids
    /// </summary>
    public class GridGenerator
    {
        #region Fields

        /// <summary>
        ///     The base point (where everything starts, most likely the Player Position)
        /// </summary>
        public Point BasePoint =
            new Point(GlobalVariables.Player.ServerPosition);

        /// <summary>
        ///     The grid
        /// </summary>
        public
            Grid
                <Point,
                    ConnectionBase<Point>> Grid;

        /// <summary>
        ///     The maximum connections
        /// </summary>
        private int maxConnections = 50000;

        /// <summary>
        ///     The PathBase deepness
        /// </summary>
        private int pathDeepness = 5;

        /// <summary>
        ///     The shared connections
        /// </summary>
        private List<ConnectionBase<Point>> sharedConnections =
            new List<ConnectionBase<Point>>();

        /// <summary>
        ///     The shared points
        /// </summary>
        private List<Point> sharedPoints =
            new List<Point>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GridGenerator" /> class.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <param name="endPosition">The end position.</param>
        internal GridGenerator(List<Obj_AI_Base> units, Vector3 endPosition)
        {
            this.Units = units;

            if (this.Units.Contains(GlobalVariables.Player))
            {
                this.Units.Remove(GlobalVariables.Player);
            }

            this.EndPosition = endPosition;
            this.EndPoint = new Point(this.EndPosition);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the end point.
        /// </summary>
        /// <value>
        ///     The end point.
        /// </value>
        public Point EndPoint { get; }

        /// <summary>
        ///     Gets or sets the end position.
        /// </summary>
        /// <value>
        ///     The end position.
        /// </value>
        public Vector3 EndPosition { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the maximum connections.
        /// </summary>
        /// <value>
        ///     The maximum connections.
        /// </value>
        internal int MaxConnections
        {
            get
            {
                return this.maxConnections;
            }
            set
            {
                if (value > 0)
                {
                    this.maxConnections = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the PathBase deepness.
        /// </summary>
        /// <value>
        ///     The PathBase deepness.
        /// </value>
        internal int PathDeepness
        {
            get
            {
                return this.pathDeepness;
            }
            set
            {
                if (value > 0)
                {
                    this.pathDeepness = value;
                }
            }
        }

        /// <summary>
        ///     Gets the units.
        /// </summary>
        /// <value>
        ///     The units.
        /// </value>
        internal List<Obj_AI_Base> Units { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Connects all points.
        /// </summary>
        public void ConnectAllPoints()
        {
            try
            {
                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(@"GridGenerator.Cs > ConnectAllPoints()");
                }

                // foreach dash
                foreach (var x in
                    this.Grid.Connections.Where(x => x is YasuoDashConnection).ToList())
                {
                    var path = GlobalVariables.Player.GetPath(x.End.Position, this.EndPosition);

                    var firstpoint = x.End;

                    for (var i = 0; i < path.Count() - 1; i++)
                    {
                        var start = new Point(path[i]);

                        if (i == 0)
                        {
                            start = firstpoint;
                        }

                        var end = new Point(path[i + 1]);

                        if (i == path.Count() - 2)
                        {
                            end = this.EndPoint;
                        }

                        var connection = new WalkConnection(start, end);

                        if (!this.Grid.Connections.Contains(connection))
                        {
                            this.Grid.Connections.Add(connection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[GridManager]: " + ex);
            }
        }

        // TODO: PRIORITY MEDIUM > Walk behind minion to dash over it
        /// <summary>
        ///     Creates a new unique grid for every unit in dash range
        /// </summary>
        public void Generate()
        {
            try
            {
                this.SoftReset();

                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(@"GridGenerator.Cs > Initialize()");
                }

                // Setting up the first points
                foreach (var unit in
                    this.Units.Where(
                        x => Geometry.Distance(x, this.BasePoint.Position) <= GlobalVariables.Spells[SpellSlot.E].Range)
                    )
                {
                    var pointToAdd = new Point(new Dash(GlobalVariables.Player.ServerPosition, unit).EndPosition);
                    this.sharedPoints.Add(pointToAdd);
                    this.sharedConnections.Add(new YasuoDashConnection(this.BasePoint, pointToAdd, unit));
                }

                // Connecting StartPoint to EndPoint
                var path2 = GlobalVariables.Player.GetPath(this.BasePoint.Position, this.EndPosition);

                if (path2 != null && path2.Length > 0)
                {
                    for (var i = 0; i < path2.Count() - 1; i++)
                    {
                        var start = new Point(path2[i]);

                        if (i == 0)
                        {
                            start = this.BasePoint;
                        }

                        var end = new Point(path2[i + 1]);

                        if (i == path2.Count() - 2)
                        {
                            end = this.EndPoint;
                        }

                        var connection = new WalkConnection(start, end);

                        if (!this.sharedConnections.Contains(connection))
                        {
                            this.sharedConnections.Add(connection);
                        }
                    }
                }
                else
                {
                    this.sharedConnections.Add(
                        new SimpleConnection<Point>(
                            this.BasePoint,
                            this.EndPoint,
                            this.BasePoint.Position.Distance(this.EndPoint.Position) / GlobalVariables.Player.MoveSpeed));
                }

                // Starts generating possible pathes
                for (var i = 0; i < this.PathDeepness; i++)
                {
                    if (!this.sharedPoints.Any())
                    {
                        break;
                    }

                    foreach (var point in this.sharedPoints.ToList())
                    {
                        var localBlacklist = this.Backtrace(point, this.MaxConnections);

                        var unitCount =
                            this.Units.Where(
                                unit =>
                                Geometry.Distance(unit, point.Position) <= GlobalVariables.Spells[SpellSlot.E].Range)
                                .Count(unit => !localBlacklist.Contains(unit));

                        // Remove point from list and continue because there are no valid dashes available around that point
                        if (unitCount == 0)
                        {
                            if (GlobalVariables.Debug)
                            {
                                Console.WriteLine(
                                    @"[GridGenerator] Removing Point because no dashes are available for that point anymore");
                            }

                            this.sharedPoints.Remove(point);
                            continue;
                        }

                        this.ProcessPoint(point, localBlacklist);
                        this.sharedPoints.Remove(point);
                    }
                }

                this.sharedPoints.Add(this.EndPoint);

                this.Grid = new Grid<Point, ConnectionBase<Point>>(
                    this.sharedConnections,
                    this.BasePoint,
                    this.EndPoint);
            }
            catch (Exception ex)
            {
                this.SoftReset();
                Console.WriteLine(ex);
            }
        }

        // TODO: REWORK (More efficient)
        /// <summary>
        ///     Searches for connections that do not belong to the grid anymore and removes them
        /// </summary>
        public void RemoveDisconnectedConnections()
        {
            if (GlobalVariables.Debug)
            {
                Console.WriteLine(@"GridGenerator.Cs > RemoveDisconnectedConnections()");
            }

            var toBeRemoved = new List<ConnectionBase<Point>>();

            var connections = this.Grid?.Connections;

            if (connections != null)
            {
                foreach (var connection in connections.ToList())
                {
                    var backtracedPath = this.Backtrace(connection.Start);

                    if (backtracedPath == null)
                    {
                        return;
                    }

                    var all = backtracedPath.All(x => x.Start != this.Grid.BasePoint);

                    if (all)
                    {
                        toBeRemoved.Add(connection);
                    }
                }

                foreach (var connection in toBeRemoved.ToList())
                {
                    connections.Remove(connection);
                }
            }

            //if (GlobalVariables.Debug)
            //{
            //    Console.WriteLine(@"GridGenerator.Cs > RemoveDisconnectedConnections()");
            //}

            //var toBeRemoved = new List<YasuoConnection>();

            //var connections = this.Grid?.Connections;

            //if (connections != null)
            //{
            //    foreach (var connection in connections.ToList())
            //    {
            //        connections.Remove(connection);

            //        if (connections.Any(x => x.end == connection.start))
            //        {
            //            continue;
            //        }

            //        if (toBeRemoved.Contains(connection))
            //        {
            //            continue;
            //        }

            //        toBeRemoved.Add(connection);
            //    }
            //}

            //var connectionsleft = true;

            //while (connectionsleft)
            //{
            //    foreach (var connection in toBeRemoved.ToList())
            //    {
            //        if (connections != null && connections.All(x => x.start != connection.end))
            //        {
            //            continue;
            //        }

            //        if (toBeRemoved.Contains(connection))
            //        {
            //            continue;
            //        }
            //        connections?.Remove(connection);
            //        toBeRemoved.Add(connection);
            //    }

            //    var count = 0;

            //    for (int index = 0; index < toBeRemoved.Count; index++)
            //    {
            //        var t = toBeRemoved[index];
            //        if (count == toBeRemoved.Count - 1 || index < toBeRemoved.Count - 1)
            //        {
            //            connectionsleft = false;
            //        }

            //        if (connections != null && connections.All(x => x.start != t.end))
            //        {
            //            count++;
            //        }
            //    }
            //}
        }

        // TODO PRIORITY: MEDIUM - LOW
        /// <summary>
        ///     Removes every PathBase that intersects with a skillshot
        /// </summary>
        public void RemovePathesThroughSkillshots(List<Skillshot> skillshots)
        {
            if (GlobalVariables.Debug)
            {
                Console.WriteLine($"GridGenerator.Cs > RemovePathesThroughSkillshots() > {skillshots.Count}");
            }

            if (this.Grid?.Connections == null || !this.Grid.Connections.Any() || this.Grid.Points == null)
            {
                return;
            }

            var skillshotDict = new Dictionary<Skillshot, Geometry.Polygon>();

            if (skillshots.Any())
            {
                foreach (var skillshot in skillshots)
                {
                    var polygon = new Geometry.Polygon();

                    switch (skillshot.SData.SpellType)
                    {
                        case LeagueSharp.Data.Enumerations.SpellType.SkillshotLine:
                            polygon = new Geometry.Polygon.Rectangle(
                                skillshot.StartPosition,
                                skillshot.EndPosition,
                                skillshot.SData.Radius);
                            break;
                        case LeagueSharp.Data.Enumerations.SpellType.SkillshotCircle:
                            polygon = new Geometry.Polygon.Circle(skillshot.EndPosition, skillshot.SData.Radius);
                            break;
                        case LeagueSharp.Data.Enumerations.SpellType.SkillshotArc:
                            polygon = new Geometry.Polygon.Sector(
                                skillshot.StartPosition,
                                skillshot.Direction,
                                skillshot.SData.Angle,
                                skillshot.SData.Radius);
                            break;
                    }

                    skillshotDict.Add(skillshot, polygon);
                }
            }

            if (skillshotDict.Any())
            {
                foreach (var skillshot in skillshotDict)
                {
                    //foreach (var point in Grid.Points.ToList())
                    //{
                    //    if (skillshot.Value.IsInside(point.Position))
                    //    {
                    //        Grid?.Points?.Remove(point);
                    //    }   
                    //}

                    foreach (var connection in
                        this.Grid.Connections.Where(x => x is YasuoDashConnection))
                    {
                        var clipperpath = skillshot.Value.ToClipperPath();
                        var connectionpolygon = new Geometry.Polygon.Line(
                            connection.Start.Position,
                            connection.End.Position);
                        var connectionclipperpath = connectionpolygon.ToClipperPath();

                        if (clipperpath.Intersect(connectionclipperpath).Any())
                        {
                            Console.WriteLine(@"Removing YasuoConnection");
                            this.Grid?.Connections?.Remove(connection);
                        }
                    }
                }
            }

            this.RemoveDisconnectedConnections();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns all units from "from" to the "BasePoint"
        ///     Usefull for building Blacklists.
        /// </summary>
        /// <param name="from">the starting point</param>
        /// <param name="limit">limit of backtrace amount</param>
        /// <returns></returns>
        private List<Obj_AI_Base> Backtrace(Point from, int limit)
        {
            try
            {
                var result = new List<Obj_AI_Base>();

                // Thresholder
                var previousPoint = from;

                // limiter is used as a fallback method to prevent game crashes if there is an error somewhere.
                var limiter = 0;

                while (true)
                {
                    limiter++;

                    if (this.sharedConnections.Count == 0)
                    {
                        break;
                    }

                    // reached starting/base point
                    if (previousPoint == this.BasePoint)
                    {
                        #region Debug

                        if (GlobalVariables.Debug)
                        {
                            Console.WriteLine(@"[BT] FINISHED: Reached FeatureBase Point");
                        }

                        #endregion

                        break;
                    }

                    var dashConnection = this.sharedConnections.OfType<YasuoDashConnection>();

                    foreach (var x in dashConnection)
                    {
                        if (previousPoint.Position != x.End.Position) continue;

                        //Console.WriteLine(@"[BT] Adding result");
                        result.Add(x.Unit);
                        previousPoint = x.Start;
                    }

                    if (limiter != limit) continue;

                    #region Debug

                    if (GlobalVariables.Debug)
                    {
                        Console.WriteLine(@"[BT] FINISHED: Reached Limit");
                    }

                    #endregion

                    break;
                }

                return result;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        /// <summary>
        ///     Returns all units from "from" to the "BasePoint"
        /// </summary>
        /// <param name="from">the starting point</param>
        /// <returns></returns>
        private List<ConnectionBase<Point>> Backtrace(Point from)
        {
            try
            {
                var result = new List<ConnectionBase<Point>>();

                // Thresholder
                var previousPoint = from;

                for (var i = 0; i < this.Grid.Connections.Count; i++)
                {
                    if (previousPoint.Position == this.Grid.Connections[i].End.Position)
                    {
                        //Console.WriteLine(@"[BT] Adding result");
                        result.Add(this.Grid.Connections[i]);
                        previousPoint = this.Grid.Connections[i].Start;
                    }

                    if (1 == this.Grid.Connections.Count - 1)
                    {
                        return result;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        /// <summary>
        ///     Searches for possible dashes around "point".
        ///     Ignores all units from the blacklist.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="blacklist"></param>
        private void ProcessPoint(Point point, List<Obj_AI_Base> blacklist)
        {
            foreach (var unit in
                this.Units.Where(
                    unit => Geometry.Distance(unit, point.Position) <= GlobalVariables.Spells[SpellSlot.E].Range))
            {
                // Blacklist Check
                if (blacklist.Count > 0 && blacklist.Contains(unit))
                {
                    continue;
                }

                // Overriding Endpoint. YasuoConnection class does not contain any wallcheck. Dash class does.
                var tempConnection = new YasuoDashConnection(
                    point,
                    new Point(point.Position.Extend(unit.ServerPosition, GlobalVariables.Spells[SpellSlot.E].Range)),
                    unit);

                this.sharedConnections.Add(tempConnection);
            }
        }

        /*
        /// <summary>
        ///     Resets all objects to default.
        /// </summary>
        private void Reset()
        {
            this.sharedConnections = new List<ConnectionBase<Djikstra.Point>>();
            this.sharedPoints = new List<Djikstra.Point>();

            this.Units = new List<Obj_AI_Base>();
            this.Grid = null;

            if (GlobalVariables.Debug)
            {
                Console.WriteLine(@"[Reset] Reseted");
            }
        }
*/

        /// <summary>
        ///     Resets everything to default but Units and Grids.
        /// </summary>
        private void SoftReset()
        {
            this.sharedConnections = new List<ConnectionBase<Point>>();
            this.sharedPoints = new List<Point>();

            if (GlobalVariables.Debug)
            {
                Console.WriteLine(@"[SoftReset] Reseted");
            }
        }

        #endregion
    }
}