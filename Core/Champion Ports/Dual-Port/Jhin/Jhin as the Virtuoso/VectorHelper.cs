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
	public static class VectorHelper {

		#region BushList
		public static List<Vector3> BushList => new List<Vector3>
		{
			new Vector3(9158,2106,60.65829f),
			new Vector3(10376,3078,50.02629f),
			new Vector3(11870,3894,-67.27751f),
			new Vector3(13326,2520,51.3669f),
			new Vector3(12450,1578,53.83765f),
			new Vector3(12494,5166,51.7294f),
			new Vector3(11578,7142,51.72698f),
			new Vector3(10018,7866,51.74104f),
			new Vector3(14058,6966,52.3063f),
			new Vector3(9960,6628,45.46295f),
			new Vector3(9442,5634,-71.2406f),
			new Vector3(8346,6450,-71.2406f),
			new Vector3(8606,4706,51.80402f),
			new Vector3(8004,3410,51.5508f),
			new Vector3(7028,3030,52.5475f),
			new Vector3(5616,3576,51.42113f),
			new Vector3(6612,4690,48.527f),
			new Vector3(4712,7190,50.80887f),
			new Vector3(3282,7820,51.99127f),
			new Vector3(2194,9998,53.44795f),
			new Vector3(3106,10814,-67.81899f),
			new Vector3(1332,12448,52.8381f),
			new Vector3(1748,12906,52.8381f),
			new Vector3(2398,13350,52.8381f),
			new Vector3(4448,11764,56.8484f),
			new Vector3(5648,12718,52.8381f),
			new Vector3(6752,11540,53.82966f),
			new Vector3(7948,11956,56.4768f),
			new Vector3(8280,10180,50.02029f),
			new Vector3(9262,11324,53.67044f),
			new Vector3(7030,14054,52.8381f),
		};
		#endregion

		public static List<LastPositionStruct> GetLastPositionInRCone() {
			return LastPosition.LastPositions.FindAll(lp => lp.LastPosition.InRCone());
		}

		public static List<Vector3> GetBushInRCone() {
			var list = BushList.FindAll(b => b.InRCone()); 
			return list;
		}

		public static Vector3 GetBushNearPosotion(Vector3 pos, List<Vector3> BushList = null) {
			if (BushList == null)
			{
				BushList = GetBushInRCone();
			}
			if (BushList == null)
			{
				return Vector3.Zero;
			}

			Vector3 temp = Vector3.Zero;
			foreach (var bush in BushList)
			{
				if (temp == Vector3.Zero || bush.Distance(pos) < temp.Distance(pos))
				{
					temp = bush;
				}
			}
			return temp;
		}

		public static Vector3 GetBushNearEnemy(AIHeroClient enemy, List<Vector3> BushList = null) {
			return GetBushNearPosotion(enemy.Position, BushList);
		}
	}
}
