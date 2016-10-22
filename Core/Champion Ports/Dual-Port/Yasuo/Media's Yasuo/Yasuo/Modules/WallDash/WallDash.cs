using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Modules.WallDash
{
    using System;
    using System.Linq;

    using CommonEx;
    using CommonEx.Classes;

    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Dash = CommonEx.Objects.Dash;

    internal class WallDash : FeatureChild<Modules>
    {
        #region Fields

        /// <summary>
        ///     The provider e
        /// </summary>
        public SweepingBladeLogicProvider ProviderE;

        /// <summary>
        ///     The provider wall dash
        /// </summary>
        public WallDashLogicProvider ProviderWallDash;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WallDash" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public WallDash(Modules parent)
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
        public override string Name => "Wall Dash";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnDraw(EventArgs args)
        {
            if (GlobalVariables.Player.IsDead || GlobalVariables.Player.IsDashing()
                || !this.Menu.Item(this.Name + "Keybind").GetValue<KeyBind>().Active
                || !GlobalVariables.Debug)
            {
                return;
            }

            var units = this.ProviderE.GetUnits(GlobalVariables.Player.ServerPosition);

            if (units != null && units.Any())
            {
                var dashes = units.Select(unit => new Dash(unit)).ToList();

                foreach (var dash in dashes)
                {
                    dash.Draw();
                }
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            var units = this.ProviderE.GetUnits(GlobalVariables.Player.ServerPosition);

            if (this.Menu.Item(this.Name + "Keybind").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                foreach (
                    var unit in
                        units.Where(
                            unit => this.ProviderWallDash.IsWallDash(unit.ServerPosition, GlobalVariables.Spells[SpellSlot.E].Range))
                    )
                {
                    if (!this.Menu.Item(this.Name + "MouseCheck").GetValue<bool>())
                    {
                        Execute(unit);
                    }
                    // Summary: if Cursor position is near dash end position, dash. That is to prevent dashes over walls that were not intended.
                    else if (
                        GlobalVariables.Player.ServerPosition.Extend(
                            unit.ServerPosition,
                            GlobalVariables.Spells[SpellSlot.E].Range).Distance(Game.CursorPos)
                        < GlobalVariables.Spells[SpellSlot.E].Range)
                    {
                        Execute(unit);
                    }
                }
            }
        }

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
            this.ProviderE = new SweepingBladeLogicProvider(475);
            this.ProviderWallDash = new WallDashLogicProvider();
            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Name + "Keybind", "Keybind").SetValue(new KeyBind('G', KeyBindType.Press)));

            this.Menu.AddItem(new MenuItem(this.Name + "MouseCheck", "Check for mouse position").SetValue(false));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinWallWidth", "Minimum wall width: ").SetValue(
                    new Slider(150, 10, (int)GlobalVariables.Spells[SpellSlot.E].Range / 2)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "Helper", "How it works").SetTooltip(
                    "Hold down the Keybind to let the assembly perform a Dash over a unit that will be a WallDash"));

            //var advanced = new Menu("Advanced Settings", this.Name + "Advanced");

            //advanced.AddItem(
            //    new MenuItem(this.Name + "WidthReduction", "WallWidth %").SetValue(new Slider(100, 0, 200)));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Executes the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void Execute(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                GlobalVariables.Spells[SpellSlot.E].CastOnUnit(target);
            }
        }

        #endregion
    }
}