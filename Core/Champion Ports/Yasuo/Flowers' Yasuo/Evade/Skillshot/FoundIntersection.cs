using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Evade
{
    using LeagueSharp.Common;
    using SharpDX;

    public struct FoundIntersection
    {
        public Vector2 ComingFrom;
        public float Distance;
        public Vector2 Point;
        public int Time;
        public bool Valid;

        public FoundIntersection(float distance, int time, Vector2 point, Vector2 comingFrom)
        {
            Distance = distance;
            ComingFrom = comingFrom;
            Valid = (point.X != 0) && (point.Y != 0);
            Point = point + EvadeManager.GridSize * (ComingFrom - point).Normalized();
            Time = time;
        }
    }
}
