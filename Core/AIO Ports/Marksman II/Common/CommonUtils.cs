using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Common
{
    static class CommonUtils
    {
        public static List<BuffType> ImpairedBuffTypes => new List<BuffType>
        {
            BuffType.Stun,
            BuffType.Snare,
            BuffType.Charm,
            BuffType.Fear,
            BuffType.Taunt,
            BuffType.Slow
        };
        internal static double IsImmobileUntil(this AIHeroClient unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return result - Game.Time;
        }

        internal static bool IsHeavilyImpaired(this AIHeroClient enemy)
        {
            return enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                   enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                   enemy.HasBuffOfType(BuffType.Taunt);
        }


        internal static float GetSlowEndTime(this Obj_AI_Base target)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Type == BuffType.Slow)
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
        }

        internal static float GetImpairedEndTime(this Obj_AI_Base target)
        {
            return target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => CommonUtils.ImpairedBuffTypes.Contains(buff.Type))
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
        }

        public static bool IsUnderTurret(this Vector3 position, bool ally)
        {
                if (
                    ObjectManager.Get<Obj_AI_Turret>().Any(
                        t =>
                            t.Health > 1 && !t.IsDead && (ally && t.IsAlly || !ally && t.IsEnemy) &&
                            position.Distance(t.Position) < 900f))
                {
                    return true;
                }

            return false;
        }

        public static Vector3 GetDashPosition(Spell spell, AIHeroClient target, float safetyDistance)
        {
            var distance = target.Distance(ObjectManager.Player);
            var dashPoints = new Geometry.Polygon.Circle(ObjectManager.Player.Position, spell.Range).Points;
            if (distance < safetyDistance)
            {
                dashPoints.AddRange(
                    new Geometry.Polygon.Circle(ObjectManager.Player.Position, safetyDistance - distance).Points);
            }
            dashPoints = dashPoints.Where(p => !p.IsWall()).OrderBy(p => p.Distance(Game.CursorPos)).ToList();
            foreach (var point in dashPoints)
            {
                var allies =
                    HeroManager.Allies.Where(
                        hero => !hero.IsDead && hero.Distance(point.To3D()) < ObjectManager.Player.AttackRange).ToList();
                var enemies =
                    HeroManager.Enemies.Where(
                        hero => hero.IsValidTarget(ObjectManager.Player.AttackRange, true, point.To3D())).ToList();
                var lowEnemies =
                    enemies.Where(
                        hero => hero.HealthPercent <= 15 && hero.HealthPercent < ObjectManager.Player.HealthPercent*1.5)
                        .ToList();

                if (!point.To3D().IsUnderTurret(false))
                {
                    if (enemies.Count == 1 &&
                        (!target.IsMelee || target.HealthPercent <= ObjectManager.Player.HealthPercent - 25 ||
                         target.Position.Distance(point.To3D()) >= safetyDistance) ||
                        allies.Count >
                        enemies.Count -
                        (ObjectManager.Player.HealthPercent >= 10*lowEnemies.Count ? lowEnemies.Count : 0))
                    {
                        return point.To3D();
                    }
                }
                else
                {
                    if (enemies.Count == 1 && lowEnemies.Any(t => t.NetworkId.Equals(target.NetworkId)))
                    {
                        return point.To3D();
                    }
                }
            }

            return Vector3.Zero;
        }

        public static bool IsWallBetween(Vector3 start, Vector3 end, int step = 3)
        {
            if (start.IsValid() && end.IsValid() && step > 0)
            {
                var distance = start.Distance(end);
                for (var i = 0; i < distance; i = i + step)
                {
                    if (NavMesh.GetCollisionFlags(start.Extend(end, i)) == CollisionFlags.Wall)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public static int GetRandomDelay(int min, int max)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(min, max);
        }


       internal static bool CanGetDamage(this AIHeroClient t)
        {
            if (t.HasBuff("kindredrnodeathbuff") && t.HealthPercent <= 10)
            {
                return false;
            }

            // Tryndamere's Undying Rage (R)
            if (t.HasBuff("Undying Rage"))
            {
                return false;
            }

            // Kayle's Intervention (R)
            if (t.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            return true;
        }
        internal static bool IsKillableAndValidTarget(this AIHeroClient target, double calculatedDamage, TargetSelector.DamageType damageType, float distance = float.MaxValue)
        {
            if (target == null || !target.IsValidTarget(distance) || target.CharData.BaseSkinName == "gangplankbarrel")
                return false;

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return false;
            }

            // Tryndamere's Undying Rage (R)
            if (target.HasBuff("Undying Rage"))
            {
                return false;
            }

            // Kayle's Intervention (R)
            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            // Poppy's Diplomatic Immunity (R)
            if (target.HasBuff("DiplomaticImmunity") && !ObjectManager.Player.HasBuff("poppyulttargetmark"))
            {
                //TODO: Get the actual target mark buff name
                return false;
            }

            // Banshee's Veil (PASSIVE)
            if (target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return false;
            }

            // Sivir's Spell Shield (E)
            if (target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return false;
            }

            // Nocturne's Shroud of Darkness (W)
            if (target.HasBuff("ShroudofDarkness"))
            {
                // TODO: Get exact Nocturne's Shourd of Darkness buff name
                return false;
            }

            if (ObjectManager.Player.HasBuff("summonerexhaust"))
                calculatedDamage *= 0.6;

            if (target.ChampionName == "Blitzcrank")
                if (!target.HasBuff("manabarriercooldown"))
                    if (target.Health + target.HPRegenRate +
                        (damageType == TargetSelector.DamageType.Physical ? target.AttackShield : target.MagicShield) +
                        target.Mana * 0.6 + target.PARRegenRate < calculatedDamage)
                        return true;

            if (target.ChampionName == "Garen")
                if (target.HasBuff("GarenW"))
                    calculatedDamage *= 0.7;

            if (target.HasBuff("FerociousHowl"))
                calculatedDamage *= 0.3;

            return target.Health + target.HPRegenRate +
                   (damageType == TargetSelector.DamageType.Physical ? target.AttackShield : target.MagicShield) <
                   calculatedDamage - 2;
        }
    }
}
