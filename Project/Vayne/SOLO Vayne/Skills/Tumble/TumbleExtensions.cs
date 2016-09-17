using System.Linq;
using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SoloVayne.Utility.Entities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Skills.Tumble
{
    static class TumbleExtensions
    {
        /// <summary>
        /// Determines whether the position is safe.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static bool IsSafe(this Vector3 position)
        {
            return position.IsSafeEx() 
                && position.IsNotIntoEnemies() 
                && HeroManager.Enemies.All(m => m.Distance(position) > 350f) 
                && (!position.UnderTurret(true) || (ObjectManager.Player.UnderTurret(true) && position.UnderTurret(true) && ObjectManager.Player.HealthPercent > 10));
            //Either it is not under turret or both the player and the position are under turret already and the health percent is greater than 10.
        }

        /// <summary>
        /// Determines whether the position is Safe using the allies/enemies logic
        /// </summary>
        /// <param name="Position">The position.</param>
        /// <returns></returns>
        public static bool IsSafeEx(this Vector3 Position)
        {
            if (Position.UnderTurret(true) && !ObjectManager.Player.UnderTurret())
            {
                return false;
            }
            var range = 1000f;
            var lowHealthAllies =
                HeroManager.Allies.Where(a => a.IsValidTarget(range, false) && a.HealthPercent < 10 && !a.IsMe);
            var lowHealthEnemies =
                HeroManager.Allies.Where(a => a.IsValidTarget(range) && a.HealthPercent < 10);
            var enemies = ObjectManager.Player.CountEnemiesInRange(range);
            var allies = ObjectManager.Player.CountAlliesInRange(range);
            var enemyTurrets = GameObjects.EnemyTurrets.Where(m => m.IsValidTarget(975f));
            var allyTurrets = GameObjects.AllyTurrets.Where(m => m.IsValidTarget(975f, false));

            return (allies - lowHealthAllies.Count() + allyTurrets.Count() * 2 + 1 >=
                enemies - lowHealthEnemies.Count() + (!ObjectManager.Player.UnderTurret(true) ? enemyTurrets.Count() * 2 : 0));
        }

        /// <summary>
        /// Determines whether the position is not into enemies.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static bool IsNotIntoEnemies(this Vector3 position)
        {
            if (!MenuExtensions.GetItemValue<bool>("solo.vayne.misc.tumble.smartQ") &&
                !MenuExtensions.GetItemValue<bool>("solo.vayne.misc.tumble.noqintoenemies"))
            {
                return true;
            }

            var enemyPoints = TumbleHelper.GetEnemyPoints();
            if (enemyPoints.ToList().Contains(position.To2D()) && !enemyPoints.Contains(ObjectManager.Player.ServerPosition.To2D()))
            {
                return false;
            }

            var closeEnemies =
                HeroManager.Enemies.FindAll(
                    en =>
                        en.IsValidTarget(1500f) &&
                        !(en.Distance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f));
            if (closeEnemies.All(enemy => position.CountEnemiesInRange(enemy.AttackRange > 350 ? enemy.AttackRange : 400) == 0))
            {
                return true;
            }

            return false;
        }
    }
}
