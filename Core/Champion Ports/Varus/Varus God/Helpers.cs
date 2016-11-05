using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Varus_God
{
    internal static class Helpers //Credit to hellsing for most of this, I fixed the damage calcs though
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static int GetBlightStacks(this Obj_AI_Base target)
        {
            var buff = target.Buffs.Find(b => b.Caster.IsMe && b.DisplayName == "VarusWDebuff");
            return buff != null ? buff.Count : 0;
        }

        public static float GetSpellDamage(this Spell spell, Obj_AI_Base target)
        {
            return spell.Slot.GetSpellDamage(target);
        }

        public static float GetSpellDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            // Validate spell level
            var spellLevel = Player.Spellbook.GetSpell(slot).Level;
            if (spellLevel == 0)
                return 0;
            spellLevel--;

            // Helpers
            var damageType = Damage.DamageType.Physical;
            double damage = 0;
            float extraDamage = 0;

            switch (slot)
            {
                case SpellSlot.Q:

                    // First Cast: Varus starts drawing back his next shot, gradually increasing its range and damage.
                    // Second Cast: Varus fires, dealing 10/47/83/120/157 (+1) to 15/70/125/180/235 (+1.6) physical damage, reduced by 15% per enemy hit (minimum 33%).
                    // While preparing to shoot Varus' Movement Speed is slowed by 20%. After 4 seconds, Piercing Arrow fails but refunds half its Mana cost.
                    var chargePercentage = Spells.Q.Range/Spells.Q.ChargedMaxRange;
                    damage = (float) ((new float[] {10, 47, 83, 120, 157}[Spells.Q.Level - 1] +
                                       new float[] {5, 23, 42, 60, 78}[Spells.Q.Level - 1]*chargePercentage) +
                                      // This line and before gets the base damage over the 2 second duration charge
                                      (chargePercentage*(Player.TotalAttackDamage() + Player.TotalAttackDamage*.6)));
                    break;

                case SpellSlot.W:

                    // Passive: Varus' basic attacks deal 10/14/18/22/26 (+0.25) bonus magic damage and apply Blight for 6 seconds (stacks 3 times).
                    damageType = Damage.DamageType.Magical;
                    damage = new float[] {10, 14, 18, 22, 26}[spellLevel] + 0.25f*Player.TotalMagicalDamage();

                    // Varus' other abilities detonate Blight, dealing magic damage equal to 2/2.75/3.5/4.25/5% (+0.02%) of the target's maximum Health per stack (Max: 360 total damage vs Monsters).
                    if (target.GetBlightStacks() > 0)
                    {
                        Player.CalcDamage(target, Damage.DamageType.Magical,
                            (new[] {2, 2.75f, 3.5f, 4.25f, 5}[spellLevel]*target.MaxHealth)*
                            target.GetBlightStacks());
                        if (target is Obj_AI_Minion)
                        {
                            // Special case: max damage 360
                            extraDamage = Math.Min(360, extraDamage);
                        }
                    }
                    break;

                case SpellSlot.E:

                    // Varus fires a hail of arrows that deals 65/100/135/170/205 (+0.6) physical damage and desecrates the ground for 4 seconds.
                    // Desecrated Ground slows enemy Movement Speed by 25/30/35/40/45% and reduces healing effects by 50%.
                    damage = new float[] {65, 100, 135, 170, 205}[spellLevel] + 0.6f*Player.TotalAttackDamage();
                    break;

                case SpellSlot.R:

                    // Varus flings out a tendril of corruption that deals 150/250/350 (+1) magic damage and immobilizes the first enemy champion hit for 2 seconds.
                    // The corruption then spreads towards nearby uninfected enemy champions, applying the same damage and immobilize if it reaches them.
                    damageType = Damage.DamageType.Magical;
                    damage = new float[] {150, 250, 350}[spellLevel] + Player.TotalMagicalDamage();
                    break;
            }
            // No damage set
            if (damage <= 0 && extraDamage <= 0)
                return 0;

            // Calculate damage on target and return (-20 to make it actually more accurate Kappa)
            return (float) Player.CalcDamage(target, damageType, damage) + extraDamage;
        }
    }
}