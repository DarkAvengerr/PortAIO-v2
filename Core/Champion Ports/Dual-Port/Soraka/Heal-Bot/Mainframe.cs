using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Soraka_HealBot.Modes;

    internal static class Mainframe
    {
        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += Events.SavingGrace;
            Obj_AI_Base.OnSpellCast += NerfEverything.Interrupts;
            Drawing.OnDraw += OnDraw;
            Orbwalking.BeforeAttack += Events.OnBeforeAttack;
            Interrupter2.OnInterruptableTarget += OtherUtils.OnInterruptableSpell;
            AntiGapcloser.OnEnemyGapcloser += OtherUtils.OnGapcloser;
            GameObject.OnCreate += Events.OnObjectCreate;
            GameObject.OnDelete += Events.OnObjectDelete;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.IsChecked("wRangeDraw") && (!Config.IsChecked("onlyReady") || Spells.W.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, Color.White);
            }

            if (Config.IsChecked("qRange") && (!Config.IsChecked("onlyReady") || Spells.Q.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Color.White);
            }

            if (Config.IsChecked("eRange") && (!Config.IsChecked("onlyReady") || Spells.E.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Color.White);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Config.IsChecked("autoR") && Spells.R.IsReady())
            {
                HealBotR.Execute();
            }

            if (Config.IsChecked("autoAssistKS") && Spells.R.IsReady())
            {
                AssistKs.Execute();
            }

            if (Spells.W.IsReady() && Config.IsChecked("autoW"))
            {
                HealBotW.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass.Execute();
            }

            if (Config.IsChecked("autoQHarass"))
            {
                AutoHarass.AutoQ();
            }

            if (Config.IsChecked("autoEHarass"))
            {
                AutoHarass.AutoE();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear.Execute();
            }
        }

        #endregion
    }
}