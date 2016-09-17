using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Aurelion_Sol_As_the_Star_Forger {
	class Program {
		static void Main(string[] args) {
			LeagueSharp.Common.CustomEvents.Game.OnGameLoad += AurelionSol.Game_OnGameLoad;
		}
	}
}
