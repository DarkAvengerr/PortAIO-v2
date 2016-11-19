
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Renekton
{
    using System;

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
            if (Bools.HasSheenBuff() && !Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                || !Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && !Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                && Targets.Target.IsValidTarget(Vars.Q.Range) && !GameObjects.Player.IsDashing()
                && !GameObjects.Player.HasBuff("RenektonPreExecute")
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.Q.Cast();
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() && !Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                && Targets.Target.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
            {
                if (GameObjects.Player.HasBuff("renektonsliceanddicedelay")
                    && !Vars.Menu["spells"]["e"]["combo2"].GetValue<MenuBool>().Value)
                {
                    return;
                }

                if (Targets.Target.HealthPercent < 10 || !Targets.Target.IsUnderEnemyTurret())
                {
                    Vars.E.Cast(Targets.Target.ServerPosition);
                }
            }
        }

        #endregion
    }
}