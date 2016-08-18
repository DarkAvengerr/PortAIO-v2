using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.LogicProvider
{
    using System.Collections.Generic;
    using System.Linq;

    using CommonEx.Extensions;
    using CommonEx.Objects;
    using CommonEx.Utility;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     LogicProvider for (R) LastBreath
    /// </summary>
    internal class LastBreathLogicProvider
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets all executions around a execution that are in range of lastbreath
        /// </summary>
        /// <param name="execution">
        ///     LastBreath execution object
        /// </param>
        /// <returns></returns>
        public List<LastBreath> GetExecutionsAround(LastBreath execution)
        {
            var result = new List<LastBreath>();

            foreach (var target in execution.AffectedEnemies)
            {
                result.Add(new LastBreath(target));
            }

            return result;
        }

        /// <summary>
        ///     Returns the execution that has the lowest remaining airbone time
        /// </summary>
        public LastBreath LeastRemainingAirboneTime(List<LastBreath> executions)
        {
            if (executions.Count > 0)
            {
                return executions.MinOrDefault(x => x.MinRemainingAirboneTime);
            }
            return null;
        }

        /// <summary>
        ///     Returns the execution where most damage can get dealt
        /// </summary>
        /// <param name="executions">
        ///     LastBreath execution object
        /// </param>
        /// <returns>
        ///     LastBreath execution object
        /// </returns>
        public LastBreath MostDamageDealt(List<LastBreath> executions)
        {
            var damageDic = new Dictionary<LastBreath, float>();

            if (executions != null && executions.Any())
            {
                foreach (var x in executions)
                {
                    damageDic.Add(x, x.DamageDealt);
                }

                return damageDic.MaxOrDefault(x => x.Value).Key;
            }
            return null;
        }

        /// <summary>
        ///     Returns the most safe enemy to use ultimate on
        /// </summary>
        public LastBreath MostSafety(List<LastBreath> executions)
        {
            return executions?.MinOrDefault(x => x.DangerValue);
        }

        // TODO
        /// <summary>
        ///     Returns a bool that determines if it is a good time to use lastbreath or not
        /// </summary>
        /// <param name="execution">The current target</param>
        /// <param name="pathBase">The PathBase to the target</param>
        /// <param name="buffer">a time buffer</param>
        /// <returns></returns>
        public bool ShouldCastNow(LastBreath execution, YasuoPath<Point, ConnectionBase<Point>> pathBase = null, int buffer = 10)
        {
            if (execution == null)
            {
                return false;
            }

            // Last possible second ultimate
            if (execution.MinRemainingAirboneTime <= 1 + buffer + Game.Ping)
            {
                return true;
            }

            // if no PathBase is given
            if (pathBase == null)
            {
                var playerpath = GlobalVariables.Player.GetPath(execution.Target.ServerPosition);
                var playerpathtime = Math.GetPathLenght(playerpath) / GlobalVariables.Player.MoveSpeed;

                // if walking is requires less time than remaining knockup time
                if (playerpathtime <= execution.MinRemainingAirboneTime + buffer + Game.Ping)
                {
                    return false;
                }

                // Instant Ult in 1 v 1 because armor pen and less time for enemiy to get spells up also you can't gapclose
                if (execution.EndPosition.CountEnemiesInRange(1500) == 0 && !execution.Target.InAutoAttackRange())
                {
                    return true;
                }
            }

            // if a PathBase is given and the PathBase time is shorter than the knockup time
            else
            {
                if (pathBase.PathCost <= execution.MinRemainingAirboneTime + buffer + Game.Ping)
                {
                    return false;
                }
            }

            return execution.MinRemainingAirboneTime <= Game.Ping + buffer;
        }

        #endregion
    }
}