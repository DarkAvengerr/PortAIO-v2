using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = SharpDX.Color;

using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Events;
using static EloBuddy.SDK.Events.Teleport;
using EloBuddy.SDK.Enumerations;

namespace UniversalMinimapHack
{
    public class HeroTracker
    {
        public HeroTracker(AIHeroClient hero, Bitmap bmp)
        {
            Hero = hero;

            RecallStatus = TeleportStatus.Unknown;
            Hero = hero;
            var image = new Render.Sprite(bmp, new Vector2(0, 0));
            image.GrayScale();
            image.Scale = new Vector2(MinimapHack.Instance().Menu.IconScale, MinimapHack.Instance().Menu.IconScale);
            image.VisibleCondition = sender => !hero.IsHPBarRendered && !hero.IsDead;
            image.PositionUpdate = delegate
            {
                Vector2 v2 = Drawing.WorldToMinimap(LastLocation);
                v2.X -= image.Width / 2f;
                v2.Y -= image.Height / 2f;
                return v2;
            };
            image.Add(0);
            LastSeen = 0;
            LastLocation = hero.ServerPosition;
            PredictedLocation = hero.ServerPosition;
            BeforeRecallLocation = hero.ServerPosition;

            Text = new Render.Text(0, 0, "", MinimapHack.Instance().Menu.SSTimerSize, Color.White)
            {
                VisibleCondition =
                    sender =>
                        !hero.IsHPBarRendered && !Hero.IsDead && MinimapHack.Instance().Menu.SSTimer && LastSeen > 20f &&
                        MinimapHack.Instance().Menu.SSTimerStart <= Game.Time - LastSeen,
                PositionUpdate = delegate
                {
                    Vector2 v2 = Drawing.WorldToMinimap(LastLocation);
                    v2.Y += MinimapHack.Instance().Menu.SSTimerOffset;
                    return v2;
                },
                TextUpdate = () => Program.Format(Game.Time - LastSeen),
                OutLined = true,
                Centered = true
            };
            Text.Add(0);

            Teleport.OnTeleport += Obj_AI_Base_OnTeleport;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private Render.Text Text { get; set; }
        private AIHeroClient Hero { get; set; }
        private TeleportStatus RecallStatus { get; set; }
        private float LastSeen { get; set; }
        private Vector3 LastLocation { get; set; }
        private Vector3 PredictedLocation { get; set; }
        private Vector3 BeforeRecallLocation { get; set; }
        private bool Pinged { get; set; }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!Hero.IsHPBarRendered && !Hero.IsDead)
            {
                float radius = Math.Abs(LastLocation.X - PredictedLocation.X);
                if (radius < MinimapHack.Instance().Menu.SSCircleSize && MinimapHack.Instance().Menu.SSCircle)
                {
                    System.Drawing.Color c = MinimapHack.Instance().Menu.SSCircleColor;
                    if (RecallStatus == TeleportStatus.Start)
                    {
                        c = System.Drawing.Color.LightBlue;
                    }
                    
                    LeagueSharp.Common.Utility.DrawCircle(LastLocation, radius, c, 1, 30, true);
                }
            }
            if (Text.Visible)
            {
                Text.OnEndScene();
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Hero.ServerPosition != LastLocation && Hero.ServerPosition != BeforeRecallLocation)
            {
                LastLocation = Hero.ServerPosition;
                PredictedLocation = Hero.ServerPosition;
                LastSeen = Game.Time;
            }

            if (!Hero.IsHPBarRendered && RecallStatus != TeleportStatus.Start)
            {
                PredictedLocation = new Vector3(
                    LastLocation.X + ((Game.Time - LastSeen) * Hero.MoveSpeed), LastLocation.Y, LastLocation.Z);
            }

            if (Hero.IsHPBarRendered && !Hero.IsDead)
            {
                Pinged = false;
                LastSeen = Game.Time;
            }

            if (LastSeen > 0f && MinimapHack.Instance().Menu.Ping && !Hero.IsHPBarRendered)
            {
                if (Game.Time - LastSeen >= MinimapHack.Instance().Menu.MinPing && !Pinged)
                {
                    TacticalMap.ShowPing(PingCategory.EnemyMissing,Hero,true);
                    Pinged = true;
                }
            }
        }

        private void Obj_AI_Base_OnTeleport(GameObject sender, TeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
            {
                return;
            }

            var decoded = new RecallInf(unit.NetworkId, args.Status, args.Type, args.Duration, args.Start);
            if (unit.NetworkId == Hero.NetworkId && decoded.Type == TeleportType.Recall)
            {
                RecallStatus = decoded.Status;
                if (decoded.Status == TeleportStatus.Finish)
                {
                    BeforeRecallLocation = Hero.ServerPosition;
                    Obj_SpawnPoint enemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
                    if (enemySpawn != null)
                    {
                        LastLocation = enemySpawn.Position;
                        PredictedLocation = enemySpawn.Position;
                    }
                    LastSeen = Game.Time;
                }
            }
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
}