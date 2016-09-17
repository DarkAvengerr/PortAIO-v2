using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Functions.Objects
{
    public static class Minions
    {
        public static List<Obj_AI_Minion> GetEnemyMinions() => ObjectManager.Get<Obj_AI_Minion>().Where(enemy => enemy.IsEnemy).ToList();

        public static List<Obj_AI_Minion> GetEnemyMinions(float range) => GetEnemyMinions().Where(minion => minion.IsValidTarget(range)).ToList();
    }
}
