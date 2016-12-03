
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Kalista
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Kalista
    {
        #region Static Fields

        /// <summary>
        ///     Gets all the important jungle locations.
        /// </summary>
        internal static readonly List<Vector3> Locations = new List<Vector3>
                                                               {
                                                                   new Vector3(9827.56f, 4426.136f, -71.2406f),
                                                                   new Vector3(4951.126f, 10394.05f, -71.2406f),
                                                                   new Vector3(10998.14f, 6954.169f, 51.72351f),
                                                                   new Vector3(7082.083f, 10838.25f, 56.2041f),
                                                                   new Vector3(3804.958f, 7875.456f, 52.11121f),
                                                                   new Vector3(7811.249f, 4034.486f, 53.81299f)
                                                               };

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the SoulBound.
        /// </summary>
        public static AIHeroClient SoulBound { get; set; } = null;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns true if the target is a perfectly valid rend target.
        /// </summary>
        public static bool IsPerfectRendTarget(Obj_AI_Base target)
        {
            var hero = target as AIHeroClient;
            return (hero == null || !Invulnerable.Check(hero)) && target.IsValidTarget(Vars.E.Range)
                   && target.HasBuff("kalistaexpungemarker");
        }

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            switch (args.Type)
            {
                case OrbwalkingType.BeforeAttack:

                    /// <summary>
                    ///     The Target Forcing Logic.
                    /// </summary>
                    var hero = args.Target as AIHeroClient;
                    var bestTarget =
                        GameObjects.EnemyHeroes.Where(
                            t =>
                            t.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                            && t.HasBuff("kalistacoopstrikemarkally"))
                            .OrderByDescending(o => Data.Get<ChampionPriorityData>().GetPriority(o.ChampionName))
                            .FirstOrDefault();
                    if (hero != null && bestTarget?.NetworkId != hero.NetworkId
                        && Vars.GetRealHealth(hero) > GameObjects.Player.GetAutoAttackDamage(hero) * 3)
                    {
                        Variables.Orbwalker.ForceTarget = bestTarget;
                        return;
                    }

                    Variables.Orbwalker.ForceTarget = null;
                    break;

                case OrbwalkingType.NonKillableMinion:

                    /// <summary>
                    ///     The E against Non-Killable Minions Logic.
                    /// </summary>
                    if (Vars.E.IsReady() && IsPerfectRendTarget(args.Target as Obj_AI_Minion)
                        && Vars.GetRealHealth(args.Target as Obj_AI_Minion)
                        < (float)GameObjects.Player.GetSpellDamage(args.Target as Obj_AI_Minion, SpellSlot.E)
                        + (float)
                          GameObjects.Player.GetSpellDamage(args.Target as Obj_AI_Minion, SpellSlot.E, DamageStage.Buff))
                    {
                        Vars.E.Cast();
                    }
                    break;
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

            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);
            if (GameObjects.Player.Spellbook.IsAutoAttacking || GameObjects.Player.IsDashing())
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
        ///     Loads Kalista.
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
            ///     Initializes the damage drawings.
            /// </summary>
            Healthbars.Initialize();
        }

        #endregion
    }
}