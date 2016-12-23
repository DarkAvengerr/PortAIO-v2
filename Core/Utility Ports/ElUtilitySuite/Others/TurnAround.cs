using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Others
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElUtilitySuite.Logging;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class TurnAround : IPlugin
    {
        // Copyright 2014 - 2015 Nikita Bernthaler

        #region Fields

        private readonly List<SpellInfo> _spellInfos = new List<SpellInfo>
        {
            new SpellInfo("Cassiopeia", "CassiopeiaR", 1000f, false, true, 0.85f),
            new SpellInfo("Cassiopeia", "cassiopeiapetrifyinggaze", 1000f, false, true, 0.85f),
            new SpellInfo("Tryndamere", "MockingShout", 900f, false, false, 0.65f)
        };

        /// <summary>
        ///     Block movement time
        /// </summary>
        private float blockMovementTime;

        /// <summary>
        ///     Last move
        /// </summary>
        private Vector3 lastMove;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "MiscMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Misc", "MiscMenu"))
                           : rootMenu.Children.First(predicate);

            var turnAroundMenu = menu.AddSubMenu(new Menu("Turn Around", "TurnAround"));
            {
                turnAroundMenu.AddItem(new MenuItem("TurnAround", "Enabled").SetValue(true));
            }

            this.Menu = turnAroundMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            EloBuddy.Player.OnIssueOrder += this.OnObjAiBaseIssueOrder;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when a spell has been casted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnObjAiBaseIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            try
            {
                if (!this.Menu.Item("TurnAround").IsActive())
                {
                    return;
                }

                if (sender.IsMe)
                {
                    if (args.Order == GameObjectOrder.MoveTo)
                    {
                        this.lastMove = args.TargetPosition;
                    }
                    if (this.blockMovementTime > Game.Time)
                    {
                        args.Process = false;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@TurnAround.cs: An error occurred: {0}", e);
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!this.Menu.Item("TurnAround").IsActive())
                {
                    return;
                }

                if (sender == null || !sender.IsValid || sender.Team == this.Player.Team
                    || this.Player.IsDead || !this.Player.IsTargetable)
                {
                    return;
                }

                var spellInfo =
                    this._spellInfos.FirstOrDefault(
                        i => args.SData.Name.Equals(i.Name, StringComparison.InvariantCultureIgnoreCase));

                if (spellInfo == null)
                {
                    return;
                }

                if ((spellInfo.Target && args.Target == this.Player)
                    || this.Player.Distance(sender.ServerPosition) + this.Player.BoundingRadius <= spellInfo.Range)
                {
                    var moveTo = this.lastMove;

                    EloBuddy.Player.IssueOrder(
                        GameObjectOrder.MoveTo,
                        sender.ServerPosition.Extend(
                            this.Player.ServerPosition,
                            this.Player.ServerPosition.Distance(sender.ServerPosition)
                            + (spellInfo.TurnOpposite ? 100 : -100)));

                    this.blockMovementTime = Game.Time + spellInfo.CastTime;

                    LeagueSharp.Common.Utility.DelayAction.Add(
                        (int)((spellInfo.CastTime + 0.1) * 1000),
                        () => EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, moveTo));
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@TurnAround.cs: An error occurred: {0}", e);
            }
        }

        #endregion

        private class SpellInfo
        {
            #region Constructors and Destructors

            /// <summary>
            ///     
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="range"></param>
            /// <param name="target"></param>
            /// <param name="turnOpposite"></param>
            /// <param name="castTime"></param>
            public SpellInfo(string owner, string name, float range, bool target, bool turnOpposite, float castTime)
            {
                this.Owner = owner;
                this.Name = name;
                this.Range = range;
                this.Target = target;
                this.TurnOpposite = turnOpposite;
                this.CastTime = castTime;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     The spell cast time
            /// </summary>
            public float CastTime { get; private set; }

            /// <summary>
            ///     The spell name
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            ///     The spell casters champion name
            /// </summary>
            public string Owner { get; private set; }

            /// <summary>
            ///     The spells range
            /// </summary>
            public float Range { get; private set; }

            /// <summary>
            ///     The target
            /// </summary>
            public bool Target { get; private set; }

            /// <summary>
            ///     Turn
            /// </summary>
            public bool TurnOpposite { get; private set; }

            #endregion
        }
    }
}