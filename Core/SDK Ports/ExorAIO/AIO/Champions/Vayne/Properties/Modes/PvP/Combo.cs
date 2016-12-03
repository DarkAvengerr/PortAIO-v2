
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

    using SharpDX;

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
            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() && !GameObjects.Player.IsDashing()
                && Vars.Menu["spells"]["e"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        !t.IsDashing() && t.IsValidTarget(Vars.E.Range)
                        && !Invulnerable.Check(t, DamageType.Magical, false)
                        && !t.IsValidTarget(GameObjects.Player.BoundingRadius)
                        && Vars.Menu["spells"]["e"]["whitelist"][t.ChampionName.ToLower()].GetValue<MenuBool>().Value))
                {
                    for (var i = 1; i < 10; i++)
                    {
                        var position = target.ServerPosition;
                        var prediction = Vars.E.GetPrediction(target).UnitPosition;
                        var prediction2 = Vars.E2.GetPrediction(target).UnitPosition;
                        var vector = Vector3.Normalize(target.ServerPosition - GameObjects.Player.ServerPosition);
                        if ((position + vector * (i * 42)).IsWall() && (position + vector * (i * 45)).IsWall()
                            && (prediction + vector * (i * 42)).IsWall() && (prediction + vector * (i * 45)).IsWall()
                            && (prediction2 + vector * (i * 42)).IsWall() && (prediction2 + vector * (i * 45)).IsWall())
                        {
                            Vars.E.CastOnUnit(target);
                        }
                    }
                }
            }
        }

        #endregion
    }
}