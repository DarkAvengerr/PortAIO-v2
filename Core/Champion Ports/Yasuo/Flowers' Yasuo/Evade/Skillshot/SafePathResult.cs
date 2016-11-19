using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Evade
{
    public struct SafePathResult
    {
        public FoundIntersection Intersection;
        public bool IsSafe;

        public SafePathResult(bool isSafe, FoundIntersection intersection)
        {
            IsSafe = isSafe;
            Intersection = intersection;
        }
    }
}
