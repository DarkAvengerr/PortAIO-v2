
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Olaf
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    using Geometry = ExorAIO.Utilities.Geometry;

    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void BuildingClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(Variables.Orbwalker.GetTarget() is Obj_HQ) && !(Variables.Orbwalker.GetTarget() is Obj_AI_Turret)
                && !(Variables.Orbwalker.GetTarget() is Obj_BarracksDampener))
            {
                return;
            }

            /// <summary>
            ///     The W BuildingClear Logic.
            /// </summary>
            if (Vars.W.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["buildings"])
                && Vars.Menu["spells"]["w"]["buildings"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.W.Cast();
            }
        }

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
            ///     The Q Clear Logics.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["clear"])
                && Vars.Menu["spells"]["q"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The LaneClear Q Logics.
                /// </summary>
                else
                {
                    /// <summary>
                    ///     The Aggressive LaneClear Q Logic.
                    /// </summary>
                    if (GameObjects.EnemyHeroes.Any(t => !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range)))
                    {
                        if (Vars.Q.GetLineFarmLocation(Targets.Minions).MinionsHit >= 3
                            && !new Geometry.Rectangle(
                                    GameObjects.Player.ServerPosition,
                                    GameObjects.Player.ServerPosition.Extend(
                                        Targets.Minions[0].ServerPosition,
                                        Vars.Q.Range),
                                    Vars.Q.Width).IsOutside(
                                        (Vector2)
                                        Vars.Q.GetPrediction(
                                            GameObjects.EnemyHeroes.FirstOrDefault(
                                                t => !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range)))
                                            .UnitPosition))
                        {
                            Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions).Position);
                        }
                    }

                    /// <summary>
                    ///     The LaneClear Q Logic.
                    /// </summary>
                    else if (
                        !GameObjects.EnemyHeroes.Any(
                            t => !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range + 100f)))
                    {
                        if (Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3)
                        {
                            Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                        }
                    }
                }
            }

            /// <summary>
            ///     The E LaneClear Logics.
            /// </summary>
            if (Vars.E.IsReady() && Vars.Menu["spells"]["e"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                foreach (var minion in
                    Targets.Minions.Where(
                        m =>
                        m.IsValidTarget(Vars.E.Range)
                        && Vars.GetRealHealth(m) < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E)))
                {
                    if (minion.GetMinionType() == MinionTypes.Siege || minion.GetMinionType() == MinionTypes.Super)
                    {
                        Vars.E.CastOnUnit(minion);
                    }
                }
            }

            if (!(Variables.Orbwalker.GetTarget() is Obj_AI_Minion))
            {
                return;
            }

            /// <summary>
            ///     The W Clear Logic.
            /// </summary>
            if (Vars.W.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["clear"])
                && Vars.Menu["spells"]["w"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.W.Cast();
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void JungleClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(Variables.Orbwalker.GetTarget() is Obj_AI_Minion)
                || !Targets.JungleMinions.Contains(Variables.Orbwalker.GetTarget() as Obj_AI_Minion))
            {
                return;
            }

            /// <summary>
            ///     The E JungleClear Logics.
            /// </summary>
            if (Vars.E.IsReady()
                && GameObjects.Player.HealthPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["clear"])
                && Vars.Menu["spells"]["e"]["clear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.E.CastOnUnit(Targets.JungleMinions[0]);
            }
        }

        #endregion
    }
}