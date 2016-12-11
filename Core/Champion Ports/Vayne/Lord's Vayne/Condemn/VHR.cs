using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using System.Collections.Generic;
using System;
using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Condemn
{
    class VHR
    {

        public static void Run()
        {
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            var pushDistance = Program.emenu.Item("PushDistance").GetValue<Slider>().Value;
            var Prediction = Program.E.GetPrediction(target);
            var endPosition = Prediction.UnitPosition.Extend
                (ObjectManager.Player.ServerPosition, -pushDistance);

            if (Prediction.Hitchance >= HitChance.VeryHigh)
            {
                if (endPosition.IsWall())
                {
                    var condemnRectangle = new Geometry.Polygon.Rectangle(target.ServerPosition.To2D(),
                        endPosition.To2D(), target.BoundingRadius);

                    if (
                        condemnRectangle.Points.Count(
                            point =>
                                NavMesh.GetCollisionFlags(point.X, point.Y)
                                    .HasFlag(CollisionFlags.Wall)) >=
                        condemnRectangle.Points.Count * (20 / 100f))
                    {
                        if (CheckTargets.CheckTarget(target, Program.E.Range))
                        {
                            Program.E.CastOnUnit(target);
                        }
                    }
                }
                else
                {
                    var step = pushDistance / 5f;
                    for (float i = 0; i < pushDistance; i += step)
                    {
                        var endPositionEx = Prediction.UnitPosition.Extend(ObjectManager.Player.ServerPosition, -i);
                        if (endPositionEx.IsWall())
                        {
                            var condemnRectangle =
                                new Geometry.Polygon.Rectangle(target.ServerPosition.To2D(),
                                    endPosition.To2D(), target.BoundingRadius);

                            if (
                                condemnRectangle.Points.Count(
                                    point =>
                                        NavMesh.GetCollisionFlags(point.X, point.Y)
                                            .HasFlag(CollisionFlags.Wall)) >=
                                condemnRectangle.Points.Count * (20 / 100f))
                            {
                                if (CheckTargets.CheckTarget(target, Program.E.Range))
                                {
                                    Program.E.CastOnUnit(target);
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }
    }
}

 

