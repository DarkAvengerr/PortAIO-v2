
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Karthus
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Tear Stacking Logic.
            /// </summary>
            if (Vars.Q.IsReady() && !Targets.Minions.Any() && Bools.HasTear(GameObjects.Player)
                && Variables.Orbwalker.ActiveMode == OrbwalkingMode.None
                && GameObjects.Player.CountEnemyHeroesInRange(1500) == 0
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["miscellaneous"]["tear"])
                && Vars.Menu["miscellaneous"]["tear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.Cast(GameObjects.Player.ServerPosition.Extend(Game.CursorPos, Vars.Q.Range - 5f));
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     If the player doesn't have the E Buff.
                /// </summary>
                if (!GameObjects.Player.HasBuff("KarthusDefile"))
                {
                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        /// <summary>
                        ///     The E Combo Enable Logic.
                        /// </summary>
                        case OrbwalkingMode.Combo:
                            if (GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.E.Range)))
                            {
                                Vars.E.Cast();
                            }
                            break;

                        /// <summary>
                        ///     The E Clear Enable Logics.
                        /// </summary>
                        case OrbwalkingMode.LaneClear:
                            if (
                                (Targets.JungleMinions.Any(
                                    m =>
                                    m.IsValidTarget(Vars.E.Range)
                                    && Vars.GetRealHealth(m) > (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q))
                                 && GameObjects.Player.ManaPercent
                                 > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["jungleclear"])
                                 && Vars.Menu["spells"]["e"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                                || (Targets.Minions.Any(
                                    m =>
                                    m.IsValidTarget(Vars.E.Range)
                                    && Vars.GetRealHealth(m) > (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q))
                                    && GameObjects.Player.ManaPercent
                                    > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["laneclear"])
                                    && Vars.Menu["spells"]["e"]["laneclear"].GetValue<MenuSliderButton>().BValue))
                            {
                                Vars.E.Cast();
                            }
                            break;
                    }
                }

                /// <summary>
                ///     If the player has the E Buff.
                /// </summary>
                else
                {
                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        /// <summary>
                        ///     The E Clear Disable Logic.
                        /// </summary>
                        case OrbwalkingMode.LaneClear:
                            if (
                                (!Targets.JungleMinions.Any(
                                    m =>
                                    m.IsValidTarget(Vars.E.Range)
                                    && Vars.GetRealHealth(m) > (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q))
                                 || GameObjects.Player.ManaPercent
                                 < ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["jungleclear"])
                                 || !Vars.Menu["spells"]["e"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                                && (!Targets.Minions.Any(
                                    m =>
                                    m.IsValidTarget(Vars.E.Range)
                                    && Vars.GetRealHealth(m) > (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q))
                                    || GameObjects.Player.ManaPercent
                                    < ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["laneclear"])
                                    || !Vars.Menu["spells"]["e"]["laneclear"].GetValue<MenuSliderButton>().BValue))
                            {
                                Vars.E.Cast();
                            }
                            break;

                        /// <summary>
                        ///     The Default Disable Logic.
                        /// </summary>
                        default:
                            if (!GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.E.Range)))
                            {
                                Vars.E.Cast();
                            }
                            break;
                    }
                }
            }
        }

        #endregion
    }
}