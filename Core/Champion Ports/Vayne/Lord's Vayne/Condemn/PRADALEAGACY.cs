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
    class PRADALEAGACY
    {
        public static void Run()
        {
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            var pP = Program.Player.ServerPosition;
            var p = target.ServerPosition;
            var pD = Program.emenu.Item("PushDistance").GetValue<Slider>().Value;
            // var mode = Vayne.emenu.Item("EMode", true).GetValue<StringList>().SelectedIndex;
            var prediction = Program.E.GetPrediction(target);
            for (var i = 15; i < pD; i += 75)
            {
                var posCF = NavMesh.GetCollisionFlags(
                    prediction.UnitPosition.To2D()
                        .Extend(
                            pP.To2D(),
                            -i)
                        .To3D());
                if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                {

                    Program.E.CastOnUnit(target);

                }
            }
        }
    }
}
