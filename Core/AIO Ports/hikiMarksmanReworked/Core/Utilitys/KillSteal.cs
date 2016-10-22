using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Utilitys
{
    class KillSteal
    {
        public static void Q(Spell spell)
        {
            foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(spell.Range) && x.Health < spell.GetDamage(x)))
            {
                spell.Cast(enemy);
            }
        }

        public static void W(Spell spell)
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spell.Range) && x.Health < spell.GetDamage(x)))
            {
                spell.Cast(enemy);
            }
        }
        public static void E(Spell spell)
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spell.Range) && x.Health < spell.GetDamage(x)))
            {
                spell.Cast(enemy);
            }
        }

        public static void R(Spell spell)
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spell.Range) && x.Health < spell.GetDamage(x)))
            {
                spell.Cast(enemy);
            }
        }
    }
}
