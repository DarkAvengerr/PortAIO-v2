using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public class ObjectCache
    {
        private static readonly HashSet<AIHeroClient> EnemyHeroes = new HashSet<AIHeroClient>();
        private static readonly HashSet<Obj_AI_Minion> EnemyMinions = new HashSet<Obj_AI_Minion>();
        private static int _lastTickUpdate = (int) (Game.Time * 0x3E8);

        static ObjectCache()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy))
            {
                EnemyHeroes.Add(hero);
            }
            foreach (
                var minion in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsEnemy && !m.IsDead && m.IsTargetable))
            {
                EnemyMinions.Add(minion);
            }
        }

        public static void OnCreate(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && !EnemyMinions.Contains(minion) && minion.IsEnemy && minion.IsTargetable)
            {
                EnemyMinions.Add(minion);
            }
        }

        public static void OnDelete(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && EnemyMinions.Contains(minion))
            {
                EnemyMinions.Remove(minion);
            }
        }

        public static void OnUpdate(EventArgs args)
        {
            if ((int) (Game.Time * 0x3E8) - _lastTickUpdate > 0x3E8)
            {
                EnemyMinions.Clear();
                foreach (var minion in
                    ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsEnemy && !m.IsDead && m.IsTargetable))
                {
                    EnemyMinions.Add(minion);
                }
                _lastTickUpdate = (int) (Game.Time * 0x3E8);
                return;
            }

            foreach (var minion in GetMinions().Where(m => !m.IsValidTarget()))
            {
                EnemyMinions.Remove(minion);
            }
        }

        public static AIHeroClient[] GetHeroes()
        {
            return EnemyHeroes.ToArray();
        }

        public static Obj_AI_Minion[] GetMinions()
        {
            return EnemyMinions.ToArray();
        }

        public static List<Obj_AI_Base> GetMinions(Vector3 from,
            float range,
            MinionTypes type = MinionTypes.All,
            MinionTeam team = MinionTeam.Enemy,
            MinionOrderTypes order = MinionOrderTypes.Health)
        {
            var result = (from minion in GetMinions()
                where minion.IsValidTarget(range, false, @from)
                let minionTeam = minion.Team
                where
                    team == MinionTeam.Neutral && minionTeam == GameObjectTeam.Neutral ||
                    team == MinionTeam.Ally &&
                    minionTeam ==
                    (ObjectManager.Player.Team == GameObjectTeam.Chaos ? GameObjectTeam.Chaos : GameObjectTeam.Order) ||
                    team == MinionTeam.Enemy &&
                    minionTeam ==
                    (ObjectManager.Player.Team == GameObjectTeam.Chaos ? GameObjectTeam.Order : GameObjectTeam.Chaos) ||
                    team == MinionTeam.NotAlly && minionTeam != ObjectManager.Player.Team ||
                    team == MinionTeam.NotAllyForEnemy &&
                    (minionTeam == ObjectManager.Player.Team || minionTeam == GameObjectTeam.Neutral) ||
                    team == MinionTeam.All
                where
                    minion.IsMelee() && type == MinionTypes.Melee || !minion.IsMelee() && type == MinionTypes.Ranged ||
                    type == MinionTypes.All
                where IsMinion(minion) || minionTeam == GameObjectTeam.Neutral
                select minion).Cast<Obj_AI_Base>().ToList();

            switch (order)
            {
                case MinionOrderTypes.Health:
                    result = result.OrderBy(o => o.Health).ToList();
                    break;
                case MinionOrderTypes.MaxHealth:
                    result = result.OrderBy(o => o.MaxHealth).Reverse().ToList();
                    break;
            }

            return result;
        }

        public static List<Obj_AI_Base> GetMinions(float range,
            MinionTypes type = MinionTypes.All,
            MinionTeam team = MinionTeam.Enemy,
            MinionOrderTypes order = MinionOrderTypes.Health)
        {
            return GetMinions(ObjectManager.Player.ServerPosition, range, type, team, order);
        }

        public static bool IsMinion(Obj_AI_Minion minion, bool includeWards = false)
        {
            var name = minion.BaseSkinName.ToLower();
            return name.Contains("minion") || (includeWards && (name.Contains("ward") || name.Contains("trinket")));
        }
    }
}