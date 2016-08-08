using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir_Creator_of_Elo;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Azir_Free_elo_Machine.Math
{
  public  struct Points
{
        public Points(float hits, Vector3 point)
        {
            this.hits = hits;
            this.point = point;
        }

        public float hits;
        public Vector3 point;
}
    static class Geometry
    {
        
        public static IEnumerable<Vector3> PointsAroundTheTarget(Vector3 pos, float dist, float prec = 15, float prec2 = 5)
        {
            if (!pos.LSIsValid())
            {
                return new List<Vector3>();
            }
            var list = new List<Vector3>();
            if (dist > 500)
            {
                prec = 20;
                prec2 = 6;
            }
            if (dist > 805)
            {
                prec = 35;
                prec2 = 8;
            }
            var angle = 360 / prec * System.Math.PI / 180.0f;
            var step = dist / prec2;
            for (var i = 0; i < prec; i++)
            {
                for (var j = 0; j < prec2; j++)
                {
                    list.Add(
                        new Vector3(
                            pos.X + (float)(System.Math.Cos(angle * i) * (j * step)),
                            pos.Y + (float)(System.Math.Sin(angle * i) * (j * step)), pos.Z));
                }
            }

            return list;
        }

        public static float Nattacks(AzirMain azir,Vector3 point,AIHeroClient target)
        {
            const float azirSoldierRange = 315;
            var attacksS=0f;
            foreach (var m in azir.SoldierManager.Soldiers)
            {
                if (m.IsDead) continue;
                var spaceToDoQ = m.ServerPosition.LSDistance(point);
                var timeToDoIt = (spaceToDoQ/azir.Spells.Q.Speed);
                var posFinalTarget = Prediction.GetPrediction(target, timeToDoIt);
                var space = azirSoldierRange - posFinalTarget.UnitPosition.LSDistance(point);

                var time = space/target.MoveSpeed;
                attacksS +=  (time/azir.Hero.AttackDelay);
            }
            return attacksS;
        }
    }
}

