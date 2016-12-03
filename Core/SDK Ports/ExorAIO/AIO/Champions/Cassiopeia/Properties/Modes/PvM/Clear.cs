
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Cassiopeia
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
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
            ///     The E Clear Logics.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["jungleclear"])
                    && Vars.Menu["spells"]["e"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    DelayAction.Add(
                        Vars.Menu["spells"]["e"]["delay"].GetValue<MenuSlider>().Value,
                        () => { Vars.E.CastOnUnit(Targets.JungleMinions[0]); });
                }

                /// <summary>
                ///     The E LaneClear Logics.
                /// </summary>
                else if (Targets.Minions.Any())
                {
                    if (GameObjects.Player.ManaPercent
                        < ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["lasthit"])
                        && Vars.Menu["spells"]["e"]["lasthit"].GetValue<MenuSliderButton>().BValue)
                    {
                        DelayAction.Add(
                            Vars.Menu["spells"]["e"]["delay"].GetValue<MenuSlider>().Value,
                            () =>
                                {
                                    foreach (var minion in
                                        Targets.Minions.Where(
                                            m =>
                                            Vars.GetRealHealth(m)
                                            < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E)
                                            + (m.HasBuffOfType(BuffType.Poison)
                                                   ? (float)
                                                     GameObjects.Player.GetSpellDamage(
                                                         m,
                                                         SpellSlot.E,
                                                         DamageStage.Empowered)
                                                   : 0)))
                                    {
                                        Vars.E.CastOnUnit(minion);
                                    }
                                });
                    }
                    else if (GameObjects.Player.ManaPercent
                             >= ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["laneclear"])
                             && Vars.Menu["spells"]["e"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                    {
                        DelayAction.Add(
                            Vars.Menu["spells"]["e"]["delay"].GetValue<MenuSlider>().Value,
                            () =>
                                {
                                    foreach (var minion in
                                        Targets.Minions.Where(m => m.HasBuffOfType(BuffType.Poison)))
                                    {
                                        Vars.E.CastOnUnit(minion);
                                    }
                                });
                    }
                }
            }

            /// <summary>
            ///     The Q Clear Logic.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                var qFarmLocation =
                    Vars.Q.GetCircularFarmLocation(
                        Targets.Minions.Where(m => !m.HasBuffOfType(BuffType.Poison)).ToList(),
                        Vars.Q.Width);

                /// <summary>
                ///     The Q JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["jungleclear"])
                    && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The Q LaneClear Logic.
                /// </summary>
                else if (qFarmLocation.MinionsHit >= 2
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["laneclear"])
                         && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.Q.Cast(qFarmLocation.Position);
                }
            }

            /// <summary>
            ///     The W Clear Logic.
            /// </summary>
            if (Vars.W.IsReady())
            {
                var wFarmLocation =
                    Vars.W.GetCircularFarmLocation(
                        Targets.Minions.Where(m => !m.HasBuffOfType(BuffType.Poison)).ToList(),
                        Vars.W.Width);

                /// <summary>
                ///     The W JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["jungleclear"])
                    && Vars.Menu["spells"]["w"]["jungleclear"].GetValue<MenuSliderButton>().BValue
                    && !Targets.JungleMinions[0].IsValidTarget(500))
                {
                    Vars.W.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The W LaneClear Logic.
                /// </summary>
                else if (wFarmLocation.MinionsHit >= 3
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["laneclear"])
                         && Vars.Menu["spells"]["w"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.W.Cast(wFarmLocation.Position);
                }
            }
        }

        #endregion
    }
}