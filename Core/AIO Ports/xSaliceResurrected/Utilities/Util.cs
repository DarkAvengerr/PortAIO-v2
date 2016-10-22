using EloBuddy; 
 using LeagueSharp.Common; 
 namespace xSaliceResurrected_Rework.Utilities
{
    using Base;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public static class Util 
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static bool UnderAllyTurret()
        {
            return
                ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret != null && (turret.IsAlly && !turret.IsDead && turret.Distance(Player) < 800));
        }

        public static bool IsWall(Vector2 pos)
        {
            return NavMesh.GetCollisionFlags(pos.To3D()).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(pos.To3D()).HasFlag(CollisionFlags.Building);
        }

        public static bool IsPassWall(Vector3 start, Vector3 end)
        {
            double count = Vector3.Distance(start, end);

            for (uint i = 0; i <= count; i += 25)
            {
                var pos = start.To2D().Extend(Player.ServerPosition.To2D(), -i);

                if (IsWall(pos))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsStunned(Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                   target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt);
        }

        public static bool CanMove(AIHeroClient Target)
        {
            return !(Target.MoveSpeed < 50) && !Target.IsStunned && 
                !Target.HasBuffOfType(BuffType.Stun) && !Target.HasBuffOfType(BuffType.Fear) &&
                !Target.HasBuffOfType(BuffType.Snare) && !Target.HasBuffOfType(BuffType.Knockup) && 
                !Target.HasBuff("Recall") && !Target.HasBuffOfType(BuffType.Knockback) && 
                !Target.HasBuffOfType(BuffType.Charm) && !Target.HasBuffOfType(BuffType.Taunt) && 
                !Target.HasBuffOfType(BuffType.Suppression) && (!Target.IsCastingInterruptableSpell()
                || Target.IsMoving);
        }

        public static AIHeroClient GetTargetFocus(float range)
        {
            var focusSelected = Champion.Menu.Item("selected", true).GetValue<bool>();

            if (TargetSelector.GetSelectedTarget() != null)
            {
                if (focusSelected && TargetSelector.GetSelectedTarget().Distance(Player.ServerPosition) < range + 100 &&
                    TargetSelector.GetSelectedTarget().Type == GameObjectType.AIHeroClient)
                {
                    return TargetSelector.GetSelectedTarget();
                }
            }

            return null;
        }
    }
}
