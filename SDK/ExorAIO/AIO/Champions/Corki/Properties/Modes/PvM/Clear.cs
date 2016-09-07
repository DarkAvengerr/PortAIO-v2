
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Corki
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
            ///     The Clear E Logics.
            /// </summary>
            if (Vars.E.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["clear"])
                && Vars.Menu["spells"]["e"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                if (Targets.Minions.Count >= 3 || Targets.JungleMinions.Any())
                {
                    Vars.E.Cast();
                }
            }

            /// <summary>
            ///     The Clear Q Logics.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["clear"])
                && Vars.Menu["spells"]["q"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                if (Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3)
                {
                    Vars.Q.Cast(Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                }

                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
                return;
            }

            /// <summary>
            ///     The Clear R Logics.
            /// </summary>
            if (Vars.R.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.R.Slot, Vars.Menu["spells"]["r"]["clear"])
                && Vars.Menu["spells"]["r"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     The LaneClear R Logic.
                /// </summary>
                if (Vars.R.GetLineFarmLocation(Targets.Minions, Vars.R.Width).MinionsHit >= 2)
                {
                    Vars.R.Cast(Vars.R.GetLineFarmLocation(Targets.Minions, Vars.R.Width).Position);
                }

                /// <summary>
                ///     The JungleClear R Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.R.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }
        }

        #endregion
    }
}