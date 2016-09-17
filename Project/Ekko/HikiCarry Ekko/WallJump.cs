using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SSHCommon;
using LeagueSharp;
using LeagueSharp.Common;
using Microsoft.SqlServer.Server;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Ekko
{
    public class WallJump
    {
        public static Vector2 Position;
        public static Vector2 WallStartPosition;
        public static Vector2 EndPosition;
        public static Vector2 LastWallStartPosition;
        public static Vector2 LastEndPosition;
        public static Vector2 Position2;
        public static Vector2 Position3;
        public static Vector2 WardJumpPosition;
        public static Vector2 LastWallJumpStartPosition;
        public static Vector2 LastWallJumpEndPosition;
        public static int SpellCount = 0;
        public static int Tick = 0;
        public static int WallJumpTick = 0;
        public static Color DrawColor;
        public static Color LastColor;

        public static void Jumpo(Spell spell, string menuName)
        {
            WallJumpo(spell, menuName);
        }
        public static void WallJumpo(Spell spell, string menuname)
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }
            CheckSpell(spell);
            if (Program.Champion.misc.Item(menuname).GetValue<KeyBind>().Active)
            {
                if (DrawColor == Color.Green)
                {
                    if (ObjectManager.Player.Distance(LastWallStartPosition) > 50)
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, LastWallStartPosition.To3D());
                    else
                        if (Prediction.GetPrediction(ObjectManager.Player, 500).UnitPosition.Distance(ObjectManager.Player.Position) <= 10 && SpellCount == 1)
                            spell.Cast(LastEndPosition);
                }
                else if (SpellCount == 1 && DrawColor == Color.ForestGreen)
                {
                    DrawColor = Color.White;

                    spell.Cast(LastWallJumpEndPosition);
                }
            }


            Vector2 dir = new Vector2(0, 0);

            float scale = 200;

            Position = ObjectManager.Player.Position.To2D() + scale * ObjectManager.Player.Direction.To2D().Perpendicular();

            if (LeagueSharp.Common.Utility.IsWall(Position))
            {
                dir = scale * ObjectManager.Player.Direction.To2D().Perpendicular();
                Continue(dir);
            }

            if (!LeagueSharp.Common.Utility.IsWall(Position) || LastColor == Color.Red)
            {
                scale = 200;

                Position2 = ObjectManager.Player.Position.To2D() + (scale * ObjectManager.Player.Direction.To2D().Perpendicular()).Perpendicular();
                Position3 = ObjectManager.Player.Position.To2D() + (scale * ObjectManager.Player.Direction.To2D().Perpendicular()).Perpendicular2();

                if (LeagueSharp.Common.Utility.IsWall(Position2))
                {
                    dir = (scale * ObjectManager.Player.Direction.To2D().Perpendicular()).Perpendicular();
                    WardJumpPosition = Position2;
                    Continue(dir, true);
                }
                else if (LeagueSharp.Common.Utility.IsWall(Position3))
                {
                    dir = (scale * ObjectManager.Player.Direction.To2D().Perpendicular()).Perpendicular2();
                    WardJumpPosition = Position3;
                    Continue(dir, true);
                }
            }
        }
        public static void WallJumpDraw()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }
            if (WallStartPosition.Distance(EndPosition) >= 100 && DrawColor != Color.ForestGreen &&
                DrawColor != Color.DarkRed)
            {
                if (!Program.Champion.misc.Item("jump").GetValue<KeyBind>().Active)
                {
                    LastWallStartPosition = WallStartPosition;
                    LastEndPosition = EndPosition;
                    LastColor = DrawColor;
                    Tick = Utils.TickCount;
                }
            }
            else if (WardJumpPosition.Distance(ObjectManager.Player.Position) >= 100 && DrawColor == Color.ForestGreen)
            {
                //walljumps
                if (!Program.Champion.misc.Item("jump").GetValue<KeyBind>().Active)
                {
                    LastWallJumpEndPosition = WardJumpPosition;
                    WallJumpTick = Utils.TickCount;
                }
            }
            else if (WardJumpPosition.Distance(ObjectManager.Player.Position) >= 100 && DrawColor == Color.DarkRed)
                Render.Circle.DrawCircle(LastWallJumpEndPosition.To3D(), 100, Color.DarkRed);

            if (LastWallStartPosition != null && LastEndPosition != null
                && Utils.TickCount - Tick <= 1000 && DrawColor != Color.ForestGreen)
            {
                Render.Circle.DrawCircle(LastWallStartPosition.To3D(), 100, LastColor);
                Render.Circle.DrawCircle(LastEndPosition.To3D(), 100, LastColor);
            }
            else if (LastWallStartPosition != null && LastEndPosition != null && Utils.TickCount - WallJumpTick <= 1000 &&
                DrawColor == Color.ForestGreen)
            {
                Render.Circle.DrawCircle(LastWallJumpEndPosition.To3D(), 100, Color.ForestGreen);
            }
        }
        public static void CheckSpell(Spell spell)
        {
            if (spell.IsReady())
                SpellCount = 1;

            if (!spell.IsReady())
                SpellCount = 0;
        }
        public static void Continue(Vector2 dir, bool wardJump = false)
        {
            EndPosition = WEndPosition(ObjectManager.Player.Position.To2D(), dir);
            WallStartPosition = WStartPosition(ObjectManager.Player.Position.To2D(), dir);

            if (!wardJump)
            {
                if (WallStartPosition.Distance(EndPosition) / 2 < 325f)
                {
                    DrawColor = Color.Green;
                }
                else
                {
                    DrawColor = Color.Red;
                }
            }
            else if (wardJump)
            {
                if (WallStartPosition.Distance(EndPosition) / 2 < 325f)
                {
                    DrawColor = Color.ForestGreen;
                }
                else
                {
                    DrawColor = Color.DarkRed;
                }

            }

        }
        public static List<Vector2> ExtendDist(Vector2 origin, Vector2 currWallPos, float dist)
        {
            //getting OPs
            List<Vector2> resultVecs = new List<Vector2>();

            for (float i = 1; i <= dist; i += .05f)
            {
                if (!LeagueSharp.Common.Utility.IsWall(origin.Extend(currWallPos, i)))
                    resultVecs.Add(origin.Extend(currWallPos, i));
            }

            return resultVecs;
        }
        public static Vector2 WStartPosition(Vector2 op, Vector2 dir)
        {
            if (!LeagueSharp.Common.Utility.IsWall(op + dir))
                return new Vector2();

            Vector2 endPos = op + dir;

            for (float i = 1; i > 0; i -= .005f)
            {
                if (!LeagueSharp.Common.Utility.IsWall(endPos))
                    break;

                endPos = op + Vector2.Multiply(dir, i);
            }

            return endPos;
        }
        public static Vector2 WEndPosition(Vector2 op, Vector2 dir)
        {
            if (!LeagueSharp.Common.Utility.IsWall(op + dir))
                return new Vector2();

            Vector2 endPos = op + dir;

            for (float i = 1; i < 500; i += .1f)
            {
                if (!LeagueSharp.Common.Utility.IsWall(endPos))
                    break;

                endPos = op + Vector2.Multiply(dir, i);
            }

            return endPos;
        }
        public static Vector2 BuildEndPos(Vector2 startPos, Vector2 currentPos)
        {
            var v1 = startPos - currentPos;

            Vector2 result = startPos + v1;

            for (float i = 1; i < 500; i += .1f)
            {
                if (!LeagueSharp.Common.Utility.IsWall(result))
                {
                    break;
                }
                result = startPos + Vector2.Multiply(v1, i);
            }

            return result;
        }
    }
}
