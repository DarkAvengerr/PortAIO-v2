using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Functions.Objects
{

    public static class Heroes
    {
        #region Public Methods

        /// <summary>
        ///     Gets the allies.
        /// </summary>
        /// <returns>
        /// </returns>
        public static List<AIHeroClient> GetAllies() => ObjectManager.Get<AIHeroClient>().Where(ally => !ally.IsEnemy).ToList();

        /// <summary>
        ///     Gets the allies.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="player">
        ///     The player.
        /// </param>
        /// <returns>
        /// </returns>
        public static List<AIHeroClient> GetAllies(float range, AIHeroClient player) => GetAllies().Where(ally => ally.Distance(player)<range).ToList();

        /// <summary>
        ///     Gets the enemies.
        /// </summary>
        /// <returns>
        /// </returns>
        public static List<AIHeroClient> GetEnemies() => ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy).ToList();

        /// <summary>
        ///     Gets the enemies.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        /// </returns>
        public static List<AIHeroClient> GetEnemies(float range) => GetEnemies().Where(enemy => enemy.IsValidTarget(range)).ToList();

        #endregion Public Methods
    }

}