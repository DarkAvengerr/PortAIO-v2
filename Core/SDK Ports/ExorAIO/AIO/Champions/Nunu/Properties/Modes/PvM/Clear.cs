
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Nunu
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
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
            ///     The Q LaneClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Minions.Any()
                && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuSliderButton>().BValue)
            {
                if (GameObjects.Player.ManaPercent
                    < ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["laneclear"])
                    && !GameObjects.Player.Buffs.Any(b => b.Name.Equals("visionary")))
                {
                    return;
                }

                Vars.Q.CastOnUnit(
                    Targets.Minions.FirstOrDefault(
                        m => Vars.GetRealHealth(m) < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)));
            }

            /// <summary>
            ///     The E Clear Logics.
            /// </summary>
            if (Vars.E.IsReady() && Vars.Menu["spells"]["e"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                if (GameObjects.Player.ManaPercent
                    < ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["clear"])
                    && !GameObjects.Player.Buffs.Any(b => b.Name.Equals("visionary")))
                {
                    return;
                }

                /// <summary>
                ///     The E LaneClear Logic.
                /// </summary>
                foreach (var minion in
                    Targets.Minions.Where(
                        m =>
                        m.IsValidTarget(Vars.E.Range)
                        && Vars.GetRealHealth(m) < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E)))
                {
                    Vars.E.CastOnUnit(minion);
                }

                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Vars.E.CastOnUnit(Targets.JungleMinions[0]);
                }
            }
        }

        #endregion
    }
}