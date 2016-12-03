
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Karma
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

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
        public static void Automatic(EventArgs args)
        {
            Variables.Orbwalker.AttackState = (!GameObjects.EnemyHeroes.Any(h => h.Buffs.Any(b => b.Caster.IsMe && b.Name.Equals("KarmaSpiritBind"))));

            if (GameObjects.Player.IsRecalling() || GameObjects.Player.HasBuff("KarmaMantra") && GameObjects.Player.HealthPercent
                <= Vars.Menu["spells"]["w"]["lifesaver"].GetValue<MenuSliderButton>().SValue
                && Vars.Menu["spells"]["w"]["lifesaver"].GetValue<MenuSliderButton>().BValue)
            {
                return;
            }

            /// <summary>
            ///     The AoE E Logic.
            /// </summary>
            if (Vars.E.IsReady() && Vars.R.IsReady() && Vars.Menu["spells"]["r"]["empe"].GetValue<MenuBool>().Value
                && GameObjects.Player.CountEnemyHeroesInRange(2000f) >= 2
                && GameObjects.Player.CountAllyHeroesInRange(600f)
                >= Vars.Menu["spells"]["e"]["aoe"].GetValue<MenuSliderButton>().SValue + 1
                && Vars.Menu["spells"]["e"]["aoe"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.R.Cast();
                Vars.E.CastOnUnit(GameObjects.Player);
            }
        }

        #endregion
    }
}