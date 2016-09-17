using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using crisMasterYi.entity;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace crisMasterYi.entity
{
    class JungleCamp
    {
        public Vector3 position { get; set; }
        public TimeSpan spawnTime { get; set; }
        public TimeSpan respawnTime { get; set; }
        public List<JungleMinion> minions { get; set; }
    }
}
