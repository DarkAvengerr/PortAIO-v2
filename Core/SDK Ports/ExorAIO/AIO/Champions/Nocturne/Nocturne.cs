
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Nocturne
{
    using System;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Nocturne
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (GameObjects.Player.IsDead || !Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.E.IsReady() && args.Sender.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                Vars.E.CastOnUnit(args.Sender);
            }
        }

        /// <summary>
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (GameObjects.Player.IsDead || !Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.E.IsReady() && args.Sender.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["interrupter"].GetValue<MenuBool>().Value)
            {
                Vars.E.CastOnUnit(args.Sender);
            }
        }

        /// <summary>
        ///     Called while processing spellcast operations.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuBool>().Value)
            {
                Logics.AutoShield(sender, args);
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
            ///     Updates the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the automatic events.
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
        ///     Loads Nautilus.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

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