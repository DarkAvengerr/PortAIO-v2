using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheBrand.Commons;
using TheBrand.Commons.ComboSystem;
using EloBuddy;

namespace TheBrand
{
    class BrandCombo : ComboProvider
    {
        // ReSharper disable once InconsistentNaming
        public bool ForceAutoAttacks;

        public BrandCombo(IEnumerable<Skill> skills, Orbwalking.Orbwalker orbwalker, float range)
            : base(range, skills, orbwalker) { }

        public BrandCombo(float range, Orbwalking.Orbwalker orbwalker, params Skill[] skills)
            : base(range, orbwalker, skills) { }

        public override void Update()
        {
            if (!(ForceAutoAttacks && ObjectManager.Player.Spellbook.IsAutoAttacking))
                base.Update();



            var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.True);
            if (target.IsValidTarget())
            {
                var passiveBuff = target.GetBuff("brandablaze");
                if (passiveBuff != null)
                {
                    IgniteManager.Update(this, GetRemainingPassiveDamage(target, passiveBuff), (int)(passiveBuff.EndTime - Game.Time) + 1);
                    return;
                }
            }

            IgniteManager.Update(this); // maybe should use GetTarget!?

        }

        public override bool ShouldBeDead(AIHeroClient target, float additionalSpellDamage = 0f)
        {
            var passive = target.GetBuff("brandablaze");
            return base.ShouldBeDead(target, passive != null ? GetRemainingPassiveDamage(target, passive) : 0f);
        }


        private float GetRemainingPassiveDamage(Obj_AI_Base target, BuffInstance passive)
        {
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, ((int)(passive.EndTime - Game.Time) + 1) * target.MaxHealth * 0.02f);
        }

        public static float GetPassiveDamage(AIHeroClient target)
        {
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, target.MaxHealth * 0.08);
        }
    }
}
