
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Caitlyn
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
    ///     The champion class.
    /// </summary>
    internal class Caitlyn
    {
        #region Public Methods and Operators

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
                    if (
                        GameObjects.EnemyHeroes.Any(
                            t => t.IsValidTarget() && Vars.GetRealHealth(t) < GameObjects.Player.GetAutoAttackDamage(t)))
                    {
                        return;
                    }

                    var target =
                        GameObjects.EnemyHeroes.FirstOrDefault(
                            t =>
                            t.InAutoAttackRange() && !Invulnerable.Check(t) && t.HasBuff("caitlynyordletrapinternal"));
                    if (target != null)
                    {
                        Variables.Orbwalker.ForceTarget = target;
                        return;
                    }

                    Variables.Orbwalker.ForceTarget = null;
                    break;
            }
        }

        /// <summary>
        ///     Fired on spell cast.
        /// </summary>
        /// <param name="spellbook">The spellbook.</param>
        /// <param name="args">The <see cref="SpellbookCastSpellEventArgs" /> instance containing the event data.</param>
        public static void OnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            if (spellbook.Owner.IsMe)
            {
                switch (args.Slot)
                {
                    case SpellSlot.W:

                        /// <summary>
                        ///     Blocks trap cast if there is another trap nearby.
                        /// </summary>
                        if (
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Any(
                                    m =>
                                    m.Distance(args.EndPosition) < 200 && m.BaseSkinName.Equals("caitlyntrap")))
                        {
                            args.Process = false;
                        }
                        break;
                    case SpellSlot.E:
                        if (Environment.TickCount - Vars.LastTick < 1000)
                        {
                            return;
                        }

                        /// <summary>
                        ///     The Dash to CursorPos Option.
                        /// </summary>
                        if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.None
                            && Vars.Menu["miscellaneous"]["reversede"].GetValue<MenuBool>().Value)
                        {
                            Vars.LastTick = Environment.TickCount;
                            Vars.E.Cast(GameObjects.Player.ServerPosition.Extend(Game.CursorPos, -Vars.E.Range));
                        }
                        break;
                }
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
                /// <summary>
                ///     Initializes the orbwalkingmodes.
                /// </summary>
                switch (Variables.Orbwalker.ActiveMode)
                {
                    case OrbwalkingMode.Combo:
                        switch (args.SData.Name)
                        {
                            case "CaitlynEntrapment":
                            case "CaitlynEntrapmentMissile":
                                if (Vars.W.IsReady() && Vars.Menu["spells"]["w"]["triplecombo"].GetValue<MenuBool>().Value)
                                {
                                    foreach (var target in
                                        GameObjects.EnemyHeroes.Where(
                                            t => t.IsValidTarget(Vars.W.Range) && !Invulnerable.Check(t)))
                                    {
                                        Vars.W.Cast(target.ServerPosition);
                                    }
                                }
                                break;
                        }

                        break;
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

            if (Vars.E.IsReady() && args.IsDirectedToPlayer && args.Sender.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                if (!Vars.E.GetPrediction(args.Sender).CollisionObjects.Any())
                {
                    //Vars.E.Cast(args.Sender.ServerPosition);
                    Vars.E.Cast(args.End);
                }
            }

            if (Vars.W.IsReady() && args.Sender.IsValidTarget(Vars.W.Range)
                && Vars.Menu["spells"]["w"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                Vars.W.Cast(args.End);
            }
        }

        /// <summary>
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (GameObjects.Player.IsDead || Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.E.IsReady() && args.Sender.IsValidTarget(Vars.E.Range)
                && Vars.Menu["spells"]["e"]["interrupter"].GetValue<MenuBool>().Value)
            {
                if (!Vars.E.GetPrediction(args.Sender).CollisionObjects.Any())
                {
                    Vars.E.Cast(Vars.E.GetPrediction(args.Sender).UnitPosition);
                }
            }

            if (Vars.W.IsReady() && args.Sender.IsValidTarget(Vars.W.Range)
                && Vars.Menu["spells"]["w"]["interrupter"].GetValue<MenuBool>().Value)
            {
                Vars.W.Cast(Vars.W.GetPrediction(args.Sender).CastPosition);
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
            ///     Updates the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

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
        ///     Loads Caitlyn.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();
        }

        #endregion
    }
}