using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding
{
    using SharpDX;

    public class NodeBase
    {
        public NodeBase(Vector3 position)
        {
            this.Position = position;
        }

        public Vector3 Position { get; set; }
    }
}
