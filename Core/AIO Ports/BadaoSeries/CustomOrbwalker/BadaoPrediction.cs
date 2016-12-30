using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoSeries.CustomOrbwalker
{
    public static class BadaoPrediction
    {
        private static int _wallCastT;
        private static Vector2 _yasuoWallCastedPos;

        static BadaoPrediction()
        {
            Obj_AI_Base.OnSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.Team != ObjectManager.Player.Team && args.SData.Name == "YasuoWMovingWall")
            {
                _wallCastT = Utils.TickCount;
                _yasuoWallCastedPos = sender.ServerPosition.To2D();
            }
        }



        public class PredictionInput
        {
            private Vector3 _from;
            private Vector3 _rangeCheckFrom;

            /// <summary>
            ///     Set to true make the prediction hit as many enemy heroes as posible.
            /// </summary>
            public bool Aoe = false;

            /// <summary>
            ///     Set to true if the unit collides with units.
            /// </summary>
            public bool Collision = false;

            /// <summary>
            ///     Array that contains the unit types that the skillshot can collide with.
            /// </summary>
            public CollisionableObjects[] CollisionObjects =
        {
            CollisionableObjects.Minions, CollisionableObjects.YasuoWall
        };

            /// <summary>
            ///     The skillshot delay in seconds.
            /// </summary>
            public float Delay;

            /// <summary>
            ///     The skillshot width's radius or the angle in case of the cone skillshots.
            /// </summary>
            public float Radius = 1f;

            /// <summary>
            ///     The skillshot range in units.
            /// </summary>
            public float Range = float.MaxValue;

            /// <summary>
            ///     The skillshot speed in units per second.
            /// </summary>
            public float Speed = float.MaxValue;

            /// <summary>
            ///     The skillshot type.
            /// </summary>
            public SkillshotType Type = SkillshotType.SkillshotLine;

            /// <summary>
            ///     The unit that the prediction will made for.
            /// </summary>
            public Obj_AI_Base Unit = ObjectManager.Player;

            /// <summary>
            ///     Set to true to increase the prediction radius by the unit bounding radius.
            /// </summary>
            public bool UseBoundingRadius = true;

            /// <summary>
            ///     The position from where the skillshot missile gets fired.
            /// </summary>
            public Vector3 From
            {
                get { return _from.To2D().IsValid() ? _from : ObjectManager.Player.ServerPosition; }
                set { _from = value; }
            }

            /// <summary>
            ///     The position from where the range is checked.
            /// </summary>
            public Vector3 RangeCheckFrom
            {
                get
                {
                    return _rangeCheckFrom.To2D().IsValid()
                        ? _rangeCheckFrom
                        : (From.To2D().IsValid() ? From : ObjectManager.Player.ServerPosition);
                }
                set { _rangeCheckFrom = value; }
            }

            internal float RealRadius
            {
                get { return  Radius; }
            }
        }

        public class PredictionOutput
        {
            internal int _aoeTargetsHitCount;
            private Vector3 _castPosition;
            private Vector3 _unitPosition;

            /// <summary>
            ///     The list of the targets that the spell will hit (only if aoe was enabled).
            /// </summary>
            public List<AIHeroClient> AoeTargetsHit = new List<AIHeroClient>();

            /// <summary>
            ///     The list of the units that the skillshot will collide with.
            /// </summary>
            public List<Obj_AI_Base> CollisionObjects = new List<Obj_AI_Base>();

            /// <summary>
            ///     Returns the hitchance.
            /// </summary>
            public HitChance Hitchance = HitChance.Impossible;

            internal PredictionInput Input;

            /// <summary>
            ///     The position where the skillshot should be casted to increase the accuracy.
            /// </summary>
            public Vector3 CastPosition
            {
                get
                {
                    return _castPosition.IsValid() && _castPosition.To2D().IsValid()
                        ? _castPosition.SetZ()
                        : Input.Unit.ServerPosition;
                }
                set { _castPosition = value; }
            }

            /// <summary>
            ///     The number of targets the skillshot will hit (only if aoe was enabled).
            /// </summary>
            //public int AoeTargetsHitCount
            //{
            //    get { return Math.Max(_aoeTargetsHitCount, AoeTargetsHit.Count); }
            //}

            /// <summary>
            ///     The position where the unit is going to be when the skillshot reaches his position.
            /// </summary>
            public Vector3 UnitPosition
            {
                get { return _unitPosition.To2D().IsValid() ? _unitPosition.SetZ() : Input.Unit.ServerPosition; }
                set { _unitPosition = value; }
            }
        }
        //public static bool BadaoCast2(this Spell spell, Obj_AI_Base target)
        //{
        //    var prediction = spell.GetBadao2Prediction(target);
        //    if (!spell.IsSkillshot)
        //        return false;
        //    //if (prediction.Hitchance < spell.MinHitChance)
        //    //    return false;
        //    return ObjectManager.Player.Spellbook.CastSpell(spell.Slot, prediction);
        //}
        //public static Vector3 GetBadao2Prediction(this Spell spell, Obj_AI_Base target)
        //{
        //    Vector3 chuot = Prediction.GetPrediction(target, 1).UnitPosition;
        //    float dis = spell.From.Distance(target.Position);
        //    float rad = target.BoundingRadius + spell.Width - 50;
        //    double x = math.t(target.MoveSpeed, spell.Speed, dis, spell.Delay + Game.Ping / 2 / 1000, rad, spell.From.To2D(), target.Position.To2D(), chuot.To2D());
        //    if (x != 0 && !target.IsDashing()) { return target.Position.Extend(chuot, (float)x * target.MoveSpeed - rad); }
        //    else return target.Position;
        //}
        public static bool BadaoCast(this Spell spell, Obj_AI_Base target)
        {
            var prediction = spell.GetBadaoPrediction(target);
            if (!spell.IsSkillshot)
                return false;
            if (prediction.Hitchance < spell.MinHitChance)
                return false;
            return ObjectManager.Player.Spellbook.CastSpell(spell.Slot, prediction.CastPosition);
        }
        public static PredictionOutput GetBadaoPrediction(this Spell spell, Obj_AI_Base target, bool collideyasuowall = true)
        {
            PredictionOutput result = null;

            if (!target.IsValidTarget(float.MaxValue, false))
            {
                return new PredictionOutput();
            }
            if (target.IsDashing())
            {
                var dashDtata = target.GetDashInfo();
                result = spell.GetBadaoStandarPrediction(target,
                    new List<Vector2>() {target.ServerPosition.To2D(), dashDtata.Path.Last()},dashDtata.Speed);
                if (result.Hitchance >= HitChance.High)
                    result.Hitchance = HitChance.Dashing;
            }
            else
            {
                //Unit is immobile.
                var remainingImmobileT = UnitIsImmobileUntil(target);
                if (remainingImmobileT >= 0d)
                {
                    var timeToReachTargetPosition = spell.Delay + target.Position.To2D().Distance(spell.From.To2D()) / spell.Speed;
                    if (spell.RangeCheckFrom.To2D().Distance(target.Position.To2D()) <= spell.Range)
                    {
                        if (timeToReachTargetPosition <=
                            remainingImmobileT + (target.BoundingRadius + spell.Width - 40)/target.MoveSpeed)
                        {
                            result = new PredictionOutput
                            {
                                CastPosition = target.ServerPosition,
                                UnitPosition = target.ServerPosition,
                                Hitchance = HitChance.Immobile
                            };
                        }

                        else result =  new PredictionOutput
                        {
                            CastPosition = target.ServerPosition,
                            UnitPosition = target.ServerPosition,
                            Hitchance = HitChance.High
                            /*timeToReachTargetPosition - remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed < 0.4d ? HitChance.High : HitChance.Medium*/
                        };
                    }
                    else
                    {
                       result = new PredictionOutput();
                    }
                }
            }
            //Normal prediction
            if (result == null)
            {
                result = spell.GetBadaoStandarPrediction(target,target.Path.ToList().To2D());
            }
            //Check for collision
            if (spell.Collision)
            {
                var positions = new List<Vector3> { result.UnitPosition, result.CastPosition, target.Position };
                var originalUnit = target;
                result.CollisionObjects = spell.GetCollision(positions);
                result.CollisionObjects.RemoveAll(x => x.NetworkId == originalUnit.NetworkId);
                result.Hitchance = result.CollisionObjects.Count > 0 ? HitChance.Collision : result.Hitchance;
            }
            //Check yasuo wall collision
            else if (collideyasuowall)
            {
                var positions = new List<Vector3> { result.UnitPosition, result.CastPosition, target.Position };
                var originalUnit = target;
                result.CollisionObjects = spell.GetCollision(positions);
                result.CollisionObjects.Any(x => x.NetworkId == ObjectManager.Player.NetworkId);
                result.Hitchance = result.CollisionObjects.Any(x => x.NetworkId == ObjectManager.Player.NetworkId) ? HitChance.Collision : result.Hitchance;
            }
            return result;


        }

        public static PredictionOutput GetBadaoStandarPrediction(this Spell spell, Obj_AI_Base target,
            List<Vector2> path, float speed = -1)
        {
            // check the unit speed input
            speed = (Math.Abs(speed - (-1)) < float.Epsilon) ? target.MoveSpeed : speed;
            // set standar output
            Vector2 castpos = target.ServerPosition.To2D();
            Vector2 unitpos = target.ServerPosition.To2D();
            HitChance hitchance = HitChance.Impossible;
            // target standing like a statue (performing an attack, casting spell, afk, aimbush.....)
            if (path.Count <= 1)
            {
                // set standar position
                castpos = target.ServerPosition.To2D();
                unitpos = target.ServerPosition.To2D();
                // target in range
                if (spell.RangeCheckFrom.To2D().Distance(castpos) <= spell.Range)
                    hitchance = HitChance.High;
                // target out of range
                else
                {
                    // skill shot circle
                    if (spell.Type == SkillshotType.SkillshotCircle)
                    {
                        // check for extra radius
                        if (spell.RangeCheckFrom.To2D().Distance(castpos) <=
                            spell.Range + spell.Width + target.BoundingRadius - 40)
                        {
                            castpos = spell.RangeCheckFrom.To2D().Extend(castpos, spell.Range);
                            hitchance = HitChance.Medium;
                        }
                        else
                        {
                            castpos = spell.RangeCheckFrom.To2D().Extend(castpos, spell.Range);
                            hitchance = HitChance.OutOfRange;
                        }
                    }
                    else
                        hitchance = HitChance.OutOfRange;
                }
                return new PredictionOutput()
                {
                    UnitPosition = unitpos.To3D(),
                    CastPosition = castpos.To3D(),
                    Hitchance = hitchance
                };
            }
            //Skillshots with only a delay
            if (Math.Abs(spell.Speed - float.MaxValue) < float.Epsilon && path.Count >= 2)
            {
                var a = path[0];
                var b = path[1];
                var distance = a.Distance(b);
                // skillshot circle
                if (spell.Type == SkillshotType.SkillshotCircle)
                {
                    //standar distance
                    var x = speed*(spell.Delay + Game.Ping/2000f + 0.06f);
                    // position 1 properties
                    var distance01 = x - (target.BoundingRadius + spell.Width)/2;
                    var pos01 = a.Extend(b, distance01);
                    // position 2 properties
                    var distance02 = x;
                    var pos02 = a.Extend(b, distance02);
                    // position 3 properties
                    var distance03 = x + (target.BoundingRadius + spell.Width)/2;
                    var pos03 = pos02.Extend(spell.From.To2D(), distance03);
                    // lines length
                    var length01 = pos01.Distance(pos02);
                    var length02 = pos02.Distance(pos03);
                    // set standar position
                    unitpos = pos02;
                    castpos = pos02;
                    // list cast poses
                    List<Vector2> poses = new List<Vector2>();
                    for (int i = 0; i <= 10; i++)
                    {
                        poses.Add(i <= 5 ? pos01.Extend(pos02, i*length01/6) : pos02.Extend(pos03, (i - 5)*length02/5));
                    }
                    // check cast pos
                    for (int i = 0; i <= 10; i++)
                    {
                        if (poses[i].Distance(spell.RangeCheckFrom.To2D()) <= spell.Range &&
                            poses[i].Distance(a) <= distance)
                        {

                            if (i <= 3)
                            {
                                hitchance = HitChance.VeryHigh;
                            }
                            else if (i <= 6)
                            {
                                hitchance = HitChance.High;
                            }
                            else
                                hitchance = HitChance.Medium;

                            return new PredictionOutput
                            {
                                UnitPosition = unitpos.To3D(),
                                CastPosition = poses[i].To3D(),
                                Hitchance = hitchance
                            };
                        }
                    }
                    // hitchance out of range
                    return new PredictionOutput
                    {
                        UnitPosition = unitpos.To3D(),
                        CastPosition = castpos.To3D(),
                        Hitchance = HitChance.OutOfRange
                    };
                }
                // skill shot line and cone
                else
                {
                    //standar distance
                    var x = speed*(spell.Delay + Game.Ping/2000f + 0.06f);
                    // position properties
                    var distance01 = x;
                    var pos01 = a.Extend(b, distance01);
                    var range01 = spell.RangeCheckFrom.To2D().Distance(pos01);
                    // set standar position
                    unitpos = pos01;
                    castpos = pos01;
                    // hitchance high
                    if (distance01 < distance && range01 <= spell.Range)
                    {
                        castpos = pos01;
                        hitchance = HitChance.High;
                        return new PredictionOutput
                        {
                            UnitPosition = unitpos.To3D(),
                            CastPosition = castpos.To3D(),
                            Hitchance = hitchance
                        };
                    }
                    // hitchance out of range
                    return new PredictionOutput
                    {
                        UnitPosition = unitpos.To3D(),
                        CastPosition = castpos.To3D(),
                        Hitchance = HitChance.OutOfRange
                    };

                }
            }
            //  skill shot with a delay and speed
            if (Math.Abs(spell.Speed - float.MaxValue) > float.Epsilon)
            {
                var a = path[0];
                var b = path[1];
                var distance = a.Distance(b);
                // standar prediction
                float dis = spell.From.To2D().Distance(a);
                float rad = 0;
                double time = math.t(speed, spell.Speed, dis, spell.Delay + Game.Ping/2f/1000 + 0.06f,
                    0, spell.From.To2D(), a, b);
                var unitpos02 = !double.IsNaN(time) ? a.Extend(b, (float) time*speed) : new Vector2();
                var castpos02 = unitpos02;
                // very high prediction
                rad = (target.BoundingRadius + spell.Width)/2;
                time = math.t(target.MoveSpeed, spell.Speed, dis, spell.Delay + Game.Ping/2f/1000 + 0.06f,
                    rad, spell.From.To2D(), a, b);
                var unitpos01 = !double.IsNaN(time) ? a.Extend(b, (float) time*speed- rad) : new Vector2();
                var castpos01 = unitpos01;
                // medium prediction
                time = math.t(target.MoveSpeed, spell.Speed, dis, spell.Delay + Game.Ping/2f/1000 + 0.06f -rad/spell.Speed,
                    0, spell.From.To2D(), a, b);
                var unitpos03 = !double.IsNaN(time) ? a.Extend(b, (float) time*speed) : new Vector2();
                var castpos03 = unitpos03.IsValid()
                    ? spell.From.To2D().Extend(unitpos03, spell.From.To2D().Distance(unitpos03) - rad)
                    : new Vector2();
                if (castpos01.IsValid() && castpos02.IsValid() && castpos03.IsValid())
                {
                    var length01 = castpos01.Distance(castpos02);
                    var length02 = castpos02.Distance(castpos03);
                    var Acosb =
                        Math.Acos(
                            Math.Abs(float.IsNaN(math.CosB(spell.From.To2D(), a, b))
                                ? 0.99f
                                : Math.Abs(math.CosB(spell.From.To2D(), a, b))))*(180/Math.PI);
                    // skillshot circle + line
                    if (spell.Type == SkillshotType.SkillshotCircle ||
                        (spell.Type == SkillshotType.SkillshotLine && Acosb <= 110 && Acosb >= 70))
                    {
                        List<Vector2> poses = new List<Vector2>();
                        for (int i = 0; i <= 10; i++)
                        {
                            poses.Add(i <= 5
                                ? castpos01.Extend(castpos02, i*length01/6)
                                : castpos02.Extend(castpos03, (i - 5)*length02/5));
                        }
                        // check cast pos
                        for (int i = 0; i <= 10; i++)
                        {
                            if (poses[i].Distance(spell.RangeCheckFrom.To2D()) <= spell.Range &&
                                poses[i].Distance(a) <= distance)
                            {

                                if (i <= 3)
                                {
                                    hitchance = HitChance.VeryHigh;
                                }
                                else if (i <= 6)
                                {
                                    hitchance = HitChance.High;
                                }
                                else
                                    hitchance = HitChance.Medium;

                                return new PredictionOutput
                                {
                                    UnitPosition = unitpos02.To3D(),
                                    CastPosition = poses[i].To3D(),
                                    Hitchance = hitchance
                                };
                            }
                        }
                        // hitchance out of range
                        return new PredictionOutput
                        {
                            UnitPosition = unitpos02.To3D(),
                            CastPosition = castpos02.To3D(),
                            Hitchance = HitChance.OutOfRange
                        };
                    }
                    // skillshot line + cone
                    else
                    {
                        var distance02 = a.Distance(castpos02);
                        var range01 = spell.RangeCheckFrom.To2D().Distance(castpos02);
                        // hitchance high
                        if (distance02 < distance && range01 <= spell.Range)
                        {
                            return new PredictionOutput
                            {
                                UnitPosition = unitpos02.To3D(),
                                CastPosition = castpos02.To3D(),
                                Hitchance = HitChance.High
                            };
                        }
                        // hitchance out of range
                        return new PredictionOutput
                        {
                            UnitPosition = unitpos02.To3D(),
                            CastPosition = castpos02.To3D(),
                            Hitchance = HitChance.OutOfRange
                        };
                    }
                }
            }
            return new PredictionOutput
            {
                UnitPosition = unitpos.To3D(),
                CastPosition = castpos.To3D(),
                Hitchance = hitchance
            };
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

        public static List<Obj_AI_Base> GetCollision(this Spell spell, List<Vector3> positions)
        {
            var objects = new List<CollisionableObjects>(){CollisionableObjects.YasuoWall,CollisionableObjects.Minions, CollisionableObjects.Heroes};
            var result = new List<Obj_AI_Base>();

            foreach (var position in positions)
            {
                foreach (var objectType in objects)
                {
                    switch (objectType)
                    {
                        case CollisionableObjects.Minions:
                            foreach (var minion in
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        minion =>
                                            minion.IsValidTarget(
                                                Math.Min(spell.Range + spell.Width + 100, 2000), true,
                                                spell.RangeCheckFrom)))
                            {
                                var target = minion;
                                var minionPrediction = spell.GetBadaoStandarPrediction(target,target.Path.ToList().To2D());
                                if (
                                    minionPrediction.UnitPosition.To2D()
                                        .Distance(spell.From.To2D(), position.To2D(), true, true) <=
                                    Math.Pow((spell.Width + 15 + minion.BoundingRadius), 2))
                                {
                                    result.Add(minion);
                                }
                            }
                            break;
                        case CollisionableObjects.Heroes:
                            foreach (var hero in
                                HeroManager.Enemies.FindAll(
                                    hero =>
                                        hero.IsValidTarget(
                                            Math.Min(spell.Range + spell.Width + 100, 2000), true, spell.RangeCheckFrom))
                                )
                            {
                                var target = hero;
                                var prediction = spell.GetBadaoStandarPrediction(target, target.Path.ToList().To2D());
                                if (
                                    prediction.UnitPosition.To2D()
                                        .Distance(spell.From.To2D(), position.To2D(), true, true) <=
                                    Math.Pow((spell.Width + 50 + hero.BoundingRadius), 2))
                                {
                                    result.Add(hero);
                                }
                            }
                            break;

                        case CollisionableObjects.Allies:
                            foreach (var hero in
                                HeroManager.Allies.FindAll(
                                    hero =>
                                       Vector3.Distance(ObjectManager.Player.ServerPosition, hero.ServerPosition) <= Math.Min(spell.Range + spell.Width + 100, 2000))
                                )
                            {
                                var target = hero;
                                var prediction = spell.GetBadaoStandarPrediction(target, target.Path.ToList().To2D());
                                if (
                                    prediction.UnitPosition.To2D()
                                        .Distance(spell.From.To2D(), position.To2D(), true, true) <=
                                    Math.Pow((spell.Width + 50 + hero.BoundingRadius), 2))
                                {
                                    result.Add(hero);
                                }
                            }
                            break;


                        case CollisionableObjects.Walls:
                            var step = position.Distance(spell.From) / 20;
                            for (var i = 0; i < 20; i++)
                            {
                                var p = spell.From.To2D().Extend(position.To2D(), step * i);
                                if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(CollisionFlags.Wall))
                                {
                                    result.Add(ObjectManager.Player);
                                }
                            }
                            break;

                        case CollisionableObjects.YasuoWall:

                            if (Utils.TickCount - _wallCastT > 4000)
                            {
                                break;
                            }

                            GameObject wall = null;
                            foreach (var gameObject in
                                ObjectManager.Get<GameObject>()
                                    .Where(
                                        gameObject =>
                                            gameObject.IsValid &&
                                            Regex.IsMatch(
                                                gameObject.Name, "_w_windwall_enemy_0.\\.troy", RegexOptions.IgnoreCase))
                                )
                            {
                                wall = gameObject;
                            }
                            if (wall == null)
                            {
                                break;
                            }
                            var level = wall.Name.Substring(wall.Name.Length - 6, 1);
                            var wallWidth = (300 + 50 * Convert.ToInt32(level));

                            var wallDirection =
                                (wall.Position.To2D() - _yasuoWallCastedPos).Normalized().Perpendicular();
                            var wallStart = wall.Position.To2D() + wallWidth / 2f * wallDirection;
                            var wallEnd = wallStart - wallWidth * wallDirection;

                            if (wallStart.Intersection(wallEnd, position.To2D(), spell.From.To2D()).Intersects)
                            {
                                var t = Utils.TickCount +
                                        (wallStart.Intersection(wallEnd, position.To2D(), spell.From.To2D())
                                            .Point.Distance(spell.From) / spell.Speed + spell.Delay) * 1000;
                                if (t < _wallCastT + 4000)
                                {
                                    result.Add(ObjectManager.Player);
                                }
                            }

                            break;
                    }
                }
            }

            return result.Distinct().ToList();
        }
        //public static float GetBadaoPrediction(this Spell spell, Vector3 position)
        //{
        //    if (!spell.IsSkillshot)
        //        return float.NaN;
        //    var x = spell.RangeCheckFrom.To2D().Distance(position.To2D());
        //    if (x > spell.Range)
        //        return float.NaN;
        //    return
        //        Game.Ping/(2f * 1000) + spell.Delay + (Math.Abs(spell.Speed - float.MaxValue) <= float.Epsilon ? 0 : x/spell.Speed);
        //}

        //public static Vector3? GetBadaoPrediction(this Obj_AI_Base target , float delay)
        //{
        //    List<Vector2> x = new List<Vector2>();
        //    x.cu
        //}
        //public class CircleHits
        //{
        //    public int Hit;
        //    public Vector2 Pos;
        //    public CircleHits(int hit, Vector2 pos)
        //    {
        //        Hit = hit;
        //        Pos = pos;
        //    }
        //}
        // public class LineHits
        //{
        //    public int Hit;
        //    public Vector2 Pos;
        //    public LineHits(int hit, Vector2 pos)
        //    {
        //        Hit = hit;
        //        Pos = pos;
        //    }
        //}
        //public class CornHits
        //{
        //    public int Hit;
        //    public Vector2 Pos;
        //    public CornHits(int hit, Vector2 pos)
        //    {
        //        Hit = hit;
        //        Pos = pos;
        //    }
        //}

        //public static CircleHits SpellCircleHits(this Spell spell)
        //{
            
        //}

    }
    public class math
    {
        public static double FindDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Vector2(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Vector2(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Vector2(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return  Math.Sqrt(dx * dx + dy * dy);
        }

        public static double ptb2(float a, float b, float c)
        {
            if (Math.Abs(a) > float.Epsilon)
            {
                float delta = b * b - 4 * a * c;
                if (delta < 0) return double.NaN;
                else
                {
                    double canDelta = Math.Sqrt(delta);
                    double x1 = (-b + canDelta) / (2 * a); double x2 = (-b - canDelta) / (2 * a);
                    double X = x1 > x2 ? x1 : x2;
                    return X > 0 ? X : double.NaN;
                }
            }
            else { double X = -c / b; return X > 0 ? X : double.NaN; }
        }
        public static float CosB(Vector2 a, Vector2 b, Vector2 c)
        {
            float a1 = c.Distance(b);
            float b1 = a.Distance(c);
            float c1 = b.Distance(a);
            if (Math.Abs(a1) < float.Epsilon || Math.Abs(c1) < float.Epsilon) { return float.NaN; }
            else { return (a1 * a1 + c1 * c1 - b1 * b1) / (2 * a1 * c1); }
        }

        public static double t(float movespeed, float spellspeed, float distance, float spelldelay, float spellradius,
            Vector2 from, Vector2 targetpos, Vector2 targetdirection)
        {
            float cosA = CosB(from, targetpos, targetdirection);
            if (float.IsNaN(cosA))
            {
                return double.NaN;
            }
            else
            {
                float a = movespeed*movespeed - spellspeed*spellspeed;
                float b = -2*spellradius*movespeed + 2*spellspeed*spellspeed*spelldelay - 2*cosA*movespeed*distance;
                float c = spellradius*spellradius + distance*distance - spellspeed*spellspeed*spelldelay*spelldelay +
                          2*cosA*distance*spellradius;
                double X = ptb2(a, b, c);
                return X;
            }
        }
    }
}
