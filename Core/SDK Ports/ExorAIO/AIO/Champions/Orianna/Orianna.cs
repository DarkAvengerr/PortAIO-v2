// ReSharper disable PossibleInvalidOperationException


#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Orianna
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
    ///     The champion class.
    /// </summary>
    internal class Orianna
    {
        #region Public Methods and Operators

        public static Vector3 GetBallPosition = Vector3.Zero;

        /// <summary>
        ///     Called upon calling a spellcast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SpellbookCastSpellEventArgs" /> instance containing the event data.</param>
        public static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.R
                && Vars.Menu["miscellaneous"]["blockr"].GetValue<MenuBool>().Value)
            {
                if (
                    !GameObjects.EnemyHeroes.Any(
                        t =>
                        t.IsValidTarget() && t.Distance((Vector2)GetBallPosition) < Vars.R.Range - 25
                        && !Invulnerable.Check(t, DamageType.Magical, false)))
                {
                    args.Process = false;
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

            if (Vars.E.IsReady() && args.Sender.IsMelee && args.IsDirectedToPlayer
                && Vars.Menu["spells"]["e"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                Vars.E.CastOnUnit(GameObjects.Player);
            }
        }

        /// <summary>
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (GameObjects.Player.IsDead || Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.R.IsReady() && ((Vector2)GetBallPosition).Distance(args.Sender.ServerPosition) < Vars.R.Range
                && Vars.Menu["spells"]["r"]["interrupter"].GetValue<MenuBool>().Value)
            {
                Vars.R.Cast();
            }
        }

        /// <summary>
        ///     Called when a <see cref="AttackableUnit" /> takes/gives damage.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AttackableUnitDamageEventArgs" /> instance containing the event data.</param>
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient || sender is Obj_AI_Turret
                || GameObjects.Jungle.Any(
                    m =>
                    m.BaseSkinName == sender.BaseSkinName
                    && GameObjects.JungleSmall.All(m2 => m2.BaseSkinName != sender.BaseSkinName)))
            {
                if (sender.IsEnemy && args.Target != null
                    && GameObjects.AllyHeroes.Any(a => a.NetworkId == args.Target.NetworkId))
                {
                    if (Vars.E.IsReady() && ((AIHeroClient)args.Target).IsValidTarget(Vars.E.Range, false)
                        && Vars.Menu["spells"]["e"]["logical"].GetValue<MenuBool>().Value
                        && Vars.Menu["spells"]["e"]["whitelist"][((AIHeroClient)args.Target).ChampionName.ToLower()]
                               .GetValue<MenuBool>().Value)
                    {
                        Vars.E.CastOnUnit((AIHeroClient)args.Target);
                    }
                }
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
        ///     Loads Orianna.
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
            ///     Initializes the ball drawings.
            /// </summary>
            BallDrawings.Initialize();
        }

        #endregion
    }
}