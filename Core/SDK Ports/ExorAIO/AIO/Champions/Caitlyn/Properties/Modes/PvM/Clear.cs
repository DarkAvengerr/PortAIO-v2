
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Caitlyn
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp.SDK;
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
            ///     The Clear Q Logics.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["jungleclear"])
                    && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }

                /// <summary>
                ///     The LaneClear Q Logics.
                /// </summary>
                else if (Targets.Minions.Any()
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["laneclear"])
                         && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    /// <summary>
                    ///     The Aggressive LaneClear Q Logic.
                    /// </summary>
                    if (GameObjects.EnemyHeroes.Any(t => !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range)))
                    {
                        if (Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3
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
                            Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).Position);
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
        }

        #endregion
    }
}