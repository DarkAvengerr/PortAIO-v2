using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikksTwistedFate.Modes;

    internal static class Mainframe
    {
        #region Static Fields

        internal static readonly Random Rng = new Random((int)DateTime.UtcNow.Ticks);

        #endregion

        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;

            // Spellbook.OnCastSpell += Computed.SafeCast;
            Obj_AI_Base.OnProcessSpellCast += Computed.OnProcessSpellCast;
            Obj_AI_Base.OnProcessSpellCast += Computed.YellowIntoQ;
            Orbwalking.BeforeAttack += Computed.OnBeforeAttack;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.IsChecked("drawQrange") && (!Config.IsChecked("drawOnlyReady") || Spells.Q.IsReady()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Color.White);
            }

            if (Config.IsChecked("drawRrange") && (!Config.IsChecked("drawOnlyReady") || Spells.R.IsReady()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Color.White);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear.Execute();
                LaneClear.Execute();
            }

            ManualCards.Execute();
            Automated.Execute();
        }

        #endregion
    }
}