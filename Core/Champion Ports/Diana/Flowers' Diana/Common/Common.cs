using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Diana
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using Prediction;
    using SharpDX;
    using SPrediction;
    using System;

    public static class Common
    {
        public static HitChance MinCommonHitChance
        {
            get
            {
                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 0)
                {
                    return HitChance.VeryHigh;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 1)
                {
                    return HitChance.High;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 2)
                {
                    return HitChance.Medium;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 3)
                {
                    return HitChance.Low;
                }

                return HitChance.VeryHigh;
            }
        }

        public static SebbyLib.Prediction.HitChance MinOKTWHitChance
        {
            get
            {
                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 0)
                {
                    return SebbyLib.Prediction.HitChance.VeryHigh;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 1)
                {
                    return SebbyLib.Prediction.HitChance.High;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 2)
                {
                    return SebbyLib.Prediction.HitChance.Medium;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 3)
                {
                    return SebbyLib.Prediction.HitChance.Low;
                }

                return SebbyLib.Prediction.HitChance.VeryHigh;
            }
        }

        public static SDKPrediction.HitChance MinSDKHitChance
        {
            get
            {
                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 0)
                {
                    return SDKPrediction.HitChance.VeryHigh;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 1)
                {
                    return SDKPrediction.HitChance.High;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 2)
                {
                    return SDKPrediction.HitChance.Medium;
                }

                if (Program.Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 3)
                {
                    return SDKPrediction.HitChance.Low;
                }

                return SDKPrediction.HitChance.VeryHigh;
            }
        }

        public static void CastTo(this Spell Spells, Obj_AI_Base target, bool AOE = false)
        {
            switch (Program.Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    {
                        var SpellPred = Spells.GetPrediction(target, AOE);

                        if (SpellPred.Hitchance >= MinCommonHitChance)
                        {
                            Spells.Cast(SpellPred.CastPosition, true);
                        }
                    }
                    break;
                case 1:
                    {
                        SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;

                        if (Spells.Type == SkillshotType.SkillshotCircle)
                        {
                            CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                        }

                        var predInput2 = new SebbyLib.Prediction.PredictionInput
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

                        var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                        if (Spells.Speed != float.MaxValue && YasuoWindWall.CollisionYasuo(ObjectManager.Player.ServerPosition, poutput2.CastPosition))
                        {
                            return;
                        }

                        if (poutput2.Hitchance >= MinOKTWHitChance)
                        {
                            Spells.Cast(poutput2.CastPosition, true);
                        }
                        else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= MinOKTWHitChance - 1)
                        {
                            Spells.Cast(poutput2.CastPosition, true);
                        }
                    }
                    break;
                case 2:
                    {
                        SDKPrediction.SkillshotType CoreType2 = SDKPrediction.SkillshotType.SkillshotLine;

                        var predInput2 = new SDKPrediction.PredictionInput
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

                        var poutput2 = SDKPrediction.GetPrediction(predInput2);

                        if (Spells.Speed != float.MaxValue && YasuoWindWall.CollisionYasuo(ObjectManager.Player.ServerPosition, poutput2.CastPosition))
                        {
                            return;
                        }

                        if (poutput2.Hitchance >= MinSDKHitChance)
                        {
                            Spells.Cast(poutput2.CastPosition, true);
                        }
                        else if (predInput2.AoE && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= MinSDKHitChance - 1)
                        {
                            Spells.Cast(poutput2.CastPosition, true);
                        }
                    }
                    break;
                case 3:
                {
                    var hero = target as AIHeroClient;

                        if (hero != null && hero.IsValid)
                        {
                            var t = hero;

                            if (t.IsValidTarget())
                            {
                                Spells.SPredictionCast(t, MinCommonHitChance);
                            }
                        }
                        else
                        {
                            Spells.CastIfHitchanceEquals(target, MinCommonHitChance);
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
            }
        }

        public static string[] AutoEnableList =
        {
            "Annie", "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "Evelynn", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus",
             "Kassadin", "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna",
             "Ryze", "Sion", "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", "Azir", "Ekko",
             "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jayce", "Jinx", "KogMaw", "Lucian", "MasterYi", "MissFortune", "Quinn", "Shaco", "Sivir",
             "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed", "Kindred", "AurelionSol"
        };

        public static bool CheckTarget(Obj_AI_Base target, float range = float.MaxValue)
        {
            if (target == null)
            {
                return false;
            }

            if (target.DistanceToPlayer() > range)
            {
                return false;
            }

            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return false;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            return !target.HasBuff("FioraW");
        }

        public static bool CheckTargetSureCanKill(Obj_AI_Base target)
        {
            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return false;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            return !target.HasBuff("FioraW");
        }

        public static double ComboDamage(AIHeroClient target)
        {
            if (CheckTarget(target))
            {
                var Damage = 0d;

                Damage += ObjectManager.Player.GetAutoAttackDamage(target) +
                          (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady()
                              ? ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q)
                              : 0d) +
                          (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).IsReady()
                              ? ObjectManager.Player.GetSpellDamage(target, SpellSlot.W)
                              : 0d) +
                          (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).IsReady()
                              ? ObjectManager.Player.GetSpellDamage(target, SpellSlot.E)
                              : 0d) +
                          (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).IsReady()
                              ? ObjectManager.Player.GetSpellDamage(target, SpellSlot.R)
                              : 0d) +
                          ((ObjectManager.Player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown &&
                            ObjectManager.Player.GetSpellSlot("SummonerDot").IsReady())
                              ? 50 + 20*ObjectManager.Player.Level - (target.HPRegenRate/5*3)
                              : 0d);

                if (target.ChampionName == "Moredkaiser")
                    Damage -= target.Mana;

                // exhaust
                if (ObjectManager.Player.HasBuff("SummonerExhaust"))
                    Damage = Damage * 0.6f;

                // blitzcrank passive
                if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
                    Damage -= target.Mana / 2f;

                // kindred r
                if (target.HasBuff("KindredRNoDeathBuff"))
                    Damage = 0;

                // tryndamere r
                if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                    Damage = 0;

                // kayle r
                if (target.HasBuff("JudicatorIntervention"))
                    Damage = 0;

                // zilean r
                if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                    Damage = 0;

                // fiora w
                if (target.HasBuff("FioraW"))
                    Damage = 0;

                return Damage;
            }

            return 0d;
        }


        public static bool CanMove(this AIHeroClient Target)
        {
            return !(Target.MoveSpeed < 50) && !Target.IsStunned && !Target.HasBuffOfType(BuffType.Stun) &&
                !Target.HasBuffOfType(BuffType.Fear) && !Target.HasBuffOfType(BuffType.Snare) &&
                !Target.HasBuffOfType(BuffType.Knockup) && !Target.HasBuff("Recall") && !Target.HasBuffOfType(BuffType.Knockback) 
                && !Target.HasBuffOfType(BuffType.Charm) && !Target.HasBuffOfType(BuffType.Taunt) && 
                !Target.HasBuffOfType(BuffType.Suppression) && (!Target.IsCastingInterruptableSpell()
                || Target.IsMoving);
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
                CollisionTime = collisionTime;
                CollisionPosition = collisionPosition;
            }

            public object this[int i] => i == 0 ? CollisionTime : (object)CollisionPosition;
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
