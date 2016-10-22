using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.OrbwalkingModes.Combo
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;
    using global::YasuoMedia.CommonEx.Classes;
    using global::YasuoMedia.CommonEx.Extensions;
    using global::YasuoMedia.CommonEx.Menu;
    using global::YasuoMedia.CommonEx.Menu.Presets;
    using global::YasuoMedia.CommonEx.Objects.Pathfinding;
    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Combo;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Dash = global::YasuoMedia.CommonEx.Objects.Dash;
    using Math = global::YasuoMedia.CommonEx.Utility.Math;

    #endregion

    internal class SweepingBlade : FeatureChild<Combo>
    {
        #region Static Fields

        /// <summary>
        ///     The PathBase copy
        /// </summary>
        internal static YasuoPath<Point, ConnectionBase<Point>> PathBaseCopy;

        #endregion

        #region Fields

        /// <summary>
        ///     The blacklist champions
        /// </summary>
        public List<Obj_AI_Base> BlacklistChampions = new List<Obj_AI_Base>();

        /// <summary>
        ///     The blacklist Menu
        /// </summary>
        public BlacklistMenu BlacklistMenu;

        /// <summary>
        ///     The PathBase
        /// </summary>
        internal YasuoPath<Point, ConnectionBase<Point>> PathBase;

        /// <summary>
        ///     The pathfinder
        /// </summary>
        internal PathfindingContainer<Point, ConnectionBase<Point>, YasuoPath<Point, ConnectionBase<Point>>> Pathfinder;

        /// <summary>
        ///     The targets
        /// </summary>
        protected List<AIHeroClient> Targets;

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
        public SweepingBlade(Combo parent)
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
        public override string Name => "(E) Sweeping Blade";

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

            var menuGenerator = new MenuGenerator(new SweepingBladeMenu(this.Menu));

            menuGenerator.Generate();

            this.BlacklistMenu = new BlacklistMenu(this.Menu, "Blacklist");

            this.Pathfinder =
                new PathfindingContainer<Point, ConnectionBase<Point>, YasuoPath<Point, ConnectionBase<Point>>>(
                    new AdvancedPathfinder(this.Menu));

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Executes on the specified unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        private static void Execute(Obj_AI_Base unit)
        {
            if (unit == null || !unit.IsValidTarget() || unit.HasBuff("YasuoDashWrapper"))
            {
                return;
            }

            GlobalVariables.CastManager.Queque.Enqueue(3, () => GlobalVariables.Spells[SpellSlot.E].CastOnUnit(unit));
        }

        /// <summary>
        ///     Gets the targets.
        /// </summary>
        private void GetTargets()
        {
            this.Targets =
                HeroManager.Enemies.Where(
                    x => x.Health > 0 && x.IsValid && x.Distance(GlobalVariables.Player.ServerPosition) <= 1000)
                    .ToList();
        }

        /// <summary>
        ///     Executes Logic to dash on champion
        /// </summary>
        private void LogicOnChampion()
        {
            var target = TargetSelector.SelectedTarget
                         ?? TargetSelector.GetTarget(
                             GlobalVariables.Spells[SpellSlot.E].Range,
                             TargetSelector.DamageType.Magical);

            if (target == null || this.BlacklistChampions.Contains(target)
                || !target.IsValidTarget(GlobalVariables.Spells[SpellSlot.E].Range))
            {
                return;
            }

            var dash = new Dash(GlobalVariables.Player.ServerPosition, target);

            if (target.Health < this.providerE.GetDamage(target) && !GlobalVariables.Spells[SpellSlot.Q].IsReady())
            {
                var meanvector =
                    Math.GetMeanVector3(
                        this.Targets.Where(x => x.Distance(dash.EndPosition) <= 1000)
                            .Select(x => x.ServerPosition)
                            .ToList());

                if (meanvector == target.ServerPosition)
                {
                    Execute(target);
                }

                if (GlobalVariables.Player.Health
                    > this.Menu.SubMenu(this.Name + "EOnChampionMenu")
                          .Item(this.Name + "MaxHealthDashOut")
                          .GetValue<Slider>()
                          .Value)
                {
                    if (dash.EndPosition.Distance(meanvector) <= GlobalVariables.Player.Distance(meanvector))
                    {
                        Execute(target);
                    }
                }
                else
                {
                    if (dash.EndPosition.Distance(meanvector) >= GlobalVariables.Player.Distance(meanvector))
                    {
                        Execute(target);
                    }
                }
            }

            if (!GlobalVariables.Player.HasQ3())
            {
                return;
            }

            // 1 v 1
            if (dash.EndPosition.CountEnemiesInRange(1000) == 1)
            {
                if (dash.HeroesHitCircular.Contains(target))
                {
                    Execute(target);
                }
            }
            else
            {
                var heroes = this.Targets.Where(x => x.Distance(dash.EndPosition) <= 1000);

                if (dash.HeroesHitCircular.Count >= heroes.Count() / 2)
                {
                    Execute(target);
                }
            }
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

            if (this.PathBase != null && this.PathBase.Connections.Count > 0)
            {
                var drawingMenu = this.Menu.SubMenu(this.Name + "Drawings");

                if (drawingMenu.Item(this.Name + "Enabled").GetValue<bool>())
                {
                    if (drawingMenu.Item(this.Name + "SmartDrawings").GetValue<bool>())
                    {
                        //if (!this.PathBase.BuildsUpShield && this.PathBase.Connections.All(x => !x.IsDash))
                        //{
                        //    return;
                        //}

                        //// TODO: Color PathBase light blue
                        //if (this.PathBase.BuildsUpShield)
                        //{
                        //}
                    }
                    if (drawingMenu.Item(this.Name + "PathDashColor").GetValue<Circle>().Active)
                    {
                        var linewidth = drawingMenu.Item(this.Name + "PathDashWidth").GetValue<Slider>().Value;
                        var color = drawingMenu.Item(this.Name + "PathDashColor").GetValue<Circle>().Color;

                        //this.PathBase.DashLineWidth = linewidth;
                        //this.PathBase.DashColor = color;
                    }

                    if (drawingMenu.Item(this.Name + "PathWalkColor").GetValue<Circle>().Active)
                    {
                        var linewidth = drawingMenu.Item(this.Name + "PathWalkWidth").GetValue<Slider>().Value;
                        var color = drawingMenu.Item(this.Name + "PathWalkColor").GetValue<Circle>().Color;

                        //this.PathBase.WalkLineWidth = linewidth;
                        //this.PathBase.WalkColor = color;
                    }

                    if (drawingMenu.Item(this.Name + "CirclesColor").GetValue<Circle>().Active)
                    {
                        var linewidth = drawingMenu.Item(this.Name + "CirclesLineWidth").GetValue<Slider>().Value;
                        var radius = drawingMenu.Item(this.Name + "CirclesRadius").GetValue<Slider>().Value;
                        var color = drawingMenu.Item(this.Name + "CirclesColor").GetValue<Circle>().Color;

                        //this.PathBase.CircleLineWidth = linewidth;
                        //this.PathBase.CircleRadius = radius;
                        //this.PathBase.CircleColor = color;
                    }

                    this.PathBase.Draw();
                }
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            this.SoftReset();

            if (GlobalVariables.Player.IsDead || GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !GlobalVariables.Spells[SpellSlot.E].IsReady())
            {
                return;
            }

            this.GetTargets();

            this.LogicOnChampion();

            this.Pathfinder.ExecutePath();

            this.PathBase = this.Pathfinder.GetPath();

            PathBaseCopy = this.PathBase;
        }

        /// <summary>
        ///     Resets the fields/properties
        /// </summary>
        private void SoftReset()
        {
            this.Targets = new List<AIHeroClient>();
            this.PathBase = null;

            PathBaseCopy = null;
        }

        #endregion
    }
}