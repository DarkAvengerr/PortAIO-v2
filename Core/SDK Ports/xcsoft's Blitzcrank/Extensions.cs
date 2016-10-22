using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcBlitzcrank
{
    internal static class Extensions
    {
        internal static float GetQDamage(this Obj_AI_Base target)
        {
            return SpellManager.Q.GetDamage(target, DamageStage.Default);
        }

        internal static float GetRDamage(this Obj_AI_Base target)
        {
            return SpellManager.R.GetDamage(target, DamageStage.Default);
        }

        internal static bool IsKillablewithR(this AIHeroClient target, bool rangeCheck = false)
        {
            return target.IsValidTarget(rangeCheck ? SpellManager.R.Range : float.MaxValue) && target.GetRDamage() > target.Health + target.MagicShield + target.HPRegenRate && !Invulnerable.Check(target, DamageType.Magical, false);
        }

        internal static CastStates CastQ(AIHeroClient target)
        {
            //타겟이 스펠면역일 경우 리턴
            if (target.HasBuffOfType(BuffType.SpellImmunity) || target.HasBuffOfType(BuffType.SpellShield))
            {
                return CastStates.NotCasted;
            }

            //타겟이 설정한 최소 거리보다 멀리있을 경우
            if (GameObjects.Player.Distance(target) > Config.Misc.QRange.QMinRange)
            {
                return SpellManager.Q.Cast(target);
            }

            return CastStates.NotCasted;
        }

        internal static double IsImmobileUntil(this AIHeroClient unit)
        {
            var result = unit.Buffs.Where(buff => buff.IsActive && Game.Time <= buff.EndTime && (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun || buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare)).Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return result - Game.Time;
        }
    }
}
