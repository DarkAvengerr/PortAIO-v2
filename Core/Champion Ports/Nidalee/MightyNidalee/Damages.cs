using System;
using System.Linq;

using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MightyNidalee
{
    class Damages : Mighty
    {


        public static double Human_Q_Damage(Obj_AI_Base target)
        {
            if (Mighty.Q.Level == 0) return 0;

            var raw = new double[] { 50, 70, 90, 110, 130 }[Mighty.Q.Level - 1]
                                + 0.4 * ObjectManager.Player.FlatMagicDamageMod;

            return R_Manager.QhumanReady ? (ObjectManager.Player.Distance(target.Position) < 525 ?
                ObjectManager.Player.CalcDamage(target as AIHeroClient, Damage.DamageType.Magical, (float)(raw)) :
                (ObjectManager.Player.Distance(target.Position) >= 1300 ?
                ObjectManager.Player.CalcDamage(target as AIHeroClient, Damage.DamageType.Magical, (float)(raw)) * 3 :
                ObjectManager.Player.CalcDamage(target as AIHeroClient, Damage.DamageType.Magical, (float)(raw))
                * (1 + (ObjectManager.Player.Distance(target.Position) - 525) / (1300 - 525) * 2))) : 0;
        }

        public static double Cougar_Q_Damage(Obj_AI_Base target)
        {
            if (Mighty.Q.Level == 0) return 0;

            var dmg = ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical,
                    new[] { 4, 20, 50, 90 }[Mighty.R.Level - 1] + (float)(0.36 * ObjectManager.Player.FlatMagicDamageMod)
                                     + (float)(0.75 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)))
                                    * ((target.MaxHealth - target.Health) / target.MaxHealth * 1.5 + 1);

            if (R_Manager.IsHunted(target))
                dmg += dmg * 0.33;

            return dmg;
        }

        public static double Cougar_W_Damage(Obj_AI_Base target)
        {
            if (Mighty.W.Level == 0) return 0;

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical,
                    new[] { 50, 100, 150, 200 }[Mighty.R.Level - 1] + (float)(0.3 * ObjectManager.Player.FlatMagicDamageMod));
        }

        public static double Cougar_E_Damage(Obj_AI_Base target)
        {
            if (Mighty.E.Level == 0) return 0;

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical,
                    new[] { 70, 130, 190, 250 }[Mighty.R.Level - 1] + (float)(0.45 * ObjectManager.Player.FlatMagicDamageMod));
        }
    }

}
