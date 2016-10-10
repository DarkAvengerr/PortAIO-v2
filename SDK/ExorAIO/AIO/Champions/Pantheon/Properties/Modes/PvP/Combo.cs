
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Pantheon
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
            if (!Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target)
                || GameObjects.Player.HasBuff("pantheonesound") || GameObjects.Player.HasBuff("pantheonpassiveshield"))
            {
                return;
            }

            if (Bools.HasSheenBuff())
            {
                if (Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange()))
                {
                    return;
                }
            }

            /// <summary>
            ///     The Combo Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.Q.CastOnUnit(Targets.Target);
            }

            /// <summary>
            ///     The Combo W Logic.
            /// </summary>
            if (Vars.W.IsReady() && Targets.Target.IsValidTarget(Vars.W.Range)
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
            {
                if (!Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                    || GameObjects.Player.GetBuffCount("pantheonpassivecounter") < 3)
                {
                    Vars.W.CastOnUnit(Targets.Target);
                }
            }

            /// <summary>
            ///     The Combo E Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.Target.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast(Targets.Target.ServerPosition);
            }
        }

        #endregion
    }
}