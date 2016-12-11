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
    class Gosu
    {
        public static void Run()
        {
            foreach (var hero in from hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValidTarget(550f))
                                 let prediction = Program.E.GetPrediction(hero)
                                 where NavMesh.GetCollisionFlags(
                                     prediction.UnitPosition.To2D()
                                         .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                             -Program.emenu.Item("PushDistance").GetValue<Slider>().Value)
                                         .To3D())
                                     .HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(
                                         prediction.UnitPosition.To2D()
                                             .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                                 -(Program.emenu.Item("PushDistance").GetValue<Slider>().Value / 2))
                                             .To3D())
                                         .HasFlag(CollisionFlags.Wall)
                                 select hero)
            {

                Program.E.CastOnUnit(hero);
            }
        }
}
}
