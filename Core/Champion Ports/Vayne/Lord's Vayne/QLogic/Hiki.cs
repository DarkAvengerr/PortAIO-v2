using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.QLogic
{
    class Hiki
    {
        public static void SafePositionQ(AIHeroClient enemy)
        {
            var range = Orbwalking.GetRealAutoAttackRange(enemy);
            var path = LeagueSharp.Common.Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                Prediction.GetPrediction(enemy, 0.25f).UnitPosition.To2D(), Program.Q.Range, range);

            if (path.Count() > 0)
            {
                var epos = path.MinOrDefault(x => x.Distance(Game.CursorPos));
                if (epos.To3D().UnderTurret(true) || epos.To3D().IsWall())
                {
                    return;
                }

                if (epos.To3D().CountEnemiesInRange(Program.Q.Range - 100) > 0)
                {
                    return;
                }
                Program.Q.Cast(epos);
            }
            if (path.Count() == 0)
            {
                var epos = ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -Program.Q.Range);
                if (epos.UnderTurret(true) || epos.IsWall())
                {
                    return;
                }

                // no intersection or target to close
                Program.Q.Cast(ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -Program.Q.Range));
            }
        }
    }
}
