
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Ryze
{
    using System;

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
            if (Bools.HasSheenBuff() && Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                || !Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     Dynamic Combo Logic.
            /// </summary>
            switch (Ryze.Stacks)
            {
                case 0:
                case 1:

                    /// <summary>
                    ///     The Q Combo Logic.
                    /// </summary>
                    if (Ryze.Stacks == 0
                        || GameObjects.Player.HealthPercent
                        > Vars.Menu["spells"]["q"]["shield"].GetValue<MenuSliderButton>().SValue
                        || !Vars.Menu["spells"]["q"]["shield"].GetValue<MenuSliderButton>().BValue)
                    {
                        if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range - 100f)
                            && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
                        {
                            Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                        }
                    }

                    /// <summary>
                    ///     The W Combo Logic.
                    /// </summary>
                    if (Vars.W.IsReady() && Targets.Target.IsValidTarget(Vars.W.Range)
                        && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
                    {
                        Vars.W.CastOnUnit(Targets.Target);
                    }

                    /// <summary>
                    ///     The E Combo Logic.
                    /// </summary>
                    if (Vars.E.IsReady() && Targets.Target.IsValidTarget(Vars.E.Range)
                        && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
                    {
                        Vars.E.CastOnUnit(Targets.Target);
                    }
                    break;
                default:

                    /// <summary>
                    ///     The Q Combo Logic.
                    /// </summary>
                    if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range - 100f)
                        && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                    }
                    break;
            }
        }

        #endregion
    }
}