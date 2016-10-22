using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
    #region

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class Dmg : Core
    {
        #region Public Methods and Operators

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (enemy == null) return 0;

            float damage = 0;

            if (Spells.W.IsReady())
            {
                damage += Spells.W.GetDamage(enemy);
            }

            if (Spells.Q.IsReady())
            {
                var qcount = 4 - Qstack;
                damage += Spells.Q.GetDamage(enemy) * qcount + (float)Player.GetAutoAttackDamage(enemy) * qcount;
            }

            if (Spells.R.IsReady())
            {
                damage += Spells.R.GetDamage(enemy);
            }

            if (!Player.Spellbook.IsAutoAttacking)
            {
                damage += (float)Player.GetAutoAttackDamage(enemy);
            }

            return damage;
        }

        public static float IgniteDamage(AIHeroClient target)
        {
            if (Spells.Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Spells.Ignite) != SpellState.Ready)
            {
                return 0f;
            }

            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public static float RDmg(AIHeroClient target)
        {
            float dmg = 0;

            if (target == null || !Spells.R.IsReady())
            {
                return 0;
            }

            dmg += Spells.R.GetDamage(target);

            return dmg;
        }

        #endregion
    }
}