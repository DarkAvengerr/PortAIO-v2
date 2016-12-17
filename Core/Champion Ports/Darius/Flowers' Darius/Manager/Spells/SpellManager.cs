using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Spells
{
    using FlowersDariusCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellManager : Logic
    {
        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 425f);
            W = new Spell(SpellSlot.W, 170f);
            E = new Spell(SpellSlot.E, 550f);
            R = new Spell(SpellSlot.R, 475f);
            E.SetSkillshot(0.20f, 100f, float.MaxValue, false, SkillshotType.SkillshotCone);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        internal static bool CanQHit(AIHeroClient target)
        {
            if (target == null)
            {
                return false;
            }

            if (target.DistanceToPlayer() > Q.Range)
            {
                return false;
            }

            if (target.DistanceToPlayer() <= 240)
            {
                return false;
            }

            if (target.Health < DamageCalculate.GetRDamage(target) && R.IsReady() && target.IsValidTarget(R.Range))
            {
                return false;
            }

            return true;
        }

        internal static int RMana => (R.Level == 0 || R.Level == 3) ? 0 : 100;

        internal static void CastItem()
        {
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
            {
                Items.UseItem(3077);
            }

            if (Items.HasItem(3074) && Items.CanUseItem(3074))
            {
                Items.UseItem(3074);
            }

            if (Items.HasItem(3053) && Items.CanUseItem(3053))
            {
                Items.UseItem(3053);
            }
        }
    }
}
