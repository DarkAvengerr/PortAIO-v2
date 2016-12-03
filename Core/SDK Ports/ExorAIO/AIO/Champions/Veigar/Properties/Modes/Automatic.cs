
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Veigar
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Tear Stacking Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Bools.HasTear(GameObjects.Player) && !GameObjects.Player.IsRecalling()
                && Variables.Orbwalker.ActiveMode == OrbwalkingMode.None
                && GameObjects.Player.CountEnemyHeroesInRange(1500) == 0
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["miscellaneous"]["tear"])
                && Vars.Menu["miscellaneous"]["tear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.Cast(Game.CursorPos);
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        Bools.IsImmobile(t) && t.IsValidTarget(Vars.W.Range)
                        && !Invulnerable.Check(t, DamageType.Magical, false)))
                {
                    Vars.W.Cast(target.ServerPosition);
                }
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady()
                && Vars.E.GetPrediction(Targets.Target).AoeTargetsHitCount
                >= Vars.Menu["spells"]["e"]["enemies"].GetValue<MenuSliderButton>().SValue
                && Vars.Menu["spells"]["e"]["enemies"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).CastPosition);
            }
        }

        #endregion
    }
}