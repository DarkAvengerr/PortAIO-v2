using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Spells
{
    using myCommon;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    internal class SpellManager : Logic
    {
        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 665f, TargetSelector.DamageType.Magical);
            W = new Spell(SpellSlot.W, 700f, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 525f, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 700f, TargetSelector.DamageType.Magical);

            W.SetSkillshot(1.50f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 450f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        internal static bool CastWMePos(string SpellName)
        {
            switch (SpellName)
            {
                case "AatroxQ":
                    return true;
                case "AkaliShadowDance":
                    return true;
                case "Headbutt":
                    return true;
                case "DianaTeleport":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "JaxLeapStrike":
                    return true;
                case "KatarinaE":
                    return true;
                case "KhazixE":
                    return true;
                case "LeonaZenithBlade":
                    return true;
                case "MaokaiTrunkLine":
                    return true;
                case "MonkeyKingNimbus":
                    return true;
                case "PantheonW":
                    return true;
                case "PoppyHeroicCharge":
                    return true;
                case "ShenShadowDash":
                    return true;
                case "SejuaniArcticAssault":
                    return true;
                case "RenektonSliceAndDice":
                    return true;
                case "Slash":
                    return true;
                case "XenZhaoSweep":
                    return true;
                case "RocketJump":
                    return true;
            }

            return false;
        }

        internal static bool CastWTargetPos(string SpellName)
        {
            switch (SpellName)
            {
                case "KatarinaR":
                    return true;
                case "GalioIdolOfDurand":
                    return true;
                case "GragasE":
                    return true;
                case "Crowstorm":
                    return true;
                case "BandageToss":
                    return true;
                case "LissandraE":
                    return true;
                case "AbsoluteZero":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "FallenOne":
                    return true;
                case "PantheonRJump":
                    return true;
                case "CaitlynAceintheHole":
                    return true;
                case "MissFortuneBulletTime":
                    return true;
                case "InfiniteDuress":
                    return true;
                case "ThreshQ":
                    return true;
                case "RocketGrab":
                    return true;
            }

            return false;
        }

        public static void CastE(AIHeroClient target) // SFX Challenger Viktor E Logic
        {
            if (!E.IsReady() || target == null)
            {
                return;
            }

            var input = new PredictionInput
            {
                Range = EWidth,
                Delay = E.Delay,
                Radius = E.Width,
                Speed = E.Speed,
                Type = E.Type,
                UseBoundingRadius = true
            };

            var input2 = new PredictionInput
            {
                Range = E.Range + EWidth,
                Delay = E.Delay,
                Radius = E.Width,
                Speed = E.Speed,
                Type = E.Type,
                UseBoundingRadius = true
            };

            var startPosition = Vector3.Zero;
            var endPosition = Vector3.Zero;

            if (target.ServerPosition.Distance(Me.ServerPosition) <= E.Range)
            {
                var castPosition = target.ServerPosition;
                var maxAdditionalHits = 0;

                foreach (
                    var target1 in
                    HeroManager.Enemies.Where(t => t.IsValidTarget((E.Range + EWidth + E.Width)*1.25f))
                        .Where(t => t.NetworkId != target.NetworkId))
                {
                    var lTarget = target1;
                    var additionalHits = 0;

                    input.Unit = lTarget;
                    input.From = castPosition;
                    input.RangeCheckFrom = castPosition;

                    var pred = Prediction.GetPrediction(input);

                    if (pred.Hitchance >= HitChance.High)
                    {
                        additionalHits++;

                        var rect = new Geometry.Polygon.Rectangle(
                            castPosition, castPosition.Extend(pred.CastPosition, EWidth), E.Width);

                        foreach (var target2 in
                            HeroManager.Enemies.Where(t => t.IsValidTarget((E.Range + EWidth + E.Width) * 1.25f)).Where(
                                t => t.NetworkId != target.NetworkId && t.NetworkId != lTarget.NetworkId))
                        {
                            input.Unit = target2;

                            var pred2 = Prediction.GetPrediction(input);

                            if (!pred2.UnitPosition.Equals(Vector3.Zero) &&
                                new Geometry.Polygon.Circle(pred2.UnitPosition, target2.BoundingRadius * 0.8f)
                                    .Points.Any(p => rect.IsInside(p)))
                            {
                                additionalHits++;
                            }
                        }
                    }

                    if (additionalHits > maxAdditionalHits)
                    {
                        maxAdditionalHits = additionalHits;
                        endPosition = pred.CastPosition;
                    }
                }

                startPosition = castPosition;

                if (endPosition.Equals(Vector3.Zero))
                {
                    if (startPosition.Distance(Me.ServerPosition) > E.Range)
                    {
                        startPosition = Me.ServerPosition.Extend(startPosition, E.Range);
                    }

                    if (target.Path.Length > 0)
                    {
                        var newPos = target.Path[0];

                        if (target.Path.Length > 1 &&
                            newPos.Distance(target.ServerPosition) <= target.BoundingRadius * 4f)
                        {
                            var nnPos = newPos.Extend(
                                target.Path[1],
                                Math.Min(target.BoundingRadius * 1.5f, newPos.Distance(target.Path[1])));

                            if (startPosition.To2D().AngleBetween(nnPos.To2D()) < 30)
                            {
                                newPos = nnPos;
                            }
                        }

                        endPosition = startPosition.Extend(newPos, EWidth);
                    }
                    else if (target.IsFacing(Me))
                    {
                        endPosition = startPosition.Extend(Me.ServerPosition, EWidth);
                    }
                    else
                    {
                        endPosition = Me.ServerPosition.Extend(
                            startPosition, startPosition.Distance(Me.ServerPosition) + EWidth);
                    }
                }
            }
            else
            {
                var totalHits = 0;

                input2.Unit = target;

                var pred = Prediction.GetPrediction(input2);

                if (!pred.UnitPosition.Equals(Vector3.Zero) && !pred.CastPosition.Equals(Vector3.Zero))
                {
                    var ranges =
                        new[] { E.Range }.Concat(
                            HeroManager.Enemies.Where(t => t.IsValidTarget((E.Range + EWidth + E.Width) * 1.25f)).Where(
                                t =>
                                    t.ServerPosition.Distance(Me.ServerPosition) < E.Range &&
                                    t.ServerPosition.Distance(target.ServerPosition) < EWidth * 1.25f)
                                .Select(t => t.ServerPosition.Distance(Me.ServerPosition)));
                    var maxDistance = (EWidth + E.Width + target.BoundingRadius) * 1.1f;

                    foreach (var range in ranges)
                    {
                        var circle =
                            new Geometry.Polygon.Circle(Me.ServerPosition, Math.Min(E.Range, range), 50).Points
                                .Where(p => p.Distance(pred.UnitPosition) <= maxDistance)
                                .Select(p => p.To3D())
                                .OrderBy(p => p.Distance(pred.CastPosition));

                        foreach (var point in circle)
                        {
                            var hits = 0;

                            input.From = point;
                            input.RangeCheckFrom = point;
                            input.Unit = target;

                            var pred2 = Prediction.GetPrediction(input);

                            if (pred2.Hitchance >= HitChance.VeryHigh)
                            {
                                var rect = new Geometry.Polygon.Rectangle(
                                    point, point.Extend(pred2.CastPosition, EWidth), E.Width);

                                foreach (
                                    var target3 in
                                    HeroManager.Enemies.Where(t => t.IsValidTarget((E.Range + EWidth + E.Width)*1.25f)))
                                {
                                    input.Unit = target3;

                                    var pred3 = Prediction.GetPrediction(input);

                                    if (!pred3.UnitPosition.Equals(Vector3.Zero) &&
                                        (target3.NetworkId.Equals(target.NetworkId) ||
                                         pred3.UnitPosition.Distance(point) <=
                                         EWidth + target3.BoundingRadius * 0.8f) &&
                                        new Geometry.Polygon.Circle(
                                            pred3.UnitPosition,
                                            target3.BoundingRadius *
                                            (target3.NetworkId.Equals(target.NetworkId) ? 0.6f : 0.8f)).Points
                                            .Any(p => rect.IsInside(p)))
                                    {
                                        hits++;
                                    }
                                }
                                if (hits > totalHits ||
                                    hits > 0 && hits == totalHits &&
                                    point.Distance(target.ServerPosition) <
                                    startPosition.Distance(target.ServerPosition))
                                {
                                    totalHits = hits;
                                    startPosition = point;
                                    endPosition = point.Extend(pred2.CastPosition, EWidth);
                                }

                                if (totalHits ==
                                    HeroManager.Enemies.Count(t => t.IsValidTarget((E.Range + EWidth + E.Width)*1.25f)))
                                {
                                    break;
                                }
                            }
                        }

                        if (totalHits == HeroManager.Enemies.Count(t => t.IsValidTarget((E.Range + EWidth + E.Width) * 1.25f)))
                        {
                            break;
                        }
                    }
                }
            }

            if (!startPosition.Equals(Vector3.Zero) && !endPosition.Equals(Vector3.Zero))
            {
                FixECast(startPosition, endPosition);
            }
        }

        internal static void FixECast(Vector3 Pos1, Vector3 Pos2)
        {
            if (Pos1.Equals(Vector3.Zero) || Pos2.Equals(Vector3.Zero))
            {
                return;
            }

            var startPos = Vector3.Zero;
            var endPos = Vector3.Zero;

            startPos = Pos1.DistanceToPlayer() > E.Range ? Me.ServerPosition.Extend(Pos1, E.Range) : Pos1;
            endPos = endPos.Distance(Pos2) > EWidth ? startPos.Extend(Pos2, EWidth) : Pos2;

            if (startPos.Equals(Vector3.Zero) || endPos.Equals(Vector3.Zero))
            {
                return;
            }

            E.Cast(startPos, endPos);
        }

        internal static MinionManager.FarmLocation GetBestLineFarmLocation(List<Vector2> minionPositions,
            Vector3 StartPos, float width, float range)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = StartPos.To2D();

            var posiblePositions = new List<Vector2>();
            posiblePositions.AddRange(minionPositions);

            var max = minionPositions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (minionPositions[j] != minionPositions[i])
                    {
                        posiblePositions.Add((minionPositions[j] + minionPositions[i]) / 2);
                    }
                }
            }

            foreach (var pos in posiblePositions)
            {
                if (pos.Distance(startPos, true) <= range * range)
                {
                    var endPos = startPos + range * (pos - startPos).Normalized();

                    var count =
                        minionPositions.Count(pos2 => pos2.Distance(startPos, endPos, true, true) <= width * width);

                    if (count >= minionCount)
                    {
                        result = endPos;
                        minionCount = count;
                    }
                }
            }

            return new MinionManager.FarmLocation(result, minionCount);
        }
    }
}
