
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Evelynn
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Q Clear Logics.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     The Q LaneClear Logic.
                /// </summary>
                if (Targets.Minions.Any() && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuBool>().Value)
                {
                    Vars.Q.Cast();
                }

                /// <summary>
                ///     The Q JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any()
                         && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuBool>().Value)
                {
                    Vars.Q.Cast();
                }
            }

            /// <summary>
            ///     The E JungleClear Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.JungleMinions.Any()
                && Vars.Menu["spells"]["q"]["clear"].GetValue<MenuBool>().Value)
            {
                Vars.E.CastOnUnit(Targets.JungleMinions[0]);
            }
        }

        #endregion
    }
}