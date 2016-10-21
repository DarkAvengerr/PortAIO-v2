using System;
using System.Collections.Generic;
using System.Linq;

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.UI;
using LeagueSharp.SDK.Utils;
using NLog;

using SharpDX;

using Color = System.Drawing.Color;

using EloBuddy;
using LeagueSharp.SDK;
namespace xcKalista
{
    internal static class SoulHandler
    {
        const string SoulBaseSkinName = "kalistaspawn";

        internal static readonly List<WCastPositionInfo> WCastPositionList;
        private static readonly List<SoulInfo> SoulList = new List<SoulInfo>();
        private static readonly Random DICE = new Random();

        static SoulHandler()
        {
            WCastPositionList = new List<WCastPositionInfo>
            {
                new WCastPositionInfo(GameMapId.SummonersRift, "Dragon", new Vector3(9827.56f, 4426.136f, -71.2406f)), new WCastPositionInfo(GameMapId.SummonersRift, "Baron", new Vector3(4951.126f, 10394.05f, -71.2406f)), new WCastPositionInfo(GameMapId.SummonersRift, GameObjects.Player.Team.HasFlag(GameObjectTeam.Order) ? "Enemy Blue" : "Ally Blue", new Vector3(10998.14f, 6954.169f, 51.72351f)), //chaos blue
                new WCastPositionInfo(GameMapId.SummonersRift, GameObjects.Player.Team.HasFlag(GameObjectTeam.Order) ? "Enemy Red" : "Ally Red", new Vector3(7082.083f, 10838.25f, 56.2041f)), //chaos red
                new WCastPositionInfo(GameMapId.SummonersRift, GameObjects.Player.Team.HasFlag(GameObjectTeam.Order) ? "Ally Blue" : "Enemy Blue", new Vector3(3804.958f, 7875.456f, 52.11121f)), //order blue
                new WCastPositionInfo(GameMapId.SummonersRift, GameObjects.Player.Team.HasFlag(GameObjectTeam.Order) ? "Ally Red" : "Enemy Red", new Vector3(7811.249f, 4034.486f, 53.81299f)) //order red
            };

            GameObject.OnCreate += Soul_OnCreate;
            GameObject.OnDelete += Soul_OnDelete;
            Obj_AI_Base.OnNewPath += Soul_OnNewPath;
            Obj_AI_Base.OnPlayAnimation += Soul_OnPlayAnimation;
            Drawing.OnDraw += Drawing_OnDraw;

            new TickOperation(0x12C, OnTick).Start();
        }

        internal static void Initialize() { }

        private static void OnTick()
        {
            if (!Config.Auto.AutoW.Enabled || !SpellManager.W.IsReady() || GameObjects.Player.Mana < Config.Auto.AutoW.MinMana || GameObjects.Player.Mana - SpellManager.W.Instance.SData.Mana < 40 || Config.Auto.AutoW.KeepWCharge && SpellManager.W.Instance.Ammo < 2 || !Variables.Orbwalker.CanMove || GameObjects.Player.IsRecalling() || GameObjects.Player.CountEnemyHeroesInRange(800) > 0 || Variables.TickCount - Variables.Orbwalker.LastAutoAttackTick < 2000)
            {
                return;
            }

            var bestWPos = WCastPositionList.Where(x => x.MapID == Game.MapId && GameObjects.Player.Distance(x.Position) < SpellManager.W.Range - 250 && Config.Auto.AutoW.Menu.GetValue<MenuBool>(x.PositionName).Value).OrderBy(x => SoulList.Count(i => i.Destination == x.PositionName)).FirstOrDefault();
            if (bestWPos != null)
            {
                var randomize = new Vector3(DICE.Next(-245, 245), DICE.Next(-245, 245), 0f);
                var randomizedPos = bestWPos.Position + randomize;
                var legitPos = new Vector3(randomizedPos.X, randomizedPos.Y, NavMesh.GetHeightForPosition(randomizedPos.X, randomizedPos.Y));
                SpellManager.W.Cast(legitPos);
                //Logging.Write()(LogLevel.Debug, $"\nRandomize: {randomize}\nOriginal: {bestWPos.Position}\nRandomized: {randomizedPos}\nLegitPos: {legitPos}");
            }
        }

        private static void Soul_OnCreate(GameObject sender, EventArgs args)
        {
            var senderAsMinion = sender as Obj_AI_Minion;
            if (senderAsMinion == null || senderAsMinion.IsEnemy || senderAsMinion.CharData.BaseSkinName != SoulBaseSkinName)
                return;

            SoulList.Add(new SoulInfo
            {
                SoulMinion = senderAsMinion
            });
        }

        private static void Soul_OnDelete(GameObject sender, EventArgs args)
        {
            var senderAsMinion = sender as Obj_AI_Minion;
            if (senderAsMinion == null || senderAsMinion.IsEnemy || senderAsMinion.CharData.BaseSkinName != SoulBaseSkinName)
                return;

            SoulList.RemoveAll(x => x.SoulMinion.NetworkId == sender.NetworkId);
        }

        private static void Soul_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            var soulinfo = SoulList.FirstOrDefault(x => x.SoulMinion.NetworkId == sender.NetworkId);
            if (soulinfo != null && soulinfo.Destination == "Unknown")
            {
                var mapPosinfo = WCastPositionList.FirstOrDefault(x => x.Position.Distance(args.Path.Last()) < 500);
                if (mapPosinfo != null)
                {
                    soulinfo.Destination = mapPosinfo.PositionName;
                }
            }
        }

        private static void Soul_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            var soulinfo = SoulList.FirstOrDefault(x => x.SoulMinion.NetworkId == sender.NetworkId);
            if (soulinfo != null && args.Animation == "SlowTurn")
            {
                soulinfo.RemainingRotations--;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
        }

        internal class WCastPositionInfo
        {
            internal readonly GameMapId MapID;
            internal readonly Vector3 Position;
            internal readonly string PositionName;

            internal WCastPositionInfo(GameMapId mapId, string positionName, Vector3 position)
            {
                MapID = mapId;
                PositionName = positionName;
                Position = position;
            }
        }

        private class SoulInfo
        {
            internal string Destination = "Unknown";
            internal int RemainingRotations = 5;
            internal Obj_AI_Minion SoulMinion;
        }
    }
}
