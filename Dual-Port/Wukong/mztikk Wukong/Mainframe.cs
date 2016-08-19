using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Wukong.Modes;

    internal static class Mainframe
    {
        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;

            Orbwalking.AfterAttack += Combo.OnPostAttack;
            Orbwalking.AfterAttack += Harass.OnPostAttack;
            Orbwalking.AfterAttack += LaneClear.OnPostAttack;
            Orbwalking.AfterAttack += JungleClear.OnPostAttack;

            Interrupter2.OnInterruptableTarget += Events.OnInterruptableSpell;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.IsChecked("draw.q") && (!Config.IsChecked("draw.onlyrdy") || Spells.Q.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Color.White);
            }

            if (Config.IsChecked("draw.e") && (!Config.IsChecked("draw.onlyrdy") || Spells.Q.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Color.White);
            }

            if (Config.IsChecked("draw.r") && (!Config.IsChecked("draw.onlyrdy") || Spells.Q.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Color.White);
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

            if (Config.IsChecked("harass.q.auto"))
            {
                Harass.AutoQ();
            }
        }

        #endregion
    }
}