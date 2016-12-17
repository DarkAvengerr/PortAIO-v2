using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hCamille.Extensions
{
    static class Calculation
    {
        private static readonly int[] ProtocolBonusPhysicalDamage = new[] { 20, 25, 30, 35, 40 }; // 20 / 25 / 30 / 35 / 40% AD
        private static readonly int[] ProtocolMixedDamage = new[] {40, 50, 60, 70, 80}; // 40 / 50 / 60 / 70 / 80% AD

        //TRUEDMG FORUMULA => 36% + (4% x Level)
        private static readonly int[] ProtocolTrueDamage = new[]
            {40, 44, 48, 52, 56, 60, 64, 68, 72, 76, 80, 84, 88, 92, 96, 100};

        private static readonly int[] TacticalDamages = new[] { 65, 95, 125, 155, 185 }; // 65 / 95 / 125 / 155 / 185 (+ 60% bonus AD)
        private static readonly int[] WallDiveDamages = new[] {70, 115, 160, 205, 250}; //  70 / 115 / 160 / 205 / 250 (+ 75% bonus AD)
        private static readonly int[] HextechDamages = new[] {5, 10, 15};
        private static readonly int[] HextechHealthScale = new[] {4, 6, 8};// (+ 4 / 6 / 8% of target's current health)

        public static string ProtocolStageOneBuffName => "camilleqprimingstart";
        public static string ProtocolStageTwoBuffName => "camilleqprimingcomplete";
        public static bool HasProtocolOneBuff => ObjectManager.Player.HasBuff(ProtocolStageOneBuffName);
        public static bool HasProtocolTwoBuff => ObjectManager.Player.HasBuff(ProtocolStageTwoBuffName);
        public static bool IsTrueProtocol => Spells.Q.Instance.Name != "CamilleQ";


        public static float ProtocolDamage(this Obj_AI_Base unit)
        {

            if (ObjectManager.Player.HasBuff(ProtocolStageOneBuffName))
            {
                var protocolonedmg = ProtocolBonusPhysicalDamage[Spells.Q.Level - 1] +
                                   0.40 * (ObjectManager.Player.BaseAttackDamage +
                                   ObjectManager.Player.FlatPhysicalDamageMod);
                return (float)ObjectManager.Player.CalcDamage(unit, Damage.DamageType.Physical, protocolonedmg);
            }
            return 0x0;

        }

        public static float ProtocolTwoDamage(this Obj_AI_Base unit)
        {
            if (ObjectManager.Player.HasBuff(ProtocolStageTwoBuffName))
            {
                var protocoltwodamage = ProtocolMixedDamage[Spells.Q.Level - 1] + 0.80 * (ObjectManager.Player.BaseAttackDamage +
                                   ObjectManager.Player.FlatPhysicalDamageMod);
                return (float)ObjectManager.Player.CalcDamage(unit, Damage.DamageType.Physical, protocoltwodamage);

            }
            else if (IsTrueProtocol)
            {
                if (ObjectManager.Player.HasBuff(ProtocolStageTwoBuffName))
                {
                    var protocoltwodamage = ProtocolMixedDamage[Spells.Q.Level - 1] + 0.80 * (ObjectManager.Player.BaseAttackDamage +
                                   ObjectManager.Player.FlatPhysicalDamageMod);

                    var truedamage = (protocoltwodamage / 100 * 36) +
                                     (protocoltwodamage / 100 * 4) * ProtocolTrueDamage[ObjectManager.Player.Level];

                    return (float)ObjectManager.Player.CalcDamage(unit, Damage.DamageType.True, truedamage);

                }
            }
            return 0x0;

        }

        public static float TacticalDamage(this Obj_AI_Base unit)
        {
            var tacticaldmg = TacticalDamages[Spells.W.Level - 1] + 0.60 * (ObjectManager.Player.BaseAttackDamage +
                                       ObjectManager.Player.FlatPhysicalDamageMod);
            return (float)ObjectManager.Player.CalcDamage(unit, Damage.DamageType.Physical, tacticaldmg);

        }

        public static float WallDiveDamage(this Obj_AI_Base unit)
        {
            var walldivedmg = WallDiveDamages[Spells.E.Level - 1] + 0.75 * (ObjectManager.Player.BaseAttackDamage +
                                       ObjectManager.Player.FlatPhysicalDamageMod);
            return (float)ObjectManager.Player.CalcDamage(unit, Damage.DamageType.Physical, walldivedmg);
        }

        public static float HextechDamage(this Obj_AI_Base unit)
        {
            var hextechdamage = HextechDamages[Spells.R.Level - 1] + (unit.Health / 100 * HextechHealthScale[Spells.R.Level - 1]);
            return (float)ObjectManager.Player.CalcDamage(unit, Damage.DamageType.Physical, hextechdamage);
        }
    }
}
