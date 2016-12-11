using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Common
{
    using SharpDX;

    internal class WardLocation
    {
        public readonly bool Grass;
        public readonly Vector3 Pos;

        public WardLocation(Vector3 pos, bool grass)
        {
            Pos = pos;
            Grass = grass;
        }
    }
}
