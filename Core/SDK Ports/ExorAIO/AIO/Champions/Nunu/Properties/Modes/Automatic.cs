
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Nunu
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["bool"].GetValue<MenuBool>().Value)
            {
                if (!GameObjects.Player.HasBuff("AbsoluteZero")
                    && GameObjects.Player.CountEnemyHeroesInRange(Vars.R.Range) > 0
                    && Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
                {
                    Vars.R.Cast();
                }
                if (GameObjects.Player.HasBuff("AbsoluteZero")
                    && !Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
                {
                    Vars.R.Cast();
                    Variables.Orbwalker.Move(Game.CursorPos);
                }
            }

            /// <summary>
            ///     The JungleClear Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuBool>().Value)
            {
                if (Targets.JungleMinions.Any())
                {
                    foreach (var minion in
                        Targets.JungleMinions.Where(
                            m =>
                            m.IsValidTarget(Vars.Q.Range)
                            && Vars.GetRealHealth(m) < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)))
                    {
                        Vars.Q.CastOnUnit(minion);
                    }
                }
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Minions.Any()
                && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                if (GameObjects.Player.MaxHealth
                    > GameObjects.Player.Health + (30 + 45 * GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).Level)
                    + GameObjects.Player.TotalMagicalDamage * 0.75)
                {
                    foreach (var minion in Targets.Minions.Where(m => m.IsValidTarget(Vars.Q.Range)))
                    {
                        Vars.Q.CastOnUnit(minion);
                    }
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuSliderButton>().BValue)
            {
                if (GameObjects.Player.ManaPercent
                    < ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["logical"])
                    && !GameObjects.Player.Buffs.Any(b => b.Name.Equals("visionary")))
                {
                    return;
                }

                switch (Variables.Orbwalker.ActiveMode)
                {
                    /// <summary>
                    ///     The W Combo Logics.
                    /// </summary>
                    case OrbwalkingMode.Combo:

                        /// <summary>
                        ///     The Ally W Combo Logic.
                        /// </summary>
                        if (
                            GameObjects.AllyHeroes.Any(
                                a =>
                                !a.IsMe && a.IsValidTarget(Vars.W.Range, false)
                                && Vars.Menu["spells"]["w"]["whitelist"][a.ChampionName.ToLower()].GetValue<MenuBool>()
                                       .Value))
                        {
                            Vars.W.CastOnUnit(
                                GameObjects.AllyHeroes.Where(
                                    a =>
                                    !a.IsMe && a.IsValidTarget(Vars.W.Range, false)
                                    && Vars.Menu["spells"]["w"]["whitelist"][a.ChampionName.ToLower()]
                                           .GetValue<MenuBool>().Value).OrderBy(o => o.TotalAttackDamage).First());
                        }

                        /// <summary>
                        ///     The Normal W Combo Logic.
                        /// </summary>
                        else
                        {
                            if (Targets.Target.IsValidTarget())
                            {
                                Vars.W.CastOnUnit(GameObjects.Player);
                            }
                        }
                        break;

                    /// <summary>
                    ///     The W Clearing Logic.
                    /// </summary>
                    case OrbwalkingMode.LaneClear:

                        /// <summary>
                        ///     Use if There are Enemy Minions in range.
                        /// </summary>
                        if (Targets.Minions.Any() || Targets.JungleMinions.Any())
                        {
                            Vars.W.CastOnUnit(GameObjects.Player);
                        }
                        break;
                }

                /// <summary>
                ///     The W Pushing Logic.
                /// </summary>
                if (Targets.Minions.Any() && GameObjects.AllyMinions.Any())
                {
                    /// <summary>
                    ///     Use if there are Super or Siege minions in W Range.
                    /// </summary>
                    foreach (var minion in GameObjects.AllyMinions.Where(m => m.IsValidTarget(Vars.W.Range, false)))
                    {
                        if (minion.GetMinionType() == MinionTypes.Super || minion.GetMinionType() == MinionTypes.Siege)
                        {
                            Vars.W.CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        #endregion
    }
}