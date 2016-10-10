using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Functions.Objects
{

    public static class Minions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        /// <returns>
        /// </returns>
        public static List<Obj_AI_Minion> GetEnemyMinions() => ObjectManager.Get<Obj_AI_Minion>().Where(enemy => enemy.IsEnemy).ToList();

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        /// </returns>
        public static List<Obj_AI_Minion> GetEnemyMinions(float range) => GetEnemyMinions().Where(minion => minion.IsValidTarget(range)).ToList();

        /// <summary>
        ///     Gets the enemy minions2.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        /// </returns>
        public static List<Obj_AI_Base> GetEnemyMinions2(float range) => MinionManager.GetMinions(range);

        #endregion Public Methods
    }

}