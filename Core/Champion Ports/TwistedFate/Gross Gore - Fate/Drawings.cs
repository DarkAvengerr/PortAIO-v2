#region Use
using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{

    internal static class Drawings
    {
        #region Properties

        internal static bool UltEnabled { get { return ObjectManager.Player.HasBuff("destiny_marker"); } }
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

        internal static void Draw()
        {
            Drawing.OnEndScene += DrawingOnOnEndScene;
            Drawing.OnDraw += OnDraw;

            Chat.Print("<font color='#1abc9c'>Ready. Play TF like Dopa!</font>");
            Chat.Print("<font color='#2980b9'>Upvote if you like.</font>");
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                if (Config.DrawRm && Spells._r.IsReadyPerfectly())
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
            if (!ObjectManager.Player.IsDead)
            {
                if (Config.DrawQ && Spells._q.IsReadyPerfectly())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells._q.Range, System.Drawing.Color.CornflowerBlue);
                }

                if (Config.DrawR && Spells._r.IsReadyPerfectly())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells._r.Range, System.Drawing.Color.PaleGreen);
                }

                #region Timers

                if (HasACard != "empty")
                {
                    if (HasGold)
                    {
                        var buffG = ObjectManager.Player.GetBuff("goldcardpreattack");
                        var timeLastG = (buffG.EndTime - Game.Time);
                        var timeLastGInt = (int)Math.Round(timeLastG, MidpointRounding.ToEven);

                        drawText("Gold Ready: " + timeLastGInt + " s", ObjectManager.Player.Position, System.Drawing.Color.LightGreen, -75);

                    }
                    else if (HasBlue)
                    {
                        var buffB = ObjectManager.Player.GetBuff("bluecardpreattack");
                        var timeLastB = (buffB.EndTime - Game.Time);
                        var timeLastBInt = (int)Math.Round(timeLastB, MidpointRounding.ToEven);

                        drawText("Blue Ready: " + timeLastBInt + " s", ObjectManager.Player.Position, System.Drawing.Color.LightGreen, -75);

                    }
                    else
                    {
                        var buffR = ObjectManager.Player.GetBuff("redcardpreattack");
                        var timeLastR = (buffR.EndTime - Game.Time);
                        var timeLastRInt = (int)Math.Round(timeLastR, MidpointRounding.ToEven);

                        drawText("Red Ready: " + timeLastRInt + " s", ObjectManager.Player.Position, System.Drawing.Color.LightGreen, -75);
                    }
                }

                if (UltEnabled)
                {
                    var buffUlt = ObjectManager.Player.GetBuff("destiny_marker");
                    var timeLastUlt = (buffUlt.EndTime - Game.Time);
                    var timeLastUltInt = (int)Math.Round(timeLastUlt, MidpointRounding.ToEven);
                    drawText(timeLastUltInt + " s to TP!", ObjectManager.Player.Position, System.Drawing.Color.LightGoldenrodYellow, -45);
                }

                #endregion

                if (Spells._r.IsReadyPerfectly())
                {
                    var target = TargetSelector.GetTarget(Spells._r.Range, Spells._q.DamageType);

                    if (target.IsValidTarget())
                    {
                        var comboDMG = Spells._q.GetDamage(target) + Spells._w.GetDamage(target) + ObjectManager.Player.GetAutoAttackDamage(target) * 3;

                        if (comboDMG > target.Health)
                        {
                            drawText("You should check: " + target.ChampionName, ObjectManager.Player.Position, System.Drawing.Color.LightGoldenrodYellow, 20);
                        }
                    }
                }
            }
        }
    }
}
