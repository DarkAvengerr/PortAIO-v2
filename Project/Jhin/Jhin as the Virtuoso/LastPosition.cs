using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin_As_The_Virtuoso {
	public class LastPosition {

		public static List<LastPositionStruct> LastPositions { get; private set; } = new List<LastPositionStruct>();
		public static Vector3 SpawnPoint { get; private set; }

		public static void Load() {
			var spawn = ObjectManager.Get<Obj_SpawnPoint>().Where(s => s.IsEnemy).FirstOrDefault();
			SpawnPoint = spawn != null ? spawn.Position : Vector3.Zero;

			foreach (var enemy in HeroManager.Enemies)
			{
				var eStruct = new LastPositionStruct(enemy) { LastPosition = SpawnPoint };
				LastPositions.Add(eStruct);
			}

			Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
			Game.OnUpdate += Game_OnUpdate;
		}

		public static List<LastPositionStruct> GetLastPositionsInRange(Vector3 pos,float range) {
			return LastPositions.Where(lp => lp.LastPosition.Distance(pos) < range && !lp.Hero.IsDead).ToList();
		}

		public static List<LastPositionStruct> GetLastPositionsInRange(Obj_AI_Base unit, float range) {
			return LastPositions.Where(lp => lp.LastPosition.Distance(unit.Position) < range && !lp.Hero.IsDead).ToList();
		}

		public static List<LastPositionStruct> GetLastPositionsInRange(Vector3 pos, float range, float time) {
			return LastPositions.Where(lp => lp.LastPosition.Distance(pos) < range && !lp.Hero.IsDead && Game.Time - lp.LastSeen < time).ToList();
		}

		public static List<LastPositionStruct> GetLastPositionsInRange(Obj_AI_Base unit, float range,float time) {
			return LastPositions.Where(lp => lp.LastPosition.Distance(unit.Position) < range && !lp.Hero.IsDead && Game.Time - lp.LastSeen < time).ToList();
		}

		private static void Game_OnUpdate(EventArgs args) {
			foreach (var lp in LastPositions)
			{
				if (!lp.Hero.IsDead && !lp.LastPosition.Equals(Vector3.Zero) &&
					lp.LastPosition.Distance(lp.Hero.Position) > 500)
				{
					lp.Teleported = false;
					lp.LastSeen = Game.Time;
					
				}
				lp.LastPosition = lp.Hero.Position;
				if (lp.Hero.IsVisible)
				{
					lp.Teleported = false;
					if (!lp.Hero.IsDead)
					{
						lp.LastSeen = Game.Time;
					}
				}
			}
		}

		private static void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args) {
			var packet = Packet.S2C.Teleport.Decoded(sender, args);
			var lastPosition = LastPositions.FirstOrDefault(e => e.Hero.NetworkId == packet.UnitNetworkId);
			if (lastPosition != null)
			{
				switch (packet.Status)
				{
					case Packet.S2C.Teleport.Status.Start:
						lastPosition.IsTeleporting = true;
						break;
					case Packet.S2C.Teleport.Status.Abort:
						lastPosition.IsTeleporting = false;
						break;
					case Packet.S2C.Teleport.Status.Finish:
						lastPosition.Teleported = true;
						lastPosition.IsTeleporting = false;
						lastPosition.LastSeen = Game.Time;
						break;
				}
			}
		}
	}

	public class LastPositionStruct {
		public LastPositionStruct(AIHeroClient hero) {
			Hero = hero;
			LastPosition = Vector3.Zero;
		}

		public AIHeroClient Hero { get; private set; }
		public bool IsTeleporting { get; set; }
		public float LastSeen { get; set; }
		public Vector3 LastPosition { get; set; }
		public bool Teleported { get; set; }
	}
}
