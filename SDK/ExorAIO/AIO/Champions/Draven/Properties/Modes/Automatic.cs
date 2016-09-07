
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Draven
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

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
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Variables.Orbwalker.ActiveMode != OrbwalkingMode.None
                && GameObjects.Player.GetBuffCount("dravenspinningattack") < 1
                && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                Vars.Q.Cast();
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["bool"].GetValue<MenuBool>().Value
                && Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
            {
                if (
                    !GameObjects.EnemyHeroes.Any(
                        t =>
                        !Invulnerable.Check(t) && t.IsValidTarget(Vars.R.Range)
                        && Vars.Menu["spells"]["r"]["whitelist"][Targets.Target.ChampionName.ToLower()]
                               .GetValue<MenuBool>().Value))
                {
                    return;
                }

                Vars.R.Cast(
                    Vars.R.GetPrediction(
                        GameObjects.EnemyHeroes.Where(
                            t =>
                            !Invulnerable.Check(t) && t.IsValidTarget(Vars.R.Range)
                            && Vars.Menu["spells"]["r"]["whitelist"][Targets.Target.ChampionName.ToLower()]
                                   .GetValue<MenuBool>().Value).OrderBy(o => o.Health).First()).UnitPosition);
            }
        }

        #endregion
    }
}