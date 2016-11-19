
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Kalista
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
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
            /// <summary>
            ///     The Soulbound declaration.
            /// </summary>
            if (Kalista.SoulBound == null)
            {
                Kalista.SoulBound =
                    GameObjects.AllyHeroes.Find(
                        a => a.Buffs.Any(b => b.Caster.IsMe && b.Name.Contains("kalistacoopstrikeally")));
            }
            else
            {
                /// <summary>
                ///     The Automatic R Logic.
                /// </summary>
                if (Vars.R.IsReady()
                    && (Kalista.SoulBound.HealthPercent <= 10
                        || Health.GetPrediction(Kalista.SoulBound, (int)(250 + Game.Ping / 2f))
                        <= Kalista.SoulBound.MaxHealth / 4) && Kalista.SoulBound.CountEnemyHeroesInRange(800f) > 0
                    && Kalista.SoulBound.IsValidTarget(Vars.R.Range, false)
                    && Vars.Menu["spells"]["r"]["lifesaver"].GetValue<MenuBool>().Value)
                {
                    Vars.R.Cast();
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() && !GameObjects.Player.IsRecalling() && !GameObjects.Player.IsUnderEnemyTurret()
                && Variables.Orbwalker.ActiveMode == OrbwalkingMode.None
                && GameObjects.Player.CountEnemyHeroesInRange(1500f) == 0
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["logical"])
                && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuSliderButton>().BValue)
            {
                foreach (var loc in
                    Vars.Locations.Where(
                        l =>
                        GameObjects.Player.Distance(l) < Vars.W.Range
                        && !ObjectManager.Get<Obj_AI_Minion>()
                                .Any(m => m.Distance(l) < 1000f && m.CharData.BaseSkinName.Equals("kalistaspawn"))))
                {
                    Vars.W.Cast(loc);
                }
            }

            /// <summary>
            ///     The Automatic E Logics.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The E Before death Logic.
                /// </summary>
                if (Health.GetPrediction(GameObjects.Player, (int)(1000 + Game.Ping / 2f)) <= 0
                    && Vars.Menu["spells"]["e"]["ondeath"].GetValue<MenuBool>().Value)
                {
                    Vars.E.Cast();
                }

                /// <summary>
                ///     The E Minion Harass Logic.
                /// </summary>
                if (GameObjects.EnemyHeroes.Any(Bools.IsPerfectRendTarget)
                    && Vars.Menu["spells"]["e"]["harass"].GetValue<MenuSliderButton>().BValue
                    && Targets.Minions.Any(
                        m =>
                        Bools.IsPerfectRendTarget(m)
                        && Vars.GetRealHealth(m)
                        < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E)
                        + (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E, DamageStage.Buff)))
                {
                    /// <summary>
                    ///     Check for Mana Manager if not in combo mode and the killable minion is only one, else do not use it.
                    /// </summary>
                    if (Variables.Orbwalker.ActiveMode != OrbwalkingMode.Combo
                        && Targets.Minions.Count(
                            m =>
                            Bools.IsPerfectRendTarget(m)
                            && Vars.GetRealHealth(m)
                            < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E)
                            + (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E, DamageStage.Buff)) == 1)
                    {
                        if (GameObjects.Player.ManaPercent
                            < ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["harass"]))
                        {
                            return;
                        }
                    }

                    /// <summary>
                    ///     Check for E Whitelist if the harassable target is only one and there is only one killable minion, else do not use the whitelist.
                    /// </summary>
                    if (GameObjects.EnemyHeroes.Count(Bools.IsPerfectRendTarget) == 1
                        && Targets.Minions.Count(
                            m =>
                            Bools.IsPerfectRendTarget(m)
                            && Vars.GetRealHealth(m)
                            < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E)
                            + (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E, DamageStage.Buff)) == 1)
                    {
                        var hero = GameObjects.EnemyHeroes.FirstOrDefault(Bools.IsPerfectRendTarget);
                        if (hero != null
                            && !Vars.Menu["spells"]["e"]["whitelist"][hero.ChampionName.ToLower()].GetValue<MenuBool>()
                                    .Value)
                        {
                            return;
                        }
                    }

                    /// <summary>
                    ///     Check for invulnerability through all the harassable targets.
                    /// </summary>
                    if (
                        GameObjects.EnemyHeroes.Where(Bools.IsPerfectRendTarget)
                            .Any(target => Invulnerable.Check(target)))
                    {
                        return;
                    }

                    Vars.E.Cast();
                }

                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                if (Vars.Menu["spells"]["e"]["junglesteal"].GetValue<MenuBool>().Value)
                {
                    if (
                        Targets.JungleMinions.Any(
                            m =>
                            Bools.IsPerfectRendTarget(m)
                            && m.Health
                            < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E)
                            + (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.E, DamageStage.Buff)))
                    {
                        Vars.E.Cast();
                    }
                }
            }
        }

        #endregion
    }
}