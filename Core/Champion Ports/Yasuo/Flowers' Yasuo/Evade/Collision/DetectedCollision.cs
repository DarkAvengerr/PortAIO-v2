using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Evade
{
    using LeagueSharp;
    using SharpDX;

    internal class DetectedCollision
    {
        public float Diff;
        public float Distance;
        public Vector2 Position;
        public CollisionObjectTypes Type;
        public Obj_AI_Base Unit;
    }
}
