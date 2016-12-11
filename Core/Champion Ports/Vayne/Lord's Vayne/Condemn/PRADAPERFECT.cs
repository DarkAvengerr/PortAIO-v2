using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using System.Collections.Generic;
using System;
using Lord_s_Vayne.Condemn;
using Lord_s_Vayne.Condemn.Prada;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Condemn
{
    class PRADAPERFECT
    {

        public static void Run()
        {
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            var pP = Program.Player.ServerPosition;
            var p = target.ServerPosition;
            var pD = Program.emenu.Item("PushDistance").GetValue<Slider>().Value;
            //  var mode = Vayne.emenu.Item("EMode", true).GetValue<StringList>().SelectedIndex;
            if (
(p.Extend(pP, -pD).IsCollisionable() || p.Extend(pP, -pD / 2f).IsCollisionable() ||
 p.Extend(pP, -pD / 3f).IsCollisionable()))
            {
                if (!target.CanMove ||
                    (target.Spellbook.IsAutoAttacking))
                {
                    Program.E.CastOnUnit(target);
                }
                var hitchance = Program.emenu.Item("EHitchance").GetValue<Slider>().Value;
                var angle = 0.20 * hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                for (var i = 15; i < pD; i += 100)
                {
                    if (pP.To2D().Extend(alpha,
                            i)
                        .To3D().IsCollisionable() && pP.To2D().Extend(beta, i).To3D().IsCollisionable())
                    {
                        Program.E.CastOnUnit(target);
                    }
                }

            }
        }
        
    }
}
                        
                   
    
