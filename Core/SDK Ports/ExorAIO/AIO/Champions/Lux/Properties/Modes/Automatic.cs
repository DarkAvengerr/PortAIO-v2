
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lux
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
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
            ///     The E Missile Manager.
            /// </summary>
            if (Vars.E.IsReady() && Lux.EMissile != null
                && GameObjects.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 1)
            {
                switch (Variables.Orbwalker.ActiveMode)
                {
                    /// <summary>
                    ///     The E Combo Logic.
                    /// </summary>
                    case OrbwalkingMode.Combo:
                        if (
                            GameObjects.EnemyHeroes.Any(
                                t =>
                                !Bools.IsImmobile(t) && !t.HasBuff("luxilluminatingfraulein")
                                && t.Distance(Lux.EMissile.Position) < Vars.E.Width - 10f))
                        {
                            Vars.E.Cast();
                        }
                        break;

                    /// <summary>
                    ///     The E Clear Logic.
                    /// </summary>
                    case OrbwalkingMode.LaneClear:
                        if (Targets.EMinions.Any() && Targets.EMinions.Count >= 3)
                        {
                            Vars.E.Cast();
                        }
                        else if (Targets.EJungleMinions.Any())
                        {
                            Vars.E.Cast();
                        }
                        break;
                }
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        Bools.IsImmobile(t) && t.IsValidTarget(Vars.Q.Range)
                        && !Invulnerable.Check(t, DamageType.Magical)))
                {
                    if (!Vars.Q.GetPrediction(target).CollisionObjects.Any())
                    {
                        Vars.Q.Cast(target.ServerPosition);
                    }
                }
            }
        }

        #endregion
    }
}