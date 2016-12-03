using EloBuddy; 
using LeagueSharp.Common; 
namespace myCommon
{
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

        public static float GetQDamage(Obj_AI_Base target, int stage = 0, bool newDamageLogic = false,
            float newDamage = 0f)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady())
            {
                return 0f;
            }

            if (newDamageLogic)
            {
                return newDamage;
            }

            return (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q, stage);
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

            return (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.W, stage);
        }

        public static float GetEDamage(Obj_AI_Base target, int stage = 0, bool newDamageLogic = false,
            float newDamage = 0f)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).IsReady())
            {
                return 0f;
            }

            if (newDamageLogic)
            {
                return newDamage;
            }

            return (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.E, stage);
        }

        public static float GetRDamage(Obj_AI_Base target, int stage = 0, bool newDamageLogic = false,
            float newDamage = 0f)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).IsReady())
            {
                return 0f;
            }

            if (newDamageLogic)
            {
                return newDamage;
            }

            return (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.R, stage);
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
