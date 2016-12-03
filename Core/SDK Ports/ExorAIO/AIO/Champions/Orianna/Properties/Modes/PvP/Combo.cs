
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Orianna
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
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady()
                && GameObjects.EnemyHeroes.Any(
                    t => t.IsValidTarget() && t.Distance((Vector2)Orianna.GetBallPosition) < Vars.W.Range)
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.W.Cast();
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady()
                && GameObjects.Player.Mana - Vars.E.Instance.SData.Mana
                > Vars.Q.Instance.SData.Mana + Vars.W.Instance.SData.Mana
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
            {
                foreach (var ally in
                    GameObjects.AllyHeroes.OrderBy(o => o.Health).Where(t => t.IsValidTarget(Vars.E.Range, false)))
                {
                    var polygon = new Geometry.Rectangle(
                        ally.ServerPosition,
                        ally.ServerPosition.Extend(
                            (Vector2)Orianna.GetBallPosition,
                            ally.Distance((Vector2)Orianna.GetBallPosition)),
                        Vars.E.Width);

                    if (
                        GameObjects.EnemyHeroes.Any(
                            t =>
                            t.IsValidTarget() && !Invulnerable.Check(t, DamageType.Magical, false)
                            && !polygon.IsOutside((Vector2)t.ServerPosition)))
                    {
                        Vars.E.CastOnUnit(ally);
                    }
                }
            }

            if (Bools.HasSheenBuff() && Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                || !Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The Combo Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                if (((Vector2)Orianna.GetBallPosition).Distance((Vector2)Targets.Target.ServerPosition)
                    > ((Vector2)Orianna.GetBallPosition).Distance((Vector2)GameObjects.Player.ServerPosition))
                {
                    if (Vars.E.IsReady() && Vars.Menu["spells"]["e"]["logical"].GetValue<MenuBool>().Value)
                    {
                        Vars.E.Cast(GameObjects.Player);
                    }
                }
                else
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).CastPosition);
                }
            }
        }

        #endregion
    }
}