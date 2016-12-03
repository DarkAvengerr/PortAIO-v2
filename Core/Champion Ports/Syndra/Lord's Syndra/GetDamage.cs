using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LordsSyndra
{
    public static class GetDamage
    {
        public static float GetComboDamage(Obj_AI_Base enemy, bool UseQ, bool UseW, bool UseE, bool UseR)
        {
            if (enemy == null)
                return 0f;
            var damage = 0d;
            var combomana = 0d;
            var useR = Program.Menu.Item("DontR" + enemy.BaseSkinName) != null &&
                       Program.Menu.Item("DontR" + enemy.BaseSkinName).GetValue<bool>() == false;

            //Add R Damage
            if (Spells.R.IsReady() && UseR && useR)
            {
                combomana += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
                if (combomana <= ObjectManager.Player.Mana) damage += GetRDamage(enemy);
                else combomana -= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
            }

            //Add Q Damage
            if (Spells.Q.IsReady() && UseQ)
            {
                combomana += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
                if (combomana <= ObjectManager.Player.Mana) damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q);
                else combomana -= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
            }

            //Add E Damage
            if (Spells.E.IsReady() && UseE)
            {
                combomana += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
                if (combomana <= ObjectManager.Player.Mana) damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.E);
                else combomana -= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
            }

            //Add W Damage
            if (Spells.W.IsReady() && UseW)
            {
                combomana += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
                if (combomana <= ObjectManager.Player.Mana) damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.W);
                else combomana -= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            }

            return (float)damage;
        }

        public static double GetRDamage(Obj_AI_Base enemy)
        {
            if (!Spells.R.IsReady()) return 0f;
            double damage = 0;
            if (Spells.IgniteSlot.IsReady())
                damage += GetIgniteDamage(enemy);
            if (Spells.R.IsReady())
                damage += Math.Min(7, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo) * 
                    ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.R, 1); ;
            return damage;
        }

        public static float GetIgniteDamage(Obj_AI_Base enemy)
        {
            if (Spells.IgniteSlot == SpellSlot.Unknown || ObjectManager.Player.Spellbook.CanUseSpell(Spells.IgniteSlot) != SpellState.Ready)
                return 0f;
            return (float)ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
        }

        public static float overkillcheckv2(Obj_AI_Base target)
        {
            double dmg = 0;
            if (Spells.Q.IsReady())
                dmg += Spells.Q.GetDamage(target);
            if (Spells.E.IsReady())
                dmg += Spells.E.GetDamage(target);
            if (ObjectManager.Player.Distance(target.Position) <= 550)
                dmg += ObjectManager.Player.GetAutoAttackDamage(target);
            if (Spells.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                Spells.W.GetDamage(target);

            return (float)dmg;
        }

    }
}
