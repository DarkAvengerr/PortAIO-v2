using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Objects.Pathfinding
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Menu.Presets;
    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    internal class Pathfinder
    {
        #region Fields

        /// <summary>
        ///     The path
        /// </summary>
        public Path Path;

        /// <summary>
        ///     The targeted vector
        /// </summary>
        public Vector3 TargetedVector;

        /// <summary>
        ///     The pathfinding menu
        /// </summary>
        internal PathfindingMenu PathfindingMenu;

        /// <summary>
        ///     The blacklist
        /// </summary>
        private readonly List<Obj_AI_Base> blacklist;

        /// <summary>
        ///     The menu
        /// </summary>
        private readonly Menu menu;

        /// <summary>
        ///     The E logicprovider
        /// </summary>
        private readonly SweepingBladeLogicProvider providerE;

        /// <summary>
        ///     The R logicprovider
        /// </summary>
        private readonly TurretLogicProvider providerTurret;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Pathfinder" /> class.
        /// </summary>
        /// <param name="pathfindingMenu">The pathfinding menu.</param>
        internal Pathfinder(PathfindingMenu pathfindingMenu)
        {
            this.PathfindingMenu = pathfindingMenu;

            this.menu = this.PathfindingMenu.Settings;

            this.blacklist = this.PathfindingMenu.BlacklistedHeroes;

            this.providerE = new SweepingBladeLogicProvider();

            this.providerTurret = new TurretLogicProvider();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the path.
        /// </summary>
        public void CalculatePath()
        {
            this.Path = null;

            if (!this.menu.Item(this.menu.Name + "Enabled").GetValue<bool>())
            {
                return;
            }

            this.FindTargetedVector();

            this.Path = this.CalculatePath(this.TargetedVector);
        }

        /// <summary>
        ///     Executes the path.
        /// </summary>
        public void ExecutePath()
        {
            if (this.Path?.Connections == null || !this.Path.Connections.Any()) return;

            #region Dashing

            var connection = this.Path.Connections.FirstOrDefault();

            if (connection == null)
            {
                return;
            }

            if (connection.IsDash)
            {
                if (this.menu.Item(this.menu.Name + "AutoDashing").GetValue<bool>()
                    && GlobalVariables.Player.Distance(connection.Unit.ServerPosition)
                    <= GlobalVariables.Spells[SpellSlot.E].Range
                    && connection.To.Position.Distance(
                        GlobalVariables.Player.ServerPosition.Extend(
                            connection.Unit.ServerPosition,
                            GlobalVariables.Spells[SpellSlot.E].Range)) <= 50)
                {
                    GlobalVariables.Spells[SpellSlot.E].CastOnUnit(connection.Unit);
                }
            }

            #endregion

            #region Walking

            // TODO: Priority Low - Med
            // Notice: Make it a way that it won't cancel AA
            if (connection.IsDash || GlobalVariables.Player.Spellbook.IsAutoAttacking) return;
            // Auto-Walk-To-Dash
            if (this.menu.Item(this.menu.Name + "AutoWalkToDash").GetValue<bool>())
            {
                // Connection considered to walk behind a unit
                if (connection.Lenght <= 50)
                {
                    // Walk logic here
                }
            }

            // Auto-Walking
            if (this.menu.Item(this.menu.Name + "AutoWalking").GetValue<bool>())
            {
                if (GlobalVariables.Player.ServerPosition.Distance(connection.To.Position) <= 50)
                {
                    this.Path.RemoveConnection(connection);
                }

                if (connection.Lenght > 50)
                {
                    // Walk logic here
                }
            }

            #endregion
        }

        #endregion

        #region Methods



        #endregion
    }
}