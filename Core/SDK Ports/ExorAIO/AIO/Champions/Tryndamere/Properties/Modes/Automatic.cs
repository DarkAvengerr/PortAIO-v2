
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Tryndamere
{
    using System;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
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
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                if ((GameObjects.Player.HealthPercent
                     <= Vars.Menu["spells"]["q"]["health"].GetValue<MenuSliderButton>().SValue
                     || !Vars.Menu["spells"]["q"]["health"].GetValue<MenuSliderButton>().BValue)
                    && (GameObjects.Player.ManaPercent
                        >= Vars.Menu["spells"]["q"]["fury"].GetValue<MenuSliderButton>().SValue
                        || !Vars.Menu["spells"]["q"]["fury"].GetValue<MenuSliderButton>().BValue))
                {
                    Vars.Q.Cast();
                }
            }

            /// <summary>
            ///     The Lifesaver R Logic.
            /// </summary>
            if (Vars.R.IsReady() && GameObjects.Player.CountEnemyHeroesInRange(1500f) > 0
                && Vars.Menu["spells"]["r"]["lifesaver"].GetValue<MenuBool>().Value)
            {
                if (GameObjects.Player.HealthPercent < 17
                    || Health.GetPrediction(GameObjects.Player, (int)(250 + Game.Ping / 2f))
                    <= GameObjects.Player.MaxHealth / 5)
                {
                    Vars.R.Cast();
                }
            }
        }

        #endregion
    }
}