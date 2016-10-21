using LeagueSharp;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;
using NLog;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista
{
    internal static class Extensions
    {
        internal static bool HasEBuff(this Obj_AI_Base target)
        {
            return target != null && target.HasBuff("kalistaexpungemarker");
        }

        internal static double GetQDamage(this Obj_AI_Base target)
        {
            return GameObjects.Player.GetSpellDamage(target, SpellSlot.Q);
        }

        internal static double GetEDamage(this Obj_AI_Base target)
        {
            return target.HasEBuff() ? GameObjects.Player.GetSpellDamage(target, SpellSlot.E) + GameObjects.Player.GetSpellDamage(target, SpellSlot.E, DamageStage.Buff) + (Config.Misc.UseEdamageAdjust ? Config.Misc.EdamageAdjustValue : 0) : 0f;
        }

        internal static bool IsKillableWithQ(this Obj_AI_Base target, bool rangeCheck = false)
        {
            return target.IsValidTarget(rangeCheck ? SpellManager.Q.Range : float.MaxValue) && target.Health + target.HPRegenRate + target.AttackShield < target.GetQDamage();
        }

        internal static bool IsKillableWithE(this Obj_AI_Base target, bool rangeCheck = false)
        {
            return target.IsValidTarget(rangeCheck ? SpellManager.E.Range : float.MaxValue) && target.Health + target.AttackShield < target.GetEDamage();
        }

        internal static bool IsKillableWithE(this AIHeroClient target, bool rangeCheck = false)
        {
            return target.IsValidTarget(rangeCheck ? SpellManager.E.Range : float.MaxValue) && target.Health + target.HPRegenRate + target.AttackShield < target.GetEDamage() && !Invulnerable.Check(target, DamageType.Physical, false);
        }

        //internal static int GetRemainingAttacksToKillWithE(this Obj_AI_Base target)
        //{
        //    var targetHealth = target.Health + target.AttackShield;
        //    var calculatedEDamagePerAdditionalSpear = CalculateEDamagePerAdditionalSpear(target);

        //    return

        //        (int)Math.Ceiling((targetHealth - (GameObjects.Player.GetSpellDamage(target, SpellSlot.E) + GameObjects.Player.GetSpellDamage(target, SpellSlot.E, Damage.DamageStage.Buff))) / (calculatedEDamagePerAdditionalSpear + GameObjects.Player.GetAutoAttackDamage(target)));
        //}

        //internal static float CalculateEBaseDamage(Obj_AI_Base target)
        //{
        //    var source = GameObjects.Player;
        //    var level = source.Spellbook.GetSpell(SpellSlot.E).Level;
        //    var baseDamage = new[] { 0, 20, 30, 40, 50, 60 }[level];
        //    var extraDamage = source.TotalAttackDamage * 0.6;

        //    return (float)source.CalculateDamage(target, DamageType.Physical, baseDamage + extraDamage);
        //}

        //internal static float CalculateEDamagePerAdditionalSpear(Obj_AI_Base target)
        //{
        //    var source = GameObjects.Player;
        //    var level = source.Spellbook.GetSpell(SpellSlot.E).Level;
        //    var baseDamage = new[] { 0, 10, 14, 19, 25, 32 }[level];
        //    var extraDamage = source.TotalAttackDamage * new[] { 0, 0.225, 0.25, 0.275, 0.30 }[level];

        //    return (float)source.CalculateDamage(target, DamageType.Physical, baseDamage + extraDamage);
        //}
    }
}
