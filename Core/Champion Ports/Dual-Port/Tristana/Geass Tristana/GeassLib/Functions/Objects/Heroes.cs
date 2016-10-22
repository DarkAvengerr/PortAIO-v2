using LeagueSharp;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Functions.Objects
{
    public static class Heroes
    {
        public static List<AIHeroClient> GetEnemies() => ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy).ToList();

        public static List<AIHeroClient> GetEnemies(float range) => GetEnemies().Where(enemy => enemy.IsValidTarget(range)).ToList();

        public static List<AIHeroClient> GetAllies() => ObjectManager.Get<AIHeroClient>().Where(ally => !ally.IsEnemy).ToList();

        public static List<AIHeroClient> GetAllies(float range, AIHeroClient player)=> GetAllies().Where(ally => ally.Distance(player) < range).ToList();
    }
}