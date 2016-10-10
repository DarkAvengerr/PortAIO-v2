using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using hYasuo.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hYasuo.Extensions
{
    class DamageCalculator
    {

        public static double GetSweepingBladeDamage(Obj_AI_Base target)
        {
            var stacksPassive = ObjectManager.Player.Buffs.Find(b => b.DisplayName.Equals("YasuoDashScalar"));
            var stacks = 1 + 0.25 * (stacksPassive?.Count ?? 0);
            var damage = ((50 + 20 * Spells.E.Level) * stacks) + (ObjectManager.Player.FlatMagicDamageMod * 0.6);
            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, damage);
        }
    }
}
