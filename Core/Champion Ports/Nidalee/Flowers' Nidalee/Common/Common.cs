using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Nidalee.Common
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using Prediction;
    using SharpDX;
    using SPrediction;
    using System;
    using Prediction = Flowers_Nidalee.Prediction;

    public static class Common
    {
        public static void CastTo(this Spell Spells, Obj_AI_Base target, bool AOE = false)
        {
            switch (Program.Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    {
                        var SpellPred = Spells.GetPrediction(target, AOE);

                        if (SpellPred.Hitchance >= HitChance.VeryHigh)
                        {
                            Spells.Cast(SpellPred.CastPosition, true);
                        }
                    }
                    break;
                case 1:
                    {
                        Prediction.OKTWPrediction.SkillshotType CoreType2 = Prediction.OKTWPrediction.SkillshotType.SkillshotLine;

                        if (Spells.Type == SkillshotType.SkillshotCircle)
                        {
                            CoreType2 = Prediction.OKTWPrediction.SkillshotType.SkillshotCircle;
                        }

                        var predInput2 = new Prediction.OKTWPrediction.PredictionInput
                        {
                            Aoe = AOE,
                            Collision = Spells.Collision,
                            Speed = Spells.Speed,
                            Delay = Spells.Delay,
                            Range = Spells.Range,
                            From = ObjectManager.Player.ServerPosition,
                            Radius = Spells.Width,
                            Unit = target,
                            Type = CoreType2
                        };

                        var poutput2 = Prediction.OKTWPrediction.Prediction.GetPrediction(predInput2);

                        if (Spells.Speed != float.MaxValue && YasuoWindWall.CollisionYasuo(ObjectManager.Player.ServerPosition, poutput2.CastPosition))
                        {
                            return;
                        }

                        if (poutput2.Hitchance >= Prediction.OKTWPrediction.HitChance.VeryHigh)
                        {
                            Spells.Cast(poutput2.CastPosition, true);
                        }
                        else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= Prediction.OKTWPrediction.HitChance.High)
                        {
                            Spells.Cast(poutput2.CastPosition, true);
                        }
                    }
                    break;
                case 2:
                    {
                        Prediction.SDKPrediction.SkillshotType CoreType2 = Prediction.SDKPrediction.SkillshotType.SkillshotLine;

                        var predInput2 = new Prediction.SDKPrediction.PredictionInput
                        {
                            AoE = AOE,
                            Collision = Spells.Collision,
                            Speed = Spells.Speed,
                            Delay = Spells.Delay,
                            Range = Spells.Range,
                            From = ObjectManager.Player.ServerPosition,
                            Radius = Spells.Width,
                            Unit = target,
                            Type = CoreType2
                        };

                        var poutput2 = Prediction.SDKPrediction.GetPrediction(predInput2);

                        if (Spells.Speed != float.MaxValue && YasuoWindWall.CollisionYasuo(ObjectManager.Player.ServerPosition, poutput2.CastPosition))
                        {
                            return;
                        }

                        if (poutput2.Hitchance >= Prediction.SDKPrediction.HitChance.VeryHigh)
                        {
                            Spells.Cast(poutput2.CastPosition, true);
                        }
                        else if (predInput2.AoE && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= Prediction.SDKPrediction.HitChance.High)
                        {
                            Spells.Cast(poutput2.CastPosition, true);
                        }
                    }
                    break;
                case 3:
                    {
                        if (target is AIHeroClient && target.IsValid)
                        {
                            var t = target as AIHeroClient;

                            if (t.IsValidTarget())
                            {
                                Spells.SPredictionCast(t, HitChance.VeryHigh);
                                return;
                            }
                        }
                        else
                        {
                            Spells.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                        }
                    }
                    break;
                case 4:
                    {
                        if (Spells.Type == SkillshotType.SkillshotCircle)
                        {
                            Spells.CastCircle(target);
                        }
                        else if (Spells.Type == SkillshotType.SkillshotLine)
                        {
                            Spells.CastLine(target);
                        }
                        else if (Spells.Type == SkillshotType.SkillshotCone)
                        {
                            Spells.CastCone(target);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public static float DistanceSquared(this Obj_AI_Base source, Vector3 position)
        {
            return source.DistanceSquared(position.To2D());
        }

        public static float DistanceSquared(this Obj_AI_Base source, Vector2 position)
        {
            return source.ServerPosition.DistanceSquared(position);
        }

        public static float DistanceSquared(this Vector3 vector3, Vector2 toVector2)
        {
            return vector3.To2D().DistanceSquared(toVector2);
        }

        public static float DistanceSquared(this Vector2 vector2, Vector2 toVector2)
        {
            return Vector2.DistanceSquared(vector2, toVector2);
        }

        public static float DistanceSquared(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.DistanceSquared(target.ServerPosition);
        }

        public static float DistanceSquared(this Vector3 vector3, Vector3 toVector3)
        {
            return vector3.To2D().DistanceSquared(toVector3);
        }

        public static float DistanceSquared(this Vector2 vector2, Vector3 toVector3)
        {
            return Vector2.DistanceSquared(vector2, toVector3.To2D());
        }

        public static float DistanceSquared(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool onlyIfOnSegment = false)
        {
            var objects = point.ProjectOn(segmentStart, segmentEnd);

            return (objects.IsOnSegment || onlyIfOnSegment == false) ? Vector2.DistanceSquared(objects.SegmentPoint, point) : float.MaxValue;
        }

        public static Vector2[] CircleCircleIntersection(this Vector2 center1, Vector2 center2, float radius1, float radius2)
        {
            var d = center1.Distance(center2);

            if (d > radius1 + radius2 || (d <= Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] { };
            }

            var a = ((radius1 * radius1) - (radius2 * radius2) + (d * d)) / (2 * d);
            var h = (float)Math.Sqrt((radius1 * radius1) - (a * a));
            var direction = (center2 - center1).Normalized();
            var pa = center1 + (a * direction);
            var s1 = pa + (h * direction.Perpendicular());
            var s2 = pa - (h * direction.Perpendicular());

            return new[] { s1, s2 };
        }

        public static bool Compare(this GameObject gameObject, GameObject @object)
        {
            return gameObject != null && gameObject.IsValid && @object != null && @object.IsValid && gameObject.NetworkId == @object.NetworkId;
        }

        public static MovementCollisionInfo VectorMovementCollision( this Vector2 pointStartA, Vector2 pointEndA, float pointVelocityA, Vector2 pointB, float pointVelocityB, float delay = 0f)
        {
            return new[]
            {
                pointStartA,
                pointEndA }
            .VectorMovementCollision(pointVelocityA, pointB, pointVelocityB, delay);
        }

        public static MovementCollisionInfo VectorMovementCollision(this Vector2[] pointA, float pointVelocityA, Vector2 pointB, float pointVelocityB, float delay = 0f)
        {
            if (pointA.Length < 1)
            {
                return default(MovementCollisionInfo);
            }

            float sP1X = pointA[0].X, sP1Y = pointA[0].Y, eP1X = pointA[1].X, eP1Y = pointA[1].Y, sP2X = pointB.X, sP2Y = pointB.Y;
            float d = eP1X - sP1X, e = eP1Y - sP1Y;
            float dist = (float)Math.Sqrt((d * d) + (e * e)), t1 = float.NaN;
            float s = Math.Abs(dist) > float.Epsilon ? pointVelocityA * d / dist : 0, k = (Math.Abs(dist) > float.Epsilon) ? pointVelocityA * e / dist : 0f;

            float r = sP2X - sP1X, j = sP2Y - sP1Y;
            var c = (r * r) + (j * j);

            if (dist > 0f)
            {
                if (Math.Abs(pointVelocityA - float.MaxValue) < float.Epsilon)
                {
                    var t = dist / pointVelocityA;

                    t1 = pointVelocityB * t >= 0f ? t : float.NaN;
                }
                else if (Math.Abs(pointVelocityB - float.MaxValue) < float.Epsilon)
                {
                    t1 = 0f;
                }
                else
                {
                    float a = (s * s) + (k * k) - (pointVelocityB * pointVelocityB), b = (-r * s) - (j * k);

                    if (Math.Abs(a) < float.Epsilon)
                    {
                        if (Math.Abs(b) < float.Epsilon)
                        {
                            t1 = (Math.Abs(c) < float.Epsilon) ? 0f : float.NaN;
                        }
                        else
                        {
                            var t = -c / (2 * b);

                            t1 = (pointVelocityB * t >= 0f) ? t : float.NaN;
                        }
                    }
                    else
                    {
                        var sqr = (b * b) - (a * c);

                        if (sqr >= 0)
                        {
                            var nom = (float)Math.Sqrt(sqr);
                            var t = (-nom - b) / a;

                            t1 = pointVelocityB * t >= 0f ? t : float.NaN;
                            t = (nom - b) / a;

                            var t2 = (pointVelocityB * t >= 0f) ? t : float.NaN;

                            if (!float.IsNaN(t2) && !float.IsNaN(t1))
                            {
                                if (t1 >= delay && t2 >= delay)
                                {
                                    t1 = Math.Min(t1, t2);
                                }
                                else if (t2 >= delay)
                                {
                                    t1 = t2;
                                }
                            }
                        }
                    }
                }
            }
            else if (Math.Abs(dist) < float.Epsilon)
            {
                t1 = 0f;
            }

            return new MovementCollisionInfo(t1, !float.IsNaN(t1) ? new Vector2(sP1X + (s * t1), sP1Y + (k * t1)) : default(Vector2));
        }

        public struct MovementCollisionInfo
        {
            public Vector2 CollisionPosition;
            public float CollisionTime;

            internal MovementCollisionInfo(float collisionTime, Vector2 collisionPosition)
            {
                this.CollisionTime = collisionTime;
                this.CollisionPosition = collisionPosition;
            }

            public object this[int i] => i == 0 ? this.CollisionTime : (object)this.CollisionPosition;
        }

        public static float DistanceToPlayer(this Obj_AI_Base source)
        {
            return ObjectManager.Player.Distance(source);
        }

        public static float DistanceToPlayer(this Vector3 position)
        {
            return position.To2D().DistanceToPlayer();
        }

        public static float DistanceToPlayer(this Vector2 position)
        {
            return ObjectManager.Player.Distance(position);
        }
    }
}
