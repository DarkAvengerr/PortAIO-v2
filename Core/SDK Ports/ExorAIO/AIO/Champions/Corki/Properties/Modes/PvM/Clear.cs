
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
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The JungleClear E Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["jungleclear"])
                    && Vars.Menu["spells"]["e"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.E.Cast();
                }
                /// <summary>
                ///     The LaneClear E Logic.
                /// </summary>
                else if (Targets.Minions.Count >= 3
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["laneclear"])
                         && Vars.Menu["spells"]["e"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.E.Cast();
                }
            }

            /// <summary>
            ///     The Clear Q Logics.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["jungleclear"])
                    && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                else if (Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["laneclear"])
                         && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.Q.Cast(Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                }
            }

            /// <summary>
            ///     The Clear R Logics.
            /// </summary>
            if (Vars.R.IsReady())
            {
                /// <summary>
                ///     The JungleClear R Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.R.Slot, Vars.Menu["spells"]["r"]["jungleclear"])
                    && Vars.Menu["spells"]["r"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.R.Cast(Targets.JungleMinions[0].ServerPosition);
                }
                /// <summary>
                ///     The LaneClear R Logic.
                /// </summary>
                else if (Vars.R.GetLineFarmLocation(Targets.Minions, Vars.R.Width).MinionsHit >= 2
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.R.Slot, Vars.Menu["spells"]["r"]["laneclear"])
                         && Vars.Menu["spells"]["r"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.R.Cast(Vars.R.GetLineFarmLocation(Targets.Minions, Vars.R.Width).Position);
                }
            }
        }

        #endregion
    }
}