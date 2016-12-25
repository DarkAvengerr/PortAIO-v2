// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce.Extensions
{
    #region

    using System;
    using System.Linq;

    using Jayce.Extensions;

    using static Config;

    using Jayce.Modes;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using static Other;

    using SharpDX;

    using static Spells;

    #endregion

    /// <summary>
    ///     The events.
    /// </summary>
    internal class Events
    {
        #region Static Fields

        /// <summary>
        /// The indicator.
        /// </summary>
        private static readonly DamageBar Indicator = new DamageBar();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += ProcessSpell;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Variables.Orbwalker.OnAction += OnAction;
            Drawing.OnDraw += OnDraw;
            LeagueSharp.SDK.Events.OnGapCloser += GapCloser;
            LeagueSharp.SDK.Events.OnInterruptableTarget += Interrupter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The gap closer.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void GapCloser(object sender, LeagueSharp.SDK.Events.GapCloserEventArgs args)
        {
            if (AGCM.Value)
                if (RangeForm())
                {
                    if (R.IsReady() && (HammerQ_CD_R == 0)) R.Cast();
                }
                else
                {
                    if (args.Sender.IsValidTarget(E1.Range) && E1.IsReady()) E.Cast(args.Sender);
                }
        }

        /// <summary>
        /// The interrupter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Interrupter(object sender, LeagueSharp.SDK.Events.InterruptableTargetEventArgs args)
        {
            if (AGCM.Value)
                if (RangeForm())
                {
                    if (R.IsReady() && (HammerQ_CD_R == 0)) R.Cast();
                }
                else
                {
                    if (args.Sender.IsValidTarget(E1.Range) && E1.IsReady()) E.Cast(args.Sender);
                }
        }

        /// <summary>
        /// The on action.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            if ((args.Type == OrbwalkingType.AfterAttack) && (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)) if (RangeForm() && args.Target.IsEnemy) if (W.IsReady() && ComboCannonW.Value) W.Cast();
        }

        /// <summary>
        /// The on do cast.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && (args.Slot == SpellSlot.Q))
                if (E.IsReady() && RangeForm())
                {
                    var Enemies = GameObjects.EnemyHeroes.Where(x => (x != null) && x.IsValidTarget(QE.Range));
                    GatePos = ObjectManager.Player.ServerPosition.Extend(args.End, 130 + Game.Ping / 2);
                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        case OrbwalkingMode.Combo:
                            if (ComboCannonE.Value) E.Cast(GatePos);
                            break;
                        case OrbwalkingMode.Hybrid:
                            if (HarassCannonE.Value) E.Cast(GatePos);
                            break;
                        case OrbwalkingMode.LaneClear:
                            if (LaneCannonE.Value) E.Cast(GatePos);
                            break;
                    }

                    foreach (var Enemy in Enemies.Where(x => CannonQEDmg(x) > x.Health)) if (CannonEKS) E.Cast(GatePos);
                }
        }

        /// <summary>
        /// The on draw.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void OnDraw(EventArgs args)
        {
            foreach (var Enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000)))
                if (DrawDmg.Value)
                {
                    Indicator.Unit = Enemy;
                    Indicator.DrawDmg((float)ComboDamage(Enemy), Color.DarkRed);
                }

            if (RangeForm())
            {
                if (CannonQERange.Value && QE.IsReady() && E.IsReady()) Render.Circle.DrawCircle(ObjectManager.Player.Position, QE.Range, System.Drawing.Color.Aqua, 2);
                if (CannonQRange.Value && Q.IsReady()) Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 2);
            }
            else
            {
                if (HammerQRange.Value && Q1.IsReady()) Render.Circle.DrawCircle(ObjectManager.Player.Position, Q1.Range, System.Drawing.Color.IndianRed, 2);
            }
        }

        /// <summary>
        /// The on update.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void OnUpdate(EventArgs args)
        {
            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo.Execute();
                    break;
                case OrbwalkingMode.Hybrid:
                    Harass.Execute();
                    break;
                case OrbwalkingMode.LaneClear:
                    LaneClear.Execute();
                    JungleClear.Execute();
                    break;
            }

            KillSteal.Execute();
            CD();
            SkinChanger();
        }

        /// <summary>
        /// The process spell.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && (args.SData.Name.ToLower() == "jayceshockblast") && RangeForm())
                if (E.IsReady())
                {
                    var Enemies = GameObjects.EnemyHeroes.Where(x => (x != null) && x.IsValidTarget(QE.Range));
                    GatePos = ObjectManager.Player.ServerPosition.Extend(args.End, 130 + Game.Ping / 2);
                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        case OrbwalkingMode.Combo:
                            if (ComboCannonE.Value) E.Cast(GatePos);
                            break;
                        case OrbwalkingMode.Hybrid:
                            if (HarassCannonE.Value) E.Cast(GatePos);
                            break;
                        case OrbwalkingMode.LaneClear:
                            if (LaneCannonE.Value) E.Cast(GatePos);
                            break;
                    }

                    foreach (var Enemy in Enemies.Where(x => CannonQEDmg(x) > x.Health)) if (CannonEKS) E.Cast(GatePos);
                }

            if (args.SData.Name == "JayceToTheSkies") HammerQ_CD = Game.Time + RealCD(HammerQ_TrueCD[Q1.Level - 1]);
            if (args.SData.Name == "JayceStaticField") HammerW_CD = Game.Time + RealCD(HammerW_TrueCD[W1.Level - 1]);
            if (args.SData.Name == "JayceThunderingBlow") HammerE_CD = Game.Time + RealCD(HammerE_TrueCD[E1.Level - 1]);

            if (args.SData.Name.ToLower() == "jayceshockblast") CannonQ_CD = Game.Time + RealCD(CannonQ_TrueCD[Q.Level - 1]);
            if (args.SData.Name.ToLower() == "jaycehypercharge") CannonW_CD = Game.Time + RealCD(CannonW_TrueCD[W.Level - 1]);
            if (args.SData.Name.ToLower() == "jayceaccelerationgate") CannonE_CD = Game.Time + RealCD(CannonE_TrueCD[E.Level - 1]);
        }

        #endregion
    }
}