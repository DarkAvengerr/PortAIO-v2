
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Diana
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
            if (Bools.HasSheenBuff() && Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange()))
            {
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() && GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.W.Range))
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.W.Cast();
            }
            if (!Targets.Target.IsValidTarget()
                || Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                || Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range))
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).CastPosition);
            }

            /// <summary>
            ///     The R Logics.
            /// </summary>
            if (Vars.R.IsReady())
            {
                /// <summary>
                ///     The R Combo Logics.
                /// </summary>
                if (Targets.Target.IsValidTarget(Vars.R.Range)
                    && Vars.Menu["spells"]["r"]["combo"].GetValue<MenuBool>().Value)
                {
                    /// <summary>
                    ///     The R Normal Combo Logic.
                    /// </summary>
                    if (Targets.Target.HasBuff("dianamoonlight")
                        && Vars.Menu["spells"]["r"]["whitelist"][Targets.Target.ChampionName.ToLower()]
                               .GetValue<MenuBool>().Value)
                    {
                        if (!Targets.Target.IsUnderEnemyTurret()
                            || Vars.GetRealHealth(Targets.Target)
                            < (float)GameObjects.Player.GetSpellDamage(Targets.Target, SpellSlot.Q) * 2
                            + (float)GameObjects.Player.GetSpellDamage(Targets.Target, SpellSlot.R) * 2
                            || !Vars.Menu["miscellaneous"]["safe"].GetValue<MenuBool>().Value)
                        {
                            Vars.R.CastOnUnit(Targets.Target);
                            return;
                        }
                    }

                    /// <summary>
                    ///     The Second R Combo Logic.
                    /// </summary>
                    if (!Vars.Q.IsReady() && !Targets.Target.HasBuff("dianamoonlight")
                        && Vars.Menu["miscellaneous"]["rcombo"].GetValue<MenuBool>().Value
                        && Vars.Menu["spells"]["r"]["whitelist"][Targets.Target.ChampionName.ToLower()]
                               .GetValue<MenuBool>().Value)
                    {
                        DelayAction.Add(
                            1000,
                            () =>
                                {
                                    if (!Vars.Q.IsReady() && !Targets.Target.HasBuff("dianamoonlight"))
                                    {
                                        Vars.R.CastOnUnit(Targets.Target);
                                    }
                                });
                    }
                }

                /// <summary>
                ///     The R Gapclose Logic.
                /// </summary>
                else if (Targets.Target.IsValidTarget(Vars.Q.Range * 2)
                         && !Targets.Target.IsValidTarget(Vars.Q.Range + 200)
                         && Vars.Menu["miscellaneous"]["gapclose"].GetValue<MenuBool>().Value)
                {
                    if (
                        Targets.Minions.Any(
                            m =>
                            m.IsValidTarget(Vars.Q.Range) && m.Distance(Targets.Target) < Vars.Q.Range
                            && Vars.GetRealHealth(m) > (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)))
                    {
                        Vars.Q.Cast(
                            Targets.Minions.Where(
                                m =>
                                m.IsValidTarget(Vars.Q.Range) && m.Distance(Targets.Target) < Vars.Q.Range
                                && Vars.GetRealHealth(m) > (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q))
                                .OrderBy(o => o.DistanceToPlayer())
                                .Last());
                    }
                    if (
                        Targets.Minions.Any(
                            m =>
                            m.HasBuff("dianamoonlight") && m.IsValidTarget(Vars.R.Range)
                            && m.Distance(Targets.Target) < Vars.Q.Range))
                    {
                        Vars.R.CastOnUnit(
                            Targets.Minions.Where(
                                m =>
                                m.HasBuff("dianamoonlight") && m.IsValidTarget(Vars.R.Range)
                                && m.Distance(Targets.Target) < Vars.Q.Range)
                                .OrderBy(o => o.DistanceToPlayer())
                                .Last());
                    }
                }
            }

            if (Vars.Q.IsReady() && Vars.R.IsReady())
            {
                return;
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() && !Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                && Targets.Target.IsValidTarget(Vars.E.Range - 25)
                && Vars.Menu["spells"]["e"]["logical"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast();
            }
        }

        #endregion
    }
}