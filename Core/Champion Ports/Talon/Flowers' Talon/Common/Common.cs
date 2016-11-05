using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Talon.Common
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public static class Common
    {
        public static bool CheckTarget(Obj_AI_Base target, float range = float.MaxValue)
        {
            if (target == null)
            {
                return false;
            }

            if (target.DistanceToPlayer() > range)
            {
                return false;
            }

            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return false;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("ShroudofDarkness"))
            {
                return false;
            }

            if (target.HasBuff("SivirShield"))
            {
                return false;
            }

            return !target.HasBuff("FioraW");
        }

        public static bool CheckTargetSureCanKill(Obj_AI_Base target)
        {
            if (target == null)
            {
                return false;
            }

            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return false;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("ShroudofDarkness"))
            {
                return false;
            }

            if (target.HasBuff("SivirShield"))
            {
                return false;
            }

            return !target.HasBuff("FioraW");
        }

        public static double ComboDamage(AIHeroClient target)
        {
            if (target != null && !target.IsDead && !target.IsZombie)
            {
                if (target.HasBuff("KindredRNoDeathBuff"))
                {
                    return 0;
                }

                if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                {
                    return 0;
                }

                if (target.HasBuff("JudicatorIntervention"))
                {
                    return 0;
                }

                if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                {
                    return 0;
                }

                if (target.HasBuff("FioraW"))
                {
                    return 0;
                }

                if (target.HasBuff("ShroudofDarkness"))
                {
                    return 0;
                }

                if (target.HasBuff("SivirShield"))
                {
                    return 0;
                }

                var damage = 0d;

                damage += ObjectManager.Player.GetAutoAttackDamage(target) +
                          (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).IsReady()
                              ? ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q)
                              : 0d) +
                          (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).IsReady()
                              ? ObjectManager.Player.GetSpellDamage(target, SpellSlot.W)
                              : 0d) +
                          (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).IsReady()
                              ? ObjectManager.Player.GetSpellDamage(target, SpellSlot.E)
                              : 0d) +
                          (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).IsReady()
                              ? ObjectManager.Player.GetSpellDamage(target, SpellSlot.R)
                              : 0d) +
                          (ObjectManager.Player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown &&
                           ObjectManager.Player.GetSpellSlot("SummonerDot").IsReady()
                              ? 50 + 20*ObjectManager.Player.Level - (target.HPRegenRate/5*3)
                              : 0d);

                if (target.ChampionName == "Moredkaiser")
                {
                    damage -= target.Mana;
                }

                if (ObjectManager.Player.HasBuff("SummonerExhaust"))
                {
                    damage = damage * 0.6f;
                }

                if (target.HasBuff("GarenW"))
                {
                    damage = damage * 0.7f;
                }

                if (target.HasBuff("ferocioushowl"))
                {
                    damage = damage * 0.7f;
                }

                if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
                {
                    damage -= target.Mana / 2f;
                }

                return damage;
            }

            return 0d;
        }


        public static bool CanMove(this AIHeroClient Target)
        {
            return !(Target.MoveSpeed < 50) && !Target.IsStunned && !Target.HasBuffOfType(BuffType.Stun) &&
                !Target.HasBuffOfType(BuffType.Fear) && !Target.HasBuffOfType(BuffType.Snare) &&
                !Target.HasBuffOfType(BuffType.Knockup) && !Target.HasBuff("Recall") && !Target.HasBuffOfType(BuffType.Knockback)
                && !Target.HasBuffOfType(BuffType.Charm) && !Target.HasBuffOfType(BuffType.Taunt) &&
                !Target.HasBuffOfType(BuffType.Suppression) && (!Target.IsCastingInterruptableSpell()
                || Target.IsMoving);
        }

        public static float DistanceSquared(this Obj_AI_Base source, Vector3 position)
        {
            return source.DistanceSquared(position.To2D());
        }

        public static float DistanceSquared(this Obj_AI_Base source, Vector2 position)
        {
            return source.ServerPosition.DistanceSquared(position);
        }

        public static float DistanceSquared(this Vector3 vector3, Vector2 toVector2)
        {
            return vector3.To2D().DistanceSquared(toVector2);
        }

        public static float DistanceSquared(this Vector2 vector2, Vector2 toVector2)
        {
            return Vector2.DistanceSquared(vector2, toVector2);
        }

        public static float DistanceSquared(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.DistanceSquared(target.ServerPosition);
        }

        public static float DistanceSquared(this Vector3 vector3, Vector3 toVector3)
        {
            return vector3.To2D().DistanceSquared(toVector3);
        }

        public static float DistanceSquared(this Vector2 vector2, Vector3 toVector3)
        {
            return Vector2.DistanceSquared(vector2, toVector3.To2D());
        }

        public static float DistanceSquared(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool onlyIfOnSegment = false)
        {
            var objects = point.ProjectOn(segmentStart, segmentEnd);

            return (objects.IsOnSegment || onlyIfOnSegment == false) ? Vector2.DistanceSquared(objects.SegmentPoint, point) : float.MaxValue;
        }

        public static float DistanceToPlayer(this Obj_AI_Base source)
        {
            return ObjectManager.Player.Distance(source);
        }

        public static float DistanceToPlayer(this Vector3 position)
        {
            return position.To2D().DistanceToPlayer();
        }

        public static float DistanceToPlayer(this Vector2 position)
        {
            return ObjectManager.Player.Distance(position);
        }

        public static float DistanceToMouse(this Obj_AI_Base source)
        {
            return Game.CursorPos.Distance(source.Position);
        }

        public static float DistanceToMouse(this Vector3 position)
        {
            return position.To2D().DistanceToMouse();
        }

        public static float DistanceToMouse(this Vector2 position)
        {
            return Game.CursorPos.Distance(position.To3D());
        }
    }
}
