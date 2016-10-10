
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Tristana
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
        public static void Harass(EventArgs args)
        {
            if (!Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Harass Logic.
            /// </summary>
            if (Vars.Q.IsReady() && GameObjects.Player.Spellbook.IsAutoAttacking
                && Vars.Menu["spells"]["q"]["harass"].GetValue<MenuBool>().Value)
            {
                Vars.Q.Cast();
            }

            /// <summary>
            ///     The E Harass Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.Target.IsValidTarget(Vars.E.Range)
                && GameObjects.Player.ManaPercent
                > Vars.Menu["spells"]["e"]["harass"].GetValue<MenuSliderButton>().SValue
                + (int)(GameObjects.Player.Spellbook.GetSpell(Vars.E.Slot).SData.Mana / GameObjects.Player.MaxMana * 100)
                && Vars.Menu["spells"]["e"]["harass"].GetValue<MenuSliderButton>().BValue
                && Vars.Menu["spells"]["e"]["whitelist"][Targets.Target.ChampionName.ToLower()].GetValue<MenuBool>()
                       .Value)
            {
                Vars.E.CastOnUnit(Targets.Target);
            }
        }

        #endregion
    }
}