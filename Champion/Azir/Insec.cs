using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using System.Text.RegularExpressions;
using Color = System.Drawing.Color;
using EloBuddy;

namespace HeavenStrikeAzir
{
    public static class Insec
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static int LastJump;
        public static Vector3 LastLeftClick = new Vector3();
        public static Vector3 InsecPoint = new Vector3();
        public static void Initialize()
        {
            Game.OnUpdate += Game_OnUpdate;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Program.drawinsecLine)
                return;
            var target = TargetSelector.GetSelectedTarget();
            if (target.LSIsValidTarget() && !target.IsZombie)
            {
                Render.Circle.DrawCircle(target.Position, 100, Color.Yellow);
                if (InsecPoint.LSIsValid())
                {
                    var point = target.LSDistance(InsecPoint) >= 400 ?
                        target.Position.LSExtend(InsecPoint, 400) : InsecPoint;
                    Drawing.DrawLine(Drawing.WorldToScreen(target.Position)
                        , Drawing.WorldToScreen(point), 3, Color.Red);
                }
            }
            if (InsecPoint.LSIsValid())
                Render.Circle.DrawCircle(InsecPoint, 100, Color.Pink);
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYDOWN && args.WParam == Program.insecpointkey)
            {
                LastLeftClick = Game.CursorPos;
            }

        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Program.insecmode)
            {
                case 0:
                    var hero = HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead)
                        .OrderByDescending(x => x.LSDistance(Player.Position)).LastOrDefault();
                    if (hero != null)
                        InsecPoint = hero.Position;
                    break;
                case 1:
                    var turret = GameObjects.AllyTurrets.OrderByDescending(x => x.LSDistance(Player.Position)).LastOrDefault();
                    if (turret != null)
                        InsecPoint = turret.Position;
                    break;
                case 2:
                    InsecPoint = Game.CursorPos;
                    break;
                case 3:
                    InsecPoint = LastLeftClick;
                    break;
            }
            if (!Program.insec)
                return;
            if (OrbwalkCommands.CanMove())
            {
                OrbwalkCommands.MoveTo(Game.CursorPos);
            }
            if (!InsecPoint.LSIsValid())
                return;
            var target = TargetSelector.GetSelectedTarget();
            if (!target.LSIsValidTarget() || target.IsZombie)
                return;
            if (!Program._r2.LSIsReady())
                return;
            //case 1
            Vector2 start1 = Player.Position.LSTo2D().LSExtend(InsecPoint.LSTo2D(), -300);
            Vector2 end1 = start1.LSExtend(Player.Position.LSTo2D(), 750);
            float width1 = Program._r.Level == 3 ? 125 * 6 / 2 :
                        Program._r.Level == 2 ? 125 * 5 / 2 :
                        125 * 4 / 2;
            var Rect1 = new Geometry.Polygon.Rectangle(start1, end1, width1 - 100);
            var Predicted1 = Prediction.GetPrediction(target, Game.Ping / 1000f + 0.25f).UnitPosition;
            if (Rect1.IsInside(target.Position) && Rect1.IsInside(Predicted1))
            {
                Program._r2.Cast(InsecPoint);
                return;
            }
            if (Environment.TickCount - LastJump < 1500)
                return;
            if (!Program._e.LSIsReady())
                return;
            //case 2
            var sold2 = Soldiers.soldier
                    .Where(x => Player.LSDistance(x.Position) <= 1100)
                    .OrderBy(x => x.Position.LSDistance(target.Position)).FirstOrDefault();
            if (sold2 != null)
            {
                if (!Program._q2.LSIsReady())
                {
                    var time = Player.Position.LSDistance(sold2.Position) / 1700f;
                    var predicted2 = Prediction.GetPrediction(target, time).UnitPosition;
                    Vector2 start2 = sold2.Position.LSTo2D().LSExtend(InsecPoint.LSTo2D(), -300);
                    Vector2 end2 = start2.LSExtend(InsecPoint.LSTo2D(), 750);
                    float width2 = Program._r.Level == 3 ? 125 * 6 / 2 :
                                Program._r.Level == 2 ? 125 * 5 / 2 :
                                125 * 4 / 2;
                    var Rect2 = new Geometry.Polygon.Rectangle(start2, end2, width2 - 100);
                    if (Rect2.IsInside(target.Position) && Rect2.IsInside(predicted2))
                    {
                        Program._e.Cast(sold2.Position);
                        LastJump = Environment.TickCount;
                        return;
                    }
                }
                if (Program._q2.LSIsReady() && target.LSDistance(sold2.Position) <= 875 - 100)
                {
                    var time = (Player.LSDistance(sold2.Position) + sold2.Position.LSDistance(target.Position)) / 1700f;
                    var predicted2 = Prediction.GetPrediction(target, time).UnitPosition;
                    Vector2 start2 = target.Position.LSTo2D().LSExtend(InsecPoint.LSTo2D(), -300);
                    Vector2 end2 = start2.LSExtend(InsecPoint.LSTo2D(), 750);
                    float width2 = Program._r.Level == 3 ? 125 * 6 / 2 :
                                Program._r.Level == 2 ? 125 * 5 / 2 :
                                125 * 4 / 2;
                    var Rect2 = new Geometry.Polygon.Rectangle(start2, end2, width2 - 100);
                    if (Rect2.IsInside(target.Position) && Rect2.IsInside(predicted2))
                    {
                        var timetime = sold2.Position.LSDistance(Player.Position) * 1000 / 1700;
                        Program._e.Cast(sold2.Position);
                        LeagueSharp.Common.Utility.DelayAction.Add((int)timetime - 150 - Program.EQdelay, () => Program._q2.Cast(target.Position));
                        LastJump = Environment.TickCount;
                        return;
                    }
                }
            }
            if(Program._w.LSIsReady())
            {
                var posWs = GeoAndExten.GetWsPosition(target.Position.LSTo2D()).Where(x => x != null);
                foreach (var posW in posWs)
                {
                    if (!Program._q2.LSIsReady())
                    {
                        var time = Player.Position.LSTo2D().LSDistance((Vector2)posW) / 1700f + 0.3f;
                        var predicted2 = Prediction.GetPrediction(target, time).UnitPosition;
                        Vector2 start2 = ((Vector2)posW).LSExtend(InsecPoint.LSTo2D(), -300);
                        Vector2 end2 = start2.LSExtend(InsecPoint.LSTo2D(), 750);
                        float width2 = Program._r.Level == 3 ? 125 * 6 / 2 :
                                    Program._r.Level == 2 ? 125 * 5 / 2 :
                                    125 * 4 / 2;
                        var Rect2 = new Geometry.Polygon.Rectangle(start2, end2, width2 - 100);
                        if (Rect2.IsInside(target.Position) && Rect2.IsInside(predicted2))
                        {
                            var timetime = ((Vector2)posW).LSDistance(Player.Position) * 1000 / 1700;
                            Program._w.Cast(Player.Position.LSTo2D().LSExtend((Vector2)posW, Program._w.Range));
                            LeagueSharp.Common.Utility.DelayAction.Add(0, () => Program._e.Cast((Vector2)posW));
                            LeagueSharp.Common.Utility.DelayAction.Add((int)timetime + 300 - 150 - Program.EQdelay, () => Program._q2.Cast(target.Position));
                            LastJump = Environment.TickCount;
                            return;
                        }
                    }
                    if (Program._q2.LSIsReady() && target.LSDistance((Vector2)posW) <= 875 - 100)
                    {
                        var time = (Player.LSDistance((Vector2)posW) + ((Vector2)posW).LSDistance(target.Position)) / 1700f + 0.3f;
                        var predicted2 = Prediction.GetPrediction(target, time).UnitPosition;
                        Vector2 start2 = target.Position.LSTo2D().LSExtend(InsecPoint.LSTo2D(), -300);
                        Vector2 end2 = start2.LSExtend(InsecPoint.LSTo2D(), 750);
                        float width2 = Program._r.Level == 3 ? 125 * 6 / 2 :
                                    Program._r.Level == 2 ? 125 * 5 / 2 :
                                    125 * 4 / 2;
                        var Rect2 = new Geometry.Polygon.Rectangle(start2, end2, width2 - 100);
                        if (Rect2.IsInside(target.Position) && Rect2.IsInside(predicted2))
                        {
                            var timetime = ((Vector2)posW).LSDistance(Player.Position) * 1000 / 1700;
                            Program._w.Cast(Player.Position.LSTo2D().LSExtend((Vector2)posW, Program._w.Range));
                            LeagueSharp.Common.Utility.DelayAction.Add(0, () => Program._e.Cast((Vector2)posW));
                            LeagueSharp.Common.Utility.DelayAction.Add((int)timetime + 300 - 150 - Program.EQdelay, () => Program._q2.Cast(target.Position));
                            LastJump = Environment.TickCount;
                            return;
                        }
                    }
                }
            }

        }
    }
}
