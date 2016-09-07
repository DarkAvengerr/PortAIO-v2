
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Ryze
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
    internal class Ryze
    {
        #region Public Properties

        /// <summary>
        ///     The Q Stacks.
        /// </summary>
        public static int Stacks
            =>
                GameObjects.Player.HasBuff("ryzeqiconnocharge")
                    ? 0
                    : GameObjects.Player.HasBuff("ryzeqiconhalfcharge") ? 1 : 2;

        #endregion

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
            if (GameObjects.Player.HealthPercent
                <= Vars.Menu["spells"]["q"]["shield"].GetValue<MenuSliderButton>().SValue
                && Vars.Menu["spells"]["q"]["shield"].GetValue<MenuSliderButton>().BValue)
            {
                return;
            }

            if (Vars.W.IsReady() && args.Sender.IsValidTarget(Vars.W.Range)
                && !Invulnerable.Check(args.Sender, DamageType.Magical, false)
                && Vars.Menu["spells"]["w"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                Vars.W.CastOnUnit(args.Sender);
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
        ///     Loads Ryze.
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