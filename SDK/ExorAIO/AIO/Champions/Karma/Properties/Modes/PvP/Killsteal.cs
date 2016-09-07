
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Karma
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The KillSteal Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Vars.Menu["spells"]["q"]["killsteal"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        t.IsValidTarget(Vars.Q.Range - 100f) && !Invulnerable.Check(t, DamageType.Magical)
                        && Vars.GetRealHealth(t)
                        < (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.Q)
                        + (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["empq"].GetValue<MenuBool>().Value
                               ? (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.Q, DamageStage.Empowered)
                               : 0)))
                {
                    if (!Vars.Q.GetPrediction(target).CollisionObjects.Any())
                    {
                        if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["empq"].GetValue<MenuBool>().Value)
                        {
                            Vars.R.Cast();
                        }
                        Vars.Q.Cast(Vars.Q.GetPrediction(target).UnitPosition);
                    }
                }
            }
        }

        #endregion
    }
}