using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Enumerations;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class ChampionInfo
    {
        public AIHeroClient Hero { get; set; }

        public Vector3 LastVisablePos { get; set; }
        public float LastVisableTime { get; set; }
        public Vector3 PredictedPos { get; set; }
        public Vector3 LastWayPoint { get; set; }

        public float StartRecallTime { get; set; }
        public float AbortRecallTime { get; set; }
        public float FinishRecallTime { get; set; }
        public bool IsJungler { get; set; }

        public Render.Sprite NormalSprite;
        public Render.Sprite HudSprite;
        public Render.Sprite MinimapSprite;
        public Render.Sprite SquareSprite;

        public ChampionInfo(AIHeroClient hero)
        {
            Hero = hero;
            if (hero.IsEnemy)
                NormalSprite = ImageLoader.CreateRadrarIcon(hero.ChampionName + "_Square_0", System.Drawing.Color.Red);
            else
                NormalSprite = ImageLoader.CreateRadrarIcon(hero.ChampionName + "_Square_0", System.Drawing.Color.GreenYellow);
            SquareSprite = ImageLoader.GetSprite(hero.ChampionName + "_Square_0");
            HudSprite = ImageLoader.CreateRadrarIcon(hero.ChampionName + "_Square_0", System.Drawing.Color.DarkGoldenrod, 100);
            MinimapSprite = ImageLoader.CreateMinimapSprite(hero.ChampionName + "_Square_0");
            LastVisableTime = Game.Time;
            LastVisablePos = hero.Position;
            PredictedPos = hero.Position;
            IsJungler = hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite"));

            StartRecallTime = 0;
            AbortRecallTime = 0;
            FinishRecallTime = 0;
            Game.OnUpdate += OnUpdate;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
                return;
            NormalSprite.VisibleCondition = sender => !Hero.IsDead;
            HudSprite.VisibleCondition = sender => !Hero.IsDead;
            //MinimapSprite.VisibleCondition = sender => !Hero.IsDead && !Hero.IsHPBarRendered;
        }
    }

    public class RecallInf
    {
        public int NetworkID;
        public int Duration;
        public int Start;
        public TeleportType Type;
        public TeleportStatus Status;

        public RecallInf(int netid, TeleportStatus stat, TeleportType tpe, int dura, int star = 0)
        {
            NetworkID = netid;
            Status = stat;
            Type = tpe;
            Duration = dura;
            Start = star;
        }
    }

    class OKTWtracker
    {
        public static List<ChampionInfo> ChampionInfoList = new List<ChampionInfo>();

        private Vector3 EnemySpawn;

        public void LoadOKTW()
        {
            EnemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy).Position;
            foreach (var hero in HeroManager.AllHeroes)
            {
                ChampionInfoList.Add(new ChampionInfo(hero));
            }

            Game.OnUpdate += OnUpdate;
            EloBuddy.SDK.Events.Teleport.OnTeleport += Obj_AI_Base_OnTeleport;
        }

        private static void Obj_AI_Base_OnTeleport(GameObject sender, EloBuddy.SDK.Events.Teleport.TeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
                return;

            var ChampionInfoOne = ChampionInfoList.Find(x => x.Hero.NetworkId == sender.NetworkId);

            var recall = new RecallInf(sender.NetworkId, args.Status, args.Type, args.Duration, args.Start);

            if (recall.Type == TeleportType.Recall)
            {
                switch (recall.Status)
                {
                    case TeleportStatus.Start:
                        ChampionInfoOne.StartRecallTime = Game.Time;
                        break;
                    case TeleportStatus.Abort:
                        ChampionInfoOne.AbortRecallTime = Game.Time;
                        break;
                    case TeleportStatus.Finish:
                        ChampionInfoOne.FinishRecallTime = Game.Time;
                        var spawnPos = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy).Position;
                        ChampionInfoOne.LastVisablePos = spawnPos;
                        ChampionInfoOne.PredictedPos = spawnPos;
                        ChampionInfoOne.LastWayPoint = spawnPos;
                        ChampionInfoOne.LastVisableTime = Game.Time;
                        break;
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Program.LagFree(0))
                return;

            foreach (var extra in ChampionInfoList.Where(x => x.Hero.IsEnemy))
            {
                var enemy = extra.Hero;
                if (enemy.IsDead)
                {
                    extra.LastVisablePos = EnemySpawn;
                    extra.LastVisableTime = Game.Time;
                    extra.PredictedPos = EnemySpawn;
                    extra.LastWayPoint = EnemySpawn;
                }
                else if (enemy.IsHPBarRendered)
                {
                    extra.LastWayPoint = extra.Hero.GetWaypoints().Last().To3D();
                    extra.PredictedPos = enemy.Position.Extend(extra.LastWayPoint, 125);
                    extra.LastVisablePos = enemy.Position;
                    extra.LastVisableTime = Game.Time;
                }
            }
        }
    }
}
