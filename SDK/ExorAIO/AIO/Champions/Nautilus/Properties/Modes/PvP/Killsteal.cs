
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Nautilus
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
                        t.IsValidTarget(Vars.Q.Range) && !t.IsValidTarget(Vars.AaRange)
                        && !Invulnerable.Check(t, DamageType.Magical, false)
                        && Vars.GetRealHealth(t) < (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.Q)))
                {
                    if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(target).UnitPosition);
                        return;
                    }
                }
            }

            /// <summary>
            ///     The KillSteal R Logic.
            /// </summary>
            if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["killsteal"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        t.IsValidTarget(Vars.R.Range) && !t.IsValidTarget(Vars.AaRange)
                        && !Invulnerable.Check(t, DamageType.Magical, false)
                        && Vars.GetRealHealth(t) < (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.R)))
                {
                    Vars.R.CastOnUnit(target);
                }
            }
        }

        #endregion
    }
}