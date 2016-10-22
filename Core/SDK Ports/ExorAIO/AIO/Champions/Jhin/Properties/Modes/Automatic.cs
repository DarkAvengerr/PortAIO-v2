
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Jhin
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The R Automatic Logic.
            /// </summary>
            if (Vars.R.IsReady() && Vars.R.Instance.Name.Equals("JhinRShot")
                && Vars.Menu["spells"]["r"]["logical"].GetValue<MenuBool>().Value)
            {
                if (
                    GameObjects.EnemyHeroes.Any(
                        t => t.IsValidTarget(Vars.R.Range) && !Jhin.Cone.IsOutside((Vector2)t.ServerPosition)))
                {
                    foreach (var target in
                        GameObjects.EnemyHeroes.Where(
                            t => t.IsValidTarget(Vars.R.Range) && !Jhin.Cone.IsOutside((Vector2)t.ServerPosition)))
                    {
                        if (Vars.Menu["spells"]["r"]["nearmouse"].GetValue<MenuBool>().Value)
                        {
                            Vars.R.Cast(
                                Vars.R.GetPrediction(
                                    GameObjects.EnemyHeroes.Where(
                                        t =>
                                        t.IsValidTarget(Vars.R.Range) && !Jhin.Cone.IsOutside((Vector2)t.ServerPosition))
                                        .OrderBy(o => o.Distance(Game.CursorPos))
                                        .First()).UnitPosition);
                            return;
                        }

                        Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                        return;
                    }
                }

                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                {
                    Vars.R.Cast(Game.CursorPos);
                }
            }

            if (Vars.R.Instance.Name.Equals("JhinRShot"))
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q LastHit Logic.
            /// </summary>
            if (Vars.Q.IsReady() && GameObjects.Player.HasBuff("JhinPassiveReload")
                && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Combo
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["lasthit"])
                && Vars.Menu["spells"]["q"]["lasthit"].GetValue<MenuSliderButton>().BValue)
            {
                foreach (var minion in
                    Targets.Minions.Where(
                        m =>
                        m.IsValidTarget(Vars.Q.Range)
                        && Vars.GetRealHealth(m) < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)))
                {
                    Vars.Q.CastOnUnit(minion);
                }
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() && Vars.Menu["spells"]["e"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t => Bools.IsImmobile(t) && !Invulnerable.Check(t) && t.IsValidTarget(Vars.E.Range)))
                {
                    Vars.E.Cast(
                        GameObjects.Player.ServerPosition.Extend(
                            target.ServerPosition,
                            GameObjects.Player.Distance(target) + target.BoundingRadius * 2));
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() && !GameObjects.Player.IsUnderEnemyTurret()
                && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        Bools.IsImmobile(t) && !Invulnerable.Check(t) && t.HasBuff("jhinespotteddebuff")
                        && t.IsValidTarget(Vars.W.Range - 150f)
                        && Vars.Menu["spells"]["w"]["whitelist"][t.ChampionName.ToLower()].GetValue<MenuBool>().Value))
                {
                    Vars.W.Cast(target.ServerPosition);
                }
            }
        }

        #endregion
    }
}