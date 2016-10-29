
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
        public static void Combo(EventArgs args)
        {
            if (!Targets.Target.IsValidTarget() || Bools.IsImmobile(Targets.Target)
                || Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() && Targets.Target.IsValidTarget(Vars.R.Range)
                && Vars.Menu["spells"]["r"]["combo"].GetValue<MenuBool>().Value
                && Vars.Menu["spells"]["r"]["whitelist"][Targets.Target.ChampionName.ToLower()].GetValue<MenuBool>()
                       .Value)
            {
                Vars.R.CastOnUnit(Targets.Target);
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && !Vars.AnyWall(GameObjects.Player.ServerPosition, Vars.Q.GetPrediction(Targets.Target).UnitPosition)
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                    return;
                }
            }

            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.Target.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast();
            }
        }

        #endregion
    }
}