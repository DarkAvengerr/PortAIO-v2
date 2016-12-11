using EloBuddy; 
using LeagueSharp.Common; 
namespace myCommon
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class DamageCalculate
    {
        public static float GetComboDamage(Obj_AI_Base target)
        {
            if (target == null || target.IsDead || target.IsZombie)
            {
                return 0;
            }

            var damage = 0d;

            damage += GetQDamage(target);
            damage += GetWDamage(target);
            damage += GetEDamage(target);
            damage += GetRDamage(target);

            var targetDagger =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        x =>
                            x.BaseSkinName == "testcuberender" && x.Health > 1 && x.IsValid &&
                            x.Distance(target) <= 340);

            if (targetDagger.Any())
            {
                damage += GetPassiveDamage(target)*targetDagger.Count();
            }

            if (ObjectManager.Player.HasBuff("SummonerExhaust"))
            {
                damage = damage * 0.6f;
            }

            if (target.BaseSkinName == "Moredkaiser")
            {
                damage -= target.Mana;
            }

            if (target.HasBuff("GarenW"))
            {
                damage = damage * 0.7f;
            }

            if (target.HasBuff("ferocioushowl"))
            {
                damage = damage * 0.7f;
            }

            if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
            {
                damage -= target.Mana / 2f;
            }

            return (float)damage;
        }
        //Katarina Damage Code Copy From BadaoKingdom
        public static float GetPassiveDamage(Obj_AI_Base target)
        {
            var hant = ObjectManager.Player.Level < 6
                ? 0
                : (ObjectManager.Player.Level < 11
                    ? 1
                    : (ObjectManager.Player.Level < 16 ? 2 : 3));
            var damage = new double[]
                             {75, 78, 83, 88, 95, 103, 112, 122, 133, 145, 159, 173, 189, 206, 224, 243, 264, 245}[
                             ObjectManager.Player.Level - 1]
                         + ObjectManager.Player.FlatPhysicalDamageMod
                         + new[] {0.55, 0.70, 0.85, 1}[hant]*ObjectManager.Player.TotalMagicalDamage;

            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, damage);
        }

        public static float GetQDamage(Obj_AI_Base target, int stage = 0, bool newDamageLogic = false,
            float newDamage = 0f)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady())
            {
                return 0f;
            }

            var damage =
                new double[] { 75, 105, 135, 165, 195 }[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level - 1
                ] + 0.3 * ObjectManager.Player.TotalMagicalDamage;

            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, damage);
        }

        public static float GetWDamage(Obj_AI_Base target, int stage = 0, bool newDamageLogic = false,
            float newDamage = 0f)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).IsReady())
            {
                return 0f;
            }

            if (newDamageLogic)
            {
                return newDamage;
            }

            return 0f;
        }

        public static float GetEDamage(Obj_AI_Base target, int stage = 0, bool newDamageLogic = false,
            float newDamage = 0f)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).IsReady())
            {
                return 0f;
            }

            var damage =
                new double[] {30, 45, 60, 75, 90}[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1] +
                0.25*ObjectManager.Player.TotalMagicalDamage + 0.65*ObjectManager.Player.TotalAttackDamage;

            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, damage);
        }

        public static float GetRDamage(Obj_AI_Base target, int stage = 0, bool newDamageLogic = false,
            float newDamage = 0f)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).IsReady())
            {
                return 0f;
            }

            var damage = new[] {375, 562.5, 750}[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                         2.85*ObjectManager.Player.TotalMagicalDamage + 3.30*ObjectManager.Player.TotalAttackDamage;

            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, damage);
        }

        public static float GetIgniteDmage(Obj_AI_Base target)
        {
            if (ObjectManager.Player.GetSpellSlot("SummonerDot") == SpellSlot.Unknown ||
                !ObjectManager.Player.GetSpellSlot("SummonerDot").IsReady())
            {
                return 0f;
            }

            return 50 + 20*ObjectManager.Player.Level - target.HPRegenRate/5*3;
        }
    }
}
