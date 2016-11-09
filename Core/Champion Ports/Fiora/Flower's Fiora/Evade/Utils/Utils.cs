using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Fiora.Evade
{
    using System;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;

    public static class Utils
    {
        public static int TickCount => (int)(Game.Time * 1000f);

        public static List<Vector2> To2DList(this Vector3[] v)
        {
            var result = new List<Vector2>();

            foreach (var point in v)
            {
                result.Add(point.To2D());
            }

            return result;
        }

        public static Obj_AI_Base Closest(List<Obj_AI_Base> targetList, Vector2 from)
        {
            var dist = float.MaxValue;

            Obj_AI_Base result = null;

            foreach (var target in targetList)
            {
                var distance = Vector2.DistanceSquared(from, target.ServerPosition.To2D());

                if (distance < dist)
                {
                    dist = distance;
                    result = target;
                }
            }

            return result;
        }

        public static bool LineSegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            var denominator = (b.X - a.X)*(d.Y - c.Y) - (b.Y - a.Y)*(d.X - c.X);

            if (denominator == 0)
            {
                return false;
            }

            var numerator1 = (a.Y - c.Y)*(d.X - c.X) - (a.X - c.X)*(d.Y - c.Y);

            var numerator2 = (a.Y - c.Y)*(b.X - a.X) - (a.X - c.X)*(b.Y - a.Y);

            if (numerator1 == 0 || numerator2 == 0)
            {
                return false;
            }

            var r = numerator1 / denominator;
            var s = numerator2 / denominator;

            return r > 0 && r < 1 && s > 0 && s < 1;
        }

        public static int ImmobileTime(Obj_AI_Base unit)
        {
            var result = 0f;

            foreach (var buff in unit.Buffs)
            {
                if (buff.IsActive && Game.Time <= buff.EndTime &&
                    (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                     buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                {
                    result = Math.Max(result, buff.EndTime);
                }
            }

            return result == 0f ? -1 : (int) (TickCount + (result - Game.Time)*1000);
        }

        public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, Color color)
        {
            var from = Drawing.WorldToScreen(start);
            var to = Drawing.WorldToScreen(end);
            Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
        }
    }
}