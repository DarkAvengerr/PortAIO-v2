
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Corki
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
                        !Invulnerable.Check(t) && !t.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                        && t.IsValidTarget(Vars.Q.Range - 100f)
                        && Vars.GetRealHealth(t) < (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.Q)))
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(target).CastPosition);
                    return;
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
                        !Invulnerable.Check(t) && t.IsValidTarget(Vars.R.Range)
                        && !t.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                        && Vars.GetRealHealth(t)
                        < (float)
                          GameObjects.Player.GetSpellDamage(
                              t,
                              SpellSlot.R,
                              GameObjects.Player.HasBuff("corkimissilebarragecounterbig")
                                  ? DamageStage.Empowered
                                  : DamageStage.Default)))
                {
                    if (!Vars.R.GetPrediction(target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                    {
                        Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                    }
                }
            }
        }

        #endregion
    }
}