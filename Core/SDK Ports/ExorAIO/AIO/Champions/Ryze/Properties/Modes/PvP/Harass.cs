
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Ryze
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
        public static void Harass(EventArgs args)
        {
            if (!Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The Harass E Logic.
            /// </summary>
            if (!Targets.Target.HasBuff("RyzeE"))
            {
                if (Vars.E.IsReady()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["harass"])
                    && Vars.Menu["spells"]["e"]["harass"].GetValue<MenuSliderButton>().BValue)
                {
                    if (Targets.Target.IsValidTarget(Vars.E.Range))
                    {
                        Vars.E.CastOnUnit(Targets.Target);
                    }
                    else
                    {
                        foreach (var minion in
                            Targets.Minions.Where(
                                m =>
                                !m.HasBuff("RyzeE") && m.IsValidTarget(Vars.E.Range)
                                && (m.Distance(Targets.Target) < 200
                                    || Targets.Minions.Any(
                                        m2 =>
                                        m2.HasBuff("RyzeE") && m2.Distance(m) < 200 && m2.Distance(Targets.Target) < 200)))
                            )
                        {
                            Vars.E.CastOnUnit(minion);
                        }
                    }
                }
            }

            /// <summary>
            ///     The Harass Q Logic.
            /// </summary>
            else
            {
                if (Vars.Q.IsReady()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["harass"])
                    && Vars.Menu["spells"]["q"]["harass"].GetValue<MenuSliderButton>().BValue)
                {
                    if (Targets.Target.IsValidTarget(Vars.Q.Range)
                        && !Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                    }
                    else
                    {
                        foreach (var minion in
                            Targets.Minions.Where(
                                m =>
                                m.HasBuff("RyzeE") && m.IsValidTarget(Vars.Q.Range)
                                && (m.Distance(Targets.Target) < 200
                                    || Targets.Minions.Any(
                                        m2 =>
                                        m2.HasBuff("RyzeE") && m2.Distance(m) < 200 && m2.Distance(Targets.Target) < 200)))
                            )
                        {
                            Vars.Q.Cast(minion.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion
    }
}