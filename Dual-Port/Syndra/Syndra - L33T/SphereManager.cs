using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public class SphereManager
    {
        private static int _objectNetworkId = -1;
        private static int _sphereTick;
        private static Vector3 _sphereVector3;
        private static int _sphereStasisTick;
        private static Vector3 _sphereStasisVector3;

        public static int ObjectNetworkId
        {
            get { return Mechanics.Spells[SpellSlot.E].Instance.Instance.ToggleState == 1 ? -1 : _objectNetworkId; }
            set { _objectNetworkId = value; }
        }

        public static void OnProcessPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] == 0xBA)
            {
                ObjectNetworkId = new GamePacket(args.PacketData) { Position = 2 }.ReadInteger();
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "SyndraQ")
                {
                    _sphereTick = (int) (Game.Time * 0x3E8);
                    _sphereVector3 = args.End;
                }
                else if (ForceOfWill(true) != null && (args.SData.Name == "SyndraW" || args.SData.Name == "syndraw2"))
                {
                    _sphereStasisTick = (int) (Game.Time * 0x3E8) + 250;
                    _sphereStasisVector3 = args.End;
                }
            }
        }

        private static Obj_AI_Minion ForceOfWill(bool sphere)
        {
            if (ObjectNetworkId == -1)
            {
                return null;
            }

            var obj = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)ObjectNetworkId);
            return (obj != null && obj.IsValid<Obj_AI_Minion>() && (obj.Name == "Seed" && sphere || !sphere))
                ? (Obj_AI_Minion) obj
                : null;
        }

        public static List<Vector3> GetSpheres(bool toGrab = false)
        {
            var result = new List<Vector3>();
            foreach (var obj in
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(obj => obj.IsValid && obj.Team == EntryPoint.Player.Team && obj.Name == "Seed"))
            {
                var valid = false;
                if (obj.NetworkId != ObjectNetworkId)
                {
                    if (
                        ObjectManager.Get<GameObject>()
                            .Any(
                                b =>
                                    b.IsValid && b.Name.Contains("_Q_") && b.Name.Contains("Syndra_") &&
                                    b.Name.Contains("idle") && obj.Position.LSDistance(b.Position) < 50))
                    {
                        valid = true;
                    }
                }

                if (valid && (!toGrab || !obj.IsMoving))
                {
                    result.Add(obj.ServerPosition);
                }
            }

            if ((int) (Game.Time * 0x3E8) - _sphereTick < 400)
            {
                result.Add(_sphereVector3);
            }

            if ((int) (Game.Time * 0x3E8) - _sphereStasisTick < 400 && (int) (Game.Time * 0x3E8) > 0)
            {
                result.Add(_sphereStasisVector3);
            }

            return result;
        }

        public static Vector3 GetGrabbableSpheres(int range)
        {
            var list = GetSpheres(true).Where(orb => EntryPoint.Player.LSDistance(orb) < range).ToList();
            return list.Count > 0 ? list[0] : new Vector3();
        }
    }
}