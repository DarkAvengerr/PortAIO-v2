
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Kalista
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
        public static void Combo(EventArgs args)
        {
            /// <summary>
            ///     Orbwalk on minions.
            /// </summary>
            if (Targets.Minions.Any(m => m.IsValidTarget(Vars.AaRange))
                && !GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.AaRange))
                && Vars.Menu["miscellaneous"]["minionsorbwalk"].GetValue<MenuBool>().Value)
            {
                EloBuddy.Player.IssueOrder(
                    GameObjectOrder.AttackUnit,
                    Targets.Minions.FirstOrDefault(m => m.IsValidTarget(Vars.AaRange)));
            }
            if (Bools.HasSheenBuff() && !Targets.Target.IsValidTarget(Vars.AaRange)
                || !Targets.Target.IsValidTarget()
                || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any()
                    || Vars.Q.GetPrediction(Targets.Target).CollisionObjects.All(
                        c =>
                        (GameObjects.EnemyHeroes.Contains(c) || Targets.Minions.Contains(c))
                        && c.Health < (float)GameObjects.Player.GetSpellDamage(c, SpellSlot.Q)))
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }

        #endregion
    }
}