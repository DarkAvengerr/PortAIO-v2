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
 namespace Lissandra_the_Ice_Goddess.Utility
{
    static class PositionalHelper
    {
        private static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    HeroManager.Enemies.Where(
                        m =>
                            m.Distance(ObjectManager.Player, true) <= Math.Pow(1500, 2) && m.IsValidTarget(1500, false) &&
                            m.CountEnemiesInRange(m.IsMelee() ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }

        /// <summary>
        /// Vayne Hunter Revolution E Safe Position
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="considerAllyTurrets">Consider ally turrets firepower?</param>
        /// <param name="considerLhEnemies">Consider Low Health enemies?</param>
        /// <returns></returns>
        public static bool IsSafePositionEx(this Vector3 position, bool considerAllyTurrets = true, bool considerLhEnemies = true)
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
                var nearestAllyTurret = ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsAlly).OrderBy(d => d.Distance(position, true)).FirstOrDefault();

                if (nearestAllyTurret != null)
                {
                    ////We're adding more allies, since the turret adds to the firepower of the team.
                    allies += 2;
                }
            }

            ////Adding 1 for my Player
            return (allies + 1 > enemies - lhEnemies);
        }

        public static bool PassesNoEIntoEnemiesCheck(this Vector3 position)
        {
                var vector2Position = position.To2D();
                if (EnemiesClose.Count() <= 1)
                {
                    return true;
                }

                if (GetEnemyPoints().Contains(vector2Position))
                {
                    return false;
                }
                return true;
        }

        public static List<Vector2> GetEnemyPoints()
        {
            var polygonsList = EnemiesClose.Select(enemy => new LissandraGeometry.Circle(enemy.ServerPosition.To2D(), (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) + enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = LissandraGeometry.ClipPolygons(polygonsList);
            var pointList = pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y)).Where(currentPoint => !currentPoint.IsWall()).ToList();
            return pointList;
        }
    }
}
