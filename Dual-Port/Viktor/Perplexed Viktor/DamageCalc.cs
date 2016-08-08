using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PerplexedViktor
{
    static class DamageCalc
    {
        static AIHeroClient Player = ObjectManager.Player;

        public static float GetQDamage(Obj_AI_Base target)
        {
            float damage = 0f;
            damage += (float) Player.LSGetSpellDamage(target, SpellSlot.Q);
            if (SpellManager.HasQBuff)
            {
                var afterQDmgs = new float[] { 20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210 };
                var afterQDmg = afterQDmgs[Player.Level - 1];
                damage = afterQDmg;
                damage += (float) (Player.TotalMagicalDamage * 0.5) + Player.TotalAttackDamage;
                damage = (float) Player.CalcDamage(target, Damage.DamageType.Magical, (double)damage);
            }
            return damage;
        }

        public static float GetEDamage(Obj_AI_Base target)
        {
            return (float) Player.LSGetSpellDamage(target, SpellSlot.E);
        }

        public static float GetRemainingUltDamage(Obj_AI_Base target)
        {
            float damage = 0f;
            int[] ultTickDamage = { 15, 30, 45 };
            if (SpellManager.UltHasBeenCasted)
            {
                int ultGoingFor = (int)(Game.Time - SpellManager.UltCastedTime);
                int ticksLeft = ultGoingFor < 7000 ? ((7000 - ultGoingFor) / 1000) * 2 : 0;
                if (ticksLeft == 0)
                    return (float)damage;
                int tickDamage = ultTickDamage[SpellManager.R.Level - 1] + (int)(Player.TotalMagicalDamage * 0.1);
                int totalTickDamage = tickDamage * ticksLeft;
                damage = totalTickDamage;
                damage = (float) Player.CalcDamage(target, Damage.DamageType.Magical, (double)damage);
            }
            return (float)damage;
        }

        public static bool RemainingUltCanKill(Obj_AI_Base target)
        {
            return target.Health < GetRemainingUltDamage(target);
        }

        public static float GetUltInitialDamage(Obj_AI_Base target)
        {
            return (float) Player.LSGetSpellDamage(target, SpellManager.R.Slot);
        }

        public static float GetUltTickDamage(Obj_AI_Base target)
        {
            float damage = 0f;
            int[] ultTickDamage = { 15, 30, 45 };
            int tickDamage = ultTickDamage[SpellManager.R.Level - 1] + (int)(Player.TotalMagicalDamage * 0.1);
            int totalTickDamage = tickDamage * 14;
            damage = totalTickDamage;
            damage = (float) Player.CalcDamage(target, Damage.DamageType.Magical, (double)damage);
            return (float)damage;
        }

        public static float GetTotalDamage(Obj_AI_Base target)
        {
            float damage = 0f;
            if (SpellManager.Q.LSIsReady() && Config.ComboQ)
                damage += GetQDamage(target);
            if (SpellManager.E.LSIsReady() && Config.ComboE)
                damage += GetEDamage(target);
            if (SpellManager.R.LSIsReady() && Config.ComboR)
            {
                if (SpellManager.UltHasBeenCasted)
                    damage += GetRemainingUltDamage(target);
                else
                    damage += GetUltInitialDamage(target) + GetUltTickDamage(target);
            }
            if (SpellManager.IgniteSlot != SpellSlot.Unknown && Player.GetSpell(SpellManager.IgniteSlot).LSIsReady())
                damage += (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            return damage;
        }
    }
}
