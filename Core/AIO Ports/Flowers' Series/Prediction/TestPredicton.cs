//using EloBuddy; 
//using LeagueSharp.Common; 
//namespace Flowers_ADC_Series.Prediction
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using LeagueSharp;
//    using LeagueSharp.Common;
//    using SharpDX;

//    public static class TestPredicton
//    {
//        public enum HitChance
//        {
//            Immobile = 8,
//            Dashing = 7,
//            VeryHigh = 6,
//            High = 5,
//            Medium = 4,
//            Low = 3,
//            Impossible = 2,
//            OutOfRange = 1,
//            Collision = 0
//        }

//        public enum SkillshotType
//        {
//            SkillshotLine,
//            SkillshotCircle,
//            SkillshotCone
//        }

//        public enum CollisionableObjects
//        {
//            Minions,
//            Heroes,
//            YasuoWall,
//            Walls
//        }

//        public class PredictionInput
//        {
//            public bool Aoe = false;
//            public bool Collision = false;
//            public float Delay;
//            public float Radius = 1f;
//            public float Range = float.MaxValue;
//            public float Speed = float.MaxValue;
//            public SkillshotType Type = SkillshotType.SkillshotLine;
//            public Obj_AI_Base Unit = ObjectManager.Player;
//            public bool UseBoundingRadius = true;
//            private Vector3 _from;
//            private Vector3 _rangeCheckFrom;

//            public CollisionableObjects[] CollisionObjects =
//            {
//                CollisionableObjects.Minions,
//                CollisionableObjects.YasuoWall
//            };

//            public Vector3 From
//            {
//                get { return _from.To2D().IsValid() ? _from : ObjectManager.Player.ServerPosition; }
//                set { _from = value; }
//            }

//            public Vector3 RangeCheckFrom
//            {
//                get
//                {
//                    return _rangeCheckFrom.To2D().IsValid()
//                        ? _rangeCheckFrom
//                        : (From.To2D().IsValid() ? From : ObjectManager.Player.ServerPosition);
//                }
//                set { _rangeCheckFrom = value; }
//            }

//            internal float RealRadius => UseBoundingRadius ? Radius + Unit.BoundingRadius : Radius;
//        }

//        public class PredictionOutput
//        {
//            private Vector3 _castPosition;
//            private Vector3 _unitPosition;

//            internal int _aoeTargetsHitCount;
//            internal PredictionInput Input;

//            public HitChance Hitchance = HitChance.Impossible;
//            public List<AIHeroClient> AoeTargetsHit = new List<AIHeroClient>();
//            public List<Obj_AI_Base> CollisionObjects = new List<Obj_AI_Base>();

//            public int AoeTargetsHitCount => Math.Max(_aoeTargetsHitCount, AoeTargetsHit.Count);

//            public Vector3 CastPosition
//            {
//                get
//                {
//                    return _castPosition.IsValid() && _castPosition.To2D().IsValid()
//                        ? _castPosition.SetZ()
//                        : Input.Unit.ServerPosition;
//                }
//                set { _castPosition = value; }
//            }

//            public Vector3 UnitPosition
//            {
//                get { return _unitPosition.To2D().IsValid() ? _unitPosition.SetZ() : Input.Unit.ServerPosition; }
//                set { _unitPosition = value; }
//            }
//        }

//        public static class Prediction
//        {
//            public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay)
//            {
//                return GetPrediction(new PredictionInput {Unit = unit, Delay = delay});
//            }

//            public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius)
//            {
//                return GetPrediction(new PredictionInput {Unit = unit, Delay = delay, Radius = radius});
//            }

//            public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius, float speed)
//            {
//                return GetPrediction(new PredictionInput {Unit = unit, Delay = delay, Radius = radius, Speed = speed});
//            }

//            public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius, float speed,
//                CollisionableObjects[] collisionable)
//            {
//                return
//                    GetPrediction(
//                        new PredictionInput
//                        {
//                            Unit = unit,
//                            Delay = delay,
//                            Radius = radius,
//                            Speed = speed,
//                            CollisionObjects = collisionable
//                        });
//            }

//            public static PredictionOutput GetPrediction(PredictionInput input, bool Arc = false)
//            {
//                return GetPrediction(input, true, true, Arc);
//            }

//            internal static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision, bool arcSpell)
//            {
//                PredictionOutput result = null;

//                if (!input.Unit.IsValidTarget(float.MaxValue, false))
//                {
//                    return new PredictionOutput();
//                }

//                if (ft)
//                {
//                    input.Delay += Game.Ping / 2000f + 0.06f;

//                    if (input.Aoe)
//                    {
//                        return AoePrediction.GetPrediction(input);
//                    }
//                }

//                if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon
//                    && input.Unit.Distance(input.RangeCheckFrom, true) > Math.Pow(input.Range * 1.5, 2))
//                {
//                    return new PredictionOutput { Input = input };
//                }

//                if (input.Unit.IsDashing())
//                {
//                    result = GetDashingPrediction(input);
//                }
//                else
//                {
//                    var remainingImmobileT = UnitIsImmobileUntil(input.Unit);

//                    if (remainingImmobileT >= 0d || IsCastingSpell(input.Unit))
//                    {
//                        result = GetImmobilePrediction(input, remainingImmobileT);
//                    }
//                }

//                if (result == null)
//                {
//                    result = GetStandardPrediction(input);
//                }

//                if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
//                {
//                    if (result.Hitchance >= HitChance.High
//                        && input.RangeCheckFrom.Distance(input.Unit.Position, true)
//                        > Math.Pow(input.Range + input.RealRadius * 3 / 4, 2))
//                    {
//                        result.Hitchance = HitChance.Medium;
//                    }

//                    if (input.RangeCheckFrom.Distance(result.UnitPosition, true)
//                        > Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.RealRadius : 0), 2))
//                    {
//                        result.Hitchance = HitChance.OutOfRange;
//                    }

//                    if (input.RangeCheckFrom.Distance(result.CastPosition, true) > Math.Pow(input.Range, 2))
//                    {
//                        if (result.Hitchance != HitChance.OutOfRange)
//                        {
//                            result.CastPosition = input.RangeCheckFrom
//                                                  + input.Range
//                                                  *
//                                                  (result.UnitPosition - input.RangeCheckFrom).To2D()
//                                                      .Normalized()
//                                                      .To3D();
//                        }
//                        else
//                        {
//                            result.Hitchance = HitChance.OutOfRange;
//                        }
//                    }
//                }

//                if (checkCollision && input.Collision)
//                {
//                    var positions = new List<Vector3> {result.UnitPosition, result.CastPosition, input.Unit.Position};
//                    var originalUnit = input.Unit;

//                    result.CollisionObjects = Collision.GetCollision(positions, input);
//                    result.CollisionObjects.RemoveAll(x => x.NetworkId == originalUnit.NetworkId);
//                    result.Hitchance = result.CollisionObjects.Count > 0 ? HitChance.Collision : result.Hitchance;
//                }

//                if (result.Hitchance >= HitChance.High)
//                {
//                    if (input.Unit is AIHeroClient)
//                    {
//                        var target = (AIHeroClient) input.Unit;

//                        if (IsCastingSpell(target) || !Common.Common.CanMove(target))
//                        {
//                            result.Hitchance = HitChance.VeryHigh;
//                            result.CastPosition = target.ServerPosition;
//                            return result;
//                        }
//                    }
//                    else
//                    {
//                        result.Hitchance = HitChance.VeryHigh;
//                        return result;
//                    }
//                }

//                return result;
//            }

//            internal static PredictionOutput GetDashingPrediction(PredictionInput input)
//            {
//                var dashData = input.Unit.GetDashInfo();
//                var result = new PredictionOutput { Input = input };

//                if (!dashData.IsBlink)
//                {
//                    var endP = dashData.Path.Last();
//                    var dashPred = GetPositionOnPath(input, new List<Vector2> {input.Unit.ServerPosition.To2D(), endP},
//                        dashData.Speed);

//                    if (dashPred.Hitchance >= HitChance.High
//                        && dashPred.UnitPosition.To2D().Distance(input.Unit.Position.To2D(), endP, true) < 200)
//                    {
//                        dashPred.CastPosition = dashPred.UnitPosition;
//                        dashPred.Hitchance = HitChance.Dashing;

//                        return dashPred;
//                    }

//                    if (dashData.Path.PathLength() > 200)
//                    {
//                        var timeToPoint = input.Delay / 2f + input.From.To2D().Distance(endP) / input.Speed - 0.25f;

//                        if (timeToPoint <=
//                            input.Unit.Distance(endP)/dashData.Speed + input.RealRadius/input.Unit.MoveSpeed)
//                        {
//                            return new PredictionOutput
//                            {
//                                CastPosition = endP.To3D(),
//                                UnitPosition = endP.To3D(),
//                                Hitchance = HitChance.Dashing
//                            };
//                        }
//                    }

//                    result.CastPosition = dashData.Path.Last().To3D();
//                    result.UnitPosition = result.CastPosition;
//                }

//                return result;
//            }

//            internal static PredictionOutput GetImmobilePrediction(PredictionInput input, double remainingImmobileT)
//            {
//                var timeToReachTargetPosition = input.Delay + input.Unit.Distance(input.From) / input.Speed;

//                if (timeToReachTargetPosition <= remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed)
//                {
//                    return new PredictionOutput
//                    {
//                        CastPosition = input.Unit.ServerPosition,
//                        UnitPosition = input.Unit.Position,
//                        Hitchance = HitChance.Immobile
//                    };
//                }

//                return new PredictionOutput
//                {
//                    Input = input,
//                    CastPosition = input.Unit.ServerPosition,
//                    UnitPosition = input.Unit.ServerPosition,
//                    Hitchance = HitChance.High
//                };
//            }

//            internal static PredictionOutput GetPositionOnPath(PredictionInput input, List<Vector2> path, float speed = -1)
//            {
//                speed = Math.Abs(speed - -1) < float.Epsilon ? input.Unit.MoveSpeed : speed;

//                if (path.Count <= 1 || (input.Unit.Spellbook.IsAutoAttacking && !input.Unit.IsDashing()))
//                {
//                    return new PredictionOutput
//                    {
//                        Input = input,
//                        UnitPosition = input.Unit.ServerPosition,
//                        CastPosition = input.Unit.ServerPosition,
//                        Hitchance = HitChance.VeryHigh
//                    };
//                }

//                var pLength = path.PathLength();
//                var tDistance = input.Delay * speed - input.RealRadius;

//                if (pLength >= tDistance && Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
//                {
//                    for (var i = 0; i < path.Count - 1; i++)
//                    {
//                        var a = path[i];
//                        var b = path[i + 1];
//                        var d = a.Distance(b);

//                        if (d >= tDistance)
//                        {
//                            var direction = (b - a).Normalized();
//                            var cp = a + direction * tDistance;
//                            var p = a
//                                    + direction
//                                    *(i == path.Count - 2
//                                        ? Math.Min(tDistance + input.RealRadius, d)
//                                        : tDistance + input.RealRadius);

//                            return new PredictionOutput
//                            {
//                                Input = input,
//                                CastPosition = cp.To3D(),
//                                UnitPosition = p.To3D(),
//                                Hitchance =
//                                    PathTracker.GetCurrentPath(input.Unit).Time < 0.1d
//                                        ? HitChance.VeryHigh
//                                        : HitChance.High
//                            };
//                        }

//                        tDistance -= d;
//                    }
//                }

//                if (pLength >= tDistance && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
//                {
//                    var d = tDistance;

//                    if (input.Type == SkillshotType.SkillshotLine || input.Type == SkillshotType.SkillshotCone)
//                    {
//                        if (input.From.Distance(input.Unit.ServerPosition, true) < 200 * 200)
//                        {
//                            d = input.Delay * speed;
//                        }
//                    }

//                    path = path.CutPath(d);

//                    var tT = 0f;

//                    for (var i = 0; i < path.Count - 1; i++)
//                    {
//                        var a = path[i];
//                        var b = path[i + 1];
//                        var tB = a.Distance(b) / speed;
//                        var direction = (b - a).Normalized();

//                        a = a - speed * tT * direction;

//                        var sol = Geometry.VectorMovementCollision(a, b, speed, input.From.To2D(), input.Speed, tT);
//                        var t = (float)sol[0];
//                        var pos = (Vector2)sol[1];

//                        if (pos.IsValid() && t >= tT && t <= tT + tB)
//                        {
//                            if (pos.Distance(b, true) < 20)
//                            {
//                                break;
//                            }

//                            var p = pos + input.RealRadius * direction;

//                            if (input.Type == SkillshotType.SkillshotLine)
//                            {
//                                var alpha = (input.From.To2D() - p).AngleBetween(a - b);

//                                if (alpha > 30 && alpha < 180 - 30)
//                                {
//                                    var beta = (float) Math.Asin(input.RealRadius/p.Distance(input.From));
//                                    var cp1 = input.From.To2D() + (p - input.From.To2D()).Rotated(beta);
//                                    var cp2 = input.From.To2D() + (p - input.From.To2D()).Rotated(-beta);

//                                    pos = cp1.Distance(pos, true) < cp2.Distance(pos, true) ? cp1 : cp2;
//                                }
//                            }

//                            return new PredictionOutput
//                            {
//                                Input = input,
//                                CastPosition = pos.To3D(),
//                                UnitPosition = p.To3D(),
//                                Hitchance =
//                                               PathTracker.GetCurrentPath(input.Unit).Time < 0.1d
//                                                   ? HitChance.VeryHigh
//                                                   : HitChance.High
//                            };
//                        }
//                        tT += tB;
//                    }
//                }

//                var position = path.Last();

//                return new PredictionOutput
//                {
//                    Input = input,
//                    CastPosition = position.To3D(),
//                    UnitPosition = position.To3D(),
//                    Hitchance = HitChance.Medium
//                };
//            }

//            internal static PredictionOutput GetStandardPrediction(PredictionInput input)
//            {
//                var speed = input.Unit.MoveSpeed;

//                if (input.Unit.Distance(input.From, true) < 200 * 200)
//                {
//                    speed /= 1.5f;
//                }

//                return GetPositionOnPath(input, input.Unit.GetWaypoints(), speed);
//            }

//            internal static bool IsCastingSpell(Obj_AI_Base sender)
//            {
//                var target = sender as AIHeroClient;

//                return target != null && target.IsChannelingImportantSpell();
//            }

//            internal static double UnitIsImmobileUntil(Obj_AI_Base unit)
//            {
//                var result =
//                    unit.Buffs.Where(
//                            buff =>
//                                buff.IsActive && Game.Time <= buff.EndTime
//                                &&
//                                (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup ||
//                                 buff.Type == BuffType.Stun
//                                 || buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
//                        .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));

//                return result - Game.Time;
//            }
//        }

//        internal static class AoePrediction
//        {
//            public static PredictionOutput GetPrediction(PredictionInput input)
//            {
//                switch (input.Type)
//                {
//                    case SkillshotType.SkillshotCircle:
//                        return Circle.GetPrediction(input);
//                    case SkillshotType.SkillshotCone:
//                        return Cone.GetPrediction(input);
//                    case SkillshotType.SkillshotLine:
//                        return Line.GetPrediction(input);
//                }

//                return new PredictionOutput();
//            }

//            internal static List<PossibleTarget> GetPossibleTargets(PredictionInput input)
//            {
//                var result = new List<PossibleTarget>();
//                var originalUnit = input.Unit;

//                foreach (var enemy in
//                    HeroManager.Enemies.FindAll(
//                        h =>
//                            h.NetworkId != originalUnit.NetworkId
//                            && h.IsValidTarget(input.Range + 200 + input.RealRadius, true, input.RangeCheckFrom)))
//                {
//                    input.Unit = enemy;

//                    var prediction = Prediction.GetPrediction(input, false, false, false);

//                    if (prediction.Hitchance >= HitChance.High)
//                    {
//                        result.Add(new PossibleTarget {Position = prediction.UnitPosition.To2D(), Unit = enemy});
//                    }
//                }
//                return result;
//            }

//            public static class Circle
//            {
//                public static PredictionOutput GetPrediction(PredictionInput input)
//                {
//                    var mainTargetPrediction = Prediction.GetPrediction(input, false, true, false);
//                    var posibleTargets = new List<PossibleTarget>
//                    {
//                        new PossibleTarget
//                        {
//                            Position = mainTargetPrediction.UnitPosition.To2D(),
//                            Unit = input.Unit
//                        }
//                    };

//                    if (mainTargetPrediction.Hitchance >= HitChance.Medium)
//                    {
//                        posibleTargets.AddRange(GetPossibleTargets(input));
//                    }

//                    while (posibleTargets.Count > 1)
//                    {
//                        var mecCircle = MEC.GetMec(posibleTargets.Select(h => h.Position).ToList());

//                        if (mecCircle.Radius <= input.RealRadius - 10
//                            && Vector2.DistanceSquared(mecCircle.Center, input.RangeCheckFrom.To2D())
//                            < input.Range * input.Range)
//                        {
//                            return new PredictionOutput
//                            {
//                                AoeTargetsHit = posibleTargets.Select(h => (AIHeroClient)h.Unit).ToList(),
//                                CastPosition = mecCircle.Center.To3D(),
//                                UnitPosition = mainTargetPrediction.UnitPosition,
//                                Hitchance = mainTargetPrediction.Hitchance,
//                                Input = input,
//                                _aoeTargetsHitCount = posibleTargets.Count
//                            };
//                        }

//                        var maxdist = -1f;
//                        var maxdistindex = 1;

//                        for (var i = 1; i < posibleTargets.Count; i++)
//                        {
//                            var distance = Vector2.DistanceSquared(posibleTargets[i].Position, posibleTargets[0].Position);

//                            if (distance > maxdist || maxdist.CompareTo(-1) == 0)
//                            {
//                                maxdistindex = i;
//                                maxdist = distance;
//                            }
//                        }

//                        posibleTargets.RemoveAt(maxdistindex);
//                    }

//                    return mainTargetPrediction;
//                }
//            }

//            public static class Cone
//            {
//                public static PredictionOutput GetPrediction(PredictionInput input)
//                {
//                    var mainTargetPrediction = Prediction.GetPrediction(input, false, true, false);
//                    var posibleTargets = new List<PossibleTarget>
//                    {
//                        new PossibleTarget
//                        {
//                            Position = mainTargetPrediction.UnitPosition.To2D(),
//                            Unit = input.Unit
//                        }
//                    };

//                    if (mainTargetPrediction.Hitchance >= HitChance.Medium)
//                    {
//                        posibleTargets.AddRange(GetPossibleTargets(input));
//                    }

//                    if (posibleTargets.Count > 1)
//                    {
//                        var candidates = new List<Vector2>();

//                        foreach (var target in posibleTargets)
//                        {
//                            target.Position = target.Position - input.From.To2D();
//                        }

//                        for (var i = 0; i < posibleTargets.Count; i++)
//                        {
//                            for (var j = 0; j < posibleTargets.Count; j++)
//                            {
//                                if (i != j)
//                                {
//                                    var p = (posibleTargets[i].Position + posibleTargets[j].Position) * 0.5f;

//                                    if (!candidates.Contains(p))
//                                    {
//                                        candidates.Add(p);
//                                    }
//                                }
//                            }
//                        }

//                        var bestCandidateHits = -1;
//                        var bestCandidate = new Vector2();
//                        var positionsList = posibleTargets.Select(t => t.Position).ToList();

//                        foreach (var candidate in candidates)
//                        {
//                            var hits = GetHits(candidate, input.Range, input.Radius, positionsList);

//                            if (hits > bestCandidateHits)
//                            {
//                                bestCandidate = candidate;
//                                bestCandidateHits = hits;
//                            }
//                        }

//                        if (bestCandidateHits > 1 && input.From.To2D().Distance(bestCandidate, true) > 50 * 50)
//                        {
//                            return new PredictionOutput
//                            {
//                                Hitchance = mainTargetPrediction.Hitchance,
//                                _aoeTargetsHitCount = bestCandidateHits,
//                                UnitPosition = mainTargetPrediction.UnitPosition,
//                                CastPosition = bestCandidate.To3D(),
//                                Input = input
//                            };
//                        }
//                    }
//                    return mainTargetPrediction;
//                }

//                internal static int GetHits(Vector2 end, double range, float angle, List<Vector2> points)
//                {
//                    return (from point in points
//                            let edge1 = end.Rotated(-angle / 2)
//                            let edge2 = edge1.Rotated(angle)
//                            where
//                            point.Distance(new Vector2(), true) < range * range && edge1.CrossProduct(point) > 0
//                            && point.CrossProduct(edge2) > 0
//                            select point).Count();
//                }
//            }

//            public static class Line
//            {
//                public static PredictionOutput GetPrediction(PredictionInput input)
//                {
//                    var mainTargetPrediction = Prediction.GetPrediction(input, false, true, false);
//                    var posibleTargets = new List<PossibleTarget>
//                    {
//                        new PossibleTarget
//                        {
//                            Position = mainTargetPrediction.UnitPosition.To2D(),
//                            Unit = input.Unit
//                        }
//                    };

//                    if (mainTargetPrediction.Hitchance >= HitChance.Medium)
//                    {
//                        posibleTargets.AddRange(GetPossibleTargets(input));
//                    }

//                    if (posibleTargets.Count > 1)
//                    {
//                        var candidates = new List<Vector2>();

//                        foreach (var target in posibleTargets)
//                        {
//                            var targetCandidates = GetCandidates(input.From.To2D(), target.Position, input.Radius,
//                                input.Range);

//                            candidates.AddRange(targetCandidates);
//                        }

//                        var bestCandidateHits = -1;
//                        var bestCandidate = new Vector2();
//                        var bestCandidateHitPoints = new List<Vector2>();
//                        var positionsList = posibleTargets.Select(t => t.Position).ToList();

//                        foreach (var candidate in candidates)
//                        {
//                            if (
//                                GetHits(input.From.To2D(), candidate, input.Radius + input.Unit.BoundingRadius / 3 - 10,
//                                    new List<Vector2> { posibleTargets[0].Position }).Count() == 1)
//                            {
//                                var hits = GetHits(input.From.To2D(), candidate, input.Radius, positionsList).ToList();
//                                var hitsCount = hits.Count;

//                                if (hitsCount >= bestCandidateHits)
//                                {
//                                    bestCandidateHits = hitsCount;
//                                    bestCandidate = candidate;
//                                    bestCandidateHitPoints = hits.ToList();
//                                }
//                            }
//                        }

//                        if (bestCandidateHits > 1)
//                        {
//                            float maxDistance = -1;

//                            Vector2 p1 = new Vector2(), p2 = new Vector2();

//                            for (var i = 0; i < bestCandidateHitPoints.Count; i++)
//                            {
//                                for (var j = 0; j < bestCandidateHitPoints.Count; j++)
//                                {
//                                    var startP = input.From.To2D();
//                                    var endP = bestCandidate;
//                                    var proj1 = positionsList[i].ProjectOn(startP, endP);
//                                    var proj2 = positionsList[j].ProjectOn(startP, endP);
//                                    var dist = Vector2.DistanceSquared(bestCandidateHitPoints[i], proj1.LinePoint)
//                                               + Vector2.DistanceSquared(bestCandidateHitPoints[j], proj2.LinePoint);

//                                    if (dist >= maxDistance &&
//                                        (proj1.LinePoint - positionsList[i]).AngleBetween(proj2.LinePoint - positionsList[j]) >
//                                        90)
//                                    {
//                                        maxDistance = dist;
//                                        p1 = positionsList[i];
//                                        p2 = positionsList[j];
//                                    }
//                                }
//                            }

//                            return new PredictionOutput
//                            {
//                                Hitchance = mainTargetPrediction.Hitchance,
//                                _aoeTargetsHitCount = bestCandidateHits,
//                                UnitPosition = mainTargetPrediction.UnitPosition,
//                                CastPosition = ((p1 + p2) * 0.5f).To3D(),
//                                Input = input
//                            };
//                        }
//                    }

//                    return mainTargetPrediction;
//                }

//                internal static Vector2[] GetCandidates(Vector2 from, Vector2 to, float radius, float range)
//                {
//                    var middlePoint = (from + to) / 2;
//                    var intersections = Geometry.CircleCircleIntersection(from, middlePoint, radius,
//                        from.Distance(middlePoint));

//                    if (intersections.Length > 1)
//                    {
//                        var c1 = intersections[0];
//                        var c2 = intersections[1];

//                        c1 = from + range * (to - c1).Normalized();
//                        c2 = from + range * (to - c2).Normalized();

//                        return new[] { c1, c2 };
//                    }

//                    return new Vector2[] { };
//                }

//                internal static IEnumerable<Vector2> GetHits(Vector2 start, Vector2 end, double radius, List<Vector2> points)
//                {
//                    return points.Where(p => p.Distance(start, end, true, true) <= radius * radius);
//                }
//            }

//            internal class PossibleTarget
//            {
//                public Vector2 Position;
//                public Obj_AI_Base Unit;
//            }
//        }

//        public static class Collision
//        {
//            private static int _wallCastT;
//            private static int _wallLevel;
//            private static readonly bool _yasuo;
//            private static Vector3 _yasuoWallPos;
//            private static Vector3 _yasuoWallCastedPos;

//            static Collision()
//            {
//                if (HeroManager.Enemies.Any(x => x.IsValidTarget() && x.ChampionName == "Yasuo"))
//                {
//                    _yasuo = true;
//                }

//                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
//            }

//            public static List<Obj_AI_Base> GetCollision(List<Vector3> positions, PredictionInput input)
//            {
//                var result = new List<Obj_AI_Base>();

//                foreach (var position in positions)
//                {
//                    foreach (var objectType in input.CollisionObjects)
//                    {
//                        switch (objectType)
//                        {
//                            case CollisionableObjects.Minions:
//                                {
//                                    foreach (var minion in
//                                        ObjectManager.Get<Obj_AI_Minion>()
//                                            .Where(
//                                                minion =>
//                                                    minion.IsValidTarget(
//                                                        Math.Min(input.Range + input.Radius + 100, 2000),
//                                                        true,
//                                                        input.RangeCheckFrom)))
//                                    {
//                                        input.Unit = minion;

//                                        var minionPrediction = Prediction.GetPrediction(input, false, false, false);

//                                        if (minionPrediction.UnitPosition.To2D()
//                                                .Distance(input.From.To2D(), position.To2D(), true, true)
//                                            <= Math.Pow((input.Radius + 15 + minion.BoundingRadius), 2))
//                                        {
//                                            result.Add(minion);
//                                        }
//                                    }
//                                    break;
//                                }
//                            case CollisionableObjects.Heroes:
//                                {
//                                    foreach (var hero in
//                                        HeroManager.Enemies.FindAll(
//                                            hero =>
//                                                hero.IsValidTarget(
//                                                    Math.Min(input.Range + input.Radius + 100, 2000),
//                                                    true,
//                                                    input.RangeCheckFrom)))
//                                    {
//                                        input.Unit = hero;

//                                        var prediction = Prediction.GetPrediction(input, false, false, false);

//                                        if (prediction.UnitPosition.To2D()
//                                                .Distance(input.From.To2D(), position.To2D(), true, true)
//                                            <= Math.Pow((input.Radius + 50 + hero.BoundingRadius), 2))
//                                        {
//                                            result.Add(hero);
//                                        }
//                                    }
//                                }
//                                break;
//                            case CollisionableObjects.Walls:
//                                {
//                                    var step = position.Distance(input.From)/20;

//                                    for (var i = 0; i < 20; i++)
//                                    {
//                                        var p = input.From.To2D().Extend(position.To2D(), step*i);

//                                        if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(CollisionFlags.Wall))
//                                        {
//                                            result.Add(ObjectManager.Player);
//                                        }
//                                    }
//                                }
//                                break;
//                            case CollisionableObjects.YasuoWall:
//                                {
//                                    if (Utils.TickCount - _wallCastT > 4000)
//                                    {
//                                        break;
//                                    }

//                                    var wallWidth = 300 + 50 * _wallLevel;
//                                    var wallDirection =
//                                        (_yasuoWallCastedPos.To2D() - _yasuoWallPos.To2D()).Normalized().Perpendicular();
//                                    var wallStart = _yasuoWallCastedPos.To2D() + wallWidth / 2f * wallDirection;
//                                    var wallEnd = wallStart - wallWidth * wallDirection;

//                                    if (wallStart.Intersection(wallEnd, position.To2D(), input.From.To2D()).Intersects)
//                                    {
//                                        var t = Utils.TickCount
//                                                + (wallStart.Intersection(wallEnd, position.To2D(), input.From.To2D())
//                                                       .Point.Distance(input.From) / input.Speed + input.Delay) * 1000;

//                                        if (t < _wallCastT + 4000)
//                                        {
//                                            result.Add(ObjectManager.Player);
//                                        }
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                }

//                return result.Distinct().ToList();
//            }

//            private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
//            {
//                if (Args.SData != null && _yasuo && sender.IsValid && sender.Team != ObjectManager.Player.Team &&
//                    Args.SData.Name == "YasuoWMovingWall")
//                {
//                    _wallCastT = Utils.TickCount;
//                    _wallLevel = sender.Spellbook.Spells[1].Level;
//                    _yasuoWallPos = sender.Position;
//                    _yasuoWallCastedPos = sender.Position.Extend(Args.End, 400);
//                }
//            }
//        }

//        internal class StoredPath
//        {
//            public List<Vector2> Path;
//            public int Tick;

//            public Vector2 EndPoint => Path.LastOrDefault();

//            public Vector2 StartPoint => Path.FirstOrDefault();

//            public double Time => (Utils.TickCount - Tick) / 1000d;

//            public int WaypointCount => Path.Count;
//        }

//        internal static class PathTracker
//        {
//            private const double MaxTime = 1.5d;
//            private static readonly Dictionary<int, List<StoredPath>> StoredPaths = new Dictionary<int, List<StoredPath>>();

//            static PathTracker()
//            {
//                Obj_AI_Base.OnNewPath += AIHeroClient_OnNewPath;
//            }

//            public static StoredPath GetCurrentPath(Obj_AI_Base unit)
//            {
//                return StoredPaths.ContainsKey(unit.NetworkId)
//                    ? StoredPaths[unit.NetworkId].LastOrDefault()
//                    : new StoredPath();
//            }

//            public static double GetMeanSpeed(Obj_AI_Base unit, double maxT)
//            {
//                var paths = GetStoredPaths(unit, MaxTime);
//                var distance = 0d;

//                if (paths.Count > 0)
//                {
//                    distance += (maxT - paths[0].Time) * unit.MoveSpeed;

//                    for (var i = 0; i < paths.Count - 1; i++)
//                    {
//                        var currentPath = paths[i];
//                        var nextPath = paths[i + 1];

//                        if (currentPath.WaypointCount > 0)
//                        {
//                            distance += Math.Min(
//                                (currentPath.Time - nextPath.Time) * unit.MoveSpeed,
//                                currentPath.Path.PathLength());
//                        }
//                    }

//                    var lastPath = paths.Last();

//                    if (lastPath.WaypointCount > 0)
//                    {
//                        distance += Math.Min(lastPath.Time * unit.MoveSpeed, lastPath.Path.PathLength());
//                    }
//                }
//                else
//                {
//                    return unit.MoveSpeed;
//                }

//                return distance / maxT;
//            }

//            public static List<StoredPath> GetStoredPaths(Obj_AI_Base unit, double maxT)
//            {
//                return StoredPaths.ContainsKey(unit.NetworkId)
//                    ? StoredPaths[unit.NetworkId].Where(p => p.Time < maxT).ToList()
//                    : new List<StoredPath>();
//            }

//            public static Vector3 GetTendency(Obj_AI_Base unit)
//            {
//                var paths = GetStoredPaths(unit, MaxTime);
//                var result = new Vector2();

//                foreach (var path in paths)
//                {
//                    result = result + (path.EndPoint - unit.ServerPosition.To2D()).Normalized();
//                }

//                result /= paths.Count;

//                return result.To3D();
//            }

//            private static void AIHeroClient_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
//            {
//                if (!(sender is AIHeroClient))
//                {
//                    return;
//                }

//                if (!StoredPaths.ContainsKey(sender.NetworkId))
//                {
//                    StoredPaths.Add(sender.NetworkId, new List<StoredPath>());
//                }

//                var newPath = new StoredPath { Tick = Utils.TickCount, Path = args.Path.ToList().To2D() };

//                StoredPaths[sender.NetworkId].Add(newPath);

//                if (StoredPaths[sender.NetworkId].Count > 50)
//                {
//                    StoredPaths[sender.NetworkId].RemoveRange(0, 40);
//                }
//            }
//        }
//    }
//}
