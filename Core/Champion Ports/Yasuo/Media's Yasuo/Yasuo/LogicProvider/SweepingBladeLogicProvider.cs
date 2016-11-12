using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.LogicProvider
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Media;
    using global::YasuoMedia.CommonEx.Objects;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SebbyLib;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Point = global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes.Point;

    #endregion

    /// <summary>
    /// </summary>
    public class SweepingBladeLogicProvider
    {
        #region Fields

        /// <summary>
        ///     The maximum range of calculating
        /// </summary>
        public float CalculationRange;

        /// <summary>
        ///     The current connections
        /// </summary>
        public List<ConnectionBase<Point>> CurrentConnections = new List<ConnectionBase<Point>>();

        /// <summary>
        ///     The current points
        /// </summary>
        public List<Point> CurrentPoints =
            new List<Point>();

        /// <summary>
        ///     The grid generator
        /// </summary>
        public GridGenerator GridGenerator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SweepingBladeLogicProvider" /> class.
        /// </summary>
        /// <param name="calculationRange">The calculation range.</param>
        public SweepingBladeLogicProvider(float calculationRange = 10000)
        {
            this.CalculationRange = calculationRange;
            Drawing.OnDraw += this.DrawGrid;
        }

        #endregion

        #region Enums

        /// <summary>
        ///     Determines the direction
        /// </summary>
        public enum Direction
        {
            TwoDirectional,

            OneDirectional
        }

        /// <summary>
        ///     Determines the unit type
        /// </summary>
        public enum Units
        {
            All,

            Minions,

            Champions,

            Mobs
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Removes invalid and unused connections and closes all points to the EndPoint
        /// </summary>
        public void FinalizeGrid()
        {
            this.GridGenerator.RemoveDisconnectedConnections();
            this.GridGenerator.ConnectAllPoints();
        }

        /// <summary>
        ///     Generates a new Grid
        /// </summary>
        /// <param name="startPosition">from</param>
        /// <param name="endPosition">to</param>
        /// <param name="units">which type of units</param>
        public void GenerateGrid(Vector3 startPosition, Vector3 endPosition, Units units)
        {
            this.Reset();

            var unitsToProcess = new List<Obj_AI_Base>();

            switch (units)
            {
                case Units.All:
                    unitsToProcess = this.GetUnits(startPosition, true, true, true);
                    break;
                case Units.Minions:
                    unitsToProcess = this.GetUnits(startPosition, true, false, false);
                    break;
                case Units.Champions:
                    unitsToProcess = this.GetUnits(startPosition, false, true, false);
                    break;
                case Units.Mobs:
                    unitsToProcess = this.GetUnits(startPosition, false, false, true);
                    break;
            }

            if (GlobalVariables.Debug)
            {
                Console.WriteLine(@"[SweepingBladeLP] GenerateGrid > Generating Grid");
            }

            if (unitsToProcess.Count > 0)
            {
                this.GridGenerator = new GridGenerator(unitsToProcess, endPosition);
            }

            // Initialize Basic Grid
            this.GridGenerator?.Generate();
        }

        /// <summary>
        ///     Gets the current dash end position.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurrentDashEndPosition()
        {
            if (!GlobalVariables.Player.IsDashing())
            {
                return Vector3.Zero;
            }

            var startPosition = GlobalVariables.Player.GetDashInfo().StartPos;
            var endPosition = GlobalVariables.Player.GetDashInfo().EndPos;

            var dash = new CommonEx.Objects.Dash(startPosition.To3D(), endPosition.To3D());

            return dash.EndPosition;
        }

        // TODO: HIGH PRIORITY
        /// <summary>
        ///     Credits: Brian
        ///     Returns the correct amount of Damage on unit
        /// </summary>
        public double GetDamage(Obj_AI_Base unit)
        {
            return GlobalVariables.Player.CalcDamage(
                unit,
                Damage.DamageType.Magical,
                (50 + 20 * GlobalVariables.Spells[SpellSlot.E].Level)
                * (1 + Math.Max(0, GlobalVariables.Player.GetBuffCount("YasuoDashScalar") * 0.25))
                + 0.6 * GlobalVariables.Player.FlatMagicDamageMod);
        }

        /// <summary>
        ///     Returns a PathBase object that represents the shortest possible Path to a given location
        /// </summary>
        /// <param name="endPosition">The vector to dash to</param>
        /// <param name="minions">dash over minions</param>
        /// <param name="champions">dash over champions</param>
        /// <param name="aroundSkillshots">dash around skillshots</param>
        // ReSharper disable once FunctionComplexityOverflow
        public YasuoPath<Point, ConnectionBase<Point>> GetPath(
            Vector3 endPosition,
            bool minions = true,
            bool champions = true,
            bool aroundSkillshots = false)
        {
            try
            {
                #region Calculator (Djikstra Algorithm)

                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(@"[SweepingBladeLP] Getpath > Calculating Shortest PathBase");
                }

                if (this.GridGenerator.Grid == null || this.GridGenerator.Grid.Connections.Count == 0)
                {
                    if (GlobalVariables.Debug)
                    {
                        Console.WriteLine(@"[SweepingBladeLP] Getpath > Returned");
                    }

                    return null;
                }

                // Inputing the grid
                var calculator = new Dijkstra<Point, ConnectionBase<Point>>(this.GridGenerator.Grid.Points, this.GridGenerator.Grid.Connections);

                calculator.SetStart(this.GridGenerator.Grid.BasePoint);

                // Set end point and return result as PathBase
                var points = calculator.GetPointsTo(this.GridGenerator.Grid.EndPoint);
                this.CurrentPoints = points;

                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(@"[SweepingBladeLP] Getpath > Result Djikstra Algorithn: " + points.Count);
                }

                #endregion

                // Solution: Make Djikstra Algorithm Generic and return T (connection) instead of points
                #region Converter (Temp-Solution / Brosciene ftw)

                var connections = new List<ConnectionBase<Point>>();

                for (var i = 0; i < points.Count - 1; i++)
                {
                    var from = points[i];
                    var to = points[i + 1];

                    connections.Add(this.GridGenerator.Grid.FindConnection(from, to));
                }

                this.CurrentConnections = connections;

                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(
                        @"[SweepingBladeLP] GeneratePath > CurrentConnections.Count: " + this.CurrentConnections.Count);
                }

                #endregion

                if (this.CurrentConnections.Count > 0)
                {
                    return new YasuoPath<Point, ConnectionBase<Point>>(this.CurrentConnections);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[GeneratePath]: " + ex);
            }
            return null;
        }

        /// <summary>
        ///     Returns a list containing all units
        /// </summary>
        /// <param name="startPosition">start point (vector)</param>
        /// <param name="minions">bool</param>
        /// <param name="champions">bool</param>
        /// <param name="mobs"></param>
        /// <returns>List(Obj_Ai_Base)</returns>
        public List<Obj_AI_Base> GetUnits(
            Vector3 startPosition,
            bool minions = true,
            bool champions = true,
            bool mobs = true)
        {
            try
            {
                // Add all units
                var units = new List<Obj_AI_Base>();

                if (minions)
                {
                    units.AddRange(Cache.GetMinions(startPosition, this.CalculationRange, MinionTeam.NotAlly));
                }

                if (champions)
                {
                    units.AddRange(HeroManager.Enemies.Where(x => x.Distance(startPosition) <= this.CalculationRange));
                }

                foreach (var x in
                    units.Where(
                        x =>
                        !x.IsValid || x.HasBuff("YasuoDashWrapper") || x.IsDead || x.IsMe
                        || x.Distance(GlobalVariables.Player.ServerPosition) > this.CalculationRange).ToList())
                {
                    units.Remove(x);
                }

                return units;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        ///     Returns Sweeping Blade speed
        /// </summary>
        /// <returns></returns>
        public float Speed() => 1025 + (GlobalVariables.Player.MoveSpeed - 345);

        #endregion

        #region Methods

        /// <summary>
        ///     Draws the grid.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void DrawGrid(EventArgs args)
        {
            if (GlobalVariables.Debug)
            {
                this.GridGenerator?.Grid?.Draw();

                if (this.CurrentConnections != null)
                {
                    foreach (var connection in this.CurrentConnections.OfType<YasuoDashConnection>())
                    {
                        //if (connection.Unit != null)
                        //{
                        //    connection.Draw(true, 3, Color.Red);
                        //}

                        //if (connection.Unit == null)
                        //{
                        //    connection.Draw(true, 3, Color.Green);
                        //}
                    }
                }

                if (this.CurrentPoints != null)
                {
                    for (var i = 0; i < this.CurrentPoints.Count; i++)
                    {
                        var point = this.CurrentPoints[i];
                        Drawing.DrawText(
                            Drawing.WorldToScreen(point.Position).X,
                            Drawing.WorldToScreen(point.Position).Y,
                            Color.Red,
                            i.ToString());
                    }
                }

                var point2 = this.GridGenerator?.Grid?.BasePoint;

                point2?.Draw(60, 5, Color.Violet);
            }
        }

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        private void Reset()
        {
            try
            {
                if (this.GridGenerator == null)
                {
                    return;
                }

                this.GridGenerator.Grid = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}