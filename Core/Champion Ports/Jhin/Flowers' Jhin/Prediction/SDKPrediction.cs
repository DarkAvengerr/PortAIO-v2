using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Jhin.Prediction
{
    using Common;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SDKPrediction
    {
        [Flags]
        public enum CollisionableObjects
        {
            Minions = 1,
            Heroes = 2,
            YasuoWall = 4,
            BraumShield = 8,
            Walls = 16
        }

        public enum HitChance
        {
            Immobile = 8,
            Dashing = 7,
            VeryHigh = 6,
            High = 5,
            Medium = 4,
            Low = 3,
            Impossible = 2,
            OutOfRange = 1,
            Collision = 0,
            None = -1
        }

        public enum SkillshotType
        {
            SkillshotLine,
            SkillshotCircle,
            SkillshotCone
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay)
        {
            return GetPrediction(new PredictionInput
            {
                Unit = unit,
                Delay = delay
            });
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius)
        {
            return GetPrediction(new PredictionInput
            {
                Unit = unit,
                Delay = delay,
                Radius = radius
            });
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius, float speed)
        {
            return GetPrediction(new PredictionInput
            {
                Unit = unit,
                Delay = delay,
                Radius = radius,
                Speed = speed
            });
        }

        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return GetPrediction(input, true, true);
        }

        internal static PredictionOutput GetDashingPrediction(PredictionInput input)
        {
            var dashData = input.Unit.GetDashInfo();
            var result = new PredictionOutput
            {
                Input = input,
                Hitchance = HitChance.Medium
            };

            if (!dashData.IsBlink)
            {
                var endP = dashData.EndPos.To3D();
                var dashPred = GetPositionOnPath(input, new List<Vector3>
                {
                    input.Unit.ServerPosition,
                    endP
                }.To2D(), dashData.Speed);

                if (dashPred.Hitchance >= HitChance.High && dashPred.UnitPosition.To2D().Distance(input.Unit.Position.To2D(), endP.To2D(), true) < 200)
                {
                    dashPred.CastPosition = dashPred.UnitPosition;
                    dashPred.Hitchance = HitChance.Dashing;

                    return dashPred;
                }

                if (dashData.Path.PathLength() > 200)
                {
                    var timeToPoint = input.Delay / 2f + (Math.Abs(input.Speed - float.MaxValue) > float.Epsilon ? input.From.Distance(endP) / input.Speed : 0) - 0.25f;

                    if (timeToPoint <= input.Unit.Distance(endP) / dashData.Speed + input.RealRadius / input.Unit.MoveSpeed)
                    {
                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = endP,
                            UnitPosition = endP,
                            Hitchance = HitChance.Dashing
                        };
                    }
                }

                result.CastPosition = endP;
                result.UnitPosition = result.CastPosition;
            }

            return result;
        }

        internal static PredictionOutput GetImmobilePrediction(PredictionInput input, double remainingImmobileT)
        {
            var result = new PredictionOutput
            {
                Input = input,
                CastPosition = input.Unit.ServerPosition,
                UnitPosition = input.Unit.ServerPosition,
                Hitchance = HitChance.High
            };

            var timeToReachTargetPosition = input.Delay + (Math.Abs(input.Speed - float.MaxValue) > float.Epsilon ? input.Unit.Distance(input.From) / input.Speed : 0);

            if (timeToReachTargetPosition <= remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed)
            {
                result.UnitPosition = input.Unit.Position;
                result.Hitchance = HitChance.Immobile;
            }

            return result;
        }

        internal static PredictionOutput GetPositionOnPath(PredictionInput input, List<Vector2> path, float speed = -1)
        {
            speed = Math.Abs(speed - -1) < float.Epsilon ? input.Unit.MoveSpeed : speed;

            if (path.Count <= 1)
            {
                return new PredictionOutput
                {
                    Input = input,
                    UnitPosition = input.Unit.ServerPosition,
                    CastPosition = input.Unit.ServerPosition,
                    Hitchance = HitChance.VeryHigh
                };
            }

            var pLength = path.PathLength();
            var dist = input.Delay * speed - input.RealRadius;

            if (pLength >= dist && Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
                var tDistance = dist;

                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var d = a.Distance(b);

                    if (d >= tDistance)
                    {
                        var direction = (b - a).Normalized();
                        var cp = a + direction * tDistance;
                        var p = a + direction * (i == path.Count - 2 ? Math.Min(tDistance + input.RealRadius, d) : tDistance + input.RealRadius);

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = cp.To3D(),
                            UnitPosition = p.To3D(),
                            Hitchance = HitChance.High
                        };
                    }

                    tDistance -= d;
                }
            }

            if (pLength >= dist && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                var tDistance = dist;

                if ((input.Type == SkillshotType.SkillshotLine || input.Type == SkillshotType.SkillshotCone) && input.Unit.DistanceSquared(input.From) < 200 * 200)
                {
                    tDistance += input.RealRadius;
                }

                path = path.CutPath(tDistance);

                var tT = 0f;

                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var tB = a.Distance(b) / speed;
                    var direction = (b - a).Normalized();

                    a = a - speed * tT * direction;

                    var sol = a.VectorMovementCollision(b, speed, input.From.To2D(), input.Speed, tT);
                    var t = (float)sol[0];
                    var pos = (Vector2)sol[1];

                    if (pos.IsValid() && t >= tT && t <= tT + tB)
                    {
                        if (pos.DistanceSquared(b) < 20)
                        {
                            break;
                        }

                        var p = pos + input.RealRadius * direction;

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = pos.To3D(),
                            UnitPosition = p.To3D(),
                            Hitchance = HitChance.High
                        };
                    }

                    tT += tB;
                }
            }

            var position = path.Last().To3D();

            return new PredictionOutput
            {
                Input = input,
                CastPosition = position,
                UnitPosition = position,
                Hitchance = HitChance.Medium
            };
        }

        internal static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision)
        {
            if (!input.Unit.IsValidTarget(float.MaxValue, false))
            {
                return new PredictionOutput();
            }

            if (ft)
            {
                input.Delay += Game.Ping / 2000f + 0.06f;

                if (input.AoE)
                {
                    return Cluster.GetAoEPrediction(input);
                }
            }

            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon && input.Unit.DistanceSquared(input.RangeCheckFrom) > Math.Pow(input.Range * 1.5, 2))
            {
                return new PredictionOutput { Input = input };
            }

            PredictionOutput result = null;

            if (input.Unit.IsDashing())
            {
                result = GetDashingPrediction(input);
            }
            else
            {
                var remainingImmobileT = UnitIsImmobileUntil(input.Unit);

                if (remainingImmobileT >= 0d)
                {
                    result = GetImmobilePrediction(input, remainingImmobileT);
                }
            }

            if (result == null)
            {
                result = GetStandardPrediction(input);
            }

            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance >= HitChance.High && input.RangeCheckFrom.DistanceSquared(input.Unit.Position) > Math.Pow(input.Range + input.RealRadius * 3 / 4, 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.UnitPosition) > Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.RealRadius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.CastPosition) > Math.Pow(input.Range, 2) && result.Hitchance != HitChance.OutOfRange)
                {
                    result.CastPosition = input.RangeCheckFrom + input.Range * (result.UnitPosition - input.RangeCheckFrom).To2D().Normalized().To3D();
                }
            }

            if (checkCollision && input.Collision && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                var positions = new List<Vector3>
                {
                    result.UnitPosition,
                    input.Unit.Position
                };

                result.CollisionObjects = GetCollision(positions, input);
                result.CollisionObjects.RemoveAll(x => x.Compare(input.Unit));

                if (result.CollisionObjects.Count > 0)
                {
                    result.Hitchance = HitChance.Collision;
                }
            }

            if (result.Hitchance == HitChance.High)
            {
                result.Hitchance = GetHitChance(input);
            }

            return result;
        }

        public static List<Obj_AI_Base> GetCollision(List<Vector3> positions, PredictionInput input)
        {
            var result = new List<Obj_AI_Base>();

            foreach (var position in positions)
            {
                switch (input.CollisionObjects)
                {
                    case CollisionableObjects.Minions:
                        foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(Math.Min(input.Range + input.Radius + 100, 2000), true, input.RangeCheckFrom)))
                        {
                            input.Unit = minion;

                            var minionPrediction = GetPrediction(input, false, false);

                            if (minionPrediction.UnitPosition.To2D().Distance(input.From.To2D(), position.To2D(), true, true) <= Math.Pow((input.Radius + 15 + minion.BoundingRadius), 2))
                            {
                                result.Add(minion);
                            }
                        }
                        break;
                    case CollisionableObjects.Heroes:
                        foreach (var hero in HeroManager.Enemies.FindAll(hero => hero.IsValidTarget(Math.Min(input.Range + input.Radius + 100, 2000), true, input.RangeCheckFrom)))
                        {
                            input.Unit = hero;

                            var prediction = GetPrediction(input, false, false);

                            if (prediction.UnitPosition.To2D().Distance(input.From.To2D(), position.To2D(), true, true) <= Math.Pow((input.Radius + 50 + hero.BoundingRadius), 2))
                            {
                                result.Add(hero);
                            }
                        }
                        break;
                    case CollisionableObjects.Walls:
                        var step = position.Distance(input.From) / 20;

                        for (var i = 0; i < 20; i++)
                        {
                            var p = input.From.To2D().Extend(position.To2D(), step * i);

                            if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(CollisionFlags.Wall))
                            {
                                result.Add(ObjectManager.Player);
                            }
                        }
                        break;
                }
            }

            return result.Distinct().ToList();
        }

        public static class Cluster
        {
            public static PredictionOutput GetAoEPrediction(PredictionInput input)
            {
                switch (input.Type)
                {
                    case SkillshotType.SkillshotCircle:
                        return Circle.GetCirclePrediction(input);
                    case SkillshotType.SkillshotCone:
                        return Cone.GetConePrediction(input);
                    case SkillshotType.SkillshotLine:
                        return Line.GetLinePrediction(input);
                }

                return new PredictionOutput();
            }

            internal static List<PossibleTarget> GetPossibleTargets(PredictionInput input)
            {
                var result = new List<PossibleTarget>();

                foreach (var enemy in HeroManager.Enemies.Where(h => !h.Compare(input.Unit) && h.IsValidTarget(input.Range + 200 + input.RealRadius, true, input.RangeCheckFrom)))
                {
                    var inputs = input.Clone() as PredictionInput;

                    if (inputs == null)
                    {
                        continue;
                    }

                    inputs.Unit = enemy;

                    var prediction = GetPrediction(inputs, false, true);

                    if (prediction.Hitchance >= HitChance.High)
                    {
                        result.Add(new PossibleTarget
                        {
                            Position = prediction.UnitPosition.To2D(),
                            Unit = enemy
                        });
                    }
                }

                return result;
            }

            public static class Circle
            {
                public static PredictionOutput GetCirclePrediction(PredictionInput input)
                {
                    var mainTargetPrediction = GetPrediction(input, false, true);
                    var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.To2D(),
                                                     Unit = input.Unit
                                                 }
                                         };

                    if (mainTargetPrediction.Hitchance >= HitChance.High)
                    {
                        posibleTargets.AddRange(GetPossibleTargets(input));
                    }

                    while (posibleTargets.Count > 1)
                    {
                        var mecCircle = ConvexHull.GetMec(posibleTargets.Select(h => h.Position).ToList());

                        if (mecCircle.Radius <= input.RealRadius - 10
                            && mecCircle.Center.DistanceSquared(input.RangeCheckFrom) < input.Range * input.Range)
                        {
                            return new PredictionOutput
                            {
                                AoeTargetsHit = posibleTargets.Select(h => (AIHeroClient)h.Unit).ToList(),
                                CastPosition = mecCircle.Center.To3D(),
                                UnitPosition = mainTargetPrediction.UnitPosition,
                                Hitchance = mainTargetPrediction.Hitchance,
                                Input = input,
                                AoeHitCount = posibleTargets.Count
                            };
                        }

                        float maxdist = -1;
                        var maxdistindex = 1;

                        for (var i = 1; i < posibleTargets.Count; i++)
                        {
                            var distance = posibleTargets[i].Position.DistanceSquared(posibleTargets[0].Position);

                            if (distance > maxdist || maxdist.CompareTo(-1) == 0)
                            {
                                maxdistindex = i;
                                maxdist = distance;
                            }
                        }

                        posibleTargets.RemoveAt(maxdistindex);
                    }

                    return mainTargetPrediction;
                }
            }

            public static class Cone
            {
                public static PredictionOutput GetConePrediction(PredictionInput input)
                {
                    var mainTargetPrediction = GetPrediction(input, false, true);
                    var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.To2D(),
                                                     Unit = input.Unit
                                                 }
                                         };

                    if (mainTargetPrediction.Hitchance >= HitChance.High)
                    {
                        posibleTargets.AddRange(GetPossibleTargets(input));
                    }

                    if (posibleTargets.Count > 1)
                    {
                        var candidates = new List<Vector2>();

                        foreach (var target in posibleTargets)
                        {
                            target.Position = target.Position - input.From.To2D();
                        }

                        for (var i = 0; i < posibleTargets.Count; i++)
                        {
                            for (var j = 0; j < posibleTargets.Count; j++)
                            {
                                if (i == j)
                                {
                                    continue;
                                }

                                var p = (posibleTargets[i].Position + posibleTargets[j].Position) * 0.5f;

                                if (!candidates.Contains(p))
                                {
                                    candidates.Add(p);
                                }
                            }
                        }

                        var bestCandidateHits = -1;
                        var bestCandidate = default(Vector2);
                        var positionsList = posibleTargets.Select(t => t.Position).ToList();

                        foreach (var candidate in candidates)
                        {
                            var hits = GetHits(candidate, input.Range, input.Radius, positionsList);

                            if (hits > bestCandidateHits)
                            {
                                bestCandidate = candidate;
                                bestCandidateHits = hits;
                            }
                        }

                        if (bestCandidateHits > 1 && input.From.DistanceSquared(bestCandidate) > 50 * 50)
                        {
                            return new PredictionOutput
                            {
                                Hitchance = mainTargetPrediction.Hitchance,
                                AoeHitCount = bestCandidateHits,
                                UnitPosition = mainTargetPrediction.UnitPosition,
                                CastPosition = bestCandidate.To3D(),
                                Input = input
                            };
                        }
                    }

                    return mainTargetPrediction;
                }

                internal static int GetHits(Vector2 end, double range, float angle, List<Vector2> points)
                {
                    return (from point in points
                            let edge1 = end.Rotated(-angle / 2)
                            let edge2 = edge1.Rotated(angle)
                            where point.DistanceSquared(default(Vector2)) < range * range && edge1.CrossProduct(point) > 0 && point.CrossProduct(edge2) > 0
                            select point).Count();
                }
            }

            public static class Line
            {
                public static PredictionOutput GetLinePrediction(PredictionInput input)
                {
                    var mainTargetPrediction = GetPrediction(input, false, true);
                    var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.To2D(),
                                                     Unit = input.Unit
                                                 }
                                         };

                    if (mainTargetPrediction.Hitchance >= HitChance.High)
                    {
                        posibleTargets.AddRange(GetPossibleTargets(input));
                    }

                    if (posibleTargets.Count > 1)
                    {
                        var candidates = new List<Vector2>();

                        foreach (var targetCandidates in posibleTargets.Select(target => GetCandidates(input.From.To2D(), target.Position, input.Radius, input.Range)))
                        {
                            candidates.AddRange(targetCandidates);
                        }

                        var bestCandidateHits = -1;
                        var bestCandidate = default(Vector2);
                        var bestCandidateHitPoints = new List<Vector2>();
                        var positionsList = posibleTargets.Select(t => t.Position).ToList();

                        foreach (var candidate in candidates)
                        {
                            if (GetHits(input.From.To2D(), candidate, input.Radius + (input.Unit.BoundingRadius / 3) - 10, new List<Vector2> { posibleTargets[0].Position }).Count() == 1)
                            {
                                var hits = GetHits(input.From.To2D(), candidate, input.Radius, positionsList).ToList();
                                var hitsCount = hits.Count;

                                if (hitsCount >= bestCandidateHits)
                                {
                                    bestCandidateHits = hitsCount;
                                    bestCandidate = candidate;
                                    bestCandidateHitPoints = hits.ToList();
                                }
                            }
                        }

                        if (bestCandidateHits > 1)
                        {
                            float maxDistance = -1;
                            Vector2 p1 = default(Vector2), p2 = default(Vector2);

                            for (var i = 0; i < bestCandidateHitPoints.Count; i++)
                            {
                                for (var j = 0; j < bestCandidateHitPoints.Count; j++)
                                {
                                    var startP = input.From.To2D();
                                    var endP = bestCandidate;
                                    var proj1 = positionsList[i].ProjectOn(startP, endP);
                                    var proj2 = positionsList[j].ProjectOn(startP, endP);
                                    var dist = bestCandidateHitPoints[i].DistanceSquared(proj1.LinePoint) + bestCandidateHitPoints[j].DistanceSquared(proj2.LinePoint);

                                    if (dist >= maxDistance && (proj1.LinePoint - positionsList[i]).AngleBetween(proj2.LinePoint - positionsList[j]) > 90)
                                    {
                                        maxDistance = dist;
                                        p1 = positionsList[i];
                                        p2 = positionsList[j];
                                    }
                                }
                            }

                            return new PredictionOutput
                            {
                                Hitchance = mainTargetPrediction.Hitchance,
                                AoeHitCount = bestCandidateHits,
                                UnitPosition = mainTargetPrediction.UnitPosition,
                                CastPosition = ((p1 + p2) * 0.5f).To3D(),
                                Input = input
                            };
                        }
                    }

                    return mainTargetPrediction;
                }

                internal static Vector2[] GetCandidates(Vector2 from, Vector2 to, float radius, float range)
                {
                    var middlePoint = (from + to) / 2;
                    var intersections = from.CircleCircleIntersection(middlePoint, radius, from.Distance(middlePoint));

                    if (intersections.Length > 1)
                    {
                        var c1 = intersections[0];
                        var c2 = intersections[1];

                        c1 = from + range * (to - c1).Normalized();
                        c2 = from + range * (to - c2).Normalized();

                        return new[] { c1, c2 };
                    }

                    return new Vector2[] { };
                }

                internal static IEnumerable<Vector2> GetHits(Vector2 start, Vector2 end, double radius, List<Vector2> points)
                {
                    return points.Where(p => p.DistanceSquared(start, end, true) <= radius * radius);
                }
            }

            internal class PossibleTarget
            {
                public Vector2 Position { get; set; }

                public Obj_AI_Base Unit { get; set; }
            }
        }

        public class ConvexHull
        {
            public static RectangleF MinMaxBox { get; set; }

            public static Vector2[] MinMaxCorners { get; set; }

            public static Vector2[] NonCulledPoints { get; set; }

            public static void FindMinimalBoundingCircle(List<Vector2> points, out Vector2 center, out float radius)
            {
                var hull = MakeConvexHull(points);
                var bestCenter = points[0];
                var bestRadius2 = float.MaxValue;

                for (var i = 0; i < hull.Count - 1; i++)
                {
                    for (var j = i + 1; j < hull.Count; j++)
                    {
                        var testCenter = new Vector2((hull[i].X + hull[j].X) / 2f, (hull[i].Y + hull[j].Y) / 2f);
                        var dx = testCenter.X - hull[i].X;
                        var dy = testCenter.Y - hull[i].Y;
                        var testRadius2 = (dx * dx) + (dy * dy);

                        if (!(testRadius2 < bestRadius2))
                        {
                            continue;
                        }

                        if (!CircleEnclosesPoints(testCenter, testRadius2, points, i, j, -1))
                        {
                            continue;
                        }

                        bestCenter = testCenter;
                        bestRadius2 = testRadius2;
                    }
                }

                for (var i = 0; i < hull.Count - 2; i++)
                {
                    for (var j = i + 1; j < hull.Count - 1; j++)
                    {
                        for (var k = j + 1; k < hull.Count; k++)
                        {
                            Vector2 testCenter;

                            float testRadius2;

                            FindCircle(hull[i], hull[j], hull[k], out testCenter, out testRadius2);

                            if (!(testRadius2 < bestRadius2))
                            {
                                continue;
                            }

                            // See if this circle encloses all of the points.
                            if (!CircleEnclosesPoints(testCenter, testRadius2, points, i, j, k))
                            {
                                continue;
                            }

                            // Save this solution.
                            bestCenter = testCenter;
                            bestRadius2 = testRadius2;
                        }

                        // for k
                    }

                    // for i
                }

                // for j
                center = bestCenter;
                if (bestRadius2.Equals(float.MaxValue))
                {
                    radius = 0;
                }
                else
                {
                    radius = (float)Math.Sqrt(bestRadius2);
                }
            }

            public static MecCircle GetMec(List<Vector2> points)
            {
                Vector2 center;

                float radius;

                var convexHull = MakeConvexHull(points);

                FindMinimalBoundingCircle(convexHull, out center, out radius);

                return new MecCircle(center, radius);
            }

            public static List<Vector2> MakeConvexHull(List<Vector2> points)
            {
                points = HullCull(points);

                Vector2[] bestPt = { points[0] };
  
                foreach (var pt in points.Where(pt => (pt.Y < bestPt[0].Y) || ((Math.Abs(pt.Y - bestPt[0].Y) < float.Epsilon) && (pt.X < bestPt[0].X))))
                {
                    bestPt[0] = pt;
                }

                var hull = new List<Vector2>
                {
                    bestPt[0]
                };

                points.Remove(bestPt[0]);

                float sweepAngle = 0;

                for (;;)
                {
                    if (points.Count == 0)
                    {
                        break;
                    }

                    var x = hull[hull.Count - 1].X;
                    var y = hull[hull.Count - 1].Y;

                    bestPt[0] = points[0];

                    float bestAngle = 3600;

                    foreach (var pt in points)
                    {
                        var testAngle = AngleValue(x, y, pt.X, pt.Y);

                        if ((!(testAngle >= sweepAngle)) || (!(bestAngle > testAngle)))
                        {
                            continue;
                        }

                        bestAngle = testAngle;
                        bestPt[0] = pt;
                    }

                    var firstAngle = AngleValue(x, y, hull[0].X, hull[0].Y);

                    if ((firstAngle >= sweepAngle) && (bestAngle >= firstAngle))
                    {
                        break;
                    }

                    hull.Add(bestPt[0]);
                    points.Remove(bestPt[0]);

                    sweepAngle = bestAngle;
                }

                return hull;
            }

            private static float AngleValue(float x1, float y1, float x2, float y2)
            {
                float t;

                var dx = x2 - x1;
                var ax = Math.Abs(dx);
                var dy = y2 - y1;
                var ay = Math.Abs(dy);

                if ((ax + ay).Equals(0))
                {
                    t = 360f / 9f;
                }
                else
                {
                    t = dy / (ax + ay);
                }

                if (dx < 0)
                {
                    t = 2 - t;
                }
                else if (dy < 0)
                {
                    t = 4 + t;
                }

                return t * 90;
            }

            private static bool CircleEnclosesPoints(Vector2 center, float radius2, IEnumerable<Vector2> points, int skip1, int skip2, int skip3)
            {
                return (from point in points.Where((t, i) => (i != skip1) && (i != skip2) && (i != skip3))
                        let dx = center.X - point.X
                        let dy = center.Y - point.Y
                        select (dx * dx) + (dy * dy)).All(testRadius2 => !(testRadius2 > radius2));
            }

            private static void FindCircle(Vector2 a, Vector2 b, Vector2 c, out Vector2 center, out float radius2)
            {
                var x1 = (b.X + a.X) / 2;
                var y1 = (b.Y + a.Y) / 2;
                var dy1 = b.X - a.X;
                var dx1 = -(b.Y - a.Y);
                var x2 = (c.X + b.X) / 2;
                var y2 = (c.Y + b.Y) / 2;
                var dy2 = c.X - b.X;
                var dx2 = -(c.Y - b.Y);
                var cx = ((y1 * dx1 * dx2) + (x2 * dx1 * dy2) - (x1 * dy1 * dx2) - (y2 * dx1 * dx2)) / ((dx1 * dy2) - (dy1 * dx2));
                var cy = ((cx - x1) * dy1 / dx1) + y1;

                center = new Vector2(cx, cy);

                var dx = cx - a.X;
                var dy = cy - a.Y;

                radius2 = (dx * dx) + (dy * dy);
            }

            private static RectangleF GetMinMaxBox(IEnumerable<Vector2> points)
            {
                Vector2 ul = new Vector2(0, 0), ur = ul, ll = ul, lr = ul;

                var minMaxCornersInfo = GetMinMaxCorners(points, ul, ur, ll, lr);

                ul = minMaxCornersInfo.UpperLeft;
                ur = minMaxCornersInfo.UpperRight;
                ll = minMaxCornersInfo.LowerLeft;
                lr = minMaxCornersInfo.LowerRight;

                var xmin = ul.X;
                var ymin = ul.Y;
                var xmax = ur.X;

                if (ymin < ur.Y)
                {
                    ymin = ur.Y;
                }

                if (xmax > lr.X)
                {
                    xmax = lr.X;
                }

                var ymax = lr.Y;

                if (xmin < ll.X)
                {
                    xmin = ll.X;
                }

                if (ymax > ll.Y)
                {
                    ymax = ll.Y;
                }

                var result = new RectangleF(xmin, ymin, xmax - xmin, ymax - ymin);

                MinMaxBox = result;

                return result;
            }

            private static MinMaxCornersInfo GetMinMaxCorners(IEnumerable<Vector2> points, Vector2 upperLeft, Vector2 upperRight, Vector2 lowerLeft, Vector2 lowerRight)
            {
                foreach (var pt in points)
                {
                    if (-pt.X - pt.Y > -upperLeft.X - upperLeft.Y)
                    {
                        upperLeft = pt;
                    }

                    if (pt.X - pt.Y > upperRight.X - upperRight.Y)
                    {
                        upperRight = pt;
                    }

                    if (-pt.X + pt.Y > -lowerLeft.X + lowerLeft.Y)
                    {
                        lowerLeft = pt;
                    }

                    if (pt.X + pt.Y > lowerRight.X + lowerRight.Y)
                    {
                        lowerRight = pt;
                    }
                }

                MinMaxCorners = new[] { upperLeft, upperRight, lowerRight, lowerLeft };

                return new MinMaxCornersInfo(upperLeft, upperRight, lowerLeft, lowerRight);
            }

            private static List<Vector2> HullCull(IReadOnlyList<Vector2> points)
            {
                var cullingBox = GetMinMaxBox(points);

                return points.Where(pt => pt.X <= cullingBox.Left || pt.X >= cullingBox.Right || pt.Y <= cullingBox.Top || pt.Y >= cullingBox.Bottom).ToList();
            }

            public struct MecCircle
            {
                public Vector2 Center;
                public float Radius;

                internal MecCircle(Vector2 center, float radius)
                {
                    Center = center;
                    Radius = radius;
                }
            }

            public struct MinMaxCornersInfo
            {
                public Vector2 LowerLeft;
                public Vector2 LowerRight;
                public Vector2 UpperLeft;
                public Vector2 UpperRight;

                public MinMaxCornersInfo(Vector2 upperLeft, Vector2 upperRight, Vector2 lowerLeft, Vector2 lowerRight)
                {
                    UpperLeft = upperLeft;
                    UpperRight = upperRight;
                    LowerLeft = lowerLeft;
                    LowerRight = lowerRight;
                }
            }
        }

        internal static PredictionOutput GetStandardPrediction(PredictionInput input)
        {
            var speed = input.Unit.MoveSpeed;

            if (input.Unit.DistanceSquared(input.From) < 200 * 200)
            {
                speed /= 1.5f;
            }

            return GetPositionOnPath(input, input.Unit.GetWaypoints(), speed);
        }

        internal static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var result = unit.Buffs.Where(buff => buff.IsValid && (buff.Type == BuffType.Knockup || buff.Type == BuffType.Snare || buff.Type == BuffType.Stun || buff.Type == BuffType.Suppression)).Aggregate(0f, (current, buff) => Math.Max(buff.EndTime, current));

            return result - Game.Time;
        }

        private static HitChance GetHitChance(PredictionInput input)
        {
            var hero = input.Unit as AIHeroClient;

            if (hero == null || !hero.IsValid || input.Radius <= 1f)
            {
                return HitChance.VeryHigh;
            }

            if (hero.IsCastingInterruptableSpell(true) || hero.IsRecalling() || (UnitTracker.GetLastStopTick(hero) < 0.1d && hero.IsRooted))
            {
                return HitChance.VeryHigh;
            }

            var wayPoints = hero.GetWaypoints();
            var lastWaypoint = wayPoints.Last();
            var heroPos = hero.Position;
            var heroServerPos = hero.ServerPosition.To2D();
            var distHeroToWaypoint = heroServerPos.Distance(lastWaypoint);
            var distHeroToFrom = heroServerPos.Distance(input.From);
            var distFromToWaypoint = input.From.To2D().Distance(lastWaypoint);
            var angle = (lastWaypoint - heroPos.To2D()).AngleBetween(input.From.To2D() - heroPos.To2D());
            var delay = input.Delay + (Math.Abs(input.Speed - float.MaxValue) > float.Epsilon ? distHeroToFrom / input.Speed : 0);
            var moveArea = hero.MoveSpeed * delay;
            var fixRange = moveArea * 0.35f;
            var minPath = 1000;

            if (input.Type == SkillshotType.SkillshotCircle)
            {
                fixRange -= input.Radius / 2;
            }

            if (distFromToWaypoint <= distHeroToFrom && distHeroToFrom > input.Range - fixRange)
            {
                return HitChance.Medium;
            }

            if (distHeroToWaypoint > 0)
            {
                if (angle < 20 || angle > 160 || (angle > 130 && distHeroToWaypoint > 400))
                {
                    return HitChance.VeryHigh;
                }

                var wallPoints = new List<Vector2>();

                for (var i = 1; i <= 15; i++)
                {
                    var circleAngle = i * 2 * Math.PI / 15;
                    var point = new Vector2(heroPos.X + 350 * (float)Math.Cos(circleAngle), heroPos.Y + 350 * (float)Math.Sin(circleAngle));

                    if (point.IsWall())
                    {
                        wallPoints.Add(point);
                    }
                }

                if (wallPoints.Count > 2 && !wallPoints.Any(i => heroPos.To2D().Distance(i) > lastWaypoint.Distance(i)))
                {
                    return HitChance.VeryHigh;
                }

                if (GamePath.PathTracker.GetCurrentPath(hero).Time > 0.25d && input.Delay < 0.3f)
                {
                    return HitChance.VeryHigh;
                }
            }

            if (distHeroToWaypoint > 0 && distHeroToWaypoint < 100)
            {
                return HitChance.Medium;
            }

            if (wayPoints.Count == 1)
            {
                return hero.Spellbook.IsAutoAttacking || UnitTracker.GetLastStopTick(hero) < 0.8d ? HitChance.High : HitChance.VeryHigh;
            }

            if (UnitTracker.IsSpamSamePos(hero))
            {
                return HitChance.VeryHigh;
            }

            if (distHeroToFrom < 250 || hero.MoveSpeed < 250 || distFromToWaypoint < 250)
            {
                return HitChance.VeryHigh;
            }

            if (distHeroToWaypoint > minPath)
            {
                return HitChance.VeryHigh;
            }

            if (hero.HealthPercent < 20 || ObjectManager.Player.HealthPercent < 20)
            {
                return HitChance.VeryHigh;
            }

            if (input.Type == SkillshotType.SkillshotCircle && GamePath.PathTracker.GetCurrentPath(hero).Time < 0.1d && distHeroToWaypoint > fixRange)
            {
                return HitChance.VeryHigh;
            }

            return HitChance.Medium;
        }

        internal class UnitTracker
        {
            private static readonly Dictionary<int, UnitTrackerEntry> DictData = new Dictionary<int, UnitTrackerEntry>();

            static UnitTracker()
            {
                Obj_AI_Base.OnNewPath += OnNewPath;
            }

            internal static double GetLastStopTick(AIHeroClient hero)
            {
                UnitTrackerEntry data;
                return DictData.TryGetValue(hero.NetworkId, out data) ? (Utils.TickCount - data.StopTick) / 1000d : 1;
            }

            internal static bool IsSpamSamePos(AIHeroClient hero)
            {
                UnitTrackerEntry data;

                if (!DictData.TryGetValue(hero.NetworkId, out data))
                {
                    return false;
                }

                if (data.Path.Count < 3)
                {
                    return false;
                }

                if (data.Path[2].Tick - data.Path[1].Tick < 180 && Utils.TickCount - data.Path[2].Tick < 90)
                {
                    var posHero = hero.Position;
                    var posPath1 = data.Path[1].Position;
                    var posPath2 = data.Path[2].Position;
                    var a = Math.Pow(posPath2.X - posHero.X, 2) + Math.Pow(posPath2.Y - posHero.Y, 2);
                    var b = Math.Pow(posHero.X - posPath1.X, 2) + Math.Pow(posHero.Y - posPath1.Y, 2);
                    var c = Math.Pow(posPath2.X - posPath1.X, 2) + Math.Pow(posPath2.Y - posPath1.Y, 2);

                    return data.Path[1].Position.Distance(data.Path[2].Position) < 50 || Math.Cos((a + b - c) / (2 * Math.Sqrt(a) * Math.Sqrt(b))) * 180 / Math.PI < 31;
                }

                return false;
            }

            private static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
            {
                if (!(sender is AIHeroClient))
                {
                    return;
                }

                if (!DictData.ContainsKey(sender.NetworkId))
                {
                    DictData.Add(sender.NetworkId, new UnitTrackerEntry());
                }

                if (args.Path.Length == 1)
                {
                    DictData[sender.NetworkId].StopTick = Utils.TickCount;
                }

                DictData[sender.NetworkId].Path.Add(new StoredPath
                {
                    Position = args.Path.Last().To2D(),
                    Tick = Utils.TickCount
                });

                if (DictData[sender.NetworkId].Path.Count > 3)
                {
                    DictData[sender.NetworkId].Path.RemoveAt(0);
                }
            }

            private class StoredPath
            {
                internal Vector2 Position { get; set; }

                internal int Tick { get; set; }
            }

            private class UnitTrackerEntry
            {
                internal List<StoredPath> Path { get; } = new List<StoredPath>();

                internal int StopTick { get; set; }
            }
        }

        public class PredictionInput : ICloneable
        {
            private Vector3 from;
            private Vector3 rangeCheckFrom;

            public bool AoE { get; set; }

            public bool Collision { get; set; }

            public CollisionableObjects CollisionObjects { get; set; } = CollisionableObjects.Minions | CollisionableObjects.YasuoWall;

            public float Delay { get; set; }

            public Vector3 From
            {
                get
                {
                    return from.IsValid() ? from : ObjectManager.Player.ServerPosition;
                }
                set
                {
                    from = value;
                }
            }

            public float Radius { get; set; } = 1f;

            public float Range { get; set; } = float.MaxValue;

            public Vector3 RangeCheckFrom
            {
                get
                {
                    return rangeCheckFrom.IsValid() ? rangeCheckFrom : From;
                }
                set
                {
                    rangeCheckFrom = value;
                }
            }

            public float Speed { get; set; } = float.MaxValue;

            public SkillshotType Type { get; set; } = SkillshotType.SkillshotLine;

            public Obj_AI_Base Unit { get; set; } = ObjectManager.Player;

            public bool UseBoundingRadius { get; set; } = true;

            internal float RealRadius => UseBoundingRadius ? Radius + Unit.BoundingRadius : Radius;

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        public class PredictionOutput
        {
            private Vector3 castPosition;
            private Vector3 unitPosition;

            public int AoeHitCount { get; set; }

            public List<AIHeroClient> AoeTargetsHit { get; set; } = new List<AIHeroClient>();

            public int AoeTargetsHitCount => Math.Max(AoeHitCount, AoeTargetsHit.Count);

            public Vector3 CastPosition
            {
                get
                {
                    return castPosition.IsValid() ? castPosition.SetZ() : Input.Unit.ServerPosition;
                }
                set
                {
                    castPosition = value;
                }
            }

            public List<Obj_AI_Base> CollisionObjects { get; set; } = new List<Obj_AI_Base>();

            public HitChance Hitchance { get; set; } = HitChance.Impossible;

            public PredictionInput Input { get; set; }

            public Vector3 UnitPosition
            {
                get
                {
                    var pos = unitPosition.IsValid() ? unitPosition.SetZ() : Input.Unit.ServerPosition;

                    return pos;
                }
                set
                {
                    unitPosition = value;
                }
            }
        }

        public class GamePath
        {
            public static class PathTracker
            {
                private const double MaxTime = 1.5d;
                private static readonly Dictionary<int, List<StoredPath>> StoredPaths = new Dictionary<int, List<StoredPath>>();

                static PathTracker()
                {
                    Obj_AI_Base.OnNewPath += AIHeroClient_OnNewPath;
                }

                public static StoredPath GetCurrentPath(Obj_AI_Base unit)
                {
                    List<StoredPath> value;

                    return StoredPaths.TryGetValue(unit.NetworkId, out value) ? value.LastOrDefault() : new StoredPath();
                }

                public static double GetMeanSpeed(Obj_AI_Base unit, double maxT)
                {
                    var paths = GetStoredPaths(unit, MaxTime);
                    var distance = 0d;

                    if (paths.Count > 0)
                    {
                        distance += (maxT - paths[0].Time) * unit.MoveSpeed;

                        for (var i = 0; i < paths.Count - 1; i++)
                        {
                            var currentPath = paths[i];
                            var nextPath = paths[i + 1];

                            if (currentPath.WaypointCount > 0)
                            {
                                distance += Math.Min((currentPath.Time - nextPath.Time) * unit.MoveSpeed, currentPath.Path.PathLength());
                            }
                        }

                        var lastPath = paths.Last();

                        if (lastPath.WaypointCount > 0)
                        {
                            distance += Math.Min(lastPath.Time * unit.MoveSpeed, lastPath.Path.PathLength());
                        }
                    }
                    else
                    {
                        return unit.MoveSpeed;
                    }

                    return distance / maxT;
                }

                public static List<StoredPath> GetStoredPaths(Obj_AI_Base unit, double maxT)
                {
                    List<StoredPath> value;

                    return StoredPaths.TryGetValue(unit.NetworkId, out value) ? value.Where(p => p.Time < maxT).ToList() : new List<StoredPath>();
                }

                private static void AIHeroClient_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
                {
                    if (!(sender is AIHeroClient))
                    {
                        return;
                    }

                    if (!StoredPaths.ContainsKey(sender.NetworkId))
                    {
                        StoredPaths.Add(sender.NetworkId, new List<StoredPath>());
                    }

                    var newPath = new StoredPath { Tick = Utils.TickCount, Path = args.Path.ToList().To2D() };

                    StoredPaths[sender.NetworkId].Add(newPath);

                    if (StoredPaths[sender.NetworkId].Count > 50)
                    {
                        StoredPaths[sender.NetworkId].RemoveRange(0, 40);
                    }
                }
            }

            public class StoredPath
            {
                public Vector2 EndPoint => Path.LastOrDefault();

                public List<Vector2> Path { get; set; }

                public Vector2 StartPoint => Path.FirstOrDefault();

                public int Tick { get; set; }

                public double Time => (Utils.TickCount - Tick) / 1000d;

                public int WaypointCount => Path.Count;
            }
        }
    }
}