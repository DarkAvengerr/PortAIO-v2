using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Extensions
{
    using System.Linq;
    using SharpDX;

    using LeagueSharp.SDK;
    using LeagueSharp;

    using static Brand.Extensions.Spells;
    using static Brand.Extensions.Config;

    internal class Other
    {
        public static float GetDistance(Vector3 a, Vector3 b)
        {
            float Kappa = (b.X - a.X) + (b.Z - a.Z);
            return Kappa;
        }

        public static int CountMinionsInRange(float range, Vector3 pos)
        {
            var Minions = GameObjects.EnemyMinions;
            int C = 0;
            foreach (var Minion in Minions.Where(x => x.IsValidTarget() && GetDistance(x.Position, pos) <= range))
            {
                C++;
            }
            return C;
        }

        public static bool Immobile(Obj_AI_Base target)
        {
            if (target.MoveSpeed < 50 || target.IsStunned || target.HasBuffOfType(BuffType.Stun) ||
                target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) || target.HasBuff("Recall") ||
                target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Charm) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SkinChanger()
        {
            //ObjectManager.//Player.SetSkin(MyHero.BaseSkinName, SM_M);
        }

        public static float TotalDamage(Obj_AI_Base target, bool _Q, bool _W, bool _E, bool _R)
        {
            var Damage = 0f;
            if (_Q && Q.IsReady())
            {
                Damage += Q.GetDamage(target) + PassiveDamage(target);
            }
            if (_W && W.IsReady())
            {
                Damage += W.GetDamage(target) + PassiveDamage(target);
            }
            if (_E && E.IsReady())
            {
                Damage += E.GetDamage(target) + PassiveDamage(target);
            }
            if (_R && R.IsReady())
            {
                Damage += R.GetDamage(target) + PassiveDamage(target);
            }
            return Damage;
        }

        public static float PassiveDamage(Obj_AI_Base target)
        {
            return (float)MyHero.CalculateDamage(target, DamageType.Magical, (target.MaxHealth * 0.08) - (target.HPRegenRate * 5));
        }

        public static float ComboDamage(Obj_AI_Base target)
        {
            return TotalDamage(target, true, true, true, true);
        }
    }
}
