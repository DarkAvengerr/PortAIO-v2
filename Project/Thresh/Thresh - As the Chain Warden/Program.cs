using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshAsurvil
{
	class Program {
		static void Main(string[] args) {
			CustomEvents.Game.OnGameLoad += Thresh.OnLoad;
		}

		
	}
}
