
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Jinx
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
    internal class Jinx
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != OrbwalkingMode.LastHit
                && Variables.Orbwalker.ActiveMode != OrbwalkingMode.LaneClear)
            {
                return;
            }

            switch (args.Type)
            {
                case OrbwalkingType.BeforeAttack:
                    var canLastHit = Vars.Menu["spells"]["q"]["lasthit"].GetValue<MenuSliderButton>().BValue
                                     && GameObjects.Player.ManaPercent
                                     > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["lasthit"]);
                    var canLaneClear = Vars.Menu["spells"]["q"]["clear"].GetValue<MenuSliderButton>().BValue
                                       && GameObjects.Player.ManaPercent
                                       > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["lasthit"]);

                    if (Vars.Q.IsReady() && args.Target != null)
                    {
                        var isUsingFishBones = GameObjects.Player.HasBuff("JinxQ");
                        var minionsInRange = GameObjects.EnemyMinions.Count(m => m.Distance(args.Target) < 160f);
                        if (isUsingFishBones)
                        {
                            if (minionsInRange < 3)
                            {
                                Vars.Q.Cast();
                            }
                        }
                        else
                        {
                            if (minionsInRange >= 3
                                && (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LastHit && canLastHit
                                    || Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear && canLaneClear))
                            {
                                Vars.Q.Cast();
                            }
                        }
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
            if (GameObjects.Player.IsDead || !Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.E.IsReady() && args.Sender.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast(args.IsDirectedToPlayer ? GameObjects.Player.ServerPosition : args.End);
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
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);
            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

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
            }
        }

        /// <summary>
        ///     Loads Jinx.
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