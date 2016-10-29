
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using System;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     The main class.
    /// </summary>
    internal class Index
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /// <summary>
            ///     Loads the special items logics.
            /// </summary>
            Activator.Specials(sender, args);

            /// <summary>
            ///     Loads the resetter-items logics.
            /// </summary>
            Activator.Resetters(sender, args);
        }
        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            if (args.Type == OrbwalkingType.BeforeAttack)
            {
                /// <summary>
                ///     Loads the offensives logics.
                /// </summary>
                Activator.Offensives(sender, args);
            }
        }

        /// <summary>
        ///     Loads the Activator.
        /// </summary>
        public static void OnLoad()
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
            ///     Initializes the smite logic.
            /// </summary>
            Activator.SmiteInit();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();

            /// <summary>
            ///     Initializes the resetters.
            /// </summary>
            Resetters.Initialize();

            /// <summary>
            ///     Initializes the healthbars.
            /// </summary>
            Healthbars.Initialize();
        }

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Loads the spells logics.
            /// </summary>
            Activator.Spells(args);

            /// <summary>
            ///     Loads the cleansers logics.
            /// </summary>
            Activator.Cleansers(args);

            /// <summary>
            ///     Loads the defensives logics.
            /// </summary>
            Activator.Defensives(args);

            /// <summary>
            ///     Loads the consumables logics.
            /// </summary>
            Activator.Consumables(args);
        }

        #endregion
    }
}