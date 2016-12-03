
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Harass(EventArgs args)
        {
            /// <summary>
            ///     The E Harass Logic.
            /// </summary>
            if (Vars.E.IsReady() && Vars.Menu["spells"]["e"]["harass"].GetValue<MenuSliderButton>().BValue
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["harass"]))
            {
                if (
                    GameObjects.EnemyHeroes.Any(
                        t =>
                        t.IsValidTarget(Vars.E.Range)
                        && (t.HasBuffOfType(BuffType.Poison)
                            || !Vars.Menu["spells"]["e"]["harasspoison"].GetValue<MenuBool>().Value)
                        && !Invulnerable.Check(t, DamageType.Magical, false)))
                {
                    foreach (var target in
                        GameObjects.EnemyHeroes.Where(
                            t =>
                            t.IsValidTarget(Vars.E.Range)
                            && (t.HasBuffOfType(BuffType.Poison)
                                || !Vars.Menu["spells"]["e"]["harasspoison"].GetValue<MenuBool>().Value)
                            && !Invulnerable.Check(t, DamageType.Magical, false)))
                    {
                        DelayAction.Add(
                            Vars.Menu["spells"]["e"]["delay"].GetValue<MenuSlider>().Value,
                            () => { Vars.E.CastOnUnit(target); });
                    }
                }
                else
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
            }

            if (!Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The Q Harass Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["harass"])
                && Vars.Menu["spells"]["q"]["harass"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).CastPosition);
                return;
            }

            /// <summary>
            ///     The W Harass Logic.
            /// </summary>
            DelayAction.Add(
                1000,
                () =>
                    {
                        if (Vars.W.IsReady() && Targets.Target.IsValidTarget(Vars.W.Range)
                            && !Targets.Target.IsValidTarget(500f)
                            && GameObjects.Player.ManaPercent
                            > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["harass"])
                            && Vars.Menu["spells"]["w"]["harass"].GetValue<MenuSliderButton>().BValue)
                        {
                            Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).CastPosition);
                        }
                    });
        }

        #endregion
    }
}