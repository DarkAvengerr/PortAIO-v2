
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.MissFortune
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
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
            /// <summary>
            ///     The Extended Q Mixed Harass Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["extended"]["mixed"])
                && Vars.Menu["spells"]["q"]["extended"]["mixed"].GetValue<MenuSliderButton>().BValue)
            {
                var passiveMultiplier = GameObjects.Player.Level < 4
                                            ? 0.25
                                            : GameObjects.Player.Level < 7
                                                  ? 0.3
                                                  : GameObjects.Player.Level < 9
                                                        ? 0.35
                                                        : GameObjects.Player.Level < 11
                                                              ? 0.4
                                                              : GameObjects.Player.Level < 13 ? 0.45 : 0.5;

                /// <summary>
                ///     Through enemy minions.
                /// </summary>
                foreach (var minion 
                    in
                    from minion in
                        Targets.Minions.Where(
                            m =>
                            m.IsValidTarget(Vars.Q.Range)
                            && (!Vars.Menu["spells"]["q"]["extended"]["mixedkill"].GetValue<MenuBool>().Value
                                || Vars.GetRealHealth(m)
                                < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)
                                + (MissFortune.PassiveTarget == null
                                   || m.NetworkId != MissFortune.PassiveTarget?.NetworkId
                                       ? GameObjects.Player.TotalAttackDamage * passiveMultiplier
                                       : 0)))
                    let polygon =
                        new Geometry.Sector(
                        (Vector2)minion.ServerPosition,
                        (Vector2)
                        minion.ServerPosition.Extend(GameObjects.Player.ServerPosition, -(Vars.Q2.Range - Vars.Q.Range)),
                        40f * (float)Math.PI / 180f,
                        Vars.Q2.Range - Vars.Q.Range - 50f)
                    let target =
                        GameObjects.EnemyHeroes.FirstOrDefault(
                            t =>
                            !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q2.Range)
                            && (t.NetworkId == MissFortune.PassiveTarget?.NetworkId
                                || Targets.Minions.All(m => polygon.IsOutside((Vector2)m.ServerPosition)))
                            && Vars.Menu["spells"]["q"]["whitelist"][t.ChampionName.ToLower()].GetValue<MenuBool>()
                                   .Value)
                    where target != null
                    where
                        !polygon.IsOutside((Vector2)target.ServerPosition)
                        && !polygon.IsOutside((Vector2)Vars.Q.GetPrediction(target).UnitPosition)
                    select minion)
                {
                    Vars.Q.CastOnUnit(minion);
                }
            }
        }

        #endregion
    }
}