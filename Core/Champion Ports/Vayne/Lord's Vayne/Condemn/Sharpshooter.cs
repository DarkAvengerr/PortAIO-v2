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
    class Sharpshooter
    {
        public static void Run()
        {
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            var prediction = Program.E.GetPrediction(target);

            if (prediction.Hitchance >= HitChance.High)
            {
                var finalPosition = prediction.UnitPosition.Extend(ObjectManager.Player.Position, -400);

                if (finalPosition.IsWall())
                {
                    Program.E.CastOnUnit(target);
                    return;
                }

                for (var i = 1; i < 400; i += 50)
                {
                    var loc3 = prediction.UnitPosition.Extend(ObjectManager.Player.Position, -i);

                    if (loc3.IsWall())
                    {
                        if (CheckTargets.CheckTarget(target, Program.E.Range))
                        {
                            Program.E.CastOnUnit(target);
                            return;
                        }
                    }
                }
            }
        }
    }
}
