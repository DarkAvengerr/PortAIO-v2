using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkSona.Extensions;
    using mztikkSona.Modes;
    using mztikkSona.OtherUtils;

    internal static class Mainframe
    {
        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += Combo.OnPreAttack;
            Orbwalking.BeforeAttack += Harass.OnPreAttack;

            Interrupter2.OnInterruptableTarget += Interrupt.OnInterruptableSpell;
            AntiGapcloser.OnEnemyGapcloser += Gapclose.OnGapclose;

            Obj_AI_Base.OnProcessSpellCast += Computed.OnProcessSpellCast;
            Obj_AI_Base.OnProcessSpellCast += AutoW.OnProcessSpellCast;

            Drawing.OnDraw += Drawings.OnDraw;
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

            if (Config.IsChecked("bW") && Spells.W.CanCast()
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("AutoW.minMana"))
            {
                AutoW.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("Harass.minMana"))
            {
                Harass.Execute();
            }

            if (Config.IsKeyPressed("assistedR"))
            {
                AssistedR.Execute();
            }

            if (Config.IsKeyPressed("fleeBind"))
            {
                Orbwalking.MoveTo(Game.CursorPos);
                Flee.Execute();
            }
        }

        #endregion
    }
}