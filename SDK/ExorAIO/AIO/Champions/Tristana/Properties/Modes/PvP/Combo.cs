
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Tristana
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
            if (Bools.HasSheenBuff() || !(Variables.Orbwalker.GetTarget() as AIHeroClient).IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() && !Invulnerable.Check(Variables.Orbwalker.GetTarget() as AIHeroClient)
                && (Variables.Orbwalker.GetTarget() as AIHeroClient).IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value
                && Vars.Menu["spells"]["e"]["whitelist"][Targets.Target.ChampionName.ToLower()].GetValue<MenuBool>()
                       .Value)
            {
                Vars.E.CastOnUnit(Variables.Orbwalker.GetTarget() as AIHeroClient);
            }
        }

        #endregion
    }
}