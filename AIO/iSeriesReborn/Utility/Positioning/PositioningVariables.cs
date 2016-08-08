using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Utility.Positioning
{
    class PositioningVariables
    {
        private const float Range = 1500f;

        public static IEnumerable<AIHeroClient> MeleeEnemiesTowardsMe
        {
            get
            {
                return
                    HeroManager.Enemies.FindAll(
                        m => m.IsMelee() && m.LSDistance(ObjectManager.Player) <= PlayerHelper.GetRealAutoAttackRange(m, ObjectManager.Player)
                            && (m.ServerPosition.LSTo2D() + (m.BoundingRadius + 25f) * m.Direction.LSTo2D().LSPerpendicular()).LSDistance(ObjectManager.Player.ServerPosition.LSTo2D()) <= m.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition)
                            && m.LSIsValidTarget(Range, false));
            }
        }

        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    HeroManager.Enemies.Where(
                        m =>
                            m.LSDistance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.LSIsValidTarget(1500, false) &&
                            m.LSCountEnemiesInRange(m.IsMelee() ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }
    }
}
