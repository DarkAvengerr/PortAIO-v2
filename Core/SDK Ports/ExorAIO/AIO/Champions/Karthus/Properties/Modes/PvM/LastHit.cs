
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Karthus
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
        public static void LastHit(EventArgs args)
        {
            /// <summary>
            ///     The Q LastHit Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["lasthit"])
                && Vars.Menu["spells"]["q"]["lasthit"].GetValue<MenuSliderButton>().BValue)
            {
                foreach (var minion in
                    Targets.Minions.Where(
                        m =>
                        Health.GetPrediction(m, 500 + Game.Ping)
                        < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)
                        * (!Targets.Minions.Any(m2 => m2.Distance(m) <= Vars.Q.Width)
                           && !GameObjects.EnemyHeroes.Any(t => t.Distance(m) <= Vars.Q.Width)
                               ? 2
                               : 1)))
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(minion).CastPosition);
                }
            }
        }

        #endregion
    }
}