
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Karma
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Karma
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            switch (args.Type)
            {
                case OrbwalkingType.BeforeAttack:

                    /// <summary>
                    ///     The 'Support Mode' Logic.
                    /// </summary>
                    if (Vars.Menu["miscellaneous"]["support"].GetValue<MenuBool>().Value)
                    {
                        switch (Variables.Orbwalker.ActiveMode)
                        {
                            case OrbwalkingMode.Hybrid:
                            case OrbwalkingMode.LastHit:
                            case OrbwalkingMode.LaneClear:
                                if (args.Target is Obj_AI_Minion
                                    && GameObjects.AllyHeroes.Any(a => a.Distance(GameObjects.Player) < 2500))
                                {
                                    args.Process = false;
                                }
                                break;
                        }
                    }

                    break;
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            if (Vars.E.IsReady() && GameObjects.Player.Distance(args.End) < 750
                && Vars.Menu["spells"]["e"]["gapcloser"].GetValue<MenuBool>().Value)
            {
                if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["empe"].GetValue<MenuBool>().Value
                    && GameObjects.AllyHeroes.Count(a => a.IsValidTarget(600f, false)) >= 2)
                {
                    Vars.R.Cast();
                }
                Vars.E.Cast();
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
                    m.CharData.BaseSkinName == sender.CharData.BaseSkinName
                    && GameObjects.JungleSmall.All(m2 => m2.CharData.BaseSkinName != sender.CharData.BaseSkinName)))
            {
                if (args.SData.Name.Equals("GangplankE"))
                {
                    return;
                }

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
        ///     Loads Karma.
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
        }

        #endregion
    }
}