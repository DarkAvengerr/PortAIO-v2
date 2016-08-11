using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using System.Text.RegularExpressions;
using Color = System.Drawing.Color;
using EloBuddy;

namespace HeavenStrikeAzir
{
    public static class OrbwalkCommands
    {
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        private static float lastAA;
        private static bool OnFinishAttack;
        private static int lastAAcommandTick;
        private static int lastMovecommandTick;
        public static void Initialize()
        {
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnDoCast;
            Game.OnUpdate += Game_OnUpdate;
            Program._orbwalker.SetAttack(false);
            Program._orbwalker.SetMovement(false);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && CanMove())
            {
                MoveTo(Game.CursorPos);
            }
        }

        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.SData.IsAutoAttack())
                LeagueSharp.Common.Utility.DelayAction.Add(50 - Game.Ping, () => OnFinishAttack = true);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name.ToLower().Contains("attack"))
                lastAA = Utils.GameTimeTickCount - Game.Ping / 2 + 50;
            if (args.SData.Name.ToLower().Contains("attacksoldier"))
                OnFinishAttack = true;
        }

        public static bool CanDoAttack()
        {
            if (lastAA <= Utils.TickCount)
            {
                return Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= lastAA + Player.AttackDelay * 1000
                    && Utils.GameTimeTickCount - lastAAcommandTick >= Game.Ping + 250;
            }

            return false;
        }
        public static void MoveTo(Vector3 Position)
        {
            if (Utils.GameTimeTickCount - lastMovecommandTick < 150)
                return;
            if (Player.Position.Distance(Position) <= 80)
                return;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Position);
            lastMovecommandTick = Utils.GameTimeTickCount;
        }
        public static bool CanMove()
        {
            if (OnFinishAttack)
                return true;
            return (Utils.GameTimeTickCount + Game.Ping / 2 >= lastAA + Player.AttackCastDelay * 1000 + 80)
                                    && Utils.GameTimeTickCount - lastAAcommandTick >= Game.Ping + 250;
        }
        public static void AttackTarget(AttackableUnit target)
        {
            Orbwalking.LastAATick = Utils.GameTimeTickCount + Game.Ping;
            lastAAcommandTick = Utils.GameTimeTickCount;
            if (Soldiers.enemies.Any(x => x.NetworkId == target.NetworkId)
                || Soldiers.soldierattackminions.Any(x => x.NetworkId == target.NetworkId))
                OnFinishAttack = true;
            else
                OnFinishAttack = false;
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
        }
        private static bool ShouldWait()
        {
            var attackcastdelay = Player.AttackCastDelay * 1000 + Game.Ping;
            var attackdelay = Player.AttackDelay * 1000;
            return
            (
                Soldiers.autoattackminions
                    .Any(
                        minion =>
                            minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                            HealthPrediction.LaneClearHealthPrediction(
                            minion, (int)attackcastdelay + (int)attackdelay * 2, 0) <=
                            Player.GetAutoAttackDamage(minion))
                ||
                Soldiers.soldierattackminions
                    .Any(
                        minion =>
                            minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                            HealthPrediction.LaneClearHealthPrediction(
                            minion, (int)attackcastdelay + (int)attackdelay * 2, 0) <=
                            Program.Wdamage(minion))
                ||
                Soldiers.splashautoattackminions
                    .Any(
                        minions => minions.SplashAutoAttackMinions
                            .Any(
                                minion =>
                                    minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                                    HealthPrediction.LaneClearHealthPrediction(
                                    minion, (int)attackcastdelay + (int)attackdelay * 2, 0) <=
                                    Program.Wdamage(minion)))
            );

        }
        public static AttackableUnit GetClearMinionsAndBuildings()
        {
            AttackableUnit result = null;
            /*Killable Minion*/

            var MinionList = new List<Obj_AI_Minion>();
            MinionList.AddRange(Soldiers.soldierattackminions);
            MinionList.AddRange(Soldiers.autoattackminions);
            MinionList = MinionList
                            .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                            .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                            .ThenBy(minion => minion.Health)
                            .ThenByDescending(minion => minion.MaxHealth).ToList();
            if (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                   Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                foreach (var minion in Soldiers.autoattackminions)
                {
                    //var t = 0;
                    var t = (int)(Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2;
                    var predHealth = HealthPrediction.GetHealthPrediction(minion, t, 0);

                    if (minion.Team != GameObjectTeam.Neutral)
                    {
                        if (predHealth > 0 && predHealth <= Player.GetAutoAttackDamage(minion, true))
                        {
                            return minion;
                        }
                    }
                }
                foreach (var minions in Soldiers.splashautoattackminions)
                {
                    var t = 0;// (int)(Player.AttackCastDelay * 1000) - 200 + Game.Ping;
                    var MainMinionPredHealth = HealthPrediction.GetHealthPrediction(minions.MainMinion, t, 0);
                    if (minions.MainMinion.Team != GameObjectTeam.Neutral)
                    {
                        if (MainMinionPredHealth > 0 && MainMinionPredHealth <= Program.Wdamage(minions.MainMinion))
                        {
                            return minions.MainMinion;
                        }
                    }
                    foreach (var minion in minions.SplashAutoAttackMinions)
                    {
                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, 0);
                        if (minion.Team != GameObjectTeam.Neutral)
                        {
                            if (minion.Health > 0 && minion.Health <= Program.Wdamage(minion))
                            {
                                return minions.MainMinion;
                            }
                        }
                    }
                }
            }
            if (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                /* turrets */
                foreach (var turret in
                    ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && Orbwalking.InAutoAttackRange(t)))
                {
                    return turret;
                }

                /* inhibitor */
                foreach (var turret in
                    ObjectManager.Get<Obj_BarracksDampener>().Where(t => t.IsValidTarget() && Orbwalking.InAutoAttackRange(t)))
                {
                    return turret;
                }

                /* nexus */
                foreach (var nexus in
                    ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && Orbwalking.InAutoAttackRange(t)))
                {
                    return nexus;
                }
            }
            /*Champions*/
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit)
            {
                if (Soldiers.enemies.Any())
                {
                    var target = Soldiers.enemies.OrderByDescending(x => x.Health).LastOrDefault();
                    if (target.IsValidTarget())
                        return target;
                }
                if (Soldiers.splashautoattackchampions.Any())
                {
                    var splashAutoAttackChampion = Soldiers.splashautoattackchampions
                        .OrderByDescending(x => x.SplashAutoAttackChampions.MinOrDefault(y => y.Health).Health).LastOrDefault();
                    if (splashAutoAttackChampion != null)
                    {
                        var target = splashAutoAttackChampion.MainMinion;
                        if (target.IsValidTarget())
                            return target;
                    }
                }
                if (!Soldiers.enemies.Any() && !Soldiers.splashautoattackchampions.Any())
                {
                    var target = Program._orbwalker.GetTarget();
                    if (target.IsValidTarget() && !target.IsZombie && target is AIHeroClient)
                    {
                        return target;
                    }
                }
            }
            /*Jungle minions*/
            if (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                result =
                    MinionList
                        .Where(
                            mob =>
                                mob.IsValidTarget() && mob.Team == GameObjectTeam.Neutral && mob.CharData.BaseSkinName != "gangplankbarrel")
                        .MaxOrDefault(mob => mob.MaxHealth);
                if (result != null)
                {
                    return result;
                }
            }


            /*Lane Clear minions*/
            if (Program._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (!ShouldWait())
                {
                    var t = 0;
                    var t2 = (int)Player.AttackDelay * 1000;
                    var arrangedsplash =
                        Soldiers.splashautoattackminions.OrderByDescending(x => x.SplashAutoAttackMinions.Count());
                    foreach (var minions in arrangedsplash)
                    {
                        var damage = Program.Wdamage(minions.MainMinion);
                        var allminions = new List<Obj_AI_Minion>();
                        allminions.Add(minions.MainMinion);
                        allminions.AddRange(minions.SplashAutoAttackMinions);
                        if (
                            allminions.All(
                                x =>
                                    HealthPrediction.LaneClearHealthPrediction(x, t + t2, 0) >= damage * 2 ||
                                    Math.Abs(HealthPrediction.LaneClearHealthPrediction(x, t + t2, 0) - x.Health) <=
                                    float.Epsilon))
                            return minions.MainMinion;
                    }
                    var arrangedattack = Soldiers.autoattackminions
                        .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                        .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                        .ThenBy(minion => minion.Health)
                        .ThenByDescending(minion => minion.MaxHealth).ToList();
                    foreach (var minion in arrangedattack)
                    {
                        var predHealth = HealthPrediction.LaneClearHealthPrediction(minion, t + t2, 0);
                        if (predHealth >= Player.GetAutoAttackDamage(minion) * 2 || Math.Abs(predHealth - minion.Health) < float.Epsilon)
                            return minion;
                    }
                }
            }
            return result;

        }
    }
}
