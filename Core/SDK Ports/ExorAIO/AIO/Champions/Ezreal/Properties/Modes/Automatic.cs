
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Ezreal
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
            ///     The Q LastHit Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Combo
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["farmhelper"])
                && Vars.Menu["spells"]["q"]["farmhelper"].GetValue<MenuSliderButton>().BValue)
            {
                foreach (var minion in
                    Targets.Minions.Where(
                        m =>
                        !m.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                        && Vars.GetRealHealth(m) > GameObjects.Player.GetAutoAttackDamage(m)
                        && Vars.GetRealHealth(m) < (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q))
                        .OrderBy(o => o.MaxHealth))
                {
                    if (!Vars.Q.GetPrediction(minion).CollisionObjects.Any())
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(minion).UnitPosition);
                    }
                }
            }

            /// <summary>
            ///     The Tear Stacking Logic.
            /// </summary>
            if (Vars.Q.IsReady() && !Targets.Minions.Any() && Bools.HasTear(GameObjects.Player)
                && Variables.Orbwalker.ActiveMode == OrbwalkingMode.None
                && GameObjects.Player.CountEnemyHeroesInRange(1500) == 0
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["miscellaneous"]["tear"])
                && Vars.Menu["miscellaneous"]["tear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.Cast(Game.CursorPos);
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
                        !Invulnerable.Check(t, DamageType.Magical, false) && t.IsValidTarget(2000f)
                        && Vars.Menu["spells"]["r"]["whitelist"][t.ChampionName.ToLower()].GetValue<MenuBool>().Value)
                        .OrderBy(o => o.Health)
                        .FirstOrDefault();
                if (target != null)
                {
                    Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                }
            }

            if (GameObjects.Player.TotalAttackDamage < GameObjects.Player.TotalMagicalDamage)
            {
                return;
            }

            /// <summary>
            ///     Initializes the orbwalkingmodes.
            /// </summary>
            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    if (!GameObjects.EnemyHeroes.Contains(Variables.Orbwalker.GetTarget()))
                    {
                        return;
                    }

                    break;
                case OrbwalkingMode.LaneClear:
                    if (!(Variables.Orbwalker.GetTarget() is Obj_HQ)
                        && !(Variables.Orbwalker.GetTarget() is Obj_AI_Turret)
                        && !(Variables.Orbwalker.GetTarget() is Obj_BarracksDampener))
                    {
                        return;
                    }

                    break;
                default:
                    if (!GameObjects.Jungle.Contains(Variables.Orbwalker.GetTarget())
                        && !GameObjects.EnemyHeroes.Contains(Variables.Orbwalker.GetTarget())
                        && !(Variables.Orbwalker.GetTarget() is Obj_HQ)
                        && !(Variables.Orbwalker.GetTarget() is Obj_AI_Turret)
                        && !(Variables.Orbwalker.GetTarget() is Obj_BarracksDampener))
                    {
                        return;
                    }

                    break;
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["logical"])
                && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuSliderButton>().BValue)
            {
                foreach (var target in
                    GameObjects.AllyHeroes.Where(
                        t => !t.IsMe && t.Spellbook.IsAutoAttacking && t.IsValidTarget(Vars.W.Range, false)))
                {
                    Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                }
            }
        }

        #endregion
    }
}