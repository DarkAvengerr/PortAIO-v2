using EloBuddy; 
using LeagueSharp.Common; 
namespace FlowersDariusCommon
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

            damage += ObjectManager.Player.GetAutoAttackDamage(target, true);

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

        public static float GetPassiveCount(Obj_AI_Base target)
        {
            return target.GetBuffCount("DariusHemo") > 0 ? target.GetBuffCount("DariusHemo") : 0;
        }

        public static float GetQDamage(Obj_AI_Base target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady())
            {
                return 0f;
            }

            var damage =
                new double[] {40, 70, 100, 130, 160}[ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level - 1] +
                1.05*ObjectManager.Player.TotalAttackDamage;

            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, damage);
        }

        public static float GetWDamage(Obj_AI_Base target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).IsReady())
            {
                return 0f;
            }

            return (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
        }

        public static float GetEDamage(Obj_AI_Base target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).IsReady())
            {
                return 0f;
            }

            return 0f;
        }

        public static float GetRDamage(Obj_AI_Base target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).IsReady())
            {
                return 0f;
            }

            var Damage = (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);

            // rengenrate
            Damage -= target.HPRegenRate;

            // passive
            if (target.HasBuff("DariusHemo"))
            {
                Damage += Damage * GetPassiveCount(target) * 0.2f;
            }

            if (target.BaseSkinName == "Moredkaiser")
            {
                Damage -= target.Mana;
            }

            // exhaust
            if (ObjectManager.Player.HasBuff("SummonerExhaust"))
            {
                Damage = Damage * 0.6f;
            }

            // blitzcrank passive
            if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
            {
                Damage -= target.Mana / 2f;
            }

            // kindred r
            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                Damage = 0;
            }

            // tryndamere r
            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
            {
                Damage = 0;
            }

            // kayle r
            if (target.HasBuff("JudicatorIntervention"))
            {
                Damage = 0;
            }

            // zilean r
            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
            {
                Damage = 0;
            }

            // fiora w
            if (target.HasBuff("FioraW"))
            {
                Damage = 0;
            }

            return Damage;
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
