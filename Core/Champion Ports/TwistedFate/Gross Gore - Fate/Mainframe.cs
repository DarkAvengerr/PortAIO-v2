using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using GrossGoreTwistedFate.Modes;
    using SharpDX;

    internal static class Mainframe
    {
        #region Static Fields

        internal static readonly Random Rng = new Random((int)DateTime.UtcNow.Ticks);

        #endregion

        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }
        internal static bool IsUlt { get { return ObjectManager.Player.HasBuff("destiny_marker"); } }
        internal static bool HasBlue { get { return ObjectManager.Player.HasBuff("bluecardpreattack"); } }
        internal static bool HasRed { get { return ObjectManager.Player.HasBuff("redcardpreattack"); } }
        internal static bool HasGold { get { return ObjectManager.Player.HasBuff("goldcardpreattack"); } }
        internal static string HasACard
        {
            get
            {
                if (ObjectManager.Player.HasBuff("bluecardpreattack"))
                    return "blue";
                if (ObjectManager.Player.HasBuff("goldcardpreattack"))
                    return "gold";
                if (ObjectManager.Player.HasBuff("redcardpreattack"))
                    return "red";
                return "empty";
            }
        }

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;

            Obj_AI_Base.OnProcessSpellCast += Computed.OnProcessSpellCast;
            Obj_AI_Base.OnProcessSpellCast += Computed.YellowIntoQ;
            Obj_AI_Base.OnProcessSpellCast += Computed.RedIntoQ;
            Orbwalking.BeforeAttack += Computed.OnBeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += Computed.Gapcloser_OnGapCloser;
            Interrupter2.OnInterruptableTarget += Computed.InterruptableSpell_OnInterruptableTarget;
            Drawing.OnEndScene += DrawingOnOnEndScene;
            Drawing.OnDraw += OnDraw;
            //LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = Computed.TwistedFateDamage;
            //LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            //CustomDamageIndicator.Initialize(Computed.TwistedFateDamage);
            //CustomDamageIndicator.Enabled = true;

            Chat.Print("<font color='#DE5291'>Ready. Play TF like Dopa!</font>");

        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {

                if (Config.IsChecked("drawRmap") && (!Config.IsChecked("drawOnlyReady") || Spells.R.IsReady()))
                {
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, 5500, System.Drawing.Color.PaleGreen, 2, 23, true);
                }
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] + (msg.Length), wts[1] + weight, color, msg);
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Config.IsChecked("drawQrange") && (!Config.IsChecked("drawOnlyReady") || Spells.Q.IsReady()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, System.Drawing.Color.CornflowerBlue);
            }

            if (Config.IsChecked("drawRrange") && (!Config.IsChecked("drawOnlyReady") || Spells.R.IsReady()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, System.Drawing.Color.PaleGreen);
            }

            if(HasACard != "none")
            {
                if (HasACard == "gold")
                {
                    var buffG = ObjectManager.Player.GetBuff("goldcardpreattack");
                    var timeLastG = (buffG.EndTime - Game.Time);
                    var timeLastGInt = (int)Math.Round(timeLastG, MidpointRounding.ToEven);

                    drawText("Gold Ready: " + timeLastGInt + " s", ObjectManager.Player.Position, System.Drawing.Color.HotPink, -75);

                }else if(HasACard == "blue")
                {
                    var buffB = ObjectManager.Player.GetBuff("bluecardpreattack");
                    var timeLastB = (buffB.EndTime - Game.Time);
                    var timeLastBInt = (int)Math.Round(timeLastB, MidpointRounding.ToEven);

                    drawText("Blue Ready: " + timeLastBInt + " s", ObjectManager.Player.Position, System.Drawing.Color.HotPink, -75);

                }else if(HasACard == "red")
                {
                    var buffR = ObjectManager.Player.GetBuff("redcardpreattack");
                    var timeLastR = (buffR.EndTime - Game.Time);
                    var timeLastRInt = (int)Math.Round(timeLastR, MidpointRounding.ToEven);

                    drawText("Red Ready: " + timeLastRInt + " s", ObjectManager.Player.Position, System.Drawing.Color.HotPink, -75);
                }
            }

            if (IsUlt)
            {
                var buffUlt = ObjectManager.Player.GetBuff("destiny_marker");
                var timeLastUlt = (buffUlt.EndTime - Game.Time);
                var timeLastUltInt = (int)Math.Round(timeLastUlt, MidpointRounding.ToEven);
                drawText(timeLastUltInt + " s to TP!", ObjectManager.Player.Position, System.Drawing.Color.LightGoldenrodYellow, -45);
            }

            if (Spells.R.IsReady())
            {
                var target = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);

                if (target.IsValidTarget())
                {
                    var comboDMG = Spells.Q.GetDamage(target) + Spells.W.GetDamage(target) + ObjectManager.Player.GetAutoAttackDamage(target) * 3;

                    if (comboDMG > target.Health)
                        drawText(target.ChampionName + " is can be bursted!", ObjectManager.Player.Position, System.Drawing.Color.LightGoldenrodYellow, -15);
                }
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
                ComboMode.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                MixedMode.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleLogic.Execute();
            }

            ManualCards.Execute();

            Automated.Execute();

            QWaveClear.Execute();

            QChampions.Execute();
        }

        #endregion
    }
}