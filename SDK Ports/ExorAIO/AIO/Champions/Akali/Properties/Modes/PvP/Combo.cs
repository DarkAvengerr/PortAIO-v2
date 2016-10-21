
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Akali
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
            if (Bools.HasSheenBuff() || !Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.Q.CastOnUnit(Targets.Target);
            }

            /// <summary>
            ///     The R Gapclose Logic.
            /// </summary>
            if (Vars.R.IsReady() && !Targets.Target.IsValidTarget(Vars.R.Range)
                && Targets.Target.IsValidTarget(Vars.R.Range * 2)
                && GameObjects.Player.GetBuffCount("AkaliShadowDance")
                >= Vars.Menu["miscellaneous"]["gapclose"].GetValue<MenuSliderButton>().SValue
                && Vars.Menu["miscellaneous"]["gapclose"].GetValue<MenuSliderButton>().BValue)
            {
                foreach (var minion in
                    Targets.Minions.Where(
                        m => m.IsValidTarget(Vars.R.Range) && m.Distance(Targets.Target) < Vars.Q.Range))
                {
                    Vars.R.CastOnUnit(minion);
                }
            }
        }

        #endregion
    }
}