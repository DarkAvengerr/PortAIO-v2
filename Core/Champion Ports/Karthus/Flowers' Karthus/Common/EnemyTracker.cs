using EloBuddy; 
 using LeagueSharp.Common; 
 using EloBuddy; 
 using LeagueSharp.Common; 
 using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Karthus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    // This Part From BaseUlt3 
    public class EnemyTracker
    {
        public static List<AIHeroClient> EnemyList;
        public static List<EnemyInfo> enemyInfo;

        public EnemyTracker()
        {
            EnemyList = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy).ToList();

            enemyInfo = EnemyList.Select(x => new EnemyInfo(x)).ToList();

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            var time = Utils.TickCount;

            foreach (var enemy in enemyInfo.Where(x => x.target.IsHPBarRendered))
                enemy.LastSeen = time;
        }

        public static float GetTargetHealth(EnemyInfo enemyinfo, float additionalTime)
        {
            if (enemyinfo.target.IsHPBarRendered)
                return enemyinfo.target.Health;

            var predictedHealth = enemyinfo.target.Health + enemyinfo.target.HPRegenRate * ((Utils.TickCount - enemyinfo.LastSeen + additionalTime * 1000) / 1000f);

            return predictedHealth > enemyinfo.target.MaxHealth ? enemyinfo.target.MaxHealth : predictedHealth;
        }
    }

    public class EnemyInfo
    {
        public AIHeroClient target;
        public int LastSeen;

        public EnemyInfo(AIHeroClient enemy)
        {
            target = enemy;
        }
    }
}
