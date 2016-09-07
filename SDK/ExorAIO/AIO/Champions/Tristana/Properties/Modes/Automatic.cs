
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
        public static void Automatic(EventArgs args)
        {
            if (Bools.HasSheenBuff() || !(Variables.Orbwalker.GetTarget() as Obj_AI_Base).IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && GameObjects.Player.Spellbook.IsAutoAttacking
                && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                var objAiBase = Variables.Orbwalker.GetTarget() as Obj_AI_Base;
                if (objAiBase != null && (!Vars.E.IsReady() || objAiBase.HasBuff("TristanaECharge")))
                {
                    Vars.Q.Cast();
                }
            }
        }

        #endregion
    }
}