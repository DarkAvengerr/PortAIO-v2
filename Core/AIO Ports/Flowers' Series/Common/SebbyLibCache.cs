using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public class SebbyLibCache
    {
        private static readonly List<Obj_AI_Base> AllMinionsObj = new List<Obj_AI_Base>();
        public static readonly List<Obj_AI_Base> MinionsListEnemy = new List<Obj_AI_Base>();
        private static readonly List<Obj_AI_Base> MinionsListAlly = new List<Obj_AI_Base>();
        private static readonly List<Obj_AI_Base> MinionsListNeutral = new List<Obj_AI_Base>();
        public static List<Obj_AI_Turret> TurretList = ObjectManager.Get<Obj_AI_Turret>().ToList();
        public static List<Obj_HQ> NexusList = ObjectManager.Get<Obj_HQ>().ToList();
        public static List<Obj_BarracksDampener> InhiList = ObjectManager.Get<Obj_BarracksDampener>().ToList();

        static SebbyLibCache()
        {
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValid))
            {
                AddMinionObject(minion);

                if (!minion.IsAlly)
                {
                    AllMinionsObj.Add(minion);
                }
            }

            GameObject.OnCreate += OnCreate;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            MinionsListEnemy.RemoveAll(minion => !IsValidMinion(minion));
            MinionsListNeutral.RemoveAll(minion => !IsValidMinion(minion));
            MinionsListAlly.RemoveAll(minion => !IsValidMinion(minion));
            AllMinionsObj.RemoveAll(minion => !IsValidMinion(minion));
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;

            if (minion != null)
            {
                AddMinionObject(minion);

                if (!minion.IsAlly)
                {
                    AllMinionsObj.Add(minion);
                }
            }
        }

        private static void AddMinionObject(Obj_AI_Minion minion)
        {
            if (minion.MaxHealth >= 225)
            {
                if (minion.Team == GameObjectTeam.Neutral)
                {
                    MinionsListNeutral.Add(minion);
                }
                else if (minion.MaxMana == 0 && minion.MaxHealth >= 250)
                {
                    if (minion.Team == GameObjectTeam.Unknown)
                    {
                        return;
                    }

                    if (minion.Team != ObjectManager.Player.Team)
                    {
                        MinionsListEnemy.Add(minion);
                    }
                    else if (minion.Team == ObjectManager.Player.Team)
                    {
                        MinionsListAlly.Add(minion);
                    }
                }
            }
        }

        public static List<Obj_AI_Base> GetMinions(Vector3 from, float range = float.MaxValue,
            MinionTeam team = MinionTeam.Enemy)
        {
            switch (team)
            {
                case MinionTeam.Enemy:
                {
                    return MinionsListEnemy.FindAll(minion => CanReturn(minion, from, range));
                }
                case MinionTeam.Ally:
                {
                    return MinionsListAlly.FindAll(minion => CanReturn(minion, from, range));
                }
                case MinionTeam.Neutral:
                {
                    return
                        MinionsListNeutral.Where(minion => CanReturn(minion, from, range))
                            .OrderByDescending(minion => minion.MaxHealth)
                            .ToList();
                }
                case MinionTeam.NotAlly:
                {
                    return AllMinionsObj.FindAll(minion => CanReturn(minion, from, range));
                }
                default:
                {
                    return AllMinionsObj.FindAll(minion => CanReturn(minion, from, range));
                }
            }
        }

        private static bool IsValidMinion(Obj_AI_Base minion)
        {
            return minion != null && minion.IsValid && !minion.IsDead;
        }

        private static bool CanReturn(Obj_AI_Base minion, Vector3 from, float range)
        {
            if (minion != null && minion.IsValid && !minion.IsDead && minion.IsVisible && minion.IsTargetable)
            {
                if (range == float.MaxValue)
                    return true;

                if (range == 0)
                {
                    return Orbwalking.InAutoAttackRange(minion);
                }

                return Vector2.DistanceSquared(from.To2D(), minion.Position.To2D()) < range * range;
            }

            return false;
        }
    }
}
