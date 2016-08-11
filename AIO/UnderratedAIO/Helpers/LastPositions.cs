using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Enumerations;

// using Beaving.s.Baseult;

namespace UnderratedAIO.Helpers
{
    public class RecallInf
    {
        public int NetworkID;
        public int Start;
        public int Duration;
        public TeleportStatus Status;
        public TeleportType Type;

        public RecallInf(int netid, TeleportStatus stat, TeleportType tpe, int dura, int star = 0)
        {
            NetworkID = netid;
            Status = stat;
            Type = tpe;
            Duration = dura;
            Start = star;
        }
    }


    internal class LastPositions
    {
        public static List<Positions> Enemies;
        private static Menu configMenu;
        public bool enabled = true;
        public static Spell R;
        private float LastUltTime;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public LastPositions(Menu config)
        {
            configMenu = config;
            R = new Spell(SpellSlot.R);
            if (player.ChampionName == "Ezreal")
            {
                R.SetSkillshot(1000f, 160f, 2000f, false, SkillshotType.SkillshotLine);
            }
            if (player.ChampionName == "Jinx")
            {
                R.SetSkillshot(600f, 140f, 1700f, false, SkillshotType.SkillshotLine);
            }
            if (player.ChampionName == "Ashe")
            {
                R.SetSkillshot(250f, 130f, 1600f, false, SkillshotType.SkillshotLine);
            }
            if (player.ChampionName == "Draven")
            {
                R.SetSkillshot(400f, 160f, 2000f, false, SkillshotType.SkillshotLine);
            }
            config.AddItem(new MenuItem("UseR", "Use R")).SetValue(true);
            config.AddItem(new MenuItem("RandomUltDrawings", "Draw possible place")).SetValue(false);
            Menu DontUlt = new Menu("Don't Ult", "DontUltRandomUlt");
            foreach (var e in HeroManager.Enemies)
            {
                DontUlt.AddItem(new MenuItem(e.ChampionName + "DontUltRandomUlt", e.ChampionName)).SetValue(false);
            }
            config.AddSubMenu(DontUlt);
            Enemies = HeroManager.Enemies.Select(x => new Positions(x)).ToList();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Teleport.OnTeleport += Obj_AI_Base_OnTeleport;
        }

        private void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
            {
                return;
            }

            var info = new RecallInf(unit.NetworkId, args.Status, args.Type, args.Duration, args.Start);
            Enemies.Find(x => x.Player.NetworkId == unit.NetworkId).RecallData.Update(info);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!configMenu.Item("RandomUltDrawings").GetValue<bool>() || !enabled)
            {
                return;
            }
            foreach (var enemy in
                Enemies.Where(
                    x =>
                        x.Player.IsValid<AIHeroClient>() && !x.Player.IsDead &&
                        x.RecallData.Recall.Status == TeleportStatus.Start &&
                        x.RecallData.Recall.Type == TeleportType.Recall)
                    .OrderBy(x => x.RecallData.GetRecallTime()))
            {
                var trueDist = Math.Abs(enemy.LastSeen - enemy.RecallData.RecallStartTime) / 1000 *
                               enemy.Player.MoveSpeed;
                var dist = (Math.Abs(enemy.LastSeen - enemy.RecallData.RecallStartTime) / 1000 * enemy.Player.MoveSpeed) -
                           enemy.Player.MoveSpeed / 3;
                if (dist > 1500)
                {
                    return;
                }

                if (dist < 50)
                {
                    dist = 50;
                }
                var line = getpos(enemy, dist);
                dist = enemy.Player.Distance(line);
                Vector3 pos = line;
                if (enemy.Player.IsVisible)
                {
                    pos = enemy.Player.Position;
                }
                else
                {
                    pos =
                        CombatHelper.PointsAroundTheTarget(enemy.Player.Position, trueDist)
                            .Where(
                                p =>
                                    !p.IsWall() && line.Distance(p) < dist &&
                                    Environment.Map.GetPath(enemy.Player, p) < trueDist)
                            .OrderByDescending(p => NavMesh.IsWallOfGrass(p, 10))
                            .ThenBy(p => line.Distance(p))
                            .FirstOrDefault();
                }
                if (pos != null)
                {
                    Drawing.DrawCircle(pos, 50, Color.Red);
                }
                Drawing.DrawCircle(line, dist, Color.LawnGreen);
            }
        }

        private Vector3 getpos(Positions enemy, float dist)
        {
            var line = enemy.Player.Position.Extend(enemy.predictedpos, dist);
            if (enemy.Player.Position.Distance(enemy.predictedpos) < dist &&
                ((enemy.LastSeen - enemy.RecallData.RecallStartTime) / 1000) < 1)
            {
                line = enemy.predictedpos;
            }
            return line;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            float time = System.Environment.TickCount;
            foreach (Positions enemyInfo in Enemies.Where(x => x.Player.IsVisible && !x.Player.IsDead && !player.IsDead)
                )
            {
                enemyInfo.LastSeen = time;
                var prediction = Prediction.GetPrediction(enemyInfo.Player, 4);
                if (prediction != null)
                {
                    enemyInfo.predictedpos = prediction.UnitPosition;
                }
            }
            if (!configMenu.Item("UseR").GetValue<bool>() || !R.IsReady() || !enabled)
            {
                return;
            }
            foreach (Positions enemy in
                Enemies.Where(
                    x =>
                        x.Player.IsValid<AIHeroClient>() && !x.Player.IsDead &&
                        !configMenu.Item(x.Player.ChampionName + "DontUltRandomUlt").GetValue<bool>() &&
                        x.RecallData.Recall.Status == TeleportStatus.Start &&
                        x.RecallData.Recall.Type == TeleportType.Recall)
                    .OrderBy(x => x.RecallData.GetRecallTime()))
            {
                if (!checkdmg(enemy.Player))
                {
                    continue;
                }
                var dist = (Math.Abs(enemy.LastSeen - enemy.RecallData.RecallStartTime) / 1000 * enemy.Player.MoveSpeed) -
                           enemy.Player.MoveSpeed / 3;
                var line = getpos(enemy, dist);
                Vector3 pos = line;
                if (enemy.Player.IsVisible)
                {
                    pos = enemy.Player.Position;
                }
                else
                {
                    var trueDist = Math.Abs(enemy.LastSeen - enemy.RecallData.RecallStartTime) / 1000 *
                                   enemy.Player.MoveSpeed;

                    if (dist > 1500)
                    {
                        return;
                    }
                    pos =
                        CombatHelper.PointsAroundTheTarget(enemy.Player.Position, trueDist)
                            .Where(
                                p =>
                                    !p.IsWall() && line.Distance(p) < dist &&
                                    Environment.Map.GetPath(enemy.Player, p) < trueDist)
                            .OrderByDescending(p => NavMesh.IsWallOfGrass(p, 10))
                            .ThenBy(p => line.Distance(p))
                            .FirstOrDefault();
                }
                if (pos != null)
                {
                    kill(enemy, new Vector3(pos.X, pos.Y, 0));
                }
            }
        }

        private bool CheckShieldTower(Vector3 pos)
        {
            return
                ObjectManager.Get<Obj_AI_Turret>()
                    .Any(t => t.Distance(pos) < 1100f && t.HasBuff("SRTurretSecondaryShielder"));
        }

        private void kill(Positions positions, Vector3 pos)
        {
            if (R.IsReady() && pos.Distance(positions.Player.Position) < 1200 && pos.CountAlliesInRange(1800) < 1)
            {
                if (checkdmg(positions.Player) && UltTime(pos) < positions.RecallData.GetRecallTime() &&
                    !isColliding(pos) && !CheckShieldTower(pos))
                {
                    R.Cast(pos);
                }
            }
        }

        private bool isColliding(Vector3 pos)
        {
            if (player.ChampionName == "Draven" && player.ChampionName == "Ashe" && player.ChampionName == "Jinx")
            {
                var width = 160f;
                if (player.ChampionName == "Ashe")
                {
                    width = 130f;
                }
                if (player.ChampionName == "Jinx")
                {
                    width = 140f;
                }
                var input = new PredictionInput { Radius = width, Unit = player, };

                input.CollisionObjects[0] = CollisionableObjects.Heroes;

                return Collision.GetCollision(new List<Vector3> { pos }, input).Any();
            }
            return false;
        }

        private float UltTime(Vector3 pos)
        {
            var dist = player.ServerPosition.Distance(pos);
            if (player.ChampionName == "Ezreal")
            {
                return (dist / 2000) * 1000 + 1000;
            }
            //Beaving's calculations
            if (player.ChampionName == "Jinx" && dist > 1350)
            {
                const float accelerationrate = 0.3f;

                var acceldifference = dist - 1350f;

                if (acceldifference > 150f)
                {
                    acceldifference = 150f;
                }

                var difference = dist - 1500f;
                return (dist /
                        ((1350f * 1700f + acceldifference * (1700f + accelerationrate * acceldifference) +
                          difference * 2200f) / dist)) * 1000 + 250;
            }
            if (player.ChampionName == "Ashe")
            {
                return (dist / 1600) * 1000 + 250;
            }
            if (player.ChampionName == "Draven")
            {
                return (dist / 2000) * 1000 + 400;
            }
            return 0;
        }

        private bool checkdmg(AIHeroClient target)
        {
            if (player.ChampionName == "Ezreal" || player.ChampionName == "Draven")
            {
                if (R.GetDamage(target) * 0.7 > target.Health)
                {
                    return true;
                }
            }
            else
            {
                var dmg = R.GetDamage(target);
                if (player.ChampionName == "Jinx")
                {
                    dmg = R.GetDamage(target, 1);
                }
                if (dmg > target.Health)
                {
                    return true;
                }
            }
            return false;
        }
    }

    internal class Positions
    {
        public AIHeroClient Player;
        public float LastSeen;
        public Vector3 predictedpos;

        public RecallData RecallData;

        public Positions(AIHeroClient player)
        {
            Player = player;
            RecallData = new RecallData(this);
        }
    }

    internal class RecallData
    {
        public Positions Positions;
        public RecallInf Recall;
        public RecallInf Aborted;
        public float AbortTime;
        public float RecallStartTime;
        public bool started;


        public RecallData(Positions positions)
        {
            Positions = positions;
            Recall = new RecallInf(
                Positions.Player.NetworkId, TeleportStatus.Unknown, TeleportType.Unknown, 0);
        }

        public float GetRecallTime()
        {
            float time = System.Environment.TickCount;
            float countdown = 0;

            if (time - AbortTime < 2000)
            {
                countdown = Aborted.Duration - (AbortTime - Aborted.Start);
            }
            else if (AbortTime > 0)
            {
                countdown = 0;
            }
            else
            {
                countdown = Recall.Start + Recall.Duration - time;
            }

            return countdown < 0 ? 0 : countdown;
        }

        public Positions Update(RecallInf newData)
        {
            if (newData.Type == TeleportType.Recall && newData.Status == TeleportStatus.Abort)
            {
                Aborted = Recall;
                AbortTime = System.Environment.TickCount;
                started = false;
            }
            else
            {
                AbortTime = 0;
            }
            if (newData.Type == TeleportType.Recall && newData.Status == TeleportStatus.Finish)
            {
                started = false;
            }
            if (newData.Type == TeleportType.Recall && newData.Status == TeleportStatus.Start)
            {
                if (!started)
                {
                    RecallStartTime = System.Environment.TickCount;
                }
                started = true;
            }
            Recall = newData;
            return Positions;
        }
    }
}