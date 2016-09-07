
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Diana
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
            ///     The Clear Q Logic.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                if (Targets.Minions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["laneclear"])
                    && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuSliderButton>().BValue
                    && Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3)
                {
                    Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                }

                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any()
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["jungleclear"])
                         && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }

            /// <summary>
            ///     The Clear W Logic.
            /// </summary>
            if (Vars.W.IsReady())
            {
                /// <summary>
                ///     The LaneClear W Logic.
                /// </summary>
                if (Targets.Minions.Any() && Targets.Minions.Count(m => m.IsValidTarget(Vars.W.Range)) >= 3
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["laneclear"])
                    && Vars.Menu["spells"]["w"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.W.Cast();
                }

                /// <summary>
                ///     The JungleClear W Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any(m => m.IsValidTarget(Vars.W.Range))
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["jungleclear"])
                         && Vars.Menu["spells"]["w"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.W.Cast();
                }
            }

            /// <summary>
            ///     The JungleClear R Logic.
            /// </summary>
            if (Vars.R.IsReady() && Targets.JungleMinions.Any(m => m.HasBuff("dianamoonlight"))
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.R.Slot, Vars.Menu["spells"]["r"]["jungleclear"])
                && Vars.Menu["spells"]["r"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.R.CastOnUnit(Targets.JungleMinions.FirstOrDefault(m => m.HasBuff("dianamoonlight")));
            }
        }

        #endregion
    }
}