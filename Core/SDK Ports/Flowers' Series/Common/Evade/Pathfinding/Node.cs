using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Common.Evade.Pathfinding
{
    using SharpDX;
    using System.Collections.Generic;

    public class Node
    {
        public Vector2 Point;
        public List<Node> Neightbours;

        public Node(Vector2 point)
        {
            Point = point;
            Neightbours = new List<Node>();
        }
    }
}
