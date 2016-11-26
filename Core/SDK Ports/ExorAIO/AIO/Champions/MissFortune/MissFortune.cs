
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.MissFortune
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class MissFortune
    {
        #region Static Fields

        /// <summary>
        ///     The last target.
        /// </summary>
        public static AttackableUnit PassiveTarget;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            /// <summary>
            ///     Stop attack and movement commands while channeling R.
            /// </summary>
            args.Process = !GameObjects.Player.HasBuff("missfortunebulletsound");
            switch (args.Type)
            {
                case OrbwalkingType.BeforeAttack:

                    /// <summary>
                    ///     The Target Forcing Logic.
                    /// </summary>
                    var hero = args.Target as AIHeroClient;
                    var bestTarget =
                        GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange()))
                            .OrderByDescending(o => Data.Get<ChampionPriorityData>().GetPriority(o.ChampionName))
                            .FirstOrDefault();
                    if (hero != null && bestTarget?.NetworkId != PassiveTarget?.NetworkId
                        && Vars.GetRealHealth(hero) > GameObjects.Player.GetAutoAttackDamage(hero) * 3
                        && hero.NetworkId == PassiveTarget?.NetworkId
                        && Vars.Menu["miscellaneous"]["passive"].GetValue<MenuBool>().Value)
                    {
                        Variables.Orbwalker.ForceTarget = bestTarget;
                        return;
                    }

                    Variables.Orbwalker.ForceTarget = null;
                    break;
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (AutoAttack.IsAutoAttack(args.SData.Name))
                {
                    /// <summary>
                    ///     Initializes the orbwalkingmodes.
                    /// </summary>
                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        case OrbwalkingMode.Combo:
                            Logics.Weaving(sender, args);
                            break;
                        case OrbwalkingMode.LaneClear:
                            Logics.JungleClear(sender, args);
                            Logics.BuildingClear(sender, args);
                            break;
                    }

                    PassiveTarget = args.Target as AttackableUnit;
                }
                else
                {
                    switch (args.SData.Name)
                    {
                        case "MissFortuneRicochetShot":
                            PassiveTarget = args.Target as AttackableUnit;
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (GameObjects.Player.IsDead || Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.E.IsReady() && args.Sender.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast(args.End);
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);
            if (GameObjects.Player.HasBuff("missfortunebulletsound"))
            {
                return;
            }

            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);
            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            /// <summary>
            ///     Initializes the orbwalkingmodes.
            /// </summary>
            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Logics.Combo(args);
                    break;
                case OrbwalkingMode.Hybrid:
                    Logics.Harass(args);
                    break;
                case OrbwalkingMode.LaneClear:
                    Logics.Clear(args);
                    break;
            }
        }

        /// <summary>
        ///     Loads Miss Fortune.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initializes the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();

            /// <summary>
            ///     Initializes the cone drawings.
            /// </summary>
            ConeDrawings.Initialize();
        }

        #endregion
    }
}