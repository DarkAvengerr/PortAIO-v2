
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Tristana
{
    using System;
    using System.Linq;

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
        public static void BuildingClear(EventArgs args)
        {
            if (!(Variables.Orbwalker.GetTarget() is Obj_AI_Turret))
            {
                return;
            }

            /// <summary>
            ///     The Q BuildingClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() && GameObjects.Player.Spellbook.IsAutoAttacking
                && Vars.Menu["spells"]["q"]["buildings"].GetValue<MenuBool>().Value)
            {
                Vars.Q.Cast();
            }

            /// <summary>
            ///     The E BuildingClear Logic.
            /// </summary>
            if (Vars.E.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["buildings"])
                && Vars.Menu["spells"]["e"]["buildings"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.E.CastOnUnit(Variables.Orbwalker.GetTarget() as Obj_AI_Turret);
            }
        }

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
            ///     The Clear Q Logics.
            /// </summary>
            var objAiBase = Variables.Orbwalker.GetTarget() as Obj_AI_Minion;
            if (Vars.Q.IsReady() && GameObjects.Player.Spellbook.IsAutoAttacking && objAiBase != null)
            {
                /// <summary>
                ///     The LaneClear & JungleClear Q Logics.
                /// </summary>
                if (Targets.Minions.Contains(objAiBase)
                    && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuBool>().Value
                    || Targets.JungleMinions.Contains(objAiBase)
                    && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuBool>().Value)
                {
                    Vars.Q.Cast();
                }
            }

            /// <summary>
            ///     The Clear E Logics.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The JungleClear E Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["jungleclear"])
                    && Vars.Menu["spells"]["e"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.E.CastOnUnit(Variables.Orbwalker.GetTarget() as Obj_AI_Minion);
                }

                /// <summary>
                ///     The LaneClear E Logics.
                /// </summary>
                else
                {
                    /// <summary>
                    ///     The Aggressive LaneClear E Logic.
                    /// </summary>
                    if (GameObjects.EnemyHeroes.Any(t => !Invulnerable.Check(t) && t.IsValidTarget(Vars.W.Range))
                        && GameObjects.Player.ManaPercent
                        > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["harass"])
                        && Vars.Menu["spells"]["e"]["harass"].GetValue<MenuSliderButton>().BValue)
                    {
                        foreach (var minion in
                            Targets.Minions.Where(
                                m =>
                                m.CountEnemyHeroesInRange(150f) > 0
                                && Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m)))
                        {
                            Vars.E.CastOnUnit(minion);
                        }
                    }
                    else
                    {
                        /// <summary>
                        ///     The Normal LaneClear E Logic.
                        /// </summary>
                        if (Targets.Minions.Any()
                            && GameObjects.Player.ManaPercent
                            > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["laneclear"])
                            && Vars.Menu["spells"]["e"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                        {
                            if (
                                Targets.Minions.Count(
                                    m => m.Distance(Variables.Orbwalker.GetTarget() as Obj_AI_Minion) < 150f) >= 3)
                            {
                                Vars.E.CastOnUnit(Variables.Orbwalker.GetTarget() as Obj_AI_Minion);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}