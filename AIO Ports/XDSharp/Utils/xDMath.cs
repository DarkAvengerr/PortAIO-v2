using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace XDSharp.Utils
{
    class xDMath
    {
        public static int FindLineCircleIntersections(Vector3 target, float radius, Vector3 Spellpos, Vector3 Player, out Vector3 intersection1, out Vector3 intersection2)
        {
            float dx, dy, dz, A, B, C, det, t;

            dx = Spellpos.X - Player.X;
            dy = Spellpos.Y - Player.Y;
            dz = Spellpos.Z - Player.Z;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (Player.X - target.X) + dy * (Player.Y - target.Y) + dz * (Player.Z - target.Z));
            C = (Player.X - target.X) * (Player.X - target.X) +
                (Player.Y - target.Y) * (Player.Y - target.Y) +
                (Player.Z - target.Z) * (Player.Z - target.Z) -
                radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                intersection1 = new Vector3(float.NaN, float.NaN, float.NaN);
                intersection2 = new Vector3(float.NaN, float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                t = -B / (2 * A);
                intersection1 =
                    new Vector3(Player.X + t * dx, Player.Y + t * dy, Player.Z + t * dz);
                intersection2 = new Vector3(float.NaN, float.NaN, float.NaN);
                return 1;
            }
            else
            {
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 =
                    new Vector3(Player.X + t * dx, Player.Y + t * dy, Player.Z + t * dz);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 =
                    new Vector3(Player.X + t * dx, Player.Y + t * dy, Player.Z + t * dz);
                return 2;
            }
        }

        public static Vector2[] CircleCircleIntersection(Vector2 center1, Vector2 center2, float radius1, float radius2)
        {
            var D = center1.Distance(center2);

            if (D > radius1 + radius2 || (D <= Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] { };
            }

            var A = (radius1 * radius1 - radius2 * radius2 + D * D) / (2 * D);
            var H = (float)Math.Sqrt(radius1 * radius1 - A * A);
            var Direction = (center2 - center1).Normalized();
            var PA = center1 + A * Direction;
            var S1 = PA + H * Direction.Perpendicular();
            var S2 = PA - H * Direction.Perpendicular();
            return new[] { S1, S2 };
        }

        public static double GetAngle(Vector2 StartPos, Vector2 EndPos)
        {
            return Math.Atan2((EndPos.Y - StartPos.Y), (EndPos.X - StartPos.X));
        }

        public static Vector2 GetLine(Vector2 StartPos,Vector2 EndPos, double Range)
        {
            Vector2 Pos;
            double x, y;

            x = StartPos.X + Math.Cos(GetAngle(StartPos,EndPos)) * Range;
            y = StartPos.Y + Math.Sin(GetAngle(StartPos,EndPos)) * Range;

            Pos.X = (float)x;
            Pos.Y = (float)y;

            return Pos;
        }



    }
}
