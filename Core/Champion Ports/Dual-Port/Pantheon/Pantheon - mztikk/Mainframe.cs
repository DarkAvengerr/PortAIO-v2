using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkPantheon.Defense;
    using mztikkPantheon.Modes;

    internal static class Mainframe
    {
        #region Properties

        internal static bool CancelEverything { get; set; }

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;

            Obj_AI_Base.OnProcessSpellCast += Events.OnProcessSpellCast;
            Obj_AI_Base.OnBuffLose += Events.OnBuffLose;

            Orbwalking.AfterAttack += Combo.OnPostAttack;
            Orbwalking.AfterAttack += Harass.OnPostAttack;

            Orbwalking.OnAttack += Events.OnAttack;
            Orbwalking.AfterAttack += Events.OnAfterAttack;
            Obj_AI_Base.OnNewPath += Events.OnNewPath;
            Interrupter2.OnInterruptableTarget += Events.OnInterruptableTarget;

            Obj_AI_Base.OnProcessSpellCast += StackPassive.OnEnemyAa;

            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = Computed.GetComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = Config.IsChecked("draw.dmg");
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.IsChecked("draw.q") && (!Config.IsChecked("draw.onlyrdy") || Spells.Q.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Color.White);
            }

            if (Config.IsChecked("draw.w") && (!Config.IsChecked("draw.onlyrdy") || Spells.W.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, Color.White);
            }

            if (Config.IsChecked("draw.e") && (!Config.IsChecked("draw.onlyrdy") || Spells.Q.IsReady()))
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Color.White);
            }

            if (Config.IsChecked("draw.r") && (!Config.IsChecked("draw.onlyrdy") || Spells.Q.IsReady()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Color.White);
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.IsChecked("draw.r2") && (!Config.IsChecked("draw.onlyrdy") || Spells.R.IsReady()))
            {
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Color.White, 1, 20, true);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || CancelEverything)
            {
                return;
            }

            if (Config.IsChecked("misc.killsteal.q"))
            {
                Killsteal.Execute();
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

            AutoHarass.Execute();
        }

        #endregion
    }
}