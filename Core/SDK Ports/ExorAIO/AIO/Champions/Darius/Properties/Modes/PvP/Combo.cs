
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Darius
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

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
            if ((Bools.HasSheenBuff() && Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange()))
                || !Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.Target.IsValidTarget(Vars.E.Range - 50f)
                && !Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).UnitPosition);
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && !Vars.W.IsReady()
                && Vars.Menu["spells"]["q"]["mode"].GetValue<MenuList>().Index != 2
                && GameObjects.EnemyHeroes.Any(
                    t =>
                    t.IsValidTarget(Vars.Q.Range)
                    && (Vars.Menu["spells"]["q"]["mode"].GetValue<MenuList>().Index == 0 || !t.IsValidTarget(205f))))
            {
                Vars.Q.Cast();
            }
        }

        #endregion
    }
}