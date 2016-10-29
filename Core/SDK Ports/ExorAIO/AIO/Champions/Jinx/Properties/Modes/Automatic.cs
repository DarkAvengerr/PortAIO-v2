
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Jinx
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
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() && Vars.Menu["spells"]["e"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        Bools.IsImmobile(t) && t.IsValidTarget(Vars.E.Range)
                        && !Invulnerable.Check(t, DamageType.Magical, false)))
                {
                    if (Vars.GetImmobileBuffEndTime(target) >= Vars.E.Delay + Game.Ping)
                    {
                        Vars.E.Cast(
                            GameObjects.Player.ServerPosition.Extend(
                                target.ServerPosition,
                                (float)(GameObjects.Player.Distance(target) + Vars.E.Width)));
                    }
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() && !GameObjects.Player.IsUnderEnemyTurret()
                && GameObjects.Player.CountEnemyHeroesInRange(Vars.Q.Range) < 3
                && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t => Bools.IsImmobile(t) && !Invulnerable.Check(t) && t.IsValidTarget(Vars.W.Range - 100f)))
                {
                    if (!Vars.W.GetPrediction(target).CollisionObjects.Any())
                    {
                        Vars.W.Cast(target.ServerPosition);
                    }
                }
            }

            /// <summary>
            ///     The Semi-Automatic R Logic.
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
                    Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                }
            }
        }

        #endregion
    }
}