
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Warwick
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
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff() || !(Variables.Orbwalker.GetTarget() as Obj_AI_Minion).IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The W Clear Logic.
            /// </summary>
            if (Vars.W.IsReady() && GameObjects.Player.Spellbook.IsAutoAttacking
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["clear"])
                && Vars.Menu["spells"]["w"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.W.Cast();
            }
            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            /// <summary>
            ///     The Q Clear Logics.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.MaxHealth
                > GameObjects.Player.Health
                + (float)
                  GameObjects.Player.GetSpellDamage(Variables.Orbwalker.GetTarget() as Obj_AI_Minion, SpellSlot.Q) * 0.8)
            {
                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                if (GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["laneclear"])
                    && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuSliderButton>().BValue
                    && Targets.Minions.Contains(Variables.Orbwalker.GetTarget() as Obj_AI_Minion))
                {
                    Vars.Q.CastOnUnit(Variables.Orbwalker.GetTarget() as Obj_AI_Minion);
                }

                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                else if (GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["jungleclear"])
                         && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuSliderButton>().BValue
                         && Targets.JungleMinions.Contains(Variables.Orbwalker.GetTarget() as Obj_AI_Minion))
                {
                    Vars.Q.CastOnUnit(Variables.Orbwalker.GetTarget() as Obj_AI_Minion);
                }
            }
        }

        #endregion
    }
}