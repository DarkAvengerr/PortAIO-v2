// Copyright 2014 - 2014 Esk0r
// Evader.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Common.Evade
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using SharpDX;
    using System.Collections.Generic;
    using System.Linq;

    public static class Evader
    {
        public static List<Vector2> GetEvadePoints(int speed = -1, int delay = 0, bool isBlink = false, bool onlyGood = false)
        {
            speed = speed == -1 ? (int) GameObjects.Player.MoveSpeed : speed;

            var goodCandidates = new List<Vector2>();
            var badCandidates = new List<Vector2>();

            var polygonList = new List<Geometry.Polygon>();

            var takeClosestPath = false;

            foreach (var skillshot in Evade.DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    if (skillshot.SpellData.TakeClosestPath && skillshot.IsDanger(Evade.PlayerPosition))
                    {
                        takeClosestPath = true;
                    }

                    polygonList.Add(skillshot.EvadePolygon);
                }
            }

            var dangerPolygons = Geometry.ClipPolygons(polygonList).ToPolygons();
            var myPosition = Evade.PlayerPosition;

            foreach (var poly in dangerPolygons)
            {
                for (var i = 0; i <= poly.Points.Count - 1; i++)
                {
                    var sideStart = poly.Points[i];
                    var sideEnd = poly.Points[(i == poly.Points.Count - 1) ? 0 : i + 1];
                    var originalCandidate = myPosition.ProjectOn(sideStart, sideEnd).SegmentPoint;
                    var distanceToEvadePoint = Vector2.DistanceSquared(originalCandidate, myPosition);

                    if (distanceToEvadePoint < 600 * 600)
                    {
                        var sideDistance = Vector2.DistanceSquared(sideEnd, sideStart);
                        var direction = (sideEnd - sideStart).Normalized();
                        var s = (distanceToEvadePoint < 200 * 200 && sideDistance > 90 * 90) ? Config.DiagonalEvadePointsCount : 0;

                        for (var j = -s; j <= s; j++)
                        {
                            var candidate = originalCandidate + j * Config.DiagonalEvadePointsStep * direction;
                            var pathToPoint = GameObjects.Player.GetPath(candidate.ToVector3()).To2DList();

                            if (!isBlink)
                            {
                                if (Evade.IsSafePath(pathToPoint, Config.EvadingFirstTimeOffset, speed, delay).IsSafe)
                                {
                                    goodCandidates.Add(candidate);
                                }

                                if (Evade.IsSafePath(pathToPoint, Config.EvadingSecondTimeOffset, speed, delay).IsSafe && j == 0)
                                {
                                    badCandidates.Add(candidate);
                                }
                            }
                            else
                            {
                                if (Evade.IsSafeToBlink(pathToPoint[pathToPoint.Count - 1], Config.EvadingFirstTimeOffset, delay))
                                {
                                    goodCandidates.Add(candidate);
                                }

                                if (Evade.IsSafeToBlink(pathToPoint[pathToPoint.Count - 1], Config.EvadingSecondTimeOffset, delay))
                                {
                                    badCandidates.Add(candidate);
                                }
                            }
                        }
                    }
                }
            }

            if (takeClosestPath)
            {
                if (goodCandidates.Count > 0)
                {
                    goodCandidates = new List<Vector2>
                    {
                        goodCandidates.MinOrDefault(vector2 => GameObjects.Player.Distance(vector2, true))
                    };
                }

                if (badCandidates.Count > 0)
                {
                    badCandidates = new List<Vector2>
                    {
                        badCandidates.MinOrDefault(vector2 => GameObjects.Player.Distance(vector2, true))
                    };
                }
            }

            return (goodCandidates.Count > 0) ? goodCandidates : (onlyGood ? new List<Vector2>() : badCandidates);
        }

        public static Vector2 GetClosestOutsidePoint(Vector2 from, List<Geometry.Polygon> polygons)
        {
            var result = new List<Vector2>();

            foreach (var poly in polygons)
            {
                for (var i = 0; i <= poly.Points.Count - 1; i++)
                {
                    var sideStart = poly.Points[i];
                    var sideEnd = poly.Points[(i == poly.Points.Count - 1) ? 0 : i + 1];

                    result.Add(from.ProjectOn(sideStart, sideEnd).SegmentPoint);
                }
            }

            return result.MinOrDefault(vector2 => vector2.Distance(from));
        }

        public static List<Obj_AI_Base> GetEvadeTargets(SpellValidTargets[] validTargets, int speed, int delay, float range, bool isBlink = false, bool onlyGood = false, bool DontCheckForSafety = false)
        {
            var badTargets = new List<Obj_AI_Base>();
            var goodTargets = new List<Obj_AI_Base>();
            var allTargets = new List<Obj_AI_Base>();

            foreach (var targetType in validTargets)
            {
                switch (targetType)
                {
                    case SpellValidTargets.AllyChampions:
                        {
                            foreach (var ally in GameObjects.AllyHeroes)
                            {
                                if (ally.IsValidTarget(range, false) && !ally.IsMe && ally.IsAlly)
                                {
                                    allTargets.Add(ally);
                                }
                            }
                        }
                        break;
                    case SpellValidTargets.AllyMinions:
                        {
                            allTargets.AddRange(GameObjects.AllyMinions.Where(x => x.IsValidTarget(range)));
                        }
                        break;
                    case SpellValidTargets.AllyWards:
                        {
                            foreach (var gameObject in GameObjects.Minions)
                            {
                                if (gameObject.Name.ToLower().Contains("ward") && gameObject.IsValidTarget(range, false) && gameObject.Team == GameObjects.Player.Team)
                                {
                                    allTargets.Add(gameObject);
                                }
                            }
                        }
                        break;
                    case SpellValidTargets.EnemyChampions:
                        {
                            foreach (var enemy in GameObjects.EnemyHeroes)
                            {
                                if (enemy.IsValidTarget(range))
                                {
                                    allTargets.Add(enemy);
                                }
                            }
                        }
                        break;
                    case SpellValidTargets.EnemyMinions:
                        {
                            allTargets.AddRange(GameObjects.EnemyMinions.Where(x => x.IsValidTarget(range)));
                        }
                        break;
                    case SpellValidTargets.EnemyWards:
                        {
                            foreach (var gameObject in GameObjects.Minions)
                            {
                                if (gameObject.Name.ToLower().Contains("ward") && gameObject.IsValidTarget(range))
                                {
                                    allTargets.Add(gameObject);
                                }
                            }
                        }
                        break;
                }
            }

            foreach (var target in allTargets)
            {
                if (DontCheckForSafety || Evade.IsSafe(target.ServerPosition.ToVector2()).IsSafe)
                {
                    if (isBlink)
                    {
                        if (Utils.TickCount - Evade.LastWardJumpAttempt < 250 || Evade.IsSafeToBlink(target.ServerPosition.ToVector2(), Config.EvadingFirstTimeOffset, delay))
                        {
                            goodTargets.Add(target);
                        }

                        if (Utils.TickCount - Evade.LastWardJumpAttempt < 250 || Evade.IsSafeToBlink(target.ServerPosition.ToVector2(), Config.EvadingSecondTimeOffset, delay))
                        {
                            badTargets.Add(target);
                        }
                    }
                    else
                    {
                        var pathToTarget = new List<Vector2>();

                        pathToTarget.Add(Evade.PlayerPosition);
                        pathToTarget.Add(target.ServerPosition.ToVector2());

                        if (Utils.TickCount - Evade.LastWardJumpAttempt < 250 || Evade.IsSafePath(pathToTarget, Config.EvadingFirstTimeOffset, speed, delay).IsSafe)
                        {
                            goodTargets.Add(target);
                        }

                        if (Utils.TickCount - Evade.LastWardJumpAttempt < 250 || Evade.IsSafePath(pathToTarget, Config.EvadingSecondTimeOffset, speed, delay).IsSafe)
                        {
                            badTargets.Add(target);
                        }
                    }
                }
            }

            return (goodTargets.Count > 0) ? goodTargets : (onlyGood ? new List<Obj_AI_Base>() : badTargets);
        }
    }
}