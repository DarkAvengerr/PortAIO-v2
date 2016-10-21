
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Karma
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
            ///     The Q Clear Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["clear"])
                && Vars.Menu["spells"]["q"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     The Q JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["empq"].GetValue<MenuBool>().Value)
                    {
                        Vars.R.Cast();
                    }
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                else if (Vars.Q.GetCircularFarmLocation(Targets.Minions, 125f).MinionsHit >= 3)
                {
                    if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["empq"].GetValue<MenuBool>().Value)
                    {
                        Vars.R.Cast();
                    }
                    Vars.Q.Cast(Vars.Q.GetCircularFarmLocation(Targets.Minions, 125f).Position);
                }
            }

            /// <summary>
            ///     The W JungleClear Logic.
            /// </summary>
            if (Vars.W.IsReady() && Targets.JungleMinions.Any()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["jungleclear"])
                && Vars.Menu["spells"]["w"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
            {
                if (Vars.R.IsReady() && Vars.Menu["spells"]["w"]["lifesaver"].GetValue<MenuSliderButton>().BValue
                    && Vars.Menu["spells"]["w"]["lifesaver"].GetValue<MenuSliderButton>().SValue
                    > GameObjects.Player.HealthPercent)
                {
                    Vars.R.Cast();
                }
                Vars.W.CastOnUnit(Targets.JungleMinions[0]);
            }
        }

        #endregion
    }
}