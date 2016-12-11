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
    class Shine
    {

        public static void Run()
        {
            foreach (var target in HeroManager.Enemies.Where(h => h.IsValidTarget(Program.E.Range)))
            {

                var pushDistance = Program.emenu.Item("PushDistance").GetValue<Slider>().Value;
                var targetPosition = Program.E.GetPrediction(target).UnitPosition;
                var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).Normalized();
                float checkDistance = pushDistance / 40f;
                for (int i = 0; i < 40; i++)
                {
                    Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                    var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                    if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building)) //not sure about building, I think its turrets, nexus etc
                    {
                        Program.E.CastOnUnit(target);
                    }
                }
            }
        }
    }
}

