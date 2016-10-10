using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SoloVayne.Skills.Condemn;
using SoloVayne.Utility;
using SOLOVayne.Skills.Tumble.WardTracker;
using SOLOVayne.Utility.General;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Skills.Tumble
{
    class TumbleLogicProvider
    {
        public CondemnLogicProvider Provider = new CondemnLogicProvider();

        /// <summary>
        /// Gets the SOLO Vayne Q position using a patented logic!
        /// </summary>
        /// <returns></returns>
        public Vector3 GetSOLOVayneQPosition()
        {
            #region The Required Variables
            var positions = TumbleHelper.GetRotatedQPositions();
            var enemyPositions = TumbleHelper.GetEnemyPoints();
            var safePositions = positions.Where(pos => !enemyPositions.Contains(pos.To2D())).ToList();
            var BestPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f);
            var AverageDistanceWeight = .60f;
            var ClosestDistanceWeight = .40f;

            var bestWeightedAvg = 0f;

            var highHealthEnemiesNear =
                    HeroManager.Enemies.Where(m => !m.IsMelee && m.IsValidTarget(1300f) && m.HealthPercent > 7).ToList();

            var alliesNear = HeroManager.Allies.Count(ally => !ally.IsMe && ally.IsValidTarget(1500f, false));

            var enemiesNear =
                HeroManager.Enemies.Where(m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(m) + 300f + 65f)).ToList();
            #endregion


            #region 1 Enemy around only
            if (ObjectManager.Player.CountEnemiesInRange(1500f) <= 1)
            {
                //Logic for 1 enemy near
                var position = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f);
                return position.IsSafeEx() ? position : Vector3.Zero;
            }
            #endregion

            if (
                    enemiesNear.Any(
                        t =>
                            t.Health + 15 <
                            ObjectManager.Player.GetAutoAttackDamage(t) * 2 + Variables.spells[SpellSlot.Q].GetDamage(t)
                            && t.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(t) + 80f))
            {
                var QPosition =
                    ObjectManager.Player.ServerPosition.Extend(
                        enemiesNear.OrderBy(t => t.Health).First().ServerPosition, 300f);

                if (!QPosition.UnderTurret(true))
                {
                    return QPosition;
                }
            }

            #region Alone, 2 Enemies, 1 Killable
            if (enemiesNear.Count() <= 2)
            {
                if (
                    enemiesNear.Any(
                        t =>
                            t.Health + 15 <
                            ObjectManager.Player.GetAutoAttackDamage(t) * 2 + Variables.spells[SpellSlot.Q].GetDamage(t)
                            && t.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(t) + 80f))
                {
                    var QPosition =
                        ObjectManager.Player.ServerPosition.Extend(
                            highHealthEnemiesNear.OrderBy(t => t.Health).First().ServerPosition, 300f);

                    if (!QPosition.UnderTurret(true))
                    {
                        return QPosition;
                    }
                }
            }
            #endregion

            #region Alone, 2 Enemies, None Killable
            if (alliesNear == 0 && highHealthEnemiesNear.Count() <= 2)
            {
                //Logic for 2 enemies Near and alone

                //If there is a killable enemy among those. 
                var backwardsPosition = (ObjectManager.Player.ServerPosition.To2D() + 300f * ObjectManager.Player.Direction.To2D()).To3D();

                if (!backwardsPosition.UnderTurret(true))
                {
                    return backwardsPosition;
                }
            }
            #endregion

            #region Already in an enemy's attack range. 
            var closeNonMeleeEnemy = TumbleHelper.GetClosestEnemy(ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f));

            if (closeNonMeleeEnemy != null 
                && ObjectManager.Player.Distance(closeNonMeleeEnemy) <= closeNonMeleeEnemy.AttackRange - 85 
                && !closeNonMeleeEnemy.IsMelee)
            {
                return ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f).IsSafeEx()
                    ? ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f)
                    : Vector3.Zero;
            }
            #endregion

            #region Logic for multiple enemies / allies around.
            foreach (var position in safePositions)
            {
                var enemy = TumbleHelper.GetClosestEnemy(position);
                if (!enemy.IsValidTarget())
                {
                    continue;
                }

                var avgDist = TumbleHelper.GetAvgDistance(position);

                if (avgDist > -1)
                {
                    var closestDist = ObjectManager.Player.ServerPosition.Distance(enemy.ServerPosition);
                    var weightedAvg = closestDist * ClosestDistanceWeight + avgDist * AverageDistanceWeight;
                    if (weightedAvg > bestWeightedAvg && position.IsSafeEx())
                    {
                        bestWeightedAvg = weightedAvg;
                        BestPosition = position;
                    }
                }
            }
            #endregion

            var endPosition = (BestPosition.IsSafe()) ? BestPosition : Vector3.Zero;

            #region Couldn't find a suitable position, tumble to nearest ally logic
            if (endPosition == Vector3.Zero)
            {
                //Try to find another suitable position. This usually means we are already near too much enemies turrets so just gtfo and tumble
                //to the closest ally ordered by most health.
                var alliesClose = HeroManager.Allies.Where(ally => !ally.IsMe && ally.IsValidTarget(1500f, false)).ToList();
                if (alliesClose.Any() && enemiesNear.Any())
                {
                    var closestMostHealth =
                    alliesClose.OrderBy(m => m.Distance(ObjectManager.Player)).ThenByDescending(m => m.Health).FirstOrDefault();

                    if (closestMostHealth != null 
                        && closestMostHealth.Distance(enemiesNear.OrderBy(m => m.Distance(ObjectManager.Player)).FirstOrDefault()) 
                        > ObjectManager.Player.Distance(enemiesNear.OrderBy(m => m.Distance(ObjectManager.Player)).FirstOrDefault()))
                    {
                        var tempPosition = ObjectManager.Player.ServerPosition.Extend(closestMostHealth.ServerPosition,
                            300f);
                        if (tempPosition.IsSafeEx())
                        {
                            endPosition = tempPosition;
                        }
                    }
                    
                }

            }
            #endregion

            #region Couldn't find an ally, tumble inside bush
            var AmInBush = NavMesh.IsWallOfGrass(ObjectManager.Player.ServerPosition, 33);
            var closeEnemies = TumbleVariables.EnemiesClose.ToList();
            //I'm not in bush, all the enemies close are outside a bush
            if (!AmInBush && endPosition == Vector3.Zero)
            {
                var PositionsComplete = TumbleHelper.GetCompleteRotatedQPositions();
                foreach (var position in PositionsComplete)
                {
                    //The end position is a wall of grass
                    //All enemies are outside of the bush and at least 340 units away
                    //There are no detected wards in that bush
                    if (NavMesh.IsWallOfGrass(position, 33) 
                        && closeEnemies.All(m => m.Distance(position) > 340f && !NavMesh.IsWallOfGrass(m.ServerPosition, 40))
                        && !WardTrackerVariables.detectedWards.Any(m => NavMesh.IsWallOfGrass(m.Position, 33) 
                            && m.Position.Distance(position) < m.WardTypeW.WardVisionRange))
                    {
                        if (position.IsSafe())
                        {
                            endPosition = position;
                            break;
                        }
                    }
                }
            }

            #endregion


            #region Couldn't even tumble to ally, just go to mouse
            if (endPosition == Vector3.Zero)
            {
                var mousePosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f);
                if (mousePosition.IsSafe())
                {
                    endPosition = mousePosition;
                }
            }
            #endregion

            if (ObjectManager.Player.HealthPercent < 10 && ObjectManager.Player.CountEnemiesInRange(1500) > 1)
            {
                var position = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f);
                return position.IsSafeEx() ? position : endPosition;
            }

            return endPosition;
        }

        /// <summary>
        /// Gets the QE position for the Tumble-Condemn combo.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetQEPosition()
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !Variables.spells[SpellSlot.E].IsEnabledAndReady())
            {
                return Vector3.Zero;
            }

            const int currentStep = 45;
            var direction = ObjectManager.Player.Direction.To2D().Perpendicular();
            for (var i = 0f; i < 360f; i += 45)
            {
                var angleRad = Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.To2D() + (300f * direction.Rotated(angleRad));
                if (Provider.GetTarget(rotatedPosition.To3D()).IsValidTarget() && rotatedPosition.To3D().IsSafe())
                {
                    return rotatedPosition.To3D();
                }
            }

            return Vector3.Zero;
        }
    }
}
