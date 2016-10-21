using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcAshe
{
    internal static class Extensions
    {
        internal static double GetRDamage(this Obj_AI_Base target)
        {
            return GameObjects.Player.GetSpellDamage(target, SpellSlot.R, DamageStage.Default);
        }

        internal static bool IsKillableWithR(this AIHeroClient target, bool rangeCheck = false, float extraDamage = 0f)
        {
            return target.IsValidTarget(rangeCheck ? SpellManager.R.Range : float.MaxValue) && target.Health + target.MagicShield < target.GetRDamage() + extraDamage && !Invulnerable.Check(target, DamageType.Magical, false);
        }

        internal static CastStates CastR(AIHeroClient target)
        {
            //타겟이 스펠면역일 경우 리턴
            if (target.HasBuffOfType(BuffType.SpellImmunity) || target.HasBuffOfType(BuffType.SpellShield))
            {
                return CastStates.NotCasted;
            }

            return SpellManager.R.Cast(target);
        }

        internal static double IsImmobileUntil(this AIHeroClient unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare || buff.Type == BuffType.Taunt))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return result - Game.Time;
        }
    }
}
