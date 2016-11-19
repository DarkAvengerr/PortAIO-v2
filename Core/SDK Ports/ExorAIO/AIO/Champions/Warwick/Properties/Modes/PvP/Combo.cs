
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Warwick
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
            if (Bools.HasSheenBuff() && Targets.Target.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() && GameObjects.Player.Spellbook.IsAutoAttacking
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
            {
                if (!Vars.Menu["miscellaneous"]["keeprmana"]
                    || GameObjects.Player.Mana > Vars.W.Instance.SData.Mana + Vars.R.Instance.SData.Mana)
                {
                    Vars.W.Cast();
                }
            }

            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuSliderButton>().BValue
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuSliderButton>().SValue
                > GameObjects.Player.HealthPercent)
            {
                if (!Vars.Menu["miscellaneous"]["keeprmana"]
                    || GameObjects.Player.Mana > Vars.Q.Instance.SData.Mana + Vars.R.Instance.SData.Mana)
                {
                    Vars.Q.CastOnUnit(Targets.Target);
                }
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady())
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        !t.IsUnderEnemyTurret() && t.IsValidTarget(Vars.R.Range)
                        && !t.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                        && Vars.Menu["spells"]["r"]["combo"].GetValue<MenuBool>().Value
                        && Vars.Menu["spells"]["r"]["whitelist"][t.ChampionName.ToLower()].GetValue<MenuBool>().Value))
                {
                    Vars.R.CastOnUnit(target);
                }
            }
        }

        #endregion
    }
}