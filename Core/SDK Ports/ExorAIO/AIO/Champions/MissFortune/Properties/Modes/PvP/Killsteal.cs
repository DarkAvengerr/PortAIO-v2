
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.MissFortune
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    using Geometry = ExorAIO.Utilities.Geometry;

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
            ///     The Q KillSteal Logic.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     Normal Q KillSteal Logic.
                /// </summary>
                if (Vars.Menu["spells"]["q"]["killsteal"].GetValue<MenuBool>().Value)
                {
                    foreach (var target in
                        GameObjects.EnemyHeroes.Where(
                            t =>
                            !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range)
                            && Vars.GetRealHealth(t) < (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.Q)))
                    {
                        Vars.Q.CastOnUnit(target);
                    }
                }

                var minionPassiveMultiplier = GameObjects.Player.Level < 4
                                                  ? 0.25
                                                  : GameObjects.Player.Level < 7
                                                        ? 0.3
                                                        : GameObjects.Player.Level < 9
                                                              ? 0.35
                                                              : GameObjects.Player.Level < 11
                                                                    ? 0.4
                                                                    : GameObjects.Player.Level < 13 ? 0.45 : 0.5;
                var heroPassiveMultiplier = minionPassiveMultiplier * 2;

                /// <summary>
                ///     Extended Q KillSteal Logic.
                /// </summary>
                if (Vars.Menu["spells"]["q"]["extended"]["exkillsteal"].GetValue<MenuBool>().Value)
                {
                    /// <summary>
                    ///     Through enemy minions.
                    /// </summary>
                    foreach (var minion 
                        in from minion in Targets.Minions.Where(m => m.IsValidTarget(Vars.Q.Range))
                           let polygon =
                               new Geometry.Sector(
                               (Vector2)minion.ServerPosition,
                               (Vector2)
                               minion.ServerPosition.Extend(
                                   GameObjects.Player.ServerPosition,
                                   -(Vars.Q2.Range - Vars.Q.Range)),
                               40f * (float)Math.PI / 180f,
                               Vars.Q2.Range - Vars.Q.Range - 50f)
                           let target =
                               GameObjects.EnemyHeroes.FirstOrDefault(
                                   t =>
                                   !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q2.Range)
                                   && Vars.GetRealHealth(t)
                                   < (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.Q, DamageStage.SecondForm)
                                   + GameObjects.Player.TotalAttackDamage * minionPassiveMultiplier
                                   + (Vars.GetRealHealth(minion)
                                      < (float)GameObjects.Player.GetSpellDamage(minion, SpellSlot.Q)
                                      + (MissFortune.PassiveTarget == null
                                         || minion.NetworkId != MissFortune.PassiveTarget?.NetworkId
                                             ? GameObjects.Player.TotalAttackDamage * minionPassiveMultiplier
                                             : 0)
                                          ? (float)
                                            GameObjects.Player.GetSpellDamage(t, SpellSlot.Q, DamageStage.SecondForm)
                                            / 2
                                          : 0)
                                   && (t.NetworkId == MissFortune.PassiveTarget?.NetworkId
                                       || Targets.Minions.All(m => polygon.IsOutside((Vector2)m.ServerPosition))))
                           where target != null
                           where
                               !polygon.IsOutside((Vector2)target.ServerPosition)
                               && !polygon.IsOutside((Vector2)Vars.Q.GetPrediction(target).UnitPosition)
                           select minion)
                    {
                        Vars.Q.CastOnUnit(minion);
                    }

                    /// <summary>
                    ///     Through enemy heroes.
                    /// </summary>
                    foreach (var target
                        in from target in GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(Vars.Q.Range))
                           let polygon =
                               new Geometry.Sector(
                               (Vector2)target.ServerPosition,
                               (Vector2)
                               target.ServerPosition.Extend(
                                   GameObjects.Player.ServerPosition,
                                   -(Vars.Q2.Range - Vars.Q.Range)),
                               40f * (float)Math.PI / 180f,
                               Vars.Q2.Range - Vars.Q.Range - 50f)
                           let target2 =
                               GameObjects.EnemyHeroes.FirstOrDefault(
                                   t =>
                                   !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q2.Range)
                                   && Vars.GetRealHealth(t)
                                   < (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.Q, DamageStage.SecondForm)
                                   + GameObjects.Player.TotalAttackDamage * heroPassiveMultiplier
                                   && (t.NetworkId == MissFortune.PassiveTarget?.NetworkId
                                       || Targets.Minions.All(m => polygon.IsOutside((Vector2)m.ServerPosition))))
                           where target2 != null
                           where
                               !polygon.IsOutside((Vector2)target2.ServerPosition)
                               && !polygon.IsOutside((Vector2)Vars.Q.GetPrediction(target).UnitPosition)
                           select target)
                    {
                        Vars.Q.CastOnUnit(target);
                    }
                }
            }
        }

        #endregion
    }
}