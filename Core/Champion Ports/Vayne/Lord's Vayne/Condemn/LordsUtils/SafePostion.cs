using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Lord_s_Vayne.Condemn.HeroExtension;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Condemn.LordsUtils
{
    public static class SafePostion
    {
        public static bool IsSafe(this Vector3 position, float range)
        {
            if (position.UnderTurret(true) && !ObjectManager.Player.UnderTurret(true))
            {
                return false;
            }

            var allies = position.CountAlliesInRange(ObjectManager.Player.AttackRange);
            var enemies = position.CountEnemiesInRange(ObjectManager.Player.AttackRange);
            var lhEnemies = position.GetLhEnemiesNear(ObjectManager.Player.AttackRange, 15).Count();

            if (enemies <= 1) ////It's a 1v1, safe to assume I can Q
            {
                return true;
            }

            if (position.UnderAllyTurret())
            {
                var nearestAllyTurret = ObjectManager.Get<Obj_AI_Turret>().Where(a => a.IsAlly).OrderBy(d => d.Distance(position, true)).FirstOrDefault();

                if (nearestAllyTurret != null)
                {
                    ////We're adding more allies, since the turret adds to the firepower of the team.
                    allies += 2;
                }
            }

            var normalCheck = (allies + 1 > enemies - lhEnemies);
            var PositionEnemiesCheck = true;

            var Vector2Position = position.To2D();
             
            var closeEnemies = PositioningHelper.EnemiesClose;

            if (!closeEnemies.All(
                    enemy =>
                        position.CountEnemiesInRange(enemy.AttackRange) <= 1))
            {
                PositionEnemiesCheck = false;
            }

            return normalCheck && PositionEnemiesCheck;
        }


    }
    public class PositioningHelper
    {
        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    HeroManager.Enemies.Where(
                        m =>
                            m.Distance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.IsValidTarget(1500, false) &&
                            m.CountEnemiesInRange(m.IsMelee() ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }
    }
    
}
