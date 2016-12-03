
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Anivia
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

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
            ///     The R Clear Logic.
            /// </summary>
            if (Vars.R.IsReady() && GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.R.Slot, Vars.Menu["spells"]["r"]["clear"])
                && Vars.Menu["spells"]["r"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     The R LaneClear Logic.
                /// </summary>
                if (Vars.R.GetCircularFarmLocation(Targets.Minions).MinionsHit >= 3)
                {
                    Vars.R.Cast(Vars.R.GetCircularFarmLocation(Targets.Minions).Position);
                }

                /// <summary>
                ///     The R JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Vars.R.Cast(Targets.JungleMinions[0].ServerPosition);
                }
                return;
            }

            /// <summary>
            ///     The Q Clear Logic.
            /// </summary>
            if (Vars.Q.IsReady() && GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["clear"])
                && Vars.Menu["spells"]["q"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     The Q JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                else if (!GameObjects.EnemyHeroes.Any(t => !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range + 100)))
                {
                    if (Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width * 2 - 10f).MinionsHit >= 3)
                    {
                        Vars.Q.Cast(
                            Vars.Q.GetCircularFarmLocation(Targets.Minions, Vars.Q.Width * 2 - 10f).Position);
                    }
                }
            }
        }

        #endregion
    }
}