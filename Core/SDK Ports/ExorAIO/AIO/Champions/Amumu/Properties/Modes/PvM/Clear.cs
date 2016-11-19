
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Amumu
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp.SDK;
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
            ///     The Q JungleGrab Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["junglegrab"])
                && Vars.Menu["spells"]["q"]["junglegrab"].GetValue<MenuSliderButton>().BValue)
            {
                if (Targets.JungleMinions.Any(m => !m.IsValidTarget(Vars.E.Range))
                    && !Targets.JungleMinions.Any(m => m.IsValidTarget(Vars.E.Range)))
                {
                    var minion = Targets.JungleMinions.FirstOrDefault(m => !m.IsValidTarget(Vars.E.Range));
                    if (minion != null)
                    {
                        Vars.Q.Cast(minion.ServerPosition);
                    }
                }
            }

            /// <summary>
            ///     The E Clear Logics.
            /// </summary>
            if (Vars.E.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["clear"])
                && Vars.Menu["spells"]["e"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     The E LaneClear Logic.
                /// </summary>
                if (Targets.Minions.Count(m => m.IsValidTarget(Vars.E.Range)) >= 3)
                {
                    Vars.E.Cast();
                }

                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any(m => m.IsValidTarget(Vars.E.Range)))
                {
                    Vars.E.Cast();
                }
            }
        }

        #endregion
    }
}