
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Caitlyn
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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic W Logic. 
            /// </summary>
            if (Vars.W.IsReady() && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        Bools.IsImmobile(t) && t.IsValidTarget(Vars.W.Range) && !t.HasBuff("caitlynyordletrapinternal"))
                    )
                {
                    if (Vars.GetImmobileBuffEndTime(target) >= Vars.W.Delay + Game.Ping)
                    {
                        Vars.W.Cast(
                            GameObjects.Player.ServerPosition.Extend(
                                target.ServerPosition,
                                (float)(GameObjects.Player.Distance(target) + Vars.W.Width * 1.25)));
                    }
                }
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.CountEnemyHeroesInRange(GameObjects.Player.GetRealAutoAttackRange()) < 3
                && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        Bools.IsImmobile(t) && !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range)
                        && t.HasBuff("caitlynyordletrapsight")))
                {
                    Vars.Q.Cast(target.ServerPosition);
                }
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["bool"].GetValue<MenuBool>().Value
                && Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
            {
                var target =
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        !Invulnerable.Check(t) && t.IsValidTarget(Vars.R.Range)
                        && Vars.Menu["spells"]["r"]["whitelist"][t.ChampionName.ToLower()].GetValue<MenuBool>().Value)
                        .OrderBy(o => o.Health)
                        .FirstOrDefault();
                if (target != null)
                {
                    Vars.R.CastOnUnit(target);
                }
            }
        }

        #endregion
    }
}