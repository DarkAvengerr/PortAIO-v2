
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Anivia
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
            if (Bools.HasSheenBuff() || !Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Wall Logic.
            /// </summary>
            if (Vars.W.IsReady() && GameObjects.Player.ManaPercent > 30 && Targets.Target.IsValidTarget(Vars.W.Range)
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value
                && Vars.Menu["spells"]["w"]["whitelist"][Targets.Target.ChampionName.ToLower()].GetValue<MenuBool>()
                       .Value)
            {
                if (
                    GameObjects.Player.Distance(
                        GameObjects.Player.ServerPosition.Extend(
                            Targets.Target.ServerPosition,
                            GameObjects.Player.Distance(Targets.Target) + Targets.Target.BoundingRadius + 20f))
                    < Vars.W.Range)
                {
                    Vars.W.Cast(
                        GameObjects.Player.ServerPosition.Extend(
                            Targets.Target.ServerPosition,
                            GameObjects.Player.Distance(Targets.Target) + Targets.Target.BoundingRadius + 20f));
                }
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(t => t.HasBuff("chilled") && t.IsValidTarget(Vars.E.Range)))
                {
                    Vars.E.CastOnUnit(target);
                }
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() && Targets.Target.IsValidTarget(Vars.R.Range)
                && GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1
                && Vars.Menu["spells"]["r"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.R.Cast(Targets.Target.ServerPosition);
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Targets.Target.IsValidTarget(Vars.Q.Range)
                && GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
            }
        }

        #endregion
    }
}