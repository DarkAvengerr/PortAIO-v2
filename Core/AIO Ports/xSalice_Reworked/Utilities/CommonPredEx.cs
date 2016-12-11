using EloBuddy; 
using LeagueSharp.Common; 
namespace xSaliceResurrected_Rework.Prediction
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public class CommonPredEx
    {
        public static PredictionOutput GetP(Vector3 pos, Spell spell, Obj_AI_Base target, float delay, bool aoe)
        {
            return Prediction.GetPrediction(new PredictionInput
            {
                Unit = target,
                Delay = spell.Delay + delay,
                Radius = spell.Width,
                Speed = spell.Speed,
                From = pos,
                Range = spell.Range,
                Collision = spell.Collision,
                Type = spell.Type,
                RangeCheckFrom = ObjectManager.Player.ServerPosition,
                Aoe = aoe,
            });
        }

        public static PredictionOutput GetP(Vector3 pos, Spell spell, Obj_AI_Base target, bool aoe)
        {
            return Prediction.GetPrediction(new PredictionInput
            {
                Unit = target,
                Delay = spell.Delay,
                Radius = spell.Width,
                Speed = spell.Speed,
                From = pos,
                Range = spell.Range,
                Collision = spell.Collision,
                Type = spell.Type,
                RangeCheckFrom = ObjectManager.Player.ServerPosition,
                Aoe = aoe,
            });
        }

        public static PredictionOutput GetPCircle(Vector3 pos, Spell spell, Obj_AI_Base target, bool aoe)
        {
            return Prediction.GetPrediction(new PredictionInput
            {
                Unit = target,
                Delay = spell.Delay,
                Radius = 1,
                Speed = float.MaxValue,
                From = pos,
                Range = float.MaxValue,
                Collision = spell.Collision,
                Type = spell.Type,
                RangeCheckFrom = pos,
                Aoe = aoe,
            });
        }

        public static object[] VectorPointProjectionOnLineSegment(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            var cx = v3.X;
            var cy = v3.Y;
            var ax = v1.X;
            var ay = v1.Y;
            var bx = v2.X;
            var by = v2.Y;
            var rL = ((cx - ax) * (bx - ax) + (cy - ay) * (by - ay)) /
                       ((float)Math.Pow(bx - ax, 2) + (float)Math.Pow(by - ay, 2));
            var pointLine = new Vector2(ax + rL * (bx - ax), ay + rL * (by - ay));

            float rS;

            if (rL < 0)
            {
                rS = 0;
            }
            else if (rL > 1)
            {
                rS = 1;
            }
            else
            {
                rS = rL;
            }

            var isOnSegment = rS.CompareTo(rL) == 0;
            var pointSegment = isOnSegment ? pointLine : new Vector2(ax + rS * (bx - ax), ay + rS * (by - ay));

            return new object[]
            {
                pointSegment,
                pointLine,
                isOnSegment
            };
        }

    }
}