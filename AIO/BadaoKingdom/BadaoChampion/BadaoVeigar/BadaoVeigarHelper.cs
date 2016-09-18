using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoVeigar
{
    public static class BadaoVeigarHelper
    {
        public static void CastQTarget(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(BadaoMainVariables.Q.Range))
                return;
            var pred = BadaoMainVariables.Q.GetPrediction(target);
            var pred2 = BadaoMainVariables.Q2.GetPrediction(target);
            List<Obj_AI_Base> pred3 = new List<Obj_AI_Base>();
            foreach (var obj in pred.CollisionObjects)
            {
                if (!pred3.Any(a => a.NetworkId == obj.NetworkId))
                    pred3.Add(obj);
            }
            if (pred2.Hitchance >= HitChance.Medium && pred3.Count() <= 1)
                BadaoMainVariables.Q2.Cast(target);
        }
        public static void CastETarget(AIHeroClient target, int extraDistance = 50)
        {
            var pred = Prediction.GetPrediction(target, 1.25f + (float)Game.Ping / 1000f);
            Vector2 UnitPredPos = pred.UnitPosition.To2D();
            if (target.IsChannelingImportantSpell() && pred.UnitPosition.Distance(target.Position) <= 50)
                UnitPredPos = target.Position.To2D();
            else if (pred.UnitPosition.Distance(target.Position) <= 20)
                return;
            else
                UnitPredPos = pred.UnitPosition.To2D().Extend(target.Position.To2D(), 0);
            List<Vector2> extraPoses = new List<Vector2>();
            List<Vector2> CastPoses = new List<Vector2>();
            Vector2 direction = UnitPredPos.Extend(target.Direction.To2D(), extraDistance);
            for (int i = 0; i < 360; i = i + 5)
            {
                extraPoses.Add(BadaoChecker.BadaoRotateAround(direction, UnitPredPos, BadaoChecker.AngleToRadian(i)));
            }
            foreach (var pos in extraPoses)
            {
                CastPoses.Add(pos.Extend(UnitPredPos, 300));
            }
            Vector2 CastPos = CastPoses.Where(x => BadaoMainVariables.E.IsInRange(x)).OrderByDescending(x => x.Distance(target.Position) <= 300)
                .ThenByDescending(x => LeagueSharp.Common.Utility.CountEnemiesInRange(x.To3D(), 300)).FirstOrDefault();
            if (CastPos != null && CastPos.IsValid())
            {
                BadaoMainVariables.E.Cast(CastPos);
            }
        }
        public static double GetQDamage(Obj_AI_Base target)
        {
            //return BadaoMainVariables.Q.GetDamage(target);
            double rawDamage = new double[] { 70, 110, 150, 190, 230 }[BadaoMainVariables.Q.Level - 1] + 0.6 * ObjectManager.Player.TotalMagicalDamage;
            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, rawDamage);

        }
        public static double GetWDamage(Obj_AI_Base target)
        {
            //return BadaoMainVariables.w.GetDamage(target);
            double rawDamage = new double[] { 100, 150, 200, 250, 300 }[BadaoMainVariables.W.Level - 1] +  ObjectManager.Player.TotalMagicalDamage;
            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, rawDamage);
        }
        public static double GetRDamage(Obj_AI_Base target)
        {
            //return BadaoMainVariables.R.GetDamage(target);
            double rawDamage = new double[] { 175, 250, 325 }[BadaoMainVariables.R.Level - 1] + 0.75 *ObjectManager.Player.TotalMagicalDamage;
            double missingpercent = 1 - (target.MaxHealth - target.Health) / target.MaxHealth;
            double multiply = missingpercent >= 2d / 3d ? 1 + 1 : 1 + missingpercent * 1.5d;
            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, rawDamage * multiply);
        }
    }
}
