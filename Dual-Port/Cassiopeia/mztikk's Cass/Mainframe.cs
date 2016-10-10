using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia
{
    using System;

    using mztikksCassiopeia.Modes;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Mainframe
    {
        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        internal static Random RDelay { get; } = new Random();

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

            Obj_AI_Base.OnProcessSpellCast += Events.OnProcessSpellCast;
            Spellbook.OnCastSpell += Events.OnSpellbookCastSpell;
            Orbwalking.OnNonKillableMinion += Events.OnUnkillableMinion;
            Interrupter2.OnInterruptableTarget += Events.OnInterruptableSpell;
            AntiGapcloser.OnEnemyGapcloser += Events.OnGapCloser;
        }

        private static void OnDraw(EventArgs args)
        {
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Config.IsKeyPressed("assistedR"))
            {
                AssistedR.Execute();
            }
            if (Config.IsChecked("eKillSteal"))
            {
                Automated.EKillSteal();
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

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit.Execute();
            }

            if (Config.IsChecked("autoEHarass")
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("manaToAutoHarass"))
            {
                Automated.AutoE();
            }

            if (Config.IsChecked("autoQHarass")
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("manaToAutoHarass"))
            {
                Automated.AutoQ();
            }

            if (Config.IsChecked("autoWHarass")
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("manaToAutoHarass"))
            {
                Automated.AutoW();
            }

            if (Config.IsChecked("clearE") && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("manaClearE")
                && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                Automated.AutoClearE();
            }

            if (Config.IsChecked("tearStackQ")
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("manaTearStack") && Computed.HasTear()
                && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                Automated.TearStack();
            }
        }

        #endregion
    }
}