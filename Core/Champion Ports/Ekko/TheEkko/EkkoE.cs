using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheEkko.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko
{
    class EkkoE : Skill
    {
        private readonly float[] _damage = { 50, 80, 110, 140, 170 };

        public EkkoE(Spell spell)
            : base(spell)
        {
        }

        public override void Cast(AIHeroClient target, bool force = false, HitChance minChance = HitChance.Low)
        {
            if (target == null) return;
            if (ObjectManager.Player.HasBuff("ekkoattackbuff") && target.Distance(ObjectManager.Player) < 500)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, target);

            }

            if (HasBeenSafeCast() || target.Distance(ObjectManager.Player) < ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius + target.BoundingRadius) return;
            SafeCast(() => Spell.Cast(target.Position));
        }

        public override float GetDamage(AIHeroClient enemy)
        {
            if (Spell.Level == 0) return 0f;
            return (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, _damage[Spell.Level - 1] + ObjectManager.Player.TotalMagicalDamage * 0.2f);
        }

        public override int GetPriority()
        {
            return 2;
        }
    }
}
