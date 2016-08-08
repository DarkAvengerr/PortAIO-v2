using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Helpers.Entity
{
    class DZTargetHelper
    {
        public static AIHeroClient GetNearlyKillableTarget(Spell Spell, SpellSlot[] slots, TargetSelector.DamageType DamageType)
        {
            var targetSelectorTarget = TargetSelector.GetTarget(Spell.Range, TargetSelector.DamageType.Magical);
            var targetSelectorTargetIsKillable = Spell.GetDamage(targetSelectorTarget) > targetSelectorTarget.Health + 5;
            
            foreach (var target in HeroManager.Enemies.Where(n => n.LSIsValidTarget(Spell.Range)))
            {
                var SpellDamage = Spell.GetDamage(target);
                if (target.Health + 5 > SpellDamage 
                    && target.Health + 5 < SpellDamage 
                    + ObjectManager.Player.LSGetAutoAttackDamage(target) 
                    + ObjectManager.Player.GetComboDamage(target, slots
                    .Except(new List<SpellSlot>() { Spell.Slot }).ToList()))
                {
                    return target;
                }
            }

            return targetSelectorTargetIsKillable ? targetSelectorTarget : null;
        }
    }
}
