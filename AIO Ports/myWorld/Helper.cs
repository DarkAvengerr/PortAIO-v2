using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld
{
    static class Helper
    {
        static Dictionary<HitChance, int> HitChanceToNum = new Dictionary<HitChance, int>()
        {
            {HitChance.Collision, -1},
            {HitChance.OutOfRange, -1},
            {HitChance.Impossible, -1},
            {HitChance.Low, 1},
            {HitChance.Medium, 2},
            {HitChance.High, 3},
            {HitChance.VeryHigh, 4},
            {HitChance.Immobile, 5},
            {HitChance.Dashing, 5},
        };
        public static bool IsManaLow(this AIHeroClient unit, float percent)
        {
            return unit.ManaPercent < percent;
        }

        public static bool IsHPLow(this AIHeroClient unit, float percent)
        {
            return unit.HealthPercent < percent;
        }

        public static int GetBuffCount(this Obj_AI_Base unit, string buffName)
        {
            foreach(BuffInstance buff in unit.Buffs.Where(b => b.Name == buffName))
            {
                if (buff.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return buff.Count;
                }
            }
            return 0;
        }
        public static float GetDistance(this Obj_AI_Base unit)
        {
            return ObjectManager.Player.Position.Distance(unit.Position);
        }

        public static float GetDistance(this Obj_AI_Base unit, Obj_AI_Base target)
        {
            return unit.Position.Distance(target.Position);
        }

        public static HitChance ToHitChance(this string input)
        {
            foreach (HitChance value in Enum.GetValues(typeof(HitChance)))
            {
                if (value.ToString() == input)
                {
                    return value;
                }
            }
            return HitChance.VeryHigh;
        }

        public static void CastIfHigherThen(this Spell input, Obj_AI_Base target, HitChance hit)
        {
            PredictionOutput output = input.GetPrediction(target);
            if(HitChanceToNum[hit] <= HitChanceToNum[output.Hitchance])
            {
                input.Cast(output.CastPosition);
            }
        }

        public static bool IsHigerThen(this HitChance input, HitChance campairHitCance)
        {
            if (HitChanceToNum[input] <= HitChanceToNum[campairHitCance])
            {
                return true;
            }
            return false;
        }

        public static Vector3 Perpendicular(this Vector3 self)
        {
            return new Vector3(-self.Z, self.Y, self.X);
        }

        public static Vector3 Perpendicular2(this Vector3 self)
        {
            return new Vector3(self.Z, self.Y, -self.X);
        }

        public static bool IsUnderTurret(this Vector3 Point)
        {
            Obj_AI_Turret EnemyTurrets = ObjectManager.Get<Obj_AI_Turret>().Find(t => t.IsEnemy && Vector3.Distance(t.Position, Point) < 950f);
            return EnemyTurrets != null;
        }
    }
}
