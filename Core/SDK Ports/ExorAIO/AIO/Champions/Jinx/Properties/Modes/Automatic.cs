
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
            ///     The Q Switching Logics.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     PowPow.Range -> FishBones Logics.
                /// </summary>
                if (!GameObjects.Player.HasBuff("JinxQ"))
                {
                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        /// <summary>
                        ///     The Q Combo Enable Logics,
                        ///     The Q Harass Enable Logics.
                        /// </summary>
                        case OrbwalkingMode.Combo:
                        case OrbwalkingMode.Hybrid:

                            /// <summary>
                            ///     Enable if:
                            ///     If you are in combo mode, the combo option is enabled. (Option check).
                            /// </summary>
                            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                            {
                                if (!Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
                                {
                                    //Console.WriteLine("ExorAIO: Jinx - Combo - Option Block.");
                                    return;
                                }
                            }

                            /// <summary>
                            ///     Enable if:
                            ///     If you are in mixed mode, the mixed option is enabled.. (Option check).
                            ///     and respects the ManaManager check. (Mana check).
                            /// </summary>
                            else if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
                            {
                                if (GameObjects.Player.ManaPercent
                                    < ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["harass"])
                                    || !Vars.Menu["spells"]["q"]["harass"].GetValue<MenuSliderButton>().BValue)
                                {
                                    //Console.WriteLine("ExorAIO: Jinx - Hybrid - ManaManager or Option Block.");
                                    return;
                                }
                            }

                            /// <summary>
                            ///     Enable if:
                            ///     No hero in PowPow Range but 1 or more heroes in FishBones range. (Range Logic).
                            /// </summary>
                            if (!GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.PowPow.Range))
                                && GameObjects.EnemyHeroes.Any(
                                    t2 =>
                                    t2.IsValidTarget(
                                        Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid
                                            ? Vars.Q.Range
                                            : Vars.W.Range)))
                            {
                                //Console.WriteLine("ExorAIO: Jinx - Combo/Hybrid - Enabled for Range Check.");
                                Vars.Q.Cast();
                                return;
                            }

                            break;

                        /// <summary>
                        ///     The Q Clear Enable Logics.
                        /// </summary>
                        case OrbwalkingMode.LaneClear:

                            /// <summary>
                            ///     Start if:
                            ///     It respects the ManaManager Check, (Mana check),
                            ///     The Clear Option is enabled. (Option check).
                            /// </summary>
                            if (GameObjects.Player.ManaPercent
                                < ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["clear"])
                                || !Vars.Menu["spells"]["q"]["clear"].GetValue<MenuSliderButton>().BValue)
                            {
                                //Console.WriteLine("ExorAIO: Jinx - Clear - ManaManager or Option Block.");
                                return;
                            }

                            /// <summary>
                            ///     The LaneClear Logics.
                            /// </summary>
                            if (
                                Targets.Minions.Any(
                                    m => Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m) * 1.1))
                            {
                                /// <summary>
                                ///     Disable if:
                                ///     The player has Runaan's Hurricane and there are more than 1 hittable Minions..
                                ///     And there more than 2 killable minions in Q explosion range (Lane AoE Logic).
                                /// </summary>
                                if (Items.HasItem(3085) && Targets.Minions.Count > 1
                                    || Targets.Minions.Where(
                                        m => Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m) * 1.1)
                                           .Count(
                                               m2 =>
                                               m2.Distance(
                                                   Targets.Minions.First(
                                                       m =>
                                                       Vars.GetRealHealth(m)
                                                       < GameObjects.Player.GetAutoAttackDamage(m) * 1.1)) < 250f) >= 3)
                                {
                                    Vars.Q.Cast();

                                    //Console.WriteLine("ExorAIO: Jinx - LaneClear - Enabled for AoE Check.");
                                    return;
                                }
                            }

                            /// <summary>
                            ///     The JungleClear Logics.
                            /// </summary>
                            else if (Targets.JungleMinions.Any())
                            {
                                /// <summary>
                                ///     Enable if:
                                ///     No monster in PowPow Range and at least 1 monster in Fishbones Range.. (Jungle Range Logic).
                                /// </summary>
                                if (!Targets.JungleMinions.Any(m => m.IsValidTarget(Vars.PowPow.Range)))
                                {
                                    Vars.Q.Cast();

                                    //Console.WriteLine("ExorAIO: Jinx - JungleClear - Enabled for Range Check.");
                                    return;
                                }

                                /// <summary>
                                ///     Enable if:
                                ///     More or equal than 1 monster in explosion range from the target monster. (Lane AoE Logic).
                                /// </summary>
                                if (Targets.JungleMinions.Count(m2 => m2.Distance(Targets.JungleMinions[0]) < 250f) >= 2)
                                {
                                    Vars.Q.Cast();

                                    //Console.WriteLine("ExorAIO: Jinx - JungleClear - Enabled for AoE Check.");
                                    return;
                                }
                            }

                            break;

                        /// <summary>
                        ///     The Q LastHit Disable Logic.
                        /// </summary>
                        case OrbwalkingMode.LastHit:

                            /// <summary>
                            ///     Start if:
                            ///     It respects the ManaManager Check, (Mana check).
                            ///     The LastHit Option is enabled. (Option check).
                            /// </summary>
                            if (GameObjects.Player.ManaPercent
                                < ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["lasthit"])
                                || !Vars.Menu["spells"]["q"]["lasthit"].GetValue<MenuSliderButton>().BValue)
                            {
                                //Console.WriteLine("ExorAIO: Jinx - LastHit - ManaManager or Option Block.");
                                return;
                            }

                            /// <summary>
                            ///     Enable if:
                            ///     Any killable minion in FishBones Range and no killable minions in PowPow Range. (LastHit Range Logic).
                            /// </summary>
                            if (
                                Targets.Minions.Any(
                                    m =>
                                    !m.IsValidTarget(Vars.PowPow.Range)
                                    && m.Health < GameObjects.Player.GetAutoAttackDamage(m) * 1.1))
                            {
                                if (
                                    !Targets.Minions.Any(
                                        m =>
                                        m.IsValidTarget(Vars.PowPow.Range)
                                        && m.Health < GameObjects.Player.GetAutoAttackDamage(m)))
                                {
                                    Vars.Q.Cast();

                                    //Console.WriteLine("ExorAIO: Jinx - LastHit - Enabled.");
                                    return;
                                }
                            }

                            break;
                    }
                }

                /// <summary>
                ///     FishBones -> PowPow.Range Logics.
                /// </summary>
                else
                {
                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        /// <summary>
                        ///     The Q Combo Enable Logics,
                        ///     The Q Harass Enable Logics.
                        /// </summary>
                        case OrbwalkingMode.Combo:
                        case OrbwalkingMode.Hybrid:

                            /// <summary>
                            ///     Disable if:
                            ///     If you are in combo mode, the combo option is disabled. (Option check).
                            /// </summary>
                            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                            {
                                if (!Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
                                {
                                    Vars.Q.Cast();

                                    //Console.WriteLine("ExorAIO: Jinx - Combo - Option Disable.");
                                    return;
                                }
                            }

                            /// <summary>
                            ///     Disable if:
                            ///     If you are in mixed mode, the mixed option is disabled.. (Option check).
                            ///     or it doesn't respect the ManaManager check. (Mana check).
                            /// </summary>
                            else if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
                            {
                                if (GameObjects.Player.ManaPercent
                                    < ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["harass"])
                                    || !Vars.Menu["spells"]["q"]["harass"].GetValue<MenuSliderButton>().BValue)
                                {
                                    Vars.Q.Cast();

                                    //Console.WriteLine("ExorAIO: Jinx - Mixed - ManaManager or Option Disable.");
                                    return;
                                }
                            }

                            /// <summary>
                            ///     Disable if:
                            ///     The target is not a hero. (Target check),
                            /// </summary>
                            if (Variables.Orbwalker.GetTarget() is AIHeroClient
                                && (Variables.Orbwalker.GetTarget() as AIHeroClient).IsValidTarget())
                            {
                                /// <summary>
                                ///     Disable if:
                                ///     No enemies in explosion range from the target. (AOE Logic),
                                ///     Any hero in PowPow Range. (Range Logic).
                                /// </summary>
                                if (GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(Vars.PowPow.Range))
                                    && (Variables.Orbwalker.GetTarget() as AIHeroClient).CountEnemyHeroesInRange(200f)
                                    < 2)
                                {
                                    Vars.Q.Cast();

                                    //Console.WriteLine("ExorAIO: Jinx - Combo/Hybrid - Disabled.");
                                    return;
                                }
                            }

                            break;

                        /// <summary>
                        ///     The Q Clear Disable Logics.
                        /// </summary>
                        case OrbwalkingMode.LaneClear:

                            /// <summary>
                            ///     Disable if:
                            ///     Doesn't respect the ManaManager Check, (Mana check).
                            ///     The Clear Option is disabled. (Option check).
                            /// </summary>
                            if (GameObjects.Player.ManaPercent
                                < ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["clear"]))
                            {
                                Vars.Q.Cast();

                                //Console.WriteLine("ExorAIO: Jinx - Clear - ManaManager or Option Disable.");
                                return;
                            }

                            if (!Vars.Menu["spells"]["q"]["clear"].GetValue<MenuSliderButton>().BValue)
                            {
                                return;
                            }

                            /// <summary>
                            ///     Disable if:
                            ///     The player has no Runaan's Hurricane or there is only 1 hittable Minion..
                            ///     And there are no killable minions in Q explosion range or the number of killable minions is less than 3 (Lane AoE Logic).
                            /// </summary>
                            if ((!Items.HasItem(3085) || Targets.Minions.Count < 2)
                                && (!Targets.Minions.Any(
                                    m => Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m) * 1.1)
                                    || Targets.Minions.Count(
                                        m2 =>
                                        m2.Distance(
                                            Targets.Minions.First(
                                                m =>
                                                Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m) * 1.1))
                                        < 250f) < 3))
                            {
                                Vars.Q.Cast();

                                //Console.WriteLine("ExorAIO: Jinx - LaneClear - Disabled.");
                                return;
                            }

                            /// <summary>
                            ///     Disable if:
                            ///     There is at least 1 monster in PowPow Range.. (Jungle Range Logic).
                            ///     .. And less than 1 monster in explosion range from the monster target (Jungle AoE Logic).
                            /// </summary>
                            if (Targets.JungleMinions.Any(m => m.IsValidTarget(Vars.PowPow.Range))
                                && Targets.JungleMinions.Count(m2 => m2.Distance(Targets.JungleMinions[0]) < 250f) < 2)
                            {
                                Vars.Q.Cast();

                                //Console.WriteLine("ExorAIO: Jinx - JungleClear - Disabled.");
                                return;
                            }

                            break;

                        /// <summary>
                        ///     The Q LastHit Disable Logic.
                        /// </summary>
                        case OrbwalkingMode.LastHit:

                            /// <summary>
                            ///     Disable if:
                            ///     Doesn't respect the ManaManager Check, (Mana check).
                            ///     The LastHit Option is disabled. (Option check).
                            /// </summary>
                            if (GameObjects.Player.ManaPercent
                                < ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["lasthit"]))
                            {
                                Vars.Q.Cast();

                                //Console.WriteLine("ExorAIO: Jinx - LastHit - ManaManager or Option Disable.");
                                return;
                            }

                            if (!Vars.Menu["spells"]["q"]["lasthit"].GetValue<MenuSliderButton>().BValue)
                            {
                                return;
                            }

                            /// <summary>
                            ///     Disable if:
                            ///     No killable minion in FishBones Range. (LastHit Range Logic).
                            /// </summary>
                            if (
                                !Targets.Minions.Any(
                                    m =>
                                    !m.IsValidTarget(Vars.PowPow.Range)
                                    && m.Health < GameObjects.Player.GetAutoAttackDamage(m) * 1.1))
                            {
                                Vars.Q.Cast();

                                //Console.WriteLine("ExorAIO: Jinx - LastHit - Range Killable Disable.");
                                return;
                            }

                            if (
                                Targets.Minions.Any(
                                    m =>
                                    m.IsValidTarget(Vars.PowPow.Range)
                                    && m.Health < GameObjects.Player.GetAutoAttackDamage(m)))
                            {
                                Vars.Q.Cast();

                                //Console.WriteLine("ExorAIO: Jinx - LastHit - Normally Killable Disable.");
                                return;
                            }

                            break;

                        /// <summary>
                        ///     The General Q Disable Logic.
                        /// </summary>
                        default:
                            Vars.Q.Cast();

                            //Console.WriteLine("ExorAIO: Jinx - General - Disabled.");
                            break;
                    }
                }
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
                    Vars.E.Cast(target.ServerPosition);
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
                        t => Bools.IsImmobile(t) && !Invulnerable.Check(t) && t.IsValidTarget(Vars.W.Range)))
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

        /// <summary>
        ///     Called before casting a spell.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SpellbookCastSpellEventArgs" /> instance containing the event data.</param>
        public static void Automatic(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            /// <summary>
            ///     The Q Switching Logics.
            /// </summary>
            if (Vars.Q.IsReady() && Vars.Menu["miscellaneous"]["blockq"].GetValue<MenuBool>().Value)
            {
                if (GameObjects.Player.HasBuff("JinxQ"))
                {
                    return;
                }

                /// <summary>
                ///     Block if:
                ///     It doesn't respect the ManaManager Check, (Mana check),
                /// </summary>
                if (GameObjects.Player.ManaPercent
                    < ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["q"]["clear"]))
                {
                    args.Process = false;
                }
            }
        }

        #endregion
    }
}