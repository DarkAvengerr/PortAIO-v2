#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

#endregion

namespace SyndraL33T
{
    public static class OrbManager
    {
        private static int _wobjectnetworkid = -1;

        public static int WObjectNetworkId
        {
            get
            {
                if (ObjectManager.Player.Spellbook.GetSpell(EloBuddy.SpellSlot.W).ToggleState == 1)
                    return -1;

                return _wobjectnetworkid;
            }
            set
            {
                _wobjectnetworkid = value;
            }
        }

        static OrbManager()
        {
            Game.OnProcessPacket += Game_OnGameProcessPacket;
        }

        public static Obj_AI_Minion WObject(bool onlyOrb)
        {
            if (WObjectNetworkId == -1) return null;
            var obj = ObjectManager.GetUnitByNetworkId<Obj_AI_Minion>((uint)WObjectNetworkId);
            if (obj != null && obj.IsValid && (obj.Name == "Seed" && onlyOrb || !onlyOrb)) return obj;
            return null;
        }

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] == 0x71)
            {
                var packet = new GamePacket(args.PacketData) { Position = 1 };
                var networkId = packet.ReadInteger();
                WObjectNetworkId = networkId;
            }
        }

        public static List<Vector3> GetOrbs(bool toGrab = false)
        {
            var result = new List<Vector3>();
            foreach (
                var obj in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(obj => obj.IsValid && obj.Team == ObjectManager.Player.Team && obj.Name == "Seed"))
            {
                var valid = false;
                if (obj.NetworkId != WObjectNetworkId)
                    if (
                        ObjectManager.Get<GameObject>()
                            .Any(
                                b =>
                                    b.IsValid && b.Name.Contains("_Q_") && b.Name.Contains("Syndra_") &&
                                    b.Name.Contains("idle") && obj.Position.Distance(b.Position) < 50))
                        valid = true;

                if (valid && (!toGrab || !obj.IsMoving))
                    result.Add(obj.ServerPosition);
            }
            return result;
        }

        public static Vector3 GetOrbToGrab(int range)
        {
            var list = GetOrbs(true).Where(orb => ObjectManager.Player.Distance(orb) < range).ToList();
            return list.Count > 0 ? list[0] : new Vector3();
        }
    }
}