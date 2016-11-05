using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Lissandra_the_Ice_Goddess.Utility
{
    static class Extensions
    {
        public static bool IsSafePosition(this Vector3 position, bool considerAllyTurrets = true, bool considerLhEnemies = true)
        {
            if (position.UnderTurret(true) && !ObjectManager.Player.UnderTurret(true))
            {
                return false;
            }

            var allies = position.CountAlliesInRange(ObjectManager.Player.AttackRange);
            var enemies = position.CountEnemiesInRange(ObjectManager.Player.AttackRange);
            var lhEnemies = considerLhEnemies ? position.GetLhEnemiesNear(ObjectManager.Player.AttackRange, 15).Count() : 0;

            if (enemies <= 1) ////It's a 1v1, safe to assume I can Q
            {
                return true;
            }

            if (position.UnderAllyTurret())
            {
                var nearestAllyTurret = ObjectManager.Get<Obj_AI_Turret>().Where(h => h.IsAlly).OrderBy(d => d.Distance(position, true)).FirstOrDefault();

                if (nearestAllyTurret != null)
                {
                    ////We're adding more allies, since the turret adds to the firepower of the team.
                    allies += 2;
                }
            }

            ////Adding 1 for my Player
            return (allies + 1 > enemies - lhEnemies);
        }
        public static List<AIHeroClient> GetLhEnemiesNear(this Vector3 position, float range, float Healthpercent)
        {
            return HeroManager.Enemies.Where(hero => hero.IsValidTarget(range, true, position) && hero.HealthPercent <= Healthpercent).ToList();
        }

        public static bool UnderAllyTurret(this Vector3 position)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(t => t.IsAlly && !t.IsDead);
        }
    }
}
