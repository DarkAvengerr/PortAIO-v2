
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Vayne
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
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The Q KillSteal Logic.
            /// </summary>
            if (Vars.Q.IsReady() && !GameObjects.Player.Spellbook.IsAutoAttacking
                && Vars.Menu["spells"]["q"]["killsteal"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        t.IsValidTarget(Vars.Q.Range) && !t.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                        && t.CountEnemyHeroesInRange(700f) <= 2
                        && Vars.GetRealHealth(t)
                        < GameObjects.Player.GetAutoAttackDamage(t)
                        + (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.Q)
                        + (t.GetBuffCount("vaynesilvereddebuff") == 2
                               ? (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.W)
                               : 0)))
                {
                    Vars.Q.Cast(target.ServerPosition);
                    Variables.Orbwalker.ForceTarget = target;
                }
            }

            /// <summary>
            ///     The E KillSteal Logic.
            /// </summary>
            if (Vars.E.IsReady() && !GameObjects.Player.Spellbook.IsAutoAttacking
                && Vars.Menu["spells"]["e"]["killsteal"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        t.IsValidTarget(Vars.E.Range)
                        && Vars.GetRealHealth(t)
                        < (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.E)
                        + (t.GetBuffCount("vaynesilvereddebuff") == 2
                               ? (float)GameObjects.Player.GetSpellDamage(t, SpellSlot.W)
                               : 0)))
                {
                    Vars.E.CastOnUnit(target);
                }
            }
        }

        #endregion
    }
}