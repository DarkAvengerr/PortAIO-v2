using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Modules.Flee
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;
    using global::YasuoMedia.CommonEx.Classes;
    using global::YasuoMedia.CommonEx.Objects.Pathfinding;
    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class SweepingBlade : FeatureChild<Modules>
    {
        #region Fields

        /// <summary>
        ///     The PathBase copy
        /// </summary>
        public PathBase<Point, ConnectionBase<Point>> PathBaseCopy;

        /// <summary>
        ///     The PathBase
        /// </summary>
        internal PathBase<Point, ConnectionBase<Point>> PathBase;

        /// <summary>
        ///     The targets
        /// </summary>
        protected List<AIHeroClient> Targets;

        /// <summary>
        ///     The pathfinder
        /// </summary>
        private PathfindingContainer<Point, ConnectionBase<Point>, PathBase<Point, ConnectionBase<Point>>> pathfinder;

        /// <summary>
        ///     The provider e
        /// </summary>
        private SweepingBladeLogicProvider providerE;

        /// <summary>
        ///     The provider turret
        /// </summary>
        private TurretLogicProvider providerTurret;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SweepingBlade" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SweepingBlade(Modules parent)
            : base(parent)
        {
            this.OnLoad();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name => "Dash end Mouse";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable()
        {
            Events.OnUpdate -= this.OnUpdate;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable()
        {
            Events.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            this.providerE = new SweepingBladeLogicProvider();
            this.providerTurret = new TurretLogicProvider();

            this.Targets = new List<AIHeroClient>();

            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.pathfinder = new PathfindingContainer<Point, ConnectionBase<Point>, PathBase<Point, ConnectionBase<Point>>>(new SimplePathfinder(this.Menu));

            this.Menu.AddItem(
                new MenuItem(this.Name + "Keybind", "Keybind").SetValue(new KeyBind('A', KeyBindType.Press)));

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Executes on the specified unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        private static void Execute(Obj_AI_Base unit)
        {
            if (unit == null || !unit.IsValidTarget())
            {
                return;
            }

            GlobalVariables.CastManager.ForceAction(() => GlobalVariables.Spells[SpellSlot.E].CastOnUnit(unit));
        }

        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnDraw(EventArgs args)
        {
            if (GlobalVariables.Player.IsDead || GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            //if (this.PathBase != null && this.PathBase.Connections.Count > 0)
            //{
            //    var drawingMenu = this.Menu.SubMenu(this.Name + "Drawings");

            //    if (drawingMenu.Item(this.Name + "Enabled").GetValue<bool>())
            //    {
            //        if (drawingMenu.Item(this.Name + "SmartDrawings").GetValue<bool>())
            //        {
            //            if (!this.PathBase.BuildsUpShield && this.PathBase.Connections.All(x => !x.IsDash))
            //            {
            //                return;
            //            }

            //            // TODO: Color PathBase light blue
            //            if (this.PathBase.BuildsUpShield)
            //            {
            //            }
            //        }
            //        if (drawingMenu.Item(this.Name + "PathDashColor").GetValue<Circle>().Active)
            //        {
            //            var linewidth = drawingMenu.Item(this.Name + "PathDashWidth").GetValue<Slider>().Value;
            //            var color = drawingMenu.Item(this.Name + "PathDashColor").GetValue<Circle>().Color;

            //            this.PathBase.DashLineWidth = linewidth;
            //            this.PathBase.DashColor = color;
            //        }

            //        if (drawingMenu.Item(this.Name + "PathWalkColor").GetValue<Circle>().Active)
            //        {
            //            var linewidth = drawingMenu.Item(this.Name + "PathWalkWidth").GetValue<Slider>().Value;
            //            var color = drawingMenu.Item(this.Name + "PathWalkColor").GetValue<Circle>().Color;

            //            this.PathBase.WalkLineWidth = linewidth;
            //            this.PathBase.WalkColor = color;
            //        }

            //        if (drawingMenu.Item(this.Name + "CirclesColor").GetValue<Circle>().Active)
            //        {
            //            var linewidth = drawingMenu.Item(this.Name + "CirclesLineWidth").GetValue<Slider>().Value;
            //            var radius = drawingMenu.Item(this.Name + "CirclesRadius").GetValue<Slider>().Value;
            //            var color = drawingMenu.Item(this.Name + "CirclesColor").GetValue<Circle>().Color;

            //            this.PathBase.CircleLineWidth = linewidth;
            //            this.PathBase.CircleRadius = radius;
            //            this.PathBase.CircleColor = color;
            //        }

            //        this.PathBase.Draw();
            //    }
            //}
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            this.SoftReset();

            if (GlobalVariables.Player.IsDead || !this.Menu.Item(this.Name + "Keybind").GetValue<KeyBind>().Active)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            this.PathBase = this.pathfinder.GetPath();

            this.pathfinder.ExecutePath();

            this.PathBaseCopy = this.PathBase;
        }

        /// <summary>
        ///     Resets the fields/properties
        /// </summary>
        private void SoftReset()
        {
            this.Targets = new List<AIHeroClient>();
            this.PathBase = null;

            this.PathBaseCopy = null;
        }

        #endregion
    }
}