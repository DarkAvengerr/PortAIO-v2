
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lucian
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Harass(EventArgs args)
        {
            if (
                !GameObjects.EnemyHeroes.Any(
                    t =>
                    !Invulnerable.Check(t) && !t.IsValidTarget(Vars.Q.Range) && t.IsValidTarget(Vars.Q2.Range - 50f)
                    && Vars.Menu["spells"]["q"]["whitelist"][t.ChampionName.ToLower()].GetValue<MenuBool>().Value))
            {
                return;
            }

            /// <summary>
            ///     The Extended Q Mixed Harass Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["extended"]["mixed"])
                && Vars.Menu["spells"]["q"]["extended"]["mixed"].GetValue<MenuSliderButton>().BValue)
            {
                /// <summary>
                ///     Through enemy minions.
                /// </summary>
                foreach (var minion 
                    in from minion in Targets.Minions.Where(m => m.IsValidTarget(Vars.Q.Range))
                       let polygon =
                           new Geometry.Rectangle(
                           GameObjects.Player.ServerPosition,
                           GameObjects.Player.ServerPosition.Extend(minion.ServerPosition, Vars.Q2.Range - 50f),
                           Vars.Q2.Width)
                       where
                           !polygon.IsOutside(
                               (Vector2)
                               Vars.Q2.GetPrediction(
                                   GameObjects.EnemyHeroes.FirstOrDefault(
                                       t =>
                                       !Invulnerable.Check(t) && !t.IsValidTarget(Vars.Q.Range)
                                       && t.IsValidTarget(Vars.Q2.Range - 50f)
                                       && Vars.Menu["spells"]["q"]["whitelist"][t.ChampionName.ToLower()]
                                              .GetValue<MenuBool>().Value)).UnitPosition)
                       select minion)
                {
                    Vars.Q.CastOnUnit(minion);
                }
            }
        }

        #endregion
    }
}