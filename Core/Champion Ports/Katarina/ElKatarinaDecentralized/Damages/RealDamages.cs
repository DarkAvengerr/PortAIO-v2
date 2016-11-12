using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKatarinaDecentralized.Damages
{
    using ElKatarinaDecentralized.Components;
    using ElKatarinaDecentralized.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class RealDamages
    {
        /// <summary>
        ///     Gets the total damage.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        ///     Total calculated damage.
        /// </returns>
        internal static float GetTotalDamage(Obj_AI_Base target)
        {
            var damage = (float)ObjectManager.Player.GetAutoAttackDamage(target);

            if (Misc.SpellQ.SpellObject.IsReady())
            {
                damage += GetRealDamage(Misc.SpellQ.SpellSlot, target);
            }

            if (Misc.SpellE.SpellObject.IsReady())
            {
                damage += GetRealDamage(Misc.SpellE.SpellSlot, target);
            }

            if (Misc.SpellR.SpellObject.IsReady())
            {
                damage += GetRealDamage(Misc.SpellR.SpellSlot, target) * 15; // max daggers
            }

            return damage;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static float GetRealDamage(Spell spell, Obj_AI_Base target)
        {
            return GetRealDamage(spell.Slot, target);
        }

        /// <summary>
        ///     Calculates the damage.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static float GetRealDamage(SpellSlot slot, Obj_AI_Base target)
        {
            var spellLevel = ObjectManager.Player.Spellbook.GetSpell(slot).Level;
            if (spellLevel == 0)
            {
                return 0;
            }

            spellLevel--;

            var damageType = Damage.DamageType.Magical;
            float damage = 0;

            switch (slot)
            {
                case SpellSlot.Q:
                    damageType = Damage.DamageType.Magical;
                    damage = new float[] { 75, 105, 135, 165, 195 }[spellLevel] + 0.3f * ObjectManager.Player.TotalMagicalDamage;
                    break;

                case SpellSlot.E:
                    damage = (new float[] { 30, 45, 60, 75, 90 }[spellLevel]
                             + 0.65f * ObjectManager.Player.TotalAttackDamage + 0.25f * ObjectManager.Player.TotalMagicalDamage);
                    break;

                case SpellSlot.R:
                    damageType = Damage.DamageType.Magical;
                    damage = new float[] { 25, 38, 50 }[spellLevel] + 0.2f * ObjectManager.Player.TotalMagicalDamage + 0.2f * ObjectManager.Player.TotalAttackDamage;
                    break;
            }

            if (damage == 0)
            {
                return 0;
            }

            return (float) ObjectManager.Player.CalcDamage(target, damageType, damage);
        }
    }
}
