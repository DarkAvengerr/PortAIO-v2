
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Akali
{
    using System;
    using System.Linq;

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
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Q JungleClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.JungleMinions.Any()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["jungleclear"])
                && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.CastOnUnit(Targets.JungleMinions[0]);
            }

            /// <summary>
            ///     The E LaneClear Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.Minions.Count(m => m.IsValidTarget(Vars.E.Range)) >= 3
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["clear"])
                && Vars.Menu["spells"]["e"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.E.Cast();
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        public static void JungleClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(Variables.Orbwalker.GetTarget() is Obj_AI_Minion)
                || !Targets.JungleMinions.Contains(Variables.Orbwalker.GetTarget() as Obj_AI_Minion))
            {
                return;
            }

            /// <summary>
            ///     The E JungleClear Logic.
            /// </summary>
            if (Vars.E.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["clear"])
                && Vars.Menu["spells"]["e"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.E.Cast();
            }
        }

        #endregion
    }
}