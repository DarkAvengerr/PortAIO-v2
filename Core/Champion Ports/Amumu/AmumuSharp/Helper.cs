using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace AmumuSharp
{
    internal class EnemyInfo
    {
        public AIHeroClient Player;
        public int LastSeen;
        //public int LastPinged;

        public EnemyInfo(AIHeroClient player)
        {
            Player = player;
        }
    }

    internal class Helper
    {
        public IEnumerable<AIHeroClient> EnemyTeam;
        public IEnumerable<AIHeroClient> OwnTeam;
        public List<EnemyInfo> EnemyInfo = new List<EnemyInfo>();

        public Helper()
        {
            var champions = ObjectManager.Get<AIHeroClient>().ToList();

            OwnTeam = champions.Where(x => x.IsAlly);
            EnemyTeam = champions.Where(x => x.IsEnemy);

            EnemyInfo = EnemyTeam.Select(x => new EnemyInfo(x)).ToList();

            Game.OnUpdate += Game_OnUpdate;
        }

        void Game_OnUpdate(EventArgs args)
        {
            var time = Utils.TickCount;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x => x.Player.IsHPBarRendered))
                enemyInfo.LastSeen = time;
        }

        public EnemyInfo GetPlayerInfo(AIHeroClient enemy)
        {
            return Program.Helper.EnemyInfo.Find(x => x.Player.NetworkId == enemy.NetworkId);
        }

        public float GetTargetHealth(EnemyInfo playerInfo, int additionalTime)
        {
            if (playerInfo.Player.IsHPBarRendered)
                return playerInfo.Player.Health;

            var predictedhealth = playerInfo.Player.Health + playerInfo.Player.HPRegenRate * ((Utils.TickCount - playerInfo.LastSeen + additionalTime) / 1000f);

            return predictedhealth > playerInfo.Player.MaxHealth ? playerInfo.Player.MaxHealth : predictedhealth;
        }

        public static T GetSafeMenuItem<T>(MenuItem item)
        {
            if (item != null)
                return item.GetValue<T>();

            return default(T);
        }
    }
}
