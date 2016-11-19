using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Prediction
{
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    internal class CPrediction
    {
        public static float BoundingRadiusMultiplicator { get; set; } = 0.65f;

        public static Result Circle(Spell spell, AIHeroClient target, HitChance hitChance, bool boundingRadius = true,
            bool extended = true)
        {
            if (spell == null || target == null)
            {
                return new Result(Vector3.Zero, new List<AIHeroClient>());
            }

            var hits = new List<AIHeroClient>();
            var center = Vector3.Zero;
            var radius = float.MaxValue;
            var range = spell.Range + (extended ? spell.Width*0.85f : 0) +
                        (boundingRadius ? target.BoundingRadius*BoundingRadiusMultiplicator : 0);
            var positions = (from t in HeroManager.Enemies
                where t.IsValidTarget(range*1.5f, true, spell.RangeCheckFrom)
                let prediction = spell.GetPrediction(t)
                where prediction.Hitchance >= hitChance
                select new Position(t, prediction.UnitPosition)).ToList();
            var spellWidth = spell.Width;

            if (positions.Any())
            {
                var mainTarget = positions.FirstOrDefault(p => p.Hero.NetworkId == target.NetworkId);
                var possibilities =
                    ProduceEnumeration(
                            positions.Where(
                                p => p.UnitPosition.Distance(mainTarget.UnitPosition) <= spell.Width*0.85f).ToList())
                        .Where(p => p.Count > 0 && p.Any(t => t.Hero.NetworkId == mainTarget.Hero.NetworkId))
                        .ToList();

                foreach (var possibility in possibilities)
                {
                    var mec = MEC.GetMec(possibility.Select(p => p.UnitPosition.To2D()).ToList());
                    var distance = spell.From.Distance(mec.Center.To3D());

                    if (mec.Radius < spellWidth && distance < range)
                    {
                        var lHits = new List<AIHeroClient>();
                        var circle =
                            new Geometry.Polygon.Circle(
                                spell.From.Extend(
                                    mec.Center.To3D(), spell.Range > distance ? distance : spell.Range), spell.Width);

                        if (boundingRadius)
                        {
                            lHits.AddRange(
                                from position in positions
                                where
                                new Geometry.Polygon.Circle(
                                    position.UnitPosition,
                                    position.Hero.BoundingRadius*BoundingRadiusMultiplicator).Points.Any(
                                    p => circle.IsInside(p))
                                select position.Hero);
                        }
                        else
                        {
                            lHits.AddRange(
                                from position in positions
                                where circle.IsInside(position.UnitPosition)
                                select position.Hero);
                        }

                        if ((lHits.Count > hits.Count || lHits.Count == hits.Count && mec.Radius < radius ||
                             lHits.Count == hits.Count &&
                             spell.From.Distance(circle.Center.To3D()) < spell.From.Distance(center)) &&
                            lHits.Any(p => p.NetworkId == target.NetworkId))
                        {
                            center = circle.Center.To3D2();
                            radius = mec.Radius;
                            hits.Clear();
                            hits.AddRange(lHits);
                        }
                    }
                }

                if (!center.Equals(Vector3.Zero))
                {
                    return new Result(center, hits);
                }
            }

            return new Result(Vector3.Zero, new List<AIHeroClient>());
        }

        public static Result Line(Spell spell, AIHeroClient target, HitChance hitChance, bool boundingRadius = true,
            bool maxRange = true)
        {
            if (spell == null || target == null)
            {
                return new Result(Vector3.Zero, new List<AIHeroClient>());
            }

            var range = (spell.IsChargedSpell && maxRange ? spell.ChargedMaxRange : spell.Range) +
                        spell.Width*0.9f +
                        (boundingRadius ? target.BoundingRadius*BoundingRadiusMultiplicator : 0);

            var positions = (from t in HeroManager.Enemies
                where t.IsValidTarget(range, true, spell.RangeCheckFrom)
                let prediction = spell.GetPrediction(t)
                where prediction.Hitchance >= hitChance
                select new Position(t, prediction.UnitPosition)).ToList();

            if (positions.Any())
            {
                var hits = new List<AIHeroClient>();
                var pred = spell.GetPrediction(target);
                if (pred.Hitchance >= hitChance)
                {
                    hits.Add(target);

                    var rect = new Geometry.Polygon.Rectangle(
                        spell.From, spell.From.Extend(pred.CastPosition, range), spell.Width);

                    if (boundingRadius)
                    {
                        hits.AddRange(
                            from point in positions.Where(p => p.Hero.NetworkId != target.NetworkId)
                            let circle =
                            new Geometry.Polygon.Circle(
                                point.UnitPosition, point.Hero.BoundingRadius*BoundingRadiusMultiplicator)
                            where circle.Points.Any(p => rect.IsInside(p))
                            select point.Hero);
                    }
                    else
                    {
                        hits.AddRange(
                            from position in positions
                            where rect.IsInside(position.UnitPosition)
                            select position.Hero);
                    }

                    return new Result(pred.CastPosition, hits);
                }
            }

            return new Result(Vector3.Zero, new List<AIHeroClient>());
        }

        private static IEnumerable<int> ConstructSetFromBits(int i)
        {
            for (var n = 0; i != 0; i /= 2, n++)
            {
                if ((i & 1) != 0)
                {
                    yield return n;
                }
            }
        }

        private static IEnumerable<List<T>> ProduceEnumeration<T>(List<T> list)
        {
            for (var i = 0; i < 1 << list.Count; i++)
            {
                yield return ConstructSetFromBits(i).Select(n => list[n]).ToList();
            }
        }

        internal struct Position
        {
            public readonly AIHeroClient Hero;
            public readonly Obj_AI_Base Base;
            public readonly Vector3 UnitPosition;

            public Position(AIHeroClient hero, Vector3 unitPosition)
            {
                Hero = hero;
                Base = null;
                UnitPosition = unitPosition;
            }

            public Position(Obj_AI_Base unit, Vector3 unitPosition)
            {
                Base = unit;
                Hero = null;
                UnitPosition = unitPosition;
            }
        }

        internal struct BasePosition
        {
            public readonly Obj_AI_Base Unit;
            public readonly Vector3 UnitPosition;

            public BasePosition(Obj_AI_Base unit, Vector3 unitPosition)
            {
                Unit = unit;
                UnitPosition = unitPosition;
            }
        }

        internal struct Result
        {
            public readonly Vector3 CastPosition;
            public readonly List<AIHeroClient> Hits;
            public readonly int TotalHits;

            public Result(Vector3 castPosition, List<AIHeroClient> hits)
            {
                CastPosition = castPosition;
                Hits = hits;
                TotalHits = hits.Count;
            }
        }
    }
}
