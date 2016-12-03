
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
        public static void Combo(EventArgs args)
        {
            /// <summary>
            ///     The Q Extended Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Vars.Menu["spells"]["q"]["extended"]["excombo"].GetValue<MenuBool>().Value)
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
                            && (!Vars.Menu["spells"]["q"]["extended"]["excombokill"].GetValue<MenuBool>().Value
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
                                || Targets.Minions.All(m => polygon.IsOutside((Vector2)m.ServerPosition))))
                    where target != null
                    where
                        !polygon.IsOutside((Vector2)target.ServerPosition)
                        && !polygon.IsOutside((Vector2)Vars.Q.GetPrediction(target).UnitPosition)
                    select minion)
                {
                    Vars.Q.CastOnUnit(minion);
                }
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() && !Bools.HasSheenBuff()
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
            {
                if (
                    GameObjects.EnemyHeroes.Any(
                        t =>
                        t.IsValidTarget(
                            Vars.Menu["spells"]["w"]["engager"].GetValue<MenuBool>().Value
                                ? Vars.R.Range
                                : GameObjects.Player.GetRealAutoAttackRange())))
                {
                    Vars.W.Cast();
                }
            }
            if (!Targets.Target.IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.Target.IsValidTarget(Vars.E.Range)
                && !Invulnerable.Check(Targets.Target, DamageType.Magical, false)
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value
                && GameObjects.Player.Mana - Vars.E.Instance.SData.Mana > Vars.R.Instance.SData.Mana)
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).CastPosition);
            }
        }

        #endregion
    }
}