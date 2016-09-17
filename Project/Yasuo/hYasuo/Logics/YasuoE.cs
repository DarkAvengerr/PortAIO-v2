using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hYasuo.Extensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = hYasuo.Extensions.Utilities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hYasuo.Logics
{
    internal static class YasuoE
    {

        public static bool IsDashable(Obj_AI_Base target)
        {
            return ObjectManager.Player.Distance(target.Position) < Spells.E.Range && 
                !target.HasBuff("YasuoDashWrapper");
        }

        public static void DashPos(Vector3 targetLoc, List<Obj_AI_Base> list, bool turrets)
        {
            Obj_AI_Base[] eMinion = { null };

            foreach (var o in list)
            {
                if (ObjectManager.Player.Distance(o) < Spells.E.Range && ObjectManager.Player.ServerPosition.Distance(o.ServerPosition) > 45)
                {
                    Vector3 ePos = GetDashingEnd(o).To3D();
                    if (targetLoc.Distance(ePos) < (Math.Abs(ObjectManager.Player.Distance(targetLoc) - (ObjectManager.Player.MoveSpeed * 0.4))) && (!turrets || !ePos.UnderTurret(true)))
                    {
                        if (eMinion[0] == null ||
                            targetLoc.Distance(ePos) < targetLoc.Distance(GetDashingEnd(eMinion[0]).To3D()))
                        {
                            eMinion[0] = o;
                        }

                    }
                }
            }

            if (eMinion[0] != null)
            {
                Spells.E.Cast(eMinion[0]);
            }
        }

        public static Vector2 GetDashingEnd(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return Vector2.Zero;
            }

            var baseX = ObjectManager.Player.Position.X;
            var baseY = ObjectManager.Player.Position.Y;
            var targetX = target.Position.X;
            var targetY = target.Position.Y;

            var vector = new Vector2(targetX - baseX, targetY - baseY);
            var sqrt = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

            var x = (float)(baseX + (Spells.E.Range * (vector.X / sqrt)));
            var y = (float)(baseY + (Spells.E.Range * (vector.Y / sqrt)));

            return new Vector2(x, y);
        }
    }
}
