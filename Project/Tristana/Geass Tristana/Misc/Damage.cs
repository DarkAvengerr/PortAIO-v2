using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Misc
{
    internal class Damage : Core
    {
        
        private float GetComboDamage(Obj_AI_Base target)
        {
            float damage = 0f;

            if (GeassLib.Functions.Calculations.Damage.CheckNoDamageBuffs((AIHeroClient)target))return damage;
            
            //if (!Champion.Player.Spellbook.IsAutoAttacking) // can auto attack
            //    if (Champion.Player.Distance(target) < Champion.Player.AttackRange) // target in auto range
            //        damage += (float)Champion.Player.GetAutoAttackDamage(target) - 50;

            if (Champion.GetSpellR.IsReady())
                if (Champion.Player.Distance(target) < Champion.GetSpellR.Range)
                    damage += Champion.GetSpellR.GetDamage(target);

            if (target.HasBuff("tristanaecharge"))
            {
                int count = target.GetBuffCount("tristanaecharge");
                if (!Champion.Player.Spellbook.IsAutoAttacking)
                    if (Champion.Player.Distance(target) < Champion.Player.AttackRange) // target in auto range
                        count++;

                damage += (float)(Champion.GetSpellE.GetDamage(target) * (count * 0.30)) + Champion.GetSpellE.GetDamage(target);

                return damage;
            }

            if (Champion.GetSpellE.IsReady())
                if (Champion.Player.Distance(target) < Champion.GetSpellE.Range)
                    damage += (float)(Champion.GetSpellE.GetDamage(target) * 0.30) + Champion.GetSpellE.GetDamage(target); // 1 auto charge

            return damage;
        }
        public float CalcDamage(Obj_AI_Base target)
        {
            return GeassLib.Functions.Calculations.Damage.CalcRealDamage(target, GetComboDamage(target));
        }
    }
}