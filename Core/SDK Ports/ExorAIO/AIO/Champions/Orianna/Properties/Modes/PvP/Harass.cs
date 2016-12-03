
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
            if (!Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The Harass W Logic.
            /// </summary>
            if (Vars.W.IsReady()
                && GameObjects.EnemyHeroes.Any(
                    t =>
                    !Invulnerable.Check(Targets.Target, DamageType.Magical, false)
                    && t.Distance((Vector2)Orianna.GetBallPosition()) < Vars.W.Range)
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["harass"])
                && Vars.Menu["spells"]["w"]["harass"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.W.Cast();
            }

            if (Bools.HasSheenBuff() && Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                || !Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target, DamageType.Magical, false))
            {
                return;
            }

            /// <summary>
            ///     The Harass Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["harass"])
                && Vars.Menu["spells"]["q"]["harass"].GetValue<MenuSliderButton>().BValue)
            {
                if (Vars.E.IsReady() && Vars.Menu["spells"]["e"]["logical"].GetValue<MenuBool>().Value
                    && ((Vector2)Orianna.GetBallPosition()).Distance((Vector2)GameObjects.Player.ServerPosition)
                    > GameObjects.Player.GetRealAutoAttackRange()
                    && ((Vector2)Orianna.GetBallPosition()).Distance((Vector2)Targets.Target.ServerPosition)
                    > ((Vector2)Orianna.GetBallPosition()).Distance((Vector2)GameObjects.Player.ServerPosition))
                {
                    Vars.E.Cast(GameObjects.Player);
                }
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).CastPosition);
            }
        }

        #endregion
    }
}