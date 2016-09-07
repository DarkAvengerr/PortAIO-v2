
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Warwick
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Minions.Any()
                && !GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.R.Range))
                && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                if (GameObjects.Player.MaxHealth
                    > GameObjects.Player.Health
                    + (float)GameObjects.Player.GetSpellDamage(Targets.Minions.FirstOrDefault(), SpellSlot.Q) * 0.8)
                {
                    Vars.Q.CastOnUnit(Targets.Minions.FirstOrDefault());
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() && GameObjects.Player.CountAllyHeroesInRange(Vars.W.Range) > 1
                && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuBool>().Value)
            {
                Vars.W.Cast();
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() && GameObjects.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1
                && Vars.Menu["spells"]["e"]["logical"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast();
            }
        }

        #endregion
    }
}