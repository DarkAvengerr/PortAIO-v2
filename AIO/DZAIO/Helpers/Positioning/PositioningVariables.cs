using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Helpers.Positioning
{
        class PositioningVariables
        {
            private const float Range = 1500f;

            public static IEnumerable<AIHeroClient> MeleeEnemies
            {
                get
                {
                    return
                        HeroManager.Enemies.FindAll(
                            m => m.IsMelee() && m.Distance(ObjectManager.Player) <= ObjectManager.Player.AttackRange + 65f
                                && (m.ServerPosition.To2D() + (m.BoundingRadius + 25f) * m.Direction.To2D().Perpendicular()).Distance(ObjectManager.Player.ServerPosition.To2D()) <= m.ServerPosition.Distance(ObjectManager.Player.ServerPosition)
                                && m.IsValidTarget(Range, false));
                }
            }

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
