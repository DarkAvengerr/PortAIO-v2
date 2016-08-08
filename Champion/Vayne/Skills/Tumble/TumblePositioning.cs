using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VayneHunter_Reborn.Skills.Condemn;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.Helpers;
using VayneHunter_Reborn.Utility.MenuUtility;
using Geometry = VayneHunter_Reborn.Utility.Helpers.Geometry;
using EloBuddy;

namespace VayneHunter_Reborn.Skills.Tumble
{
    static class TumblePositioning
    {
        public static bool IsSafe(this Vector3 position, bool noQIntoEnemiesCheck = false)
        {
            if (position.LSUnderTurret(true) && !ObjectManager.Player.LSUnderTurret(true))
            {
                return false;
            }

            var allies = position.LSCountAlliesInRange(ObjectManager.Player.AttackRange);
            var enemies = position.LSCountEnemiesInRange(ObjectManager.Player.AttackRange);
            var lhEnemies = position.GetLhEnemiesNear(ObjectManager.Player.AttackRange, 15).Count();

            if (enemies <= 1) ////It's a 1v1, safe to assume I can Q
            {
                return true;
            }

            if (position.UnderAllyTurret_Ex())
            {
                var nearestAllyTurret = ObjectManager.Get<Obj_AI_Turret>().Where(a => a.IsAlly).OrderBy(d => d.LSDistance(position, true)).FirstOrDefault();

                if (nearestAllyTurret != null)
                {
                    ////We're adding more allies, since the turret adds to the firepower of the team.
                    allies += 2;
                }
            }

            ////Adding 1 for my Player
            var normalCheck = (allies + 1 > enemies - lhEnemies);
            var QEnemiesCheck = true;

            if (MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.noqenemies") && noQIntoEnemiesCheck)
            {
                if (!MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.noqenemies.old"))
                {
                    var Vector2Position = position.LSTo2D();
                    var enemyPoints = MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.dynamicqsafety")
                        ? GetEnemyPoints()
                        : GetEnemyPoints(false);
                    if (enemyPoints.Contains(Vector2Position) &&
                        !MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.qspam"))
                    {
                        QEnemiesCheck = false;
                    }

                    var closeEnemies =
                    HeroManager.Enemies.FindAll(en => en.LSIsValidTarget(1500f) && !(en.LSDistance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f))
                    .OrderBy(en => en.LSDistance(position));

                    if (
                        !closeEnemies.All(
                            enemy =>
                                position.LSCountEnemiesInRange(
                                    MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.dynamicqsafety")
                                        ? enemy.AttackRange
                                        : 405f) <= 1))
                    {
                        QEnemiesCheck = false;
                    }
                }
                else
                {
                    var closeEnemies =
                    HeroManager.Enemies.FindAll(en => en.LSIsValidTarget(1500f)).OrderBy(en => en.LSDistance(position));
                    if (closeEnemies.Any())
                    {
                        QEnemiesCheck =
                            !closeEnemies.All(
                                enemy =>
                                    position.LSCountEnemiesInRange(
                                        MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.dynamicqsafety")
                                            ? enemy.AttackRange
                                            : 405f) <= 1);
                    }
                }
                
            }

            return normalCheck && QEnemiesCheck;
        }

        public static Vector3 GetSmartQPosition()
        {
            if (!MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.smartq") ||
                !Variables.spells[SpellSlot.E].IsEnabledAndReady(Variables.Orbwalker.ActiveMode))
            {
                return Vector3.Zero;
            }

            const int currentStep = 30;
            var direction = ObjectManager.Player.Direction.LSTo2D().LSPerpendicular();
            for (var i = 0f; i < 360f; i += currentStep)
            {
                var angleRad = LeagueSharp.Common.Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.LSTo2D() + (300f * direction.LSRotated(angleRad));
                if (CondemnLogic.GetCondemnTarget(rotatedPosition.To3D()).LSIsValidTarget() && rotatedPosition.To3D().IsSafe())
                {
                    return rotatedPosition.To3D();
                }
            }

            return Vector3.Zero;
        }

        public static List<Vector2> GetEnemyPoints(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList = Variables.EnemiesClose.Select(enemy => new Geometry.Circle(enemy.ServerPosition.LSTo2D(), (dynamic ? (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) : staticRange) + enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = Geometry.ClipPolygons(polygonsList);
            var pointList = pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y)).Where(currentPoint => !currentPoint.LSIsWall()).ToList();
            return pointList;
        }
       
    }
}
