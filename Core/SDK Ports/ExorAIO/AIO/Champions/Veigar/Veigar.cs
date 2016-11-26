
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Veigar
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Veigar
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            switch (args.Type)
            {
                case OrbwalkingType.BeforeAttack:
                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        case OrbwalkingMode.Combo:

                            /// <summary>
                            ///     The 'No AA in Combo' Logic.
                            /// </summary>
                            if (Vars.Menu["miscellaneous"]["noaacombo"].GetValue<MenuBool>().Value)
                            {
                                if (Vars.Q.IsReady() || Vars.W.IsReady() || Vars.E.IsReady() || !Bools.HasSheenBuff()
                                    || GameObjects.Player.ManaPercent > 10)
                                {
                                    args.Process = false;
                                }
                            }
                            break;
                        case OrbwalkingMode.Hybrid:
                        case OrbwalkingMode.LastHit:
                        case OrbwalkingMode.LaneClear:

                            /// <summary>
                            ///     The 'No AA if Q Ready' Logic.
                            /// </summary>
                            if (Vars.Menu["miscellaneous"]["qfarmmode"].GetValue<MenuBool>().Value)
                            {
                                if (Vars.Q.IsReady())
                                {
                                    args.Process = false;
                                }
                            }

                            /// <summary>
                            ///     The 'Support Mode' Logic.
                            /// </summary>
                            if (Vars.Menu["miscellaneous"]["support"].GetValue<MenuBool>().Value)
                            {
                                if (args.Target is Obj_AI_Minion
                                    && GameObjects.AllyHeroes.Any(a => a.Distance(GameObjects.Player) < 2500))
                                {
                                    args.Process = false;
                                }
                            }
                            break;
                    }

                    break;
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (GameObjects.Player.IsDead || Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.E.IsReady() && args.Sender.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast(args.Sender.ServerPosition);
            }
        }

        /// <summary>
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (GameObjects.Player.IsDead || Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.E.IsReady() && args.Sender.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["interrupter"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast(args.Sender.ServerPosition);
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
                case OrbwalkingMode.LastHit:
                    Logics.LastHit(args);
                    break;
                case OrbwalkingMode.LaneClear:
                    Logics.Clear(args);
                    break;
            }
        }

        /// <summary>
        ///     Loads Veigar.
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