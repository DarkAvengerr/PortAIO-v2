using LeagueSharp;
using LeagueSharp.Common;

//By Hellsing 
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKalista
{
    public static class Damages
    {
        private static AIHeroClient player = ObjectManager.Player;

        private static float[] _rawRendDamage = new float[] { 20, 30, 40, 50, 60 };
        private static float[] _rawRendDamageMultiplier = new float[] { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static float[] _rawRendDamagePerSpear = new float[] { 10, 14, 19, 25, 32 };
        private static float[] _rawRendDamagePerSpearMultiplier = new float[] { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            var hero = target as AIHeroClient;
            return GetRendDamage(target) > GetActualHealth(target) && (hero == null);
        }

        /// <summary>
        ///     Gets the targets health including the shield amount
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The targets health
        /// </returns>
        public static float GetActualHealth(Obj_AI_Base target)
        {
            return target.Health;
        }

        public static float GetRendDamage(AIHeroClient target)
        {
            return (float)GetRendDamage(target, -1);
        }

        public static float GetRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            return ((float)player.CalcDamage(target, Damage.DamageType.Physical, GetRawRendDamage(target, customStacks)) - 20 * 0.98f);
        }

        public static bool HasRendBuff(this Obj_AI_Base target)
        {
            return target.GetRendBuff() != null;
        }

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.Find(b => b.Caster.IsMe && b.IsValidBuff() && b.DisplayName.ToLower() == "kalistaexpungemarker");
        }

        public static float GetRawRendDamage(Obj_AI_Base target, int customStacks = -1)
        {     
            if (target.GetBuffCount("kalistaexpungemarker") != 0 || customStacks > -1)
            {
                return (_rawRendDamage[Kalista.spells[Spells.E].Level - 1] + _rawRendDamageMultiplier[Kalista.spells[Spells.E].Level - 1] * player.TotalAttackDamage()) + 
                       ((customStacks < 0 ? target.GetBuffCount("kalistaexpungemarker") : customStacks) - 1) * 
                       (_rawRendDamagePerSpear[Kalista.spells[Spells.E].Level - 1] + _rawRendDamagePerSpearMultiplier[Kalista.spells[Spells.E].Level - 1] * player.TotalAttackDamage()); 
            }

            return 0;
        }

        public static float GetTotalDamage(AIHeroClient target)
        {
            double damage = player.GetAutoAttackDamage(target);

            if (Kalista.spells[Spells.Q].IsReady())
                damage += player.GetSpellDamage(target, SpellSlot.Q);

            if (Kalista.spells[Spells.E].IsReady())
                damage += GetRendDamage(target);

            return (float)damage;
        }

        public static float TotalAttackDamage(this Obj_AI_Base target)
        {
            return target.BaseAttackDamage + target.FlatPhysicalDamageMod;
        }
    }
}
