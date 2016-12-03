
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Jinx
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
        public static void Combo(EventArgs args)
        {
            var target = Variables.Orbwalker.GetTarget() as AIHeroClient ?? Targets.Target;
            /// <summary>
            ///     The Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && target != null
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuSliderButton>().BValue)
            {
                const float SplashRange = 160f;
                var isUsingFishBones = GameObjects.Player.HasBuff("JinxQ");
                var minSplashRangeEnemies = Vars.Menu["spells"]["q"]["combo"].GetValue<MenuSliderButton>().SValue;

                //CountEnemiesInRange takes into account the main target too,
                //so if there is another enemy champion near the main target, xd.CountEnemiesInRange(near_pos) will return 2 (xd + the other enemy) and not 1 (the other enemy only).
                if (!isUsingFishBones)
                {
                    if (GameObjects.Player.Distance(target) > Vars.PowPow.Range
                        || target.CountEnemyHeroesInRange(SplashRange) >= minSplashRangeEnemies)
                    {
                        Vars.Q.Cast();
                    }
                }
            }

            if (Bools.HasSheenBuff() && Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                || !Targets.Target.IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The E AoE Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.Target.IsValidTarget(Vars.E.Range)
                && !Invulnerable.Check(Targets.Target, DamageType.Magical, false)
                && Targets.Target.CountEnemyHeroesInRange(Vars.E.Width)
                >= Vars.Menu["spells"]["e"]["aoe"].GetValue<MenuSliderButton>().SValue
                && Vars.Menu["spells"]["e"]["aoe"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.E.Cast(
                    GameObjects.Player.ServerPosition.Extend(
                        Targets.Target.ServerPosition,
                        GameObjects.Player.Distance(Targets.Target) + Targets.Target.BoundingRadius * 2));
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() && !GameObjects.Player.IsUnderEnemyTurret()
                && Targets.Target.IsValidTarget(Vars.W.Range - 100f)
                && GameObjects.Player.CountEnemyHeroesInRange(Vars.Q.Range) < 3
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
            {
                if (!Vars.W.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
                    Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }

        #endregion
    }
}