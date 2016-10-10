using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using BadaoMissFortune;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoMissFortune
{
    class BadaoMissFortuneHelper
    {
        // can use skill
        public static bool UseQ1Combo()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoMissFortuneVariables.ComboQ1.GetValue<bool>();
        }
        public static bool UseQ2Combo()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoMissFortuneVariables.ComboQ2.GetValue<bool>();
        }
        public static bool UseWCombo()
        {
            return BadaoMainVariables.W.IsReady() && BadaoMissFortuneVariables.ComboW.GetValue<bool>();
        }
        public static bool UseECombo()
        {
            return BadaoMainVariables.E.IsReady() && BadaoMissFortuneVariables.ComboE.GetValue<bool>();
        }
        public static bool UseRCombo()
        {
            return BadaoMainVariables.R.IsReady() && BadaoMissFortuneVariables.ComboR.GetValue<bool>();
        }
        public static bool UseRComboWise()
        {
            return BadaoMainVariables.R.IsReady() && BadaoMissFortuneVariables.ComboRWise.GetValue<bool>();
        }
        public static bool UseQ1Harass()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoMissFortuneVariables.HarassQ1.GetValue<bool>();
        }
        public static bool UseQ2Harass()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoMissFortuneVariables.HarassQ2.GetValue<bool>();
        }
        public static bool UseEHarass()
        {
            return BadaoMainVariables.E.IsReady() && BadaoMissFortuneVariables.HarassE.GetValue<bool>();
        }
        public static bool UseQLaneClear()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoMissFortuneVariables.LaneClearQ.GetValue<bool>();
        }
        public static bool UseWLaneClear()
        {
            return BadaoMainVariables.W.IsReady() && BadaoMissFortuneVariables.LaneClearW.GetValue<bool>();
        }
        public static bool UseELaneClear()
        {
            return BadaoMainVariables.E.IsReady() && BadaoMissFortuneVariables.LaneClearE.GetValue<bool>();
        }
        public static bool UseQJungleClear()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoMissFortuneVariables.JungleClearQ.GetValue<bool>();
        }
        public static bool UseWJungleClear()
        {
            return BadaoMainVariables.W.IsReady() && BadaoMissFortuneVariables.JungleClearW.GetValue<bool>();
        }
        public static bool UseEJungleClear()
        {
            return BadaoMainVariables.E.IsReady() && BadaoMissFortuneVariables.JungleClearE.GetValue<bool>();
        }
        public static bool UseQ2Auto(AIHeroClient target)
        {
            return BadaoMainVariables.Q.IsReady()
                && BadaoMissFortuneConfig.config.SubMenu("Auto").Item("AutoQ2" + target.NetworkId).GetValue<bool>();
        }
        public static bool CanHarassMana()
        {
            return ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100 >= BadaoMissFortuneVariables.HarassMana.GetValue<Slider>().Value;
        }
        public static bool CanLaneClearMana()
        {
            return ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100 >= BadaoMissFortuneVariables.LaneClearMana.GetValue<Slider>().Value;
        }
        public static bool CanJungleClearMana()
        {
            return ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100 >= BadaoMissFortuneVariables.JungleClearMana.GetValue<Slider>().Value;
        }
        public static bool CanAutoMana()
        {
            return ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100 >= BadaoMissFortuneVariables.AutoMana.GetValue<Slider>().Value;
        }
        // damage caculation
        public static float GetAADamage(AIHeroClient target)
        {
            if (BadaoMissFortuneVariables.TapTarget.BadaoIsValidTarget() && target.BadaoIsValidTarget() &&
                target.NetworkId == BadaoMissFortuneVariables.TapTarget.NetworkId)
                return (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                                 ObjectManager.Player.TotalAttackDamage);
            else
                return (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                                     ObjectManager.Player.TotalAttackDamage)
                       + (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                         (new double[] { 0.6, 0.6, 0.6, 0.7, 0.7, 0.7, 0.8, 0.8, 0.9, 0.9, 1 }
                         [ObjectManager.Player.Level > 11 ? 10 : ObjectManager.Player.Level - 1]
                         * ObjectManager.Player.TotalAttackDamage));
        }
        public static float Q1Damage(Obj_AI_Base target)
        {
            if (target.BadaoIsValidTarget())
            {
                if (BadaoMissFortuneVariables.TapTarget.BadaoIsValidTarget() &&
                    target.NetworkId == BadaoMissFortuneVariables.TapTarget.NetworkId)
                {
                    return BadaoMainVariables.Q.GetDamage(target);
                }
                if (target is Obj_AI_Minion)
                {
                    return
                        BadaoMainVariables.Q.GetDamage(target)
                        + (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                        (new double[] { 0.6, 0.6, 0.6, 0.7, 0.7, 0.7, 0.8, 0.8, 0.9, 0.9, 1 }
                        [ObjectManager.Player.Level > 11 ? 10 : ObjectManager.Player.Level - 1]
                        * ObjectManager.Player.TotalAttackDamage * 0.5f));
                }
                if (target is AIHeroClient)
                {
                    return
                        BadaoMainVariables.Q.GetDamage(target)
                        + (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                        (new double[] { 0.6, 0.6, 0.6, 0.7, 0.7, 0.7, 0.8, 0.8, 0.9, 0.9, 1 }
                        [ObjectManager.Player.Level > 11 ? 10 : ObjectManager.Player.Level - 1]
                        * ObjectManager.Player.TotalAttackDamage));
                }
                return BadaoMainVariables.Q.GetDamage(target);
            }
            return 0;
        }
        public static float Q2Damage(Obj_AI_Base target, bool dead = false)
        {
            if (!dead)
            {
                if (target is Obj_AI_Minion)
                {
                    return
                        BadaoMainVariables.Q.GetDamage(target,1)
                        + (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                        (new double[] { 0.6, 0.6, 0.6, 0.7, 0.7, 0.7, 0.8, 0.8, 0.9, 0.9, 1 }
                        [ObjectManager.Player.Level > 11 ? 10 : ObjectManager.Player.Level - 1]
                        * ObjectManager.Player.TotalAttackDamage * 0.5f));
                }
                if (target is AIHeroClient)
                {
                    return
                        BadaoMainVariables.Q.GetDamage(target,1)
                        + (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                        (new double[] { 0.6, 0.6, 0.6, 0.7, 0.7, 0.7, 0.8, 0.8, 0.9, 0.9, 1 }
                        [ObjectManager.Player.Level > 11 ? 10 : ObjectManager.Player.Level - 1]
                        * ObjectManager.Player.TotalAttackDamage));
                }
            }
            if (dead)
            {
                if (target is Obj_AI_Minion)
                {
                    return
                        BadaoMainVariables.Q.GetDamage(target, 1) * 0.5f
                        + (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                        (new double[] { 0.6, 0.6, 0.6, 0.7, 0.7, 0.7, 0.8, 0.8, 0.9, 0.9, 1 }
                        [ObjectManager.Player.Level > 11 ? 10 : ObjectManager.Player.Level - 1]
                        * ObjectManager.Player.TotalAttackDamage * 0.5f));
                }
                if (target is AIHeroClient)
                {
                    return
                        BadaoMainVariables.Q.GetDamage(target, 1) * 0.5f
                        + (float)Damage.CalcDamage(ObjectManager.Player, target, Damage.DamageType.Physical,
                        (new double[] { 0.6, 0.6, 0.6, 0.7, 0.7, 0.7, 0.8, 0.8, 0.9, 0.9, 1 }
                        [ObjectManager.Player.Level > 11 ? 10 : ObjectManager.Player.Level - 1]
                        * ObjectManager.Player.TotalAttackDamage));
                }
            }
            return BadaoMainVariables.Q.GetDamage(target, 1);
        }
        public static float RDamage(Obj_AI_Base target)
        {
            return (float)(new double[] { 12, 14, 16 }[BadaoMainVariables.R.Instance.Level - 1] * BadaoMainVariables.R.GetDamage(target)
                                  * (1 + ObjectManager.Player.Crit * 0.2));
        }
        public static void RPrediction(Vector2 CastPos, Obj_AI_Base TargetToCheck, out Vector2 CenterPolar, out Vector2 CenterEnd, out Vector2 x1, out Vector2 x2)
        {
            //changeable
            float goc = 36f;
            //process
            float goc1rad = (float)Math.PI * (90f - goc / 2f) / 180f;
            float backward = TargetToCheck.BoundingRadius / (float)Math.Cos(goc1rad);
            CenterPolar = ObjectManager.Player.Position.To2D().Extend(CastPos, -backward);
            CenterEnd = ObjectManager.Player.Position.To2D().Extend(CastPos, 1400);
            Vector2 Rangestraight = ObjectManager.Player.Position.To2D().Extend(CastPos, ObjectManager.Player.BoundingRadius
                                                                                + ObjectManager.Player.AttackRange + TargetToCheck.BoundingRadius);
            float goc2rad = (float)Math.PI * (goc / 2f + 90f) / 180f - (float)Math.Acos(TargetToCheck.BoundingRadius /
                (ObjectManager.Player.BoundingRadius + ObjectManager.Player.AttackRange + TargetToCheck.BoundingRadius));
            x1 = BadaoChecker.BadaoRotateAround(Rangestraight, ObjectManager.Player.Position.To2D(), goc2rad);
            x2 = BadaoChecker.BadaoRotateAround(Rangestraight, ObjectManager.Player.Position.To2D(), -goc2rad);
        }
        //compare damage 
        public static bool Rdamepior()
        {
            float Rdame = (float)(new double[] { 12, 14, 16 }[BadaoMainVariables.R.Instance.Level - 1] * BadaoMainVariables.R.GetDamage(ObjectManager.Player)
                                  * (1 + ObjectManager.Player.Crit * 0.2));
            float Playerdame = (float)Damage.CalcDamage(ObjectManager.Player, ObjectManager.Player, Damage.DamageType.Physical,
                                                 ObjectManager.Player.TotalAttackDamage * 3 / ObjectManager.Player.AttackDelay)
                                                 * (1 + ObjectManager.Player.Crit)
                               + (BadaoMainVariables.Q.IsReady() ? BadaoMainVariables.Q.GetDamage(ObjectManager.Player) : 0);
            return Rdame > Playerdame;
        }
        // prediction
        public static PredictionOutput PredQBase(Obj_AI_Base unit)
        {
            var Qpred = BadaoMainVariables.Q.GetPrediction(unit);
            return Prediction.GetPrediction(unit, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                            1400 + Game.Ping / 1000));
        }
    }
}
