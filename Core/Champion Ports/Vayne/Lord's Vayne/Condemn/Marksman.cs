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
    class Marksman
    {
        public static void Run()
        {
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            for (var i = 1; i < 8; i++)
            {
                var targetBehind = target.Position +
                                   Vector3.Normalize(target.ServerPosition - ObjectManager.Player.Position) * i * 50;

                if (targetBehind.IsWall() && target.IsValidTarget(Program.E.Range))
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
