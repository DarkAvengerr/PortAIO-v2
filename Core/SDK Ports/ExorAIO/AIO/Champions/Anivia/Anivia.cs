
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Anivia
{
    using System;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Anivia
    {
        #region Static Fields

        /// <summary>
        ///     Defines the missile object for the Q.
        /// </summary>
        public static GameObject QMissile;

        /// <summary>
        ///     Defines the missile object for the R.
        /// </summary>
        public static GameObject RMissile;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Called when an object gets created by the game.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                /// <summary>
                ///     Defines the missile object for the Q.
                /// </summary>
                if (obj.Name.Equals("cryo_FlashFrost_Player_mis.troy"))
                {
                    QMissile = obj;
                }

                /// <summary>
                ///     Defines the missile object for the R.
                /// </summary>
                if (obj.Name.Contains("cryo_storm"))
                {
                    RMissile = obj;
                }
            }
        }

        /// <summary>
        ///     Called when an object gets deleted by the game.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                /// <summary>
                ///     Removes the missile object for the Q.
                /// </summary>
                if (obj.Name.Equals("cryo_FlashFrost_Player_mis.troy"))
                {
                    QMissile = null;
                }

                /// <summary>
                ///     Removes the missile object for the R.
                /// </summary>
                if (obj.Name.Contains("cryo_storm"))
                {
                    RMissile = null;
                }
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            if (Vars.W.IsReady() && args.IsDirectedToPlayer && args.Sender.IsValidTarget(Vars.W.Range)
                && Vars.Menu["spells"]["w"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                Vars.W.Cast(
                    GameObjects.Player.ServerPosition.Extend(
                        args.Sender.ServerPosition,
                        GameObjects.Player.BoundingRadius));
            }
        }

        /// <summary>
        ///     Fired on interruptable spell.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            if (Vars.W.IsReady() && args.Sender.IsValidTarget(Vars.W.Range)
                && Vars.Menu["spells"]["w"]["interrupter"].GetValue<MenuBool>().Value)
            {
                if (
                    GameObjects.Player.Distance(
                        GameObjects.Player.ServerPosition.Extend(
                            args.Sender.ServerPosition,
                            GameObjects.Player.Distance(args.Sender) + 20f)) < Vars.W.Range)
                {
                    Vars.W.Cast(
                        GameObjects.Player.ServerPosition.Extend(
                            args.Sender.ServerPosition,
                            GameObjects.Player.Distance(args.Sender) + 20f));
                }
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);
            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            /// <summary>
            ///     Initializes the orbwalkingmodes.
            /// </summary>
            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Logics.Combo(args);
                    break;
                case OrbwalkingMode.Hybrid:
                    Logics.Harass(args);
                    break;
                case OrbwalkingMode.LaneClear:
                    Logics.Clear(args);
                    break;
            }
        }

        /// <summary>
        ///     Loads Anivia.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initializes the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();
        }

        #endregion
    }
}