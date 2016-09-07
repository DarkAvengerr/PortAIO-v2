
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.KogMaw
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

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
            if (Bools.HasSheenBuff() && GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.AaRange)))
            {
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() && GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.W.Range))
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.W.Cast();
            }
            if (!Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && GameObjects.Player.Mana > Vars.Q.Instance.SData.Mana + Vars.W.Instance.SData.Mana
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                if (!Vars.Q.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
                }
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() && Targets.Target.IsValidTarget(Vars.E.Range - 100f)
                && GameObjects.Player.Mana > Vars.E.Instance.SData.Mana + Vars.W.Instance.SData.Mana
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).UnitPosition);
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() && Targets.Target.HealthPercent < 50 && Targets.Target.IsValidTarget(Vars.R.Range)
                && GameObjects.Player.Mana
                > Vars.W.Instance.SData.Mana + 50 * (GameObjects.Player.GetBuffCount("kogmawlivingartillerycost") + 1)
                && Vars.Menu["spells"]["r"]["combo"].GetValue<MenuSliderButton>().BValue
                && Vars.Menu["spells"]["r"]["combo"].GetValue<MenuSliderButton>().SValue
                > GameObjects.Player.GetBuffCount("kogmawlivingartillerycost"))
            {
                Vars.R.Cast(Vars.R.GetPrediction(Targets.Target).CastPosition);
            }
        }

        #endregion
    }
}