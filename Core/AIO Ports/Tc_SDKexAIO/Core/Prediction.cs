// <copyright file="Movement.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;
    using LeagueSharp.SDK.Polygons;

    using SharpDX;

    internal static class Prediction
    {

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay });
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius });
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius, float speed)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius, Speed = speed });
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius, float speed, CollisionableObjects collisionable)
        {
            return GetPrediction(new PredictionInput
            {
                Unit = unit,
                Delay = delay,
                Radius = radius,
                Speed = speed,
                CollisionObjects = collisionable
            });
        }
        
        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return GetPrediction(input, true, true);
        }

        internal static PredictionOutput GetDashingPrediction(PredictionInput input)
        {
            var dashData = input.Unit.GetDashInfo();
            var result = new PredictionOutput { Input = input, Hitchance = HitChance.High };

            // Normal dashes.
            if (!dashData.IsBlink)
            {
                var endP = dashData.EndPos.ToVector3();

                // Mid air:
                var dashPred = GetPositionOnPath(
                    input,
                    new List<Vector3> { input.Unit.ServerPosition, endP }.ToVector2(),
                    dashData.Speed);

                if (dashPred.Hitchance >= HitChance.High
                    && dashPred.UnitPosition.ToVector2()
                           .Distance(input.Unit.Position.ToVector2(), endP.ToVector2(), true) < 200)
                {
                    dashPred.CastPosition = dashPred.UnitPosition;
                    dashPred.Hitchance = HitChance.Dashing;
                    return dashPred;
                }

                // At the end of the dash:
                if (dashData.Path.PathLength() > 200)
                {
                    var timeToPoint = input.Delay / 2f
                                      + (Math.Abs(input.Speed - float.MaxValue) > float.Epsilon
                                             ? input.From.Distance(endP) / input.Speed
                                             : 0) - 0.25f;

                    if (timeToPoint
                        <= input.Unit.Distance(endP) / dashData.Speed + input.RealRadius / input.Unit.MoveSpeed)
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

                // Figure out where the unit is going.
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
            var timeToReachTargetPosition = input.Delay
                                            + (Math.Abs(input.Speed - float.MaxValue) > float.Epsilon
                                                   ? input.Unit.Distance(input.From) / input.Speed
                                                   : 0);

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

            // Skillshots with only a delay
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
                        var p = a
                                + direction
                                * ((i == path.Count - 2)
                                       ? Math.Min(tDistance + input.RealRadius, d)
                                       : (tDistance + input.RealRadius));

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = cp.ToVector3(),
                            UnitPosition = p.ToVector3(),
                            Hitchance = HitChance.High
                        };
                    }

                    tDistance -= d;
                }
            }

            // Skillshot with a delay and speed.
            if (pLength >= dist && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                var tDistance = dist;

                if ((input.Type == SkillshotType.SkillshotLine || input.Type == SkillshotType.SkillshotCone)
                    && input.Unit.DistanceSquared(input.From) < 200 * 200)
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
                    var sol = a.VectorMovementCollision(b, speed, input.From.ToVector2(), input.Speed, tT);
                    var t = (float)sol[0];
                    var pos = (Vector2)sol[1];

                    if (pos.IsValid() && t >= tT && t <= tT + tB)
                    {
                        if (pos.DistanceSquared(b) < 20)
                        {
                            break;
                        }

                        var p = pos + input.RealRadius * direction;

                        if (input.Type == SkillshotType.SkillshotLine)
                        {
                            var alpha = (input.From.ToVector2() - p).AngleBetween(a - b);

                            if (alpha > 30 && alpha < 180 - 30)
                            {
                                var beta = (float)Math.Asin(input.RealRadius / p.Distance(input.From));
                                var cp1 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(beta);
                                var cp2 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(-beta);

                                pos = cp1.DistanceSquared(pos) < cp2.DistanceSquared(pos) ? cp1 : cp2;
                            }
                        }

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = pos.ToVector3(),
                            UnitPosition = p.ToVector3(),
                            Hitchance = HitChance.High
                        };
                    }
                    tT += tB;
                }
            }

            var position = path.Last().ToVector3();
            return new PredictionOutput
            { Input = input, CastPosition = position, UnitPosition = position, Hitchance = HitChance.High };
        }

        internal static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision)
        {

            if (!input.Unit.IsValidTarget(float.MaxValue, false))
            {
                return new PredictionOutput();
            }

            if (ft)
            {
                // Increase the delay due to the latency and server tick:
                input.Delay += Game.Ping / 2000f + 0.06f;

                if (input.AoE)
                {
                    return AoePrediction.GetAoEPrediction(input);
                }
            }

            // Target too far away.
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon
                && input.Unit.DistanceSquared(input.RangeCheckFrom) > Math.Pow(input.Range * 1.5, 2))
            {
                return new PredictionOutput { Input = input };
            }

            PredictionOutput result = null;

            // Unit is dashing.
            if (input.Unit.IsDashing())
            {
                result = GetDashingPrediction(input);
            }
            else
            {
                // Unit is immobile.
                var remainingImmobileT = UnitIsImmobileUntil(input.Unit);

                if (remainingImmobileT >= 0d)
                {
                    result = GetImmobilePrediction(input, remainingImmobileT);
                }
                else
                {
                    input.Range = input.Range * 100f;
                }
            }

            // Normal prediction
            if (result == null)
            {
                result = GetStandardPrediction(input);
            }

            // Check if the unit position is in range
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance >= HitChance.High
                    && input.RangeCheckFrom.DistanceSquared(input.Unit.Position)
                    > Math.Pow(input.Range + input.RealRadius * 3 / 4, 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.UnitPosition)
                    > Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.RealRadius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.CastPosition) > Math.Pow(input.Range, 2)
                    && result.Hitchance != HitChance.OutOfRange)
                {
                    result.CastPosition = input.RangeCheckFrom
                                          + input.Range
                                          * (result.UnitPosition - input.RangeCheckFrom).ToVector2()
                                                .Normalized()
                                                .ToVector3();
                }
            }

            // Check for collision
            if (checkCollision && input.Collision && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                var positions = new List<Vector3> { result.UnitPosition, input.Unit.Position };
                var originalUnit = input.Unit;
                result.CollisionObjects = Collision.GetCollision(positions, input);
                result.CollisionObjects.RemoveAll(x => x.Compare(input.Unit));
                result.Hitchance = result.CollisionObjects.Count > 0 ? HitChance.Collision : result.Hitchance;
            }

            return result;
        }

        internal static PredictionOutput GetStandardPrediction(PredictionInput input)
        {
            var speed = input.Unit.MoveSpeed;

            if (input.Unit.DistanceSquared(input.From) < 200 * 200)
            {
                // input.Delay /= 2;
                speed /= 1.5f;
            }
            var result = GetPositionOnPath(input, input.Unit.GetWaypoints(), speed);
            if (result.Hitchance >= HitChance.High && input.Unit is AIHeroClient) { }

            return result;
        }

        internal static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        private static HitChance GetHitChance(PredictionInput input)
        {
            var hero = input.Unit as AIHeroClient;

            if (hero == null || !hero.IsValid || input.Radius <= 1f)
            {
                return HitChance.VeryHigh;
            }

            if (hero.IsCastingInterruptableSpell(true) || hero.IsRecalling()
                || (UnitTracker.GetLastStopTick(hero) < 0.1d && hero.IsRooted))
            {
                return HitChance.VeryHigh;
            }

            var wayPoints = hero.GetWaypoints();
            var lastWaypoint = wayPoints.Last();
            var heroPos = hero.Position;
            var heroServerPos = hero.ServerPosition.ToVector2();
            var distHeroToWaypoint = heroServerPos.Distance(lastWaypoint);
            var distHeroToFrom = heroServerPos.Distance(input.From);
            var distFromToWaypoint = input.From.Distance(lastWaypoint);
            var angle = (lastWaypoint - heroPos.ToVector2()).AngleBetween(input.From - heroPos);
            var delay = input.Delay
                        + (Math.Abs(input.Speed - float.MaxValue) > float.Epsilon ? distHeroToFrom / input.Speed : 0);
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
                    var point = new Vector2(
                        heroPos.X + 350 * (float)Math.Cos(circleAngle),
                        heroPos.Y + 350 * (float)Math.Sin(circleAngle));

                    if (point.IsWall())
                    {
                        wallPoints.Add(point);
                    }
                }

                if (wallPoints.Count > 2 && !wallPoints.Any(i => heroPos.Distance(i) > lastWaypoint.Distance(i)))
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
                return hero.Spellbook.IsAutoAttacking || UnitTracker.GetLastStopTick(hero) < 0.8d
                           ? HitChance.High
                           : HitChance.VeryHigh;
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

            if (hero.HealthPercent < 20 || GameObjects.Player.HealthPercent < 20)
            {
                return HitChance.VeryHigh;
            }

            if (input.Type == SkillshotType.SkillshotCircle && GamePath.PathTracker.GetCurrentPath(hero).Time < 0.1d
                && distHeroToWaypoint > fixRange)
            {
                return HitChance.VeryHigh;
            }

            return HitChance.Medium;
        }

    }

    internal static class UnitTracker
    {

        private static readonly Dictionary<int, UnitTrackerEntry> DictData = new Dictionary<int, UnitTrackerEntry>();


        static UnitTracker()
        {
            Obj_AI_Base.OnNewPath += OnNewPath;
        }

        internal static double GetLastStopTick(AIHeroClient hero)
        {
            UnitTrackerEntry data;
            return DictData.TryGetValue(hero.NetworkId, out data) ? (Variables.TickCount - data.StopTick) / 1000d : 1;
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

            if (data.Path[2].Tick - data.Path[1].Tick < 180 && Variables.TickCount - data.Path[2].Tick < 90)
            {
                var posHero = hero.Position;
                var posPath1 = data.Path[1].Position;
                var posPath2 = data.Path[2].Position;

                var a = Math.Pow(posPath2.X - posHero.X, 2) + Math.Pow(posPath2.Y - posHero.Y, 2);
                var b = Math.Pow(posHero.X - posPath1.X, 2) + Math.Pow(posHero.Y - posPath1.Y, 2);
                var c = Math.Pow(posPath2.X - posPath1.X, 2) + Math.Pow(posPath2.Y - posPath1.Y, 2);

                return data.Path[1].Position.Distance(data.Path[2].Position) < 50
                       || Math.Cos((a + b - c) / (2 * Math.Sqrt(a) * Math.Sqrt(b))) * 180 / Math.PI < 31;
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
                DictData[sender.NetworkId].StopTick = Variables.TickCount;
            }

            DictData[sender.NetworkId].Path.Add(
                new StoredPath { Position = args.Path.Last().ToVector2(), Tick = Variables.TickCount });

            if (DictData[sender.NetworkId].Path.Count > 3)
            {
                DictData[sender.NetworkId].Path.RemoveAt(0);
            }
        }

        private class StoredPath
        {
            #region Properties

            internal Vector2 Position { get; set; }

            internal int Tick { get; set; }

            #endregion
        }

        private class UnitTrackerEntry
        {
            #region Properties

            internal List<StoredPath> Path { get; } = new List<StoredPath>();

            internal int StopTick { get; set; }

            #endregion
        }
    }

    public class PredictionInput : ICloneable
    {

        private Vector3 from;
        private Vector3 rangeCheckFrom;
        public bool AoE { get; set; }

        public bool Collision { get; set; }

        public CollisionableObjects CollisionObjects { get; set; } = CollisionableObjects.Minions
                                                                     | CollisionableObjects.YasuoWall;
        public float Delay { get; set; }
        public Vector3 From
        {
            get
            {
                return this.from.IsValid() ? this.from : ObjectManager.Player.ServerPosition;
            }

            set
            {
                this.from = value;
            }
        }

        public float Radius { get; set; } = 1f;

        public float Range { get; set; } = float.MaxValue;

        public Vector3 RangeCheckFrom
        {
            get
            {
                return this.rangeCheckFrom.IsValid() ? this.rangeCheckFrom : this.From;
            }

            set
            {
                this.rangeCheckFrom = value;
            }
        }

        public float Speed { get; set; } = float.MaxValue;

        public SkillshotType Type { get; set; } = SkillshotType.SkillshotLine;
        public Obj_AI_Base Unit { get; set; } = ObjectManager.Player;

        public bool UseBoundingRadius { get; set; } = true;
        internal float RealRadius => this.UseBoundingRadius ? this.Radius + this.Unit.BoundingRadius : this.Radius;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class PredictionOutput
    {

        private Vector3 castPosition;
        private Vector3 unitPosition;
        public int AoeHitCount { get; set; }
        public List<AIHeroClient> AoeTargetsHit { get; set; } = new List<AIHeroClient>();

        public int AoeTargetsHitCount => Math.Max(this.AoeHitCount, this.AoeTargetsHit.Count);
        public Vector3 CastPosition
        {
            get
            {
                return this.castPosition.IsValid() ? this.castPosition.SetZ() : this.Input.Unit.ServerPosition;
            }
            set
            {
                this.castPosition = value;
            }
        }

        public List<Obj_AI_Base> CollisionObjects { get; set; } = new List<Obj_AI_Base>();

        public HitChance Hitchance { get; set; } = HitChance.Impossible;

        public PredictionInput Input { get; set; }

        public Vector3 UnitPosition
        {
            get
            {
                return this.unitPosition.IsValid() ? this.unitPosition.SetZ() : this.Input.Unit.ServerPosition;
            }
            set
            {
                this.unitPosition = value;
            }
        }
    }

    internal static class AoePrediction
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

            foreach (var enemy in
                GameObjects.EnemyHeroes.Where(
                    h =>
                    !h.Compare(input.Unit)
                    && h.IsValidTarget(input.Range + 200 + input.RealRadius, true, input.RangeCheckFrom)))
            {
                var inputs = input.Clone() as PredictionInput;

                if (inputs == null)
                {
                    continue;
                }

                inputs.Unit = enemy;
                var prediction = Prediction.GetPrediction(inputs, false, true);

                if (prediction.Hitchance >= HitChance.High)
                {
                    result.Add(new PossibleTarget { Position = prediction.UnitPosition.ToVector2(), Unit = enemy });
                }
            }

            return result;
        }

        public static class Circle
        {
            public static PredictionOutput GetCirclePrediction(PredictionInput input)
            {
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.ToVector2(),
                                                     Unit = input.Unit
                                                 }
                                         };

                if (mainTargetPrediction.Hitchance >= HitChance.High)
                {
                    // Add the posible targets in range:
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
                            CastPosition = mecCircle.Center.ToVector3(),
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
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.ToVector2(),
                                                     Unit = input.Unit
                                                 }
                                         };

                if (mainTargetPrediction.Hitchance >= HitChance.High)
                {
                    // Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                if (posibleTargets.Count > 1)
                {
                    var candidates = new List<Vector2>();

                    foreach (var target in posibleTargets)
                    {
                        target.Position = target.Position - input.From.ToVector2();
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
                            CastPosition = bestCandidate.ToVector3(),
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
                        where
                            point.DistanceSquared(default(Vector2)) < range * range && edge1.CrossProduct(point) > 0
                            && point.CrossProduct(edge2) > 0
                        select point).Count();
            }
        }

        public static class Line
        {

            public static PredictionOutput GetLinePrediction(PredictionInput input)
            {
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.ToVector2(),
                                                     Unit = input.Unit
                                                 }
                                         };

                if (mainTargetPrediction.Hitchance >= HitChance.High)
                {
                    // Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                if (posibleTargets.Count > 1)
                {
                    var candidates = new List<Vector2>();
                    foreach (var targetCandidates in
                        posibleTargets.Select(
                            target => GetCandidates(input.From.ToVector2(), target.Position, input.Radius, input.Range))
                        )
                    {
                        candidates.AddRange(targetCandidates);
                    }

                    var bestCandidateHits = -1;
                    var bestCandidate = default(Vector2);
                    var bestCandidateHitPoints = new List<Vector2>();
                    var positionsList = posibleTargets.Select(t => t.Position).ToList();

                    foreach (var candidate in candidates)
                    {
                        if (
                            GetHits(
                                input.From.ToVector2(),
                                candidate,
                                input.Radius + (input.Unit.BoundingRadius / 3) - 10,
                                new List<Vector2> { posibleTargets[0].Position }).Count() == 1)
                        {
                            var hits = GetHits(input.From.ToVector2(), candidate, input.Radius, positionsList).ToList();
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

                        // Center the position
                        for (var i = 0; i < bestCandidateHitPoints.Count; i++)
                        {
                            for (var j = 0; j < bestCandidateHitPoints.Count; j++)
                            {
                                var startP = input.From.ToVector2();
                                var endP = bestCandidate;
                                var proj1 = positionsList[i].ProjectOn(startP, endP);
                                var proj2 = positionsList[j].ProjectOn(startP, endP);
                                var dist = bestCandidateHitPoints[i].DistanceSquared(proj1.LinePoint)
                                           + bestCandidateHitPoints[j].DistanceSquared(proj2.LinePoint);

                                if (dist >= maxDistance
                                    && (proj1.LinePoint - positionsList[i]).AngleBetween(
                                        proj2.LinePoint - positionsList[j]) > 90)
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
                            CastPosition = ((p1 + p2) * 0.5f).ToVector3(),
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

            internal static IEnumerable<Vector2> GetHits(
                Vector2 start,
                Vector2 end,
                double radius,
                List<Vector2> points)
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

    internal static class Collision
    {

        private static MissileClient yasuoWallLeft, yasuoWallRight;
        private static RectanglePoly yasuoWallPoly;
        static Collision()
        {
            GameObject.OnCreate += (sender, args) =>
            {
                var missile = sender as MissileClient;
                var spellCaster = missile?.SpellCaster as AIHeroClient;

                if (spellCaster == null || spellCaster.ChampionName != "Yasuo"
                    || spellCaster.Team == GameObjects.Player.Team)
                {
                    return;
                }

                switch (missile.SData.Name)
                {
                    case "YasuoWMovingWallMisL":
                        yasuoWallLeft = missile;
                        break;
                    case "YasuoWMovingWallMisR":
                        yasuoWallRight = missile;
                        break;
                }
            };
            GameObject.OnDelete += (sender, args) =>
            {
                var missile = sender as MissileClient;

                if (missile == null)
                {
                    return;
                }

                if (missile.Compare(yasuoWallLeft))
                {
                    yasuoWallLeft = null;
                }
                else if (missile.Compare(yasuoWallRight))
                {
                    yasuoWallRight = null;
                }
            };
        }

        public static List<Obj_AI_Base> GetCollision(List<Vector3> positions, PredictionInput input)
        {
            var result = new List<Obj_AI_Base>();

            foreach (var position in positions)
            {
                if (input.CollisionObjects.HasFlag(CollisionableObjects.Minions))
                {
                    result.AddRange(
                        GameObjects.EnemyMinions.Where(i => i.IsMinion() || i.IsPet())
                            .Concat(GameObjects.Jungle)
                            .Where(
                                minion =>
                                minion.IsValidTarget(
                                    Math.Min(input.Range + input.Radius + 100, 2000),
                                    true,
                                    input.RangeCheckFrom)
                                && (minion.Distance(input.From) < 10 + minion.BoundingRadius
                                    || minion.Distance(position) < minion.BoundingRadius
                                    || IsHitCollision(minion, input, position, minion.IsMoving ? 50 : 15))));
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.Heroes))
                {
                    result.AddRange(
                        GameObjects.EnemyHeroes.Where(
                            hero =>
                            hero.IsValidTarget(
                                Math.Min(input.Range + input.Radius + 100, 2000),
                                true,
                                input.RangeCheckFrom) && IsHitCollision(hero, input, position, 50)));
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.Walls))
                {
                    var step = position.Distance(input.From) / 20;
                    for (var i = 0; i < 20; i++)
                    {
                        if (input.From.ToVector2().Extend(position, step * i).IsWall())
                        {
                            result.Add(GameObjects.Player);
                        }
                    }
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.YasuoWall))
                {
                    if (yasuoWallLeft == null || yasuoWallRight == null)
                    {
                        continue;
                    }

                    yasuoWallPoly = new RectanglePoly(yasuoWallLeft.Position, yasuoWallRight.Position, 75);

                    var intersections = new List<Vector2>();
                    for (var i = 0; i < yasuoWallPoly.Points.Count; i++)
                    {
                        var inter =
                            yasuoWallPoly.Points[i].Intersection(
                                yasuoWallPoly.Points[i != yasuoWallPoly.Points.Count - 1 ? i + 1 : 0],
                                input.From.ToVector2(),
                                position.ToVector2());

                        if (inter.Intersects)
                        {
                            intersections.Add(inter.Point);
                        }
                    }

                    if (intersections.Count > 0)
                    {
                        result.Add(GameObjects.Player);
                    }
                }
            }

            return result.Distinct().ToList();
        }

        private static bool IsHitCollision(Obj_AI_Base collision, ICloneable input, Vector3 pos, float extraRadius)
        {
            var inputSub = input.Clone() as PredictionInput;

            if (inputSub == null)
            {
                return false;
            }

            inputSub.Unit = collision;

            return
                Prediction.GetPrediction(inputSub, false, false)
                    .UnitPosition.ToVector2()
                    .DistanceSquared(inputSub.From.ToVector2(), pos.ToVector2(), true)
                <= Math.Pow(inputSub.Radius + inputSub.Unit.BoundingRadius + extraRadius, 2);
        }
    }

    internal static class GamePath
    {

        public static class PathTracker
        {

            private const double MaxTime = 1.5d;

            private static readonly Dictionary<int, List<StoredPath>> StoredPaths =
                new Dictionary<int, List<StoredPath>>();

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
                    // Assume that the unit was moving for the first path:
                    distance += (maxT - paths[0].Time) * unit.MoveSpeed;

                    for (var i = 0; i < paths.Count - 1; i++)
                    {
                        var currentPath = paths[i];
                        var nextPath = paths[i + 1];

                        if (currentPath.WaypointCount > 0)
                        {
                            distance += Math.Min(
                                (currentPath.Time - nextPath.Time) * unit.MoveSpeed,
                                currentPath.Path.PathLength());
                        }
                    }

                    // Take into account the last path:
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
                return StoredPaths.TryGetValue(unit.NetworkId, out value)
                           ? value.Where(p => p.Time < maxT).ToList()
                           : new List<StoredPath>();
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

                var newPath = new StoredPath { Tick = Variables.TickCount, Path = args.Path.ToList().ToVector2() };
                StoredPaths[sender.NetworkId].Add(newPath);

                if (StoredPaths[sender.NetworkId].Count > 50)
                {
                    StoredPaths[sender.NetworkId].RemoveRange(0, 40);
                }
            }
        }

        public class StoredPath
        {

            public Vector2 EndPoint => this.Path.LastOrDefault();
            public List<Vector2> Path { get; set; }
            public Vector2 StartPoint => this.Path.FirstOrDefault();
            public int Tick { get; set; }
            public double Time => (Variables.TickCount - this.Tick) / 1000d;
            public int WaypointCount => this.Path.Count;
        }
    }

    internal static class Health
    {

        private static readonly Dictionary<int, PredictedDamage> ActiveAttacks = new Dictionary<int, PredictedDamage>();
        private static int lastTick;
        static Health()
        {
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnObjAiBaseProcessSpellCast;
            Spellbook.OnStopCast += OnSpellbookStopCast;
            GameObject.OnDelete += OnGameObjectDelete;
            Obj_AI_Base.OnSpellCast += OnObjAiBaseDoCast;
        }
        public static Obj_AI_Base GetAggroTurret(Obj_AI_Minion minion)
        {
            var activeTurret =
                ActiveAttacks.Values.FirstOrDefault(m => m.Source is Obj_AI_Turret && m.Target.Compare(minion));
            return activeTurret?.Source;
        }

        public static float GetPrediction(
            Obj_AI_Base unit,
            int time,
            int delay = 70,
            HealthPredictionType type = HealthPredictionType.Default)
        {
            return type == HealthPredictionType.Simulated
                       ? GetPredictionSimulated(unit, time)
                       : GetPredictionDefault(unit, time, delay);
        }

        public static bool HasMinionAggro(Obj_AI_Minion minion)
        {
            return ActiveAttacks.Values.Any(m => m.Source is Obj_AI_Minion && m.Target.Compare(minion));
        }

        public static bool HasTurretAggro(Obj_AI_Minion minion)
        {
            return ActiveAttacks.Values.Any(m => m.Source is Obj_AI_Turret && m.Target.Compare(minion));
        }

        public static int TurretAggroStartTick(Obj_AI_Minion minion)
        {
            var activeTurret =
                ActiveAttacks.Values.FirstOrDefault(m => m.Source is Obj_AI_Turret && m.Target.Compare(minion));
            return activeTurret?.StartTick ?? 0;
        }

        private static float GetPredictionDefault(Obj_AI_Base unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values.Where(i => i.Target.Compare(unit) && !i.Processed))
            {
                var attackDamage = 0f;

                if (attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.IsValidTarget())
                {
                    var landTime = attack.StartTick + attack.Delay
                                   + 1000
                                   * (attack.Source.IsMelee
                                          ? 0
                                          : Math.Max(unit.Distance(attack.Source) - attack.Source.BoundingRadius, 0)
                                            / attack.ProjectileSpeed) + delay;

                    if (landTime < Variables.TickCount + time)
                    {
                        attackDamage = attack.Damage;
                    }
                }

                predictedDamage += attackDamage;
            }

            return unit.Health - predictedDamage;
        }

        private static float GetPredictionSimulated(Obj_AI_Base unit, int time)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values.Where(i => i.Target.Compare(unit)))
            {
                var n = 0;

                if (Variables.TickCount - 100 <= attack.StartTick + attack.AnimationTime
                    && attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.IsValidTarget())
                {
                    var fromT = attack.StartTick;
                    var toT = Variables.TickCount + time;

                    while (fromT < toT)
                    {
                        if (fromT >= Variables.TickCount
                            && fromT + attack.Delay / 1000
                            + (attack.Source.IsMelee
                                   ? 0
                                   : Math.Max(unit.Distance(attack.Source) - attack.Source.BoundingRadius, 0)
                                     / attack.ProjectileSpeed) < toT)
                        {
                            n++;
                        }

                        fromT += (int)attack.AnimationTime;
                    }
                }

                predictedDamage += n * attack.Damage;
            }

            return unit.Health - predictedDamage;
        }

        private static void OnGameObjectDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }

            var aiBase = sender as Obj_AI_Base;

            if (aiBase != null)
            {
                var objNetworkId = aiBase.NetworkId;

                if (ActiveAttacks.ContainsKey(objNetworkId))
                {
                    ActiveAttacks.Remove(objNetworkId);
                    return;
                }

                foreach (var activeAttack in ActiveAttacks.Values.Where(i => i.Target.Compare(aiBase)))
                {
                    ActiveAttacks.Remove(activeAttack.Source.NetworkId);
                }
                return;
            }

            var missile = sender as MissileClient;

            if (missile?.SpellCaster != null)
            {
                var casterNetworkId = missile.SpellCaster.NetworkId;

                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks[casterNetworkId].Processed = true;
                }
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Variables.TickCount - lastTick <= 1000)
            {
                return;
            }

            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < Variables.TickCount - 3000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));

            lastTick = Variables.TickCount;
        }

        private static void OnObjAiBaseDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMelee)
            {
                var casterNetworkId = sender.NetworkId;

                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks[casterNetworkId].Processed = true;
                }
            }
        }
        private static void OnObjAiBaseProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValidTarget(2000, false) || !AutoAttack.IsAutoAttack(args.SData.Name) || !sender.IsAlly)
            {
                return;
            }

            if (!(sender is Obj_AI_Minion) && !(sender is Obj_AI_Turret))
            {
                return;
            }

            var target = args.Target as Obj_AI_Minion;

            if (target == null)
            {
                return;
            }

            ActiveAttacks.Remove(sender.NetworkId);
            ActiveAttacks.Add(
                sender.NetworkId,
                new PredictedDamage(
                    sender,
                    target,
                    Variables.TickCount - (Game.Ping / 2),
                    sender.AttackCastDelay * 1000,
                    (sender.AttackDelay * 1000) - (sender is Obj_AI_Turret ? 70 : 0),
                    sender.IsMelee ? int.MaxValue : (int)args.SData.MissileSpeed,
                    (float)sender.GetAutoAttackDamage(target)));
        }

        private static void OnSpellbookStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsValid && args.StopAnimation && args.DestroyMissile)
            {
                var casterNetworkId = sender.NetworkId;

                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks.Remove(casterNetworkId);
                }
            }
        }

        private class PredictedDamage
        {

            public readonly float AnimationTime;
            public readonly float Damage;
            public readonly float Delay;
            public readonly int ProjectileSpeed;
            public readonly Obj_AI_Base Source;
            public readonly int StartTick;
            public readonly Obj_AI_Base Target;

            public PredictedDamage(
                Obj_AI_Base source,
                Obj_AI_Base target,
                int startTick,
                float delay,
                float animationTime,
                int projectileSpeed,
                float damage)
            {
                this.Source = source;
                this.Target = target;
                this.StartTick = startTick;
                this.Delay = delay;
                this.ProjectileSpeed = projectileSpeed;
                this.Damage = damage;
                this.AnimationTime = animationTime;
            }
            public bool Processed { get; set; }
        }
    }
}