
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Kalista
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
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
            ///     The Q Clear Logics.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["clear"])
                && Vars.Menu["spells"]["q"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     The Q LaneClear Logic.
                /// </summary>
                if (
                    Vars.Q.GetLineFarmLocation(
                        Targets.Minions.Where(m => m.Health < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q))
                        .ToList(),
                        Vars.Q.Width).MinionsHit >= 3)
                {
                    Vars.Q.Cast(
                        Vars.Q.GetLineFarmLocation(
                            Targets.Minions.Where(
                                m => m.Health < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)).ToList(),
                            Vars.Q.Width).Position);
                }

                /// <summary>
                ///     The Q JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }

            /// <summary>
            ///     The E LaneClear Logics.
            /// </summary>
            if (Vars.E.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["laneclear"])
                && Vars.Menu["spells"]["e"]["laneclear"].GetValue<MenuSliderButton>().BValue)
            {
                if (
                    Targets.Minions.Count(
                        m =>
                        Kalista.IsPerfectRendTarget(m)
                        && Vars.GetRealHealth(m)
                        < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E)
                        + (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E, DamageStage.Buff)) >= 2)
                {
                    Vars.E.Cast();
                }
            }
        }

        #endregion
    }
}