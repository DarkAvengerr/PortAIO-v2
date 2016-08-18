using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Objects.Pathfinding
{
    #region Using Directives

    using System;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes;
    using global::YasuoMedia.CommonEx.Menu;
    using global::YasuoMedia.Yasuo.LogicProvider;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.Modules;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Point = global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes.Point;

    #endregion

    internal class AdvancedPathfinder :
        IPathfinder
            <Point,
            ConnectionBase<Point>, YasuoPath<Point, ConnectionBase<Point>>>
    {
        #region Fields

        /// <summary>
        ///     The Path
        /// </summary>
        public
            YasuoPath
                <Point,
                    ConnectionBase<Point>> PathBase;

        /// <summary>
        ///     The targeted vector
        /// </summary>
        public Vector3 TargetedVector;

        /// <summary>
        ///     The menu
        /// </summary>
        private readonly Menu menu;

        /// <summary>
        ///     The E logicprovider
        /// </summary>
        private SweepingBladeLogicProvider providerE;

        /// <summary>
        ///     The R logicprovider
        /// </summary>
        private TurretLogicProvider providerTurret;

        #endregion

        #region Constructors and Destructors

        public AdvancedPathfinder(Menu menu)
        {
            var menuGenerator = new MenuGenerator(new AdvancedPathfinderMenu(menu, "Advanced Pathfinder"));

            menuGenerator.Generate();

            this.menu = menu.SubMenu(menu.Name + "Advanced Pathfinder");
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Executes the PathBase.
        /// </summary>
        public void ExecutePath()
        {
            try
            {
                if (this.PathBase?.Connections == null || !this.PathBase.Connections.Any()) return;

                #region Dashing

                var connection = this.PathBase.Connections.FirstOrDefault();

                if (connection == null)
                {
                    return;
                }

                var connection2 = connection as YasuoDashConnection;

                if (connection2 != null)
                {
                    if (this.menu.Item(this.menu.Name + "AutoDashing").GetValue<bool>()
                        && GlobalVariables.Player.Distance(connection2.Unit.ServerPosition)
                        <= GlobalVariables.Spells[SpellSlot.E].Range)
                    {
                        if (connection2.End.Position.Distance(this.PathBase.EndPosition.Position)
                            > GlobalVariables.Player.Distance(this.PathBase.EndPosition.Position))
                        {
                            GlobalVariables.CastManager.Queque.Enqueue(
                                3,
                                () => GlobalVariables.Spells[SpellSlot.E].CastOnUnit(connection2.Unit));
                        }
                    }
                }

                #endregion

                #region Walking

                // Auto-Walking
                if (this.menu.Item(this.menu.Name + "AutoWalking").GetValue<bool>())
                {
                    if (GlobalVariables.Player.ServerPosition.Distance(connection.End.Position) <= 50)
                    {
                        this.PathBase.RemoveConnection(connection);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                this.Reset();
                Console.WriteLine(ex);
            }
        }

        public YasuoPath<Point, ConnectionBase<Point>> GeneratePath()
        {
            this.FindTargetedVector();

            this.PathBase = this.CalculatePath(this.TargetedVector);

            return this.PathBase;
        }

        public void Initialize()
        {
            this.providerE = new SweepingBladeLogicProvider();
            this.providerTurret = new TurretLogicProvider();
        }

        #endregion

        #region Methods

        private YasuoPath<Point, ConnectionBase<Point>> CalculatePath(Vector3 position)
        {
            if (position == Vector3.Zero || !position.IsValid())
            {
                return null;
            }

            this.providerE.GenerateGrid(
                GlobalVariables.Player.ServerPosition,
                position,
                SweepingBladeLogicProvider.Units.All);

            if (this.providerE.GridGenerator.Grid == null || !this.providerE.GridGenerator.Grid.Connections.Any())
            {
                return null;
            }

            // TODO: PRIORITY MEDIUM > Make some more settings for this, such as Danger Value of skillshot etc.
            //if (menu.Item(menu.Name + "PathAroundSkillShots").GetValue<bool>())
            //{
            //    var skillshotList = Tracker.DetectedSkillshots.Where(x => x.SData.DangerValue > 1).ToList();

            //    this.providerE.GridGenerator.RemovePathesThroughSkillshots(skillshotList);
            //}

            // TODO: PRIORITY MEDIUM > Make some more settings for this, such as minions under turret etc. Ref; TurretLP
            if (this.menu.Item(this.menu.Name + "DontDashUnderTurret").GetValue<bool>())
            {
                foreach (var connection in this.providerE.GridGenerator.Grid.Connections.OfType<YasuoDashConnection>().ToList())
                    { 
                    if (this.providerTurret.IsSafePosition(connection.End.Position)) continue;

                    this.providerE.GridGenerator.Grid.Connections.Remove(connection);
                    //this.providerE.GridGenerator.RemoveDisconnectedConnections();
                }
            }

            this.providerE.FinalizeGrid();

            return this.providerE.GetPath(position);
        }

        /// <summary>
        ///     Finds the targeted vector.
        /// </summary>
        private void FindTargetedVector()
        {
            Vector3 result;

            switch (this.menu.Item(this.menu.Name + "Mode").GetValue<StringList>().SelectedIndex)
            {
                // Mouse
                case 0:
                    result = Game.CursorPos;
                    break;

                // Enemy
                case 1:
                    var target = TargetSelector.GetTarget(5000, TargetSelector.DamageType.Physical, true);

                    if (target == null || !target.IsValid || target.IsZombie
                        || Game.CursorPos.Distance(target.Position)
                        > this.menu.Item(this.menu.Name + "MinCursorDistance").GetValue<Slider>().Value)
                    {
                        result = Game.CursorPos;
                        break;
                    }

                    if (!this.menu.Item(this.menu.Name + "Prediction").GetValue<bool>())
                    {
                        result = target.ServerPosition;
                        break;
                    }

                    if (this.menu.Item(this.menu.Name + "Prediction").GetValue<bool>()
                        && this.menu.Item(this.menu.Name + "PredictionEnhanced").GetValue<bool>())
                    {
                        var tempVec = target.ServerPosition;

                        var path = this.CalculatePath(tempVec);

                        if (path != null)
                        {
                            var time = path.PathCost;

                            result = Prediction.GetPrediction(target, time).CastPosition;
                            break;
                        }
                        result = target.ServerPosition;
                        break;
                    }

                    result =
                        Prediction.GetPrediction(
                            target,
                            GlobalVariables.Player.Distance(target.ServerPosition) / this.providerE.Speed())
                            .UnitPosition;

                    break;
                default:
                    result = Game.CursorPos;
                    break;
            }

            this.TargetedVector = result;
        }

        private void Reset()
        {
            this.PathBase = null;
        }

        #endregion
    }
}