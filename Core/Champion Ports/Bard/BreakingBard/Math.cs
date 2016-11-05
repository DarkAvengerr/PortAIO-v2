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
 namespace BreakingBard
{
    class Math
    {
        static float PI = 3.14159265358979323846f;

        public static Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
        {     
            float x = origin.X + (float)(radius * System.Math.Cos(angleInDegrees * Math.PI / 180));
            float y = origin.Y + (float)(radius * System.Math.Sin(angleInDegrees * Math.PI / 180));

            return new Vector2(x, y);
        }

        public static List<Vector2> GetAllDirectionVectorsOfCircle(Vector2 origin, float radius)
        {
            //getting OPs
            List<Vector2> resultVecs = new List<Vector2>();

            for (int degrees = 1; degrees < 361; degrees++)
            {
                resultVecs.Add(PointOnCircle(radius, degrees, origin));
            }

            return resultVecs;
        }

        public static Vector2 IsWall_DownScaleVector(Vector2 OP_from, Vector2 dirVec)
        {
            //down scales the vector until it reaches the min dist being connected with the wall

            Vector2 lastScaledVec = new Vector2(0, 0);

            int calcs = 0;

            for (float i = 1f; i > 0; i -= 0.001f)
            {
                Vector2 scaledVec = OP_from + Vector2.Multiply(dirVec, i);

                if (!LeagueSharp.Common.Utility.IsWall(scaledVec))
                    return lastScaledVec;

                calcs++;
                lastScaledVec = scaledVec;
            }

            return new Vector2(0, 0);
        }

        public static Vector2 ExtendVectorToX_CheckDist(Vector2 OP_from, Vector2 Dir_from, float maxValue,
            float minDist_Player_Enemy)
        {
            Vector2 finalVec = new Vector2(0, 0);

            for (float i = 1.01f; Vector2.Multiply(Dir_from, i).Length() < maxValue; i += 0.01f)
            {
                Vector2 result = OP_from + Vector2.Multiply(Dir_from, i);

                if (result.Distance(ObjectManager.Player) <= 450 && result.Distance(OP_from + Dir_from) >=
                    minDist_Player_Enemy)
                {
                    finalVec = result;
                    break;
                }
            }

            //OP vec
            return finalVec;
        }
    }
}
