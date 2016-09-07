
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Renekton
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;

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
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && GameObjects.Player.ManaPercent >= 50 && !GameObjects.Player.IsUnderEnemyTurret()
                && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                if (GameObjects.Player.HasBuff("RenektonPreExecute")
                    || Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                {
                    return;
                }

                foreach (var target in GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(Vars.Q.Range)))
                {
                    if (!Vars.W.IsReady() || !target.IsValidTarget(Vars.W.Range))
                    {
                        Vars.Q.Cast();
                    }
                }
            }

            /// <summary>
            ///     The Automatic R Logic.
            /// </summary>
            if (Vars.R.IsReady() && GameObjects.Player.CountEnemyHeroesInRange(700f) > 0)
            {
                if (Health.GetPrediction(GameObjects.Player, (int)(250 + Game.Ping / 2f))
                    <= GameObjects.Player.MaxHealth / 6
                    && Vars.Menu["spells"]["r"]["lifesaver"].GetValue<MenuBool>().Value)
                {
                    Vars.R.Cast();
                }
                else if (GameObjects.Player.CountEnemyHeroesInRange(Vars.R.Range) >= 2
                         && Vars.Menu["spells"]["r"]["aoe"].GetValue<MenuBool>().Value)
                {
                    Vars.R.Cast();
                }
            }
        }

        #endregion
    }
}